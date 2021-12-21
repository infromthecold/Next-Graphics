using NextGraphics.Models;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Exporting.Common
{
	public class ExportData
	{
		public static int MAX_BLOCKS = 256;
		public static int MAX_OBJECTS = 256;
		public static int MAX_IMAGES = 64;
		public static int MAX_CHARS = 256;

		public ExportData(MainModel model)
		{
			Model = model;
		}

		public MainModel Model { get; }

		#region Data prebuilt before export

		public int[] SortIndexes = new int[MAX_CHARS + 1];
		public IndexedBitmap[] Blocks = new IndexedBitmap[MAX_BLOCKS];
		public SpriteInfo[] Sprites = new SpriteInfo[MAX_BLOCKS];
		public IndexedBitmap[] Chars = new IndexedBitmap[MAX_OBJECTS + 1];
		internal IndexedBitmap[] TempData = new IndexedBitmap[MAX_OBJECTS + 1];

		#endregion

		#region Data available during export

		public ExportParameters Parameters { get; set; }

		public string LabelName { get; set; } = "";
		public string BlockLabelName { get; set; } = "";

		public Point ImageOffset { get; set; } = new Point();

		public int BlockSize { get; set; } = 0;
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
		}

		#endregion
	}
}
