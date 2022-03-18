using NextGraphics.Exporting;
using NextGraphics.Exporting.Common;
using NextGraphics.Utils;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;

namespace NextGraphics.Models
{
	public abstract class SourceTilemap : SourceFile<TilemapData>
	{
		#region Initialization & Disposal

		public SourceTilemap(string filename) : base(filename)
		{
		}

		public SourceTilemap(string filename, TilemapData data) : base(filename, data)
		{
		}

		#endregion

		#region Creation

		/// <summary>
		/// Creates a new <see cref="SourceTilemap"/> based on the type (which is the same as <see cref="MainModel.AddTilemapsFilterIndex"/>). If type is unknown, null is returned!
		/// </summary>
		public static SourceTilemap Create(string filename)
		{
			switch (Path.GetExtension(filename).ToLower())
			{
				case ".map": return new SourceTilemapMap(filename);
				case ".stm": return new SourceTilemapStm(filename);
				case ".txm": return new SourceTilemapText(filename);
				case ".txt": return new SourceTilemapText(filename);
				default: return null;
			}
		}

		/// <summary>
		/// Creates a new bitmap with current data and tiles from the given <see cref="Exporter"/>. Model should be mapped otherwise tiles won't match. If data is missing, an empty image will be returned.
		/// </summary>
		public Bitmap CreateBitmap(Exporter exporter, RenderingOptions options = null)
		{
			if (!IsDataValid) return new Bitmap(50, 50).Clear();

			var model = exporter.Data.Model;
			var export = exporter.Data;

			// We ignore scale when rendering images.
			var outputSize = new Size(Data.Width * model.GridWidth, Data.Height * model.GridHeight);

			var result = new Bitmap(outputSize.Width, outputSize.Height);
			var errorPen = new Pen(Color.Yellow);

			result.Clear();

			using (var g = Graphics.FromImage(result))
			{
				g.InterpolationMode = InterpolationMode.NearestNeighbor;

				for (var y = 0; y < Data.Height; y++)
				{
					for (var x = 0; x < Data.Width; x++)
					{
						var tile = Data.GetTile(x, y);

						var rect = new Rectangle(x * model.GridWidth, y * model.GridHeight, model.GridWidth, model.GridHeight);

						if (tile.Index >= export.Chars.Length || export.Chars[tile.Index] == null)
						{
							g.DrawLine(errorPen, rect.Left, rect.Top, rect.Right, rect.Bottom);
							g.DrawLine(errorPen, rect.Left, rect.Bottom, rect.Right, rect.Top);
						}
						else
						{
							var tileSource = export.Chars[tile.Index];

							tileSource.CopyTo(
								result,
								model.Palette,
								rect.Location,
								tile.FlippedX,
								tile.FlippedY,
								tile.RotatedClockwise);
						}
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Renders overlay on top of existing image. It is expected that the image was rendered using <see cref="CreateBitmap"/> and correctly scaled according to `scaleFactor` parameter.
		/// </summary>
		public void RenderOverlay(Graphics g, Size size, double scaleFactor, Exporter exporter, RenderingOptions options = null)
		{
			if (!IsDataValid) return;
			if (scaleFactor < 1) return;
			if (options == null || !options.RenderTileIndex) return;

			var fontSize = 6 * scaleFactor * 0.4;
			if (fontSize < 5) fontSize = 5;
			if (fontSize > 10) fontSize = 10;

			var model = exporter.Data.Model;
			var indexFont = new Font(FontFamily.GenericMonospace, (int)fontSize);
			var indexBrush = new SolidBrush(Color.Black);

			var intrinsicRect = new Rectangle(0, 0, model.GridWidth, model.GridHeight);
			var scaledRect = new RectangleF(0, 0, intrinsicRect.Width * (float)scaleFactor, intrinsicRect.Height * (float)scaleFactor);

			for (var y = 0; y < Data.Height; y++)
			{
				for (var x = 0; x < Data.Width; x++)
				{
					var tile = Data.GetTile(x, y);

					intrinsicRect.X = x * model.GridWidth;
					intrinsicRect.Y = y * model.GridHeight;

					scaledRect.X = intrinsicRect.X * (float)scaleFactor;
					scaledRect.Y = intrinsicRect.Y * (float)scaleFactor;

					g.DrawString(
						tile.Index.ToString(),
						indexFont,
						indexBrush,
						scaledRect);
				}
			}
		}

		#endregion
	}

	#region Declarations

	public class TilemapData : IDisposable
	{
		/// <summary>
		/// Width of the tilemap in tiles.
		/// </summary>
		public int Width { get; private set; }

		/// <summary>
		/// Height of the tilemap in tiles.
		/// </summary>
		public int Height { get; private set; }

		public IOrderedEnumerable<Tile> DistinctTiles { get => distinctTilesList.OrderBy(t => t.Index); }

		private Tile[,] tiles;
		private readonly List<Tile> distinctTilesList = new List<Tile>();

		public TilemapData(int width, int height)
		{
			Width = width;
			Height = height;
			tiles = new Tile[Height, Width];
		}

		public void Dispose()
		{
			tiles = null;
		}

		public void Clear()
		{
			distinctTilesList.Clear();
		}

		public Tile GetTile(int x, int y)
		{
			// The main reason for preferring this method instead of accessing data in array directly is to reduce the chance for bugs where y and x coordinates would be swapped accidentally. Also VisualStudio tends to offer x before y while typing, so using this order further reduces the chance of error. And of course moves the "complexity" from multiple to one place.
			return tiles[y, x];
		}

		public void SetTile(int x, int y, Tile tile)
		{
			// Same reason as `GetTile` but also ensures distinct tiles are created.
			tiles[y, x] = tile;

			// If this tile is not yet present in distinct list, add it.
			foreach (var existing in distinctTilesList)
			{
				if (existing.Index == tile.Index) return;
			}
			distinctTilesList.Add(tile);
		}

		public class Tile
		{
			public int Index { get; set; } = 0;
			public int PaletteBank { get; set; } = 0;
			public bool FlippedX { get; set; } = false;
			public bool FlippedY { get; set; } = false;
			public bool RotatedClockwise { get; set; } = false;

			public Tile(
				int index, 
				bool flippedX = false, 
				bool flippedY = false, 
				bool rotatedClockwise = false)
			{
				Index = index;
				FlippedX = flippedX;
				FlippedY = flippedY;
				RotatedClockwise = rotatedClockwise;
			}

			public void UpdatePaletteBank(ExportData data)
			{
				// If auto-banking is not supported, we use 0.
				var tile = data.Blocks[Index];
				PaletteBank = tile.IsAutoBankingSupported ? tile.PaletteBank : 0;
			}
		}
	}

	public class RenderingOptions
	{
		public bool RenderTileIndex { get; set; } = false;
	}

	#endregion
}
