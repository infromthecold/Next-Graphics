using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace NextGraphics.Models
{
	public class SourceImage : SourceFile<Bitmap>
	{
		#region Initialization & Disposal

		public SourceImage(string filename) : base(filename)
		{
		}

		public SourceImage(string filename, Bitmap bitmap) : base(filename, bitmap)
		{
		}

		#endregion

		#region Overrides

		protected override Bitmap OnLoadDataFromFile(string filename)
		{
			try
			{
				Bitmap result = null;

				using (var fs = new System.IO.FileStream(filename, System.IO.FileMode.Open))
				{
					var bmp = new Bitmap(fs);
					result = new Bitmap(bmp.Width, bmp.Height);
					result = (Bitmap)bmp.Clone();
				}

				return IsSupported(new Bitmap(result)) ? new Bitmap(result) : null;
			}
			catch
			{
				return null;
			}
		}

		#endregion

		#region Helpers

		public void CopyRegionIntoBlock(
			Palette palette,
			Rectangle srcRegion,
			bool reduce,
			ref IndexedBitmap outBlock, 
			ref SpriteInfo outInfo)
		{
			// clip because images may not be in blocks size 

			if (srcRegion.Y + srcRegion.Height > Data.Height)
			{
				srcRegion.Height = Data.Height - srcRegion.Y;
			}

			if (srcRegion.X + srcRegion.Width > Data.Width)
			{
				srcRegion.Width = Data.Width - srcRegion.X;
			}

			if (reduce)
			{
				// so we make the output block all transparent
				for (int y = 0; y < srcRegion.Height; y++)
				{
					for (int x = 0; x < srcRegion.Width; x++)
					{
						outBlock.SetPixel(x, y, (short)palette.TransparentIndex);
					}
				}
				for (int y = 0; y < srcRegion.Height; y++)
				{
					for (int x = 0; x < srcRegion.Width; x++)
					{

						if (palette.ClosestColor(Data.GetPixel(srcRegion.X + x, srcRegion.Y + y), -1, palette.StartIndex) != (short)palette.TransparentIndex)
						{
							outInfo.OffsetY = (short)y;
							goto checkLeft;
						}
					}
				}

			checkLeft:
				for (int x = 0; x < srcRegion.Width; x++)
				{
					for (int y = 0; y < srcRegion.Height; y++)
					{
						if (palette.ClosestColor(Data.GetPixel(srcRegion.X + x, srcRegion.Y + y), -1, palette.StartIndex) != (short)palette.TransparentIndex)
						{
							outInfo.OffsetX = (short)x;
							goto xYDone;
						}
					}
				}

			xYDone:
				for (int y = outInfo.OffsetY; y < srcRegion.Height; y++)
				{
					for (int x = outInfo.OffsetX; x < srcRegion.Width; x++)
					{
						outBlock.SetPixel(x - outInfo.OffsetX, y - outInfo.OffsetY, palette.ClosestColor(Data.GetPixel(srcRegion.X + x, srcRegion.Y + y), -1, palette.StartIndex));
					}
				}
			}
			else
			{
				outInfo.ClearOffset();
				for (int y = 0; y < srcRegion.Height; y++)
				{
					for (int x = 0; x < srcRegion.Width; x++)
					{
						outBlock.SetPixel(x, y, palette.ClosestColor(Data.GetPixel(srcRegion.X + x, srcRegion.Y + y), -1, palette.StartIndex));
					}
				}
			}
		}

		private static bool IsSupported(Bitmap bitmap)
		{
			switch (bitmap.PixelFormat)
			{
				case PixelFormat.Format16bppRgb555:
				case PixelFormat.Format16bppArgb1555:
				case PixelFormat.Format16bppRgb565:
				case PixelFormat.Format24bppRgb:
				case PixelFormat.Format32bppArgb:
				//case PixelFormat.Format32bppPArgb:						
				case PixelFormat.Format32bppRgb:
				case PixelFormat.Format8bppIndexed:
					return true;
			}
			return false;
		}

		#endregion
	}
}
