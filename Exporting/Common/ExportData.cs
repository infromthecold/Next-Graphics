using NextGraphics.Models;

using System;
using System.Drawing;

namespace NextGraphics.Exporting.Common
{
	public class ExportData
	{
		public static int MAX_BLOCKS = 256;
		public static int MAX_OBJECTS = 256;
		public static int MAX_IMAGES = 64;
		public static int MAX_CHARS = 256;

		public MainModel Model { get; }

		public ExportParameters Parameters { get; set; }

		#region Initialization & Disposal

		public ExportData(MainModel model, ExportParameters parameters)
		{
			Model = model;
			Parameters = parameters;
		}

		#endregion

		#region Data prebuilt before export

		public int[] SortIndexes = new int[MAX_CHARS + 1];

		public IndexedBitmap[] Blocks = new IndexedBitmap[MAX_BLOCKS];
		
		public SpriteInfo[] Sprites = new SpriteInfo[MAX_BLOCKS];
		
		public IndexedBitmap[] Chars = new IndexedBitmap[MAX_OBJECTS + 1];
		
		internal IndexedBitmap[] TempData = new IndexedBitmap[MAX_OBJECTS + 1];

		public Point ImageOffset { get; set; } = new Point();

		/// <summary>
		/// Convenience for quicker access to <see cref="MainModel.ObjectSize"/>. Eventually we should cleanup all usages to get the value from <see cref="MainModel"/> though.
		/// </summary>
		public int ObjectSize { get => Model.ObjectSize; }

		/// <summary>
		/// Number of bytes each block takes.
		/// </summary>
		public int BlockSize { get; set; } = 0;

		/// <summary>
		/// Number of generated characters.
		/// </summary>
		public int CharactersCount { get; set; } = 0;

		/// <summary>
		/// Number of generated blocks.
		/// </summary>
		public int BlocksCount { get; set; } = 0;

		/// <summary>
		/// Indicates whether remapping was completed or not.
		/// </summary>
		public bool IsRemapped { get; set; } = false;

		#endregion

		#region Data available during export

		public int BinarySize { get; set; } = 0;

		public int PixelFileSize { get; set; } = 0;

		#endregion

		#region Helpers

		public void Clear()
		{
			for (int b = 0; b < MAX_BLOCKS; b++)
			{
				if (Blocks[b] != null)
				{
					Blocks[b].Dispose();
					Blocks[b] = null;
				}

				if (Sprites[b] != null)
				{
					Sprites[b].Dispose();
					Sprites[b] = null;
				}
			}

			for (int c = 0; c < MAX_OBJECTS; c++)
			{
				if (TempData[c] != null)
				{
					TempData[c].Dispose();
					TempData[c] = null;
				}
			}

			BlocksCount = 0;
			CharactersCount = 0;
			IsRemapped = false;
		}

		#endregion
	}
}
