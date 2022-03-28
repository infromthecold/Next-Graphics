using NextGraphics.Exporting;
using NextGraphics.Exporting.Common;
using NextGraphics.Utils;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;

namespace NextGraphics.Models
{
	public abstract class SourceTilemap : SourceFile<TilemapData>
	{
		private MainModel Model { get; set; }

		public Bitmap SourceBitmap { get; private set; } = null;

		public bool IsSourceImage { get => SourceBitmap != null; }

		#region Initialization & Disposal

		public SourceTilemap(string filename, MainModel model) : base(filename, autoLoad: false)
		{
			// Since we need to first assign the model before loading, we set autoLoad to false when calling base class constructor (which would otherwise load the data), then load manually after we're ready. We must also reset the autoload flag before loading otherwise reload will be ignored.
			Model = model;
			AutoLoad = true;
			Reload();
		}

		public SourceTilemap(string filename, MainModel model, TilemapData data) : base(filename, data)
		{
			// Note in this case we don't have to change auto loading in base class - loading is skipped when providing data manually.
			Model = model;
		}

		/// <summary>
		/// Creates a new <see cref="SourceTilemap"/> based on the type (which is the same as <see cref="MainModel.AddTilemapsFilterIndex"/>). If type is unknown, null is returned!
		/// </summary>
		public static SourceTilemap Create(string filename, MainModel model)
		{
			switch (Path.GetExtension(filename).ToLower())
			{
				case ".map": return new SourceTilemapMap(filename, model);
				case ".stm": return new SourceTilemapStm(filename, model);
				case ".txm": return new SourceTilemapText(filename, model);
				case ".txt": return new SourceTilemapText(filename, model);
				case ".bmp": return new SourceTilemapImage(filename, model);
				case ".png": return new SourceTilemapImage(filename, model);
				default: return null;
			}
		}

		#endregion

		#region Overrides

		protected sealed override TilemapData OnLoadDataFromFile(string filename)
		{
			// We prevent subclasses from overriding the default method and we instead divert to the variant with MainModel. Subclasses should support settings from the model as much as possible.
			return OnLoadDataFromFile(filename, Model);
		}

		#endregion

		#region Subclass

		/// <summary>
		/// Called when data needs to be loaded from the given file.
		/// </summary>
		protected abstract TilemapData OnLoadDataFromFile(string filename, MainModel model);

		/// <summary>
		/// Subclasses that are loaded from a bitmap, should register it with this method.
		/// </summary>
		public void AssignSourceBitmap(Bitmap bitmap)
		{
			// If we have a bitmap assigned, we can always reload data, even if filename is otherwise not provided.
			AutoLoad = bitmap != null;

			SourceBitmap = bitmap;
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Updates tile banks and other information from the given mapped data.
		/// </summary>
		public void UpdateTiles(ExportData data)
		{
			for (int y = 0; y < Data.Height; y++)
			{
				for (int x = 0; x < Data.Width; x++)
				{
					Data.GetTile(x, y).UpdatePaletteBank(data);
				}
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
			if (options == null) return;

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

					if (options.RenderTileIndex)
					{
						g.DrawString(
							tile.Index.ToString(),
							indexFont,
							indexBrush,
							scaledRect);
					}

					if (options.RenderTileAttributes && scaleFactor >= 2)
					{
						var rect = new RectangleF(
							scaledRect.X,
							scaledRect.Bottom - indexFont.Height,
							scaledRect.Width,
							scaledRect.Height - indexFont.Height);

						// Render palette bank in bottom left.
						g.DrawString(
							tile.PaletteBank.ToString(),
							indexFont,
							indexBrush,
							rect);

						// Render attributes in bottom right.
						var debug = new StringBuilder();
						if (tile.FlippedX) debug.Append("X");
						if (tile.FlippedY) debug.Append("Y");
						if (tile.RotatedClockwise) debug.Append("R");

						if (debug.Length > 0) {
							var format = new StringFormat
							{
								Alignment = StringAlignment.Far
							};

							g.DrawString(
								debug.ToString(),
								indexFont,
								indexBrush,
								rect,
								format);
						}
					}
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

		#region Initialization & Disposal

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

		#endregion

		#region Helpers

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

		#endregion

		#region Declarations

		public class Tile
		{
			public int Index { get; set; } = 0;
			public int PaletteBank { get; set; } = 0;
			public bool FlippedX { get; set; } = false;
			public bool FlippedY { get; set; } = false;
			public bool RotatedClockwise { get; set; } = false;

			public Tile()
			{
			}

			public void UpdatePaletteBank(ExportData data)
			{
				// If auto-banking is not supported, we use 0.
				var tile = data.Blocks[Index];
				PaletteBank = tile.IsAutoBankingSupported ? tile.PaletteBank : 0;
			}
		}

		#endregion
	}

	public class RenderingOptions
	{
		public bool RenderTileIndex { get; set; } = false;
		public bool RenderTileAttributes { get; set; } = false;
	}

	#endregion
}
