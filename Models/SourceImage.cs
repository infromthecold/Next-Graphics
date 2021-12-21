using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Models
{
	public class SourceImage : IDisposable
	{
		/// <summary>
		/// File name and full path.
		/// </summary>
		public string Filename {
			get => _filename;
			set {
				if (value == _filename) return;
				_filename = value;
				Image = LoadBitmapFromFile(_filename);
			}
		}
		private string _filename;

		/// <summary>
		/// The image on the path specified by <see cref="Filename"/>. Automatically updated as <see cref="Filename"/> changes. If loading fails, this is null.
		/// </summary>
		public Bitmap Image { get; private set; }

		/// <summary>
		/// A helper for checking if <see cref="Image"/> is valid (instead of writting Image != null...
		/// </summary>
		public bool IsImageValid { get => Image != null; }

		#region Initialization & Disposal

		public SourceImage(string filename)
		{
			// Assigning filename to property will trigger bitmap loading as well in the setter.
			Filename = filename;
		}

		~SourceImage()
		{
			DisposeImage();
		}

		public void Dispose()
		{
			DisposeImage();
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

			if (srcRegion.Y + srcRegion.Height > Image.Height)
			{
				srcRegion.Height = Image.Height - srcRegion.Y;
			}

			if (srcRegion.X + srcRegion.Width > Image.Width)
			{
				srcRegion.Width = Image.Width - srcRegion.X;
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

						if (palette.ClosestColor(Image.GetPixel(srcRegion.X + x, srcRegion.Y + y), -1, palette.StartIndex) != (short)palette.TransparentIndex)
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
						if (palette.ClosestColor(Image.GetPixel(srcRegion.X + x, srcRegion.Y + y), -1, palette.StartIndex) != (short)palette.TransparentIndex)
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
						outBlock.SetPixel(x - outInfo.OffsetX, y - outInfo.OffsetY, palette.ClosestColor(Image.GetPixel(srcRegion.X + x, srcRegion.Y + y), -1, palette.StartIndex));
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
						outBlock.SetPixel(x, y, palette.ClosestColor(Image.GetPixel(srcRegion.X + x, srcRegion.Y + y), -1, palette.StartIndex));
					}
				}
			}
		}

		public void ReloadImage()
		{
			DisposeImage();

			Image = LoadBitmapFromFile(_filename);
		}

		private void DisposeImage()
		{
			if (IsImageValid)
			{
				Image.Dispose();
				Image = null;
			}
		}

		private static Bitmap LoadBitmapFromFile(string filename)
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
