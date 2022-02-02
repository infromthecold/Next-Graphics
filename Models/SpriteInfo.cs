using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Models
{
	/// <summary>
	/// A sprite/object definition
	/// </summary>
	public class SpriteInfo : IDisposable
	{
		public bool Disposed { get; private set; }
		public BlockInfo[] infos { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public int Top { get; set; }
		public int Left { get; set; }
		public int Right { get; set; }
		public int Bottom { get; set; }

		public bool Used { get; private set; }

		public short OffsetX { get; set; }
		public short OffsetY { get; set; }

		public int Size { get => Width * Height; }
		public Rectangle Frame { get => new Rectangle(Left, Top, Right - Left, Bottom - Top); }


		#region Initialization & disposal

		public SpriteInfo(int width, int height)
		{

			OffsetX = 0;
			OffsetY = 0;
			Width = width;
			Height = height;
			Used = true;
			infos = new BlockInfo[Size];
			for (int b = 0; b < Size; b++)
			{
				infos[b] = new BlockInfo();
			}
			Disposed = false;
		}

		~SpriteInfo()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (Disposed)
			{
				return;
			}
			Disposed = true;
		}

		#endregion

		#region Helpers

		public void SetData(
			int x, 
			int y, 
			bool repeated, 
			bool flippedX, 
			bool flippedY, 
			bool rotated, 
			bool transparent, 
			short originalId, 
			short paletteOffset, 
			bool hasTran)
		{
			int index = x + (y * Width);
			infos[index].Repeated = repeated;
			infos[index].FlippedX = flippedX;
			infos[index].FlippedY = flippedY;
			infos[index].Rotated = rotated;
			infos[index].Transparent = transparent;
			infos[index].PaletteOffset = paletteOffset;
			infos[index].HasTransparent = hasTran;
			if ((originalId & 1) == 0)
			{
				infos[index].SecondHalf = false;
			}
			else
			{
				infos[index].SecondHalf = true;
			}
			infos[index].OriginalID = originalId;
			infos[index].Position = new Point(x, y);
		}

		public bool GetRepeated(int x, int y)
		{
			int index = x + (y * (Width));
			return infos[index].Repeated;
		}

		public byte GetPaletteOffset(int x, int y)
		{
			int index = x + (y * (Width));
			return (byte)infos[index].PaletteOffset;
		}

		public bool GetFlippedX(int x, int y)
		{
			int index = x + (y * (Width));
			return infos[index].FlippedX;
		}

		public bool GetFlippedY(int x, int y)
		{
			int index = x + (y * (Width));
			return infos[index].FlippedY;
		}

		public bool GetRotated(int x, int y)
		{
			int index = x + (y * (Width));
			return infos[index].Rotated;
		}

		public Int16 GetId(int x, int y)
		{
			int index = x + (y * (Width));
			return infos[index].OriginalID;
		}

		public Int16 GetXPos(int x, int y)
		{
			int index = x + (y * (Width));
			return (Int16)infos[index].Position.X;
		}

		public Int16 GetYpos(int x, int y)
		{
			int index = x + (y * (Width));
			return (Int16)infos[index].Position.Y;
		}

		public bool GetTransparent(int x, int y)
		{
			int index = x + (y * (Width));
			return infos[index].Transparent;
		}

		public bool GetWhichHalf(int x, int y)
		{
			int index = x + (y * (Width));
			return infos[index].SecondHalf;
		}

		public int GetNonTransparentPixelsCount()
		{
			var result = Size;

			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					if (GetTransparent(x, y))
					{
						result--;
					}
				}
			}

			return result;
		}

		public void ClearOffset()
		{
			OffsetX = 0;
			OffsetY = 0;
		}

		#endregion
	}
}
