using NextGraphics.Exporting.Common;
using NextGraphics.Exporting.Exporters;
using NextGraphics.Exporting.Exporters.Base;
using NextGraphics.Exporting.Exporters.ZXNext;
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
			if (Data.Model.BinaryOutput && Data.Model.BinaryBlocksOutput)
			{
				// Note: assembler exporter must be last because it needs data produced by binary exporters.
				exporters.Add(new ZXNextBinaryTilesDataExporter());
				exporters.Add(new ZXNextBinaryTilesMapExporter());
				exporters.Add(new ZXNextBinaryTilesExporter());
				exporters.Add(new ZXNextBinaryPaletteExporter());
				exporters.Add(new ZXNextAssemblerExporter());
			}
			else if (Data.Model.BinaryOutput)
			{
				// Note: assembler exporter must be last because it needs data produced by binary exporters.
				exporters.Add(new ZXNextBinaryTilesDataExporter());
				exporters.Add(new ZXNextBinaryTilesMapExporter());
				exporters.Add(new ZXNextBinaryPaletteExporter());
				exporters.Add(new ZXNextAssemblerExporter());
			}
			else
			{
				exporters.Add(new ZXNextAssemblerExporter());
			}

			if (Data.Model.BlocksAsImage)
			{
				exporters.Add(new ZXNextBlocksAsImageExporter());
			}

			if (Data.Model.TilesAsImage)
			{
				exporters.Add(new ZXNextTilesAsImageExporter());
				exporters.Add(new ZXNextTilesInfoExporter());
			}
		}

		private void RegisterSpriteExporters(List<BaseExporter> exporters)
		{
			if (Data.Model.BinaryOutput && Data.Model.BinaryBlocksOutput)
			{
				// Note: assembler exporter must be last because it needs data produced by binary exporters.
				exporters.Add(new ZXNextBinaryTilesDataExporter());
				exporters.Add(new ZXNextBinaryTilesExporter());
				exporters.Add(new ZXNextBinaryPaletteExporter());
				exporters.Add(new ZXNextAssemblerExporter());
			}
			else if (Data.Model.BinaryOutput)
			{
				// Note: assembler exporter must be last because it needs data produced by binary exporters.
				exporters.Add(new ZXNextBinaryTilesDataExporter());
				exporters.Add(new ZXNextBinaryPaletteExporter());
				exporters.Add(new ZXNextAssemblerExporter());
			}
			else
			{
				exporters.Add(new ZXNextAssemblerExporter());
			}

			// Note: sprites mode exports each block as its own image, but it's controller by tiles flag (not sure if this is a bug or feature, leaving it as such, it's really simple to change!
			if (Data.Model.TilesAsImage)
			{
				exporters.Add(new ZXNextSpritesBlocksAsImageExporter());
				exporters.Add(new ZXNextSpritesTilesAsImageExporter());
			}
		}

		#endregion

		#region Preparing data

		/// <summary>
		/// Prepares all the data needed for export by cutting and remaping images.
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
		public void GenerateInfoString(Action<Color, String> appender)
		{
			if (!Data.IsRemapped || Data.ObjectSize == 0) return;

			var count = (Data.Model.GridWidth / Data.ObjectSize) * (Data.Model.GridHeight / Data.ObjectSize);

			for (int b = 0; b < Data.BlocksCount; b++)
			{
				appender(Color.Black, $"{b}\t");

				for (int chr = 0; chr < count; chr++)
				{
					appender(Color.Black, "\t");

					var color = Data.Sprites[b].infos[chr].HasTransparent ? Color.Red : Color.Black;
					appender(color, $"{Data.SortIndexes[Data.Sprites[b].infos[chr].OriginalID]},");
				}

				appender(Color.Black, Environment.NewLine);
			}

			appender(Color.Black, Environment.NewLine);
			appender(Color.Black, "COUNTS");
			appender(Color.Black, Environment.NewLine);

			int[] counts = new int[ExportData.MAX_BLOCKS];
			for (int c = 0; c < counts.Length; c++)
			{
				counts[c] = 0;
			}

			for (int b = 0; b < Data.BlocksCount; b++)
			{
				appender(Color.Black, $"{b}\t");

				for (int chr = 0; chr < count; chr++)
				{
					counts[Data.SortIndexes[Data.Sprites[b].infos[chr].OriginalID]]++;
				}
			}

			for (int c = 0; c < counts.Length; c++)
			{
				appender(Color.Black, $"{c}\t{counts[c]}{Environment.NewLine}");
			}
		}

		#endregion
	}
}
