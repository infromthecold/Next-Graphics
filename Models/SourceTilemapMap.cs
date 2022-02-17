using NextGraphics.Utils;

using System;
using System.IO;

namespace NextGraphics.Models
{
	/// <summary>
	/// Support for GBA .map files. Doesn't support remapping tile indexes and tile attributes. Requires tiles image from preferred bitmap editor exactly in the order the tiles will be exported from Next Graphics (presumably that image is attached to the project).
	/// 
	/// Binary format, all values little endian
	/// 
	/// Offset	Size	Description
	/// 0		4		width (number of columns)
	/// 4		4		height (number of rows)
	/// 8+		2		tile index (repeated width x height times)
	/// </summary>
	public class SourceTilemapMap : SourceTilemap
	{
		#region Initialization & Disposal

		public SourceTilemapMap(string filename) : base(filename)
		{
		}

		public SourceTilemapMap(string filename, TilemapData data) : base(filename, data)
		{
		}

		#endregion

		#region Overrides

		protected override TilemapData OnLoadDataFromFile(string filename)
		{
			try
			{
				using (var stream = new FileStream(filename, FileMode.Open))
				{
					using (var reader = new BinaryReader(stream))
					{
						var width = reader.ReadInt32();
						var height = reader.ReadInt32();

						var result = new TilemapData((int)width, (int)height);

						for (int y = 0; y < height; y++)
						{
							for (int x = 0; x < width; x++)
							{
								result.Tiles[y, x] = reader.ReadInt16();
							}
						}

						return result;
					}
				}
			}
			catch
			{
				return null;
			}
		}

		#endregion
	}
}
