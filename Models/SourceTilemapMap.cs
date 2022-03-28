using NextGraphics.Utils;

using System;
using System.IO;

namespace NextGraphics.Models
{
	/// <summary>
	/// Support for GBA .map files. Doesn't respect <see cref="MainModel"/> settings, but supports flipped tiles (X and Y), but no rotations (well, maybe the format supports rotations, but Pro Motion NG creates a new tilemap in such case, so can't verify).
	/// 
	/// Requires tiles image from preferred bitmap editor exactly in the order the tiles will be exported from Next Graphics (ideally that image is attached to the project as source image).
	/// 
	/// Binary format, all values little endian
	/// 
	/// Offset	Size	Description
	/// 0		4		width (number of columns)
	/// 4		4		height (number of rows)
	/// 8+		2		[width * height] tiles, each 2 bytes
	/// 
	/// Tile format
	/// Offset	Size	Description
	/// 0		1		tile index
	/// 1		1		tile attributes
	/// 
	/// Tile attributes (bits)
	/// 76543210
	///     ||
	///     |+---- 1 if flipped X
	///     +----- 1 if flipped Y
	/// 
	/// Possible combinations
	/// 76543210	HEX
	/// --------
	/// 00000000	00	regular tile
	/// 00000100	04	flipped X
	/// 00001000	08	flipped Y
	/// 00001100	0C	flipped X & Y
	/// </summary>
	public class SourceTilemapMap : SourceTilemap
	{
		#region Initialization & Disposal

		public SourceTilemapMap(string filename, MainModel model) : base(filename, model)
		{
		}

		public SourceTilemapMap(string filename, MainModel model, TilemapData data) : base(filename, model, data)
		{
		}

		#endregion

		#region Overrides

		protected override TilemapData OnLoadDataFromFile(string filename, MainModel model)
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
								var index = reader.ReadByte();

								var attributes = reader.ReadByte();
								var flippedX = (attributes & 0b00000100) > 0;
								var flippedY = (attributes & 0b00001000) > 0;

								result.SetTile(x, y, new TilemapData.Tile
								{
									Index = index,
									FlippedX = flippedX,
									FlippedY = flippedY
								});
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
