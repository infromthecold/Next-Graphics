using NextGraphics.Utils;

using System;
using System.IO;

namespace NextGraphics.Models
{
	/// <summary>
	/// Support for STMP .stm files. Supports flipped tiles (X and Y), but no rotations (well, maybe the format supports rotations, but Pro Motion NG creates a new tilemap in such case, so can't verify).
	/// 
	/// Requires tiles image from preferred bitmap editor exactly in the order the tiles will be exported from Next Graphics (ideally that image is attached to the project as source image).
	/// 
	/// Binary format, all values little endian
	/// 
	/// Offset	Size	Description
	/// 0		4		fixed 0x53544D50 (= ASCII "STMP")
	/// 4		2		width (number of columns)
	/// 6		2		height (number of rows)
	/// 8+		4		[width * height] tiles, each 4 bytes
	/// 
	/// Tile format:
	/// 
	/// Offset	Size	Description
	/// 0		2		tile index
	/// 2		1		if 1 flipped Y, otherwise no horizontal flip
	/// 3		1		if 1 flipped Y, otherwise no vertical flip
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
								var index = reader.ReadInt16();
								var flippedX = reader.ReadByte() == 1;
								var flippedY = reader.ReadByte() == 1;

								result.SetTile(x, y, new TilemapData.Tile(
									index,
									flippedX,
									flippedY,
									false));
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
