using NextGraphics.Utils;

using System;
using System.IO;

namespace NextGraphics.Models
{
	/// <summary>
	/// Support for STMP .stm files. Doesn't support remapping tile indexes and tile attributes. Requires tiles image from preferred bitmap editor exactly in the order the tiles will be exported from Next Graphics (presumably that image is attached to the project).
	/// 
	/// Binary format, all values little endian
	/// 
	/// Offset	Size	Description
	/// 0		4		fixed 0x53544D50 (= ASCII "STMP")
	/// 4		2		width (number of columns)
	/// 6		2		height (number of rows)
	/// 8+		4		tile index (repeated width x height times)
	/// </summary>
	public class SourceTilemapStm : SourceTilemap
	{
		#region Initialization & Disposal

		public SourceTilemapStm(string filename) : base(filename)
		{
		}

		public SourceTilemapStm(string filename, TilemapData data) : base(filename, data)
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
						reader.Skip(4);	// STMP

						var width = reader.ReadInt16();
						var height = reader.ReadInt16();

						var result = new TilemapData((int)width, (int)height);

						for (int y = 0; y < height; y++)
						{
							for (int x = 0; x < width; x++)
							{
								var index = reader.ReadInt32();

								result.Tiles[y, x] = new TilemapData.Tile(index);
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
