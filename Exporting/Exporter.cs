using NextGraphics.Exporting.Common;
using NextGraphics.Exporting.Exporters;
using NextGraphics.Exporting.Exporters.Base;
using NextGraphics.Exporting.Exporters.ZXNext;
using NextGraphics.Exporting.PaletteMapping;
using NextGraphics.Exporting.Remapping;
using NextGraphics.Models;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NextGraphics.Exporting
{
	/// <summary>
	/// Exporter is responsible for generating all export related data and managing export itself.
	/// 
	/// Though this is the entry point for generating export data (including intermediate data required for export), it's more of a coordinator for various underlying export classes where export actually happens.
	/// </summary>
	public class Exporter
	{
		public ExportData Data { get; private set; }

		#region Initialization & Disposal

		public Exporter(MainModel model, ExportParameters parameters)
		{
			this.Data = new ExportData(model, parameters);
		}

		#endregion

		#region Exporting

		public void Export()
		{
			if (Data.Parameters.ExportCallbacks != null)
			{
				Data.Parameters.ExportCallbacks.OnExportStarted();
			}

			var exporters = new List<BaseExporter>();

			switch (Data.Model.OutputType)
			{
				case OutputType.Tiles:
					RegisterTilesExporters(exporters);
					break;

				case OutputType.Sprites:
					RegisterSpriteExporters(exporters);
					break;
			}

			// Run all exporters.
			foreach (var exporter in exporters)
			{
				exporter.Export(Data);
			}

			if (Data.Parameters.ExportCallbacks != null)
			{
				Data.Parameters.ExportCallbacks.OnExportCompleted();
			}
		}

		private void RegisterTilesExporters(List<BaseExporter> exporters)
		{
			if (Data.Model.BinaryOutput && Data.Model.BinaryFramesAttributesOutput)
			{
				// Note: assembler exporter must be last because it needs data produced by binary exporters.
				exporters.Add(new ZXNextBinaryDataExporter());
				exporters.Add(new ZXNextBinaryTileAttributesExporter());
				exporters.Add(new ZXNextBinaryTilemapsExporter());
				exporters.Add(new ZXNextBinaryPaletteExporter());
				exporters.Add(new ZXNextAssemblerExporter());
			}
			else if (Data.Model.BinaryOutput)
			{
				// Note: assembler exporter must be last because it needs data produced by binary exporters.
				exporters.Add(new ZXNextBinaryDataExporter());
				exporters.Add(new ZXNextBinaryTilemapsExporter());
				exporters.Add(new ZXNextBinaryPaletteExporter());
				exporters.Add(new ZXNextAssemblerExporter());
			}
			else
			{
				exporters.Add(new ZXNextAssemblerExporter());
			}

			if (Data.Model.TilesExportAsImage)
			{
				exporters.Add(new ZXNextTilesAsImageExporter());
				exporters.Add(new ZXNextTilesInfoExporter());
			}
		}

		private void RegisterSpriteExporters(List<BaseExporter> exporters)
		{
			if (Data.Model.BinaryOutput && Data.Model.BinaryFramesAttributesOutput)
			{
				// Note: assembler exporter must be last because it needs data produced by binary exporters.
				exporters.Add(new ZXNextBinaryDataExporter());
				exporters.Add(new ZXNextBinarySpriteAttributesExporter());
				exporters.Add(new ZXNextBinaryPaletteExporter());
				exporters.Add(new ZXNextAssemblerExporter());
			}
			else if (Data.Model.BinaryOutput)
			{
				// Note: assembler exporter must be last because it needs data produced by binary exporters.
				exporters.Add(new ZXNextBinaryDataExporter());
				exporters.Add(new ZXNextBinaryPaletteExporter());
				exporters.Add(new ZXNextAssemblerExporter());
			}
			else
			{
				exporters.Add(new ZXNextAssemblerExporter());
			}

			if (Data.Model.SpritesExportAsImages)
			{
				exporters.Add(new ZXNextSpritesAsImagesExporter());
				exporters.Add(new ZXNextSpritesAsImageExporter());
			}
		}

		#endregion

		#region Preparing data

		/// <summary>
		/// Prepares the palette from the given bitmap.
		/// </summary>
		public PaletteMapper.Palette MapPalette(Bitmap bitmap)
		{
			return new PaletteMapper(Data).Map(bitmap);
		}

		/// <summary>
		/// Loads palette from the given source palette file.
		/// </summary>
		/// <param name="filename"></param>
		public PaletteMapper.Palette LoadPalette(string filename)
		{
			return new PaletteMapper(Data).Load(filename);
		}

		/// <summary>
		/// Prepares all the data needed for export by cutting and remaping images. This method expects palette is already set.
		/// </summary>
		/// <remarks>
		/// Note: for the moment being this need to be called manually before <see cref="Export"/>, but ideally it should be called automatically as part of export - an idea for the future improvement.
		/// </remarks>
		public void Remap()
		{
			new Remapper(Data).Remap();
		}

		/// <summary>
		/// Rebuilds an image from pre-existing tiles data.
		/// </summary>
		/// <remarks>
		/// Note: we're not changing any image internally, we're simply recalculating pixels and reporting the color for each coordinate. It's caller responsibility to update its image by acting on callback data.
		/// </remarks>
		public void RebuildFromTiles(Action<int, int, Color> pixelCallback)
		{
			int blocksX = 0;
			int blocksY = 0;

			for (int s = 0; s < Data.BlocksCount; s++)      // do all the blocks
			{
				for (int y = 0; y < Data.Sprites[s].Height; y++)
				{
					for (int x = 0; x < Data.Sprites[s].Width; x++)
					{
						byte tileId = (byte)Data.SortIndexes[Data.Sprites[s].GetId(x, y)];
						for (int pixelY = 0; pixelY < 8; pixelY++)
						{
							for (int pixelX = 0; pixelX < 8; pixelX++)
							{
								int readX = pixelX;
								int readY = pixelY;

								if (Data.Sprites[s].GetFlippedX(x, y) == true)
								{
									readX = 7 - pixelX;
								}

								if (Data.Sprites[s].GetFlippedY(x, y) == true)
								{
									readY = 7 - pixelY;
								}

								if (Data.Sprites[s].GetRotated(x, y) == true)
								{
									int temp = readX;
									readX = readY;
									readY = temp;
									readX = 7 - readX;
								}

								Color readColour = Data.Model.Palette[Data.Chars[tileId].GetPixel(readX, readY)].ToColor();

								pixelCallback(
									(x * 8) + pixelX + (blocksX * Data.Model.GridWidth),
									(y * 8) + pixelY + (blocksY * Data.Model.GridHeight),
									readColour);
							}
						}

					}
				}

				blocksX++;
				if (blocksX >= 20)
				{
					blocksX = 0;
					blocksY++;
				}
			}
		}

		/// <summary>
		/// Generates information string by calling out given appender closure for each piece of text passing it the colour and the text itself. It's responsibility of the caller to clear the display before calling this method.
		/// </summary>
		/// <remarks>
		/// This function should be called after <see cref="Remap"/> since it uses data generated there.
		/// </remarks>
		public void GenerateInfoString(Action<Color, string> appender)
		{
			void append(string text = "", Color? color = null)
			{
				appender(color ?? Color.Black, text);
			}

			void appendLine(string text = "", Color? color = null)
			{
				append(text, color);
				append(Environment.NewLine, color);
			}

			if (Data.IsRemapped && Data.ObjectSize > 0)
			{
				var count = (Data.Model.GridWidth / Data.ObjectSize) * (Data.Model.GridHeight / Data.ObjectSize);

				appendLine("BLOCKS");

				for (int b = 0; b < Data.BlocksCount; b++)
				{
					append($"{b}\t");

					for (int chr = 0; chr < count; chr++)
					{
						append("\t");

						var color = Data.Sprites[b].Infos[chr].HasTransparent ? Color.Red : Color.Black;
						var text = $"{Data.SortIndexes[Data.Sprites[b].Infos[chr].OriginalID]},";
						append(text, color);
					}

					appendLine();
				}

				appendLine();
				appendLine("COUNTS");

				int[] counts = new int[ExportData.MAX_BLOCKS];
				for (int c = 0; c < counts.Length; c++)
				{
					counts[c] = 0;
				}

				for (int b = 0; b < Data.BlocksCount; b++)
				{
					append($"{b}\t");

					for (int chr = 0; chr < count; chr++)
					{
						var id = Data.Sprites[b].Infos[chr].OriginalID;
						counts[Data.SortIndexes[id]]++;
					}
				}

				for (int c = 0; c < counts.Length; c++)
				{
					appendLine($"{c}\t{counts[c]}");
				}
			}

			var tilemaps = Data.Model.SourceTilemaps().ToList();
			if (tilemaps.Count > 0)
			{
				appendLine();
				appendLine("TILEMAPS");

				foreach (var tilemap in tilemaps)
				{
					appendLine();
					appendLine($"{tilemap.Filename}");

					if (tilemap.IsDataValid)
					{
						appendLine($"{tilemap.Data.Width}x{tilemap.Data.Height} tiles");
						appendLine();

						foreach (var tile in tilemap.Data.DistinctTiles)
						{
							append($"tile {tile.Index} ");
							appendLine($"palette bank {tile.PaletteBank}", Color.Gray);
						}
						appendLine();

						for (int y = 0; y < tilemap.Data.Height; y++)
						{
							append($"{y}: ");

							for (int x = 0; x < tilemap.Data.Width; x++)
							{
								var tile = tilemap.Data.GetTile(x, y);

								var flippedX = tile.FlippedX ? "x" : " ";
								var flippedY = tile.FlippedY ? "y" : " ";
								var rotated = tile.RotatedClockwise ? "r" : " ";
								
								append($"{tile.Index}");
								append($"{flippedX}{flippedY}{rotated}", Color.Gray);
								if (x < tilemap.Data.Width - 1) append(", ");
							}

							appendLine();
						}
					}
					else
					{
						appendLine("No data available");
					}
				}
			}
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Clears all the data, so next time full export will be run.
		/// </summary>
		public void Clear()
		{
			// At the moment, we only clear export data. But it's convenient to have this method too, so callers can clear directly on exporter and not think of all the inner objects that also need clearing
			Data.Clear();
		}

		#endregion
	}
}
