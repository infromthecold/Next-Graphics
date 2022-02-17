using System;
using System.IO;

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

		/// <summary>
		/// Two dimensional array of all tiles; first index represents row, second column within the row.
		/// </summary>
		public int[,] Tiles { get; set; }

		public TilemapData(int width, int height)
		{
			Width = width;
			Height = height;
			Tiles = new int[Height, Width];
		}

		public void Dispose()
		{
			Tiles = null;
		}
	}

	#endregion
}
