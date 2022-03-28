using NextGraphics.Utils;

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

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

				using (var fs = new FileStream(filename, FileMode.Open))
				{
					var bmp = new Bitmap(fs);
					result = new Bitmap(bmp.Width, bmp.Height);
					result = (Bitmap)bmp.Clone();
				}

				return IsSupported(result) ? result : null;
			}
			catch
			{
				return null;
			}
		}

		#endregion

		#region Helpers

		public void CopyRegionIntoBlock(
			MainModel model,
			Rectangle region,
			IndexedBitmap destBlock,
			SpriteInfo destSpriteInfo = null)
		{
			Data.CopyRegionIntoBlock(model, region, destBlock, destSpriteInfo);
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
				case PixelFormat.Format32bppRgb:
				case PixelFormat.Format8bppIndexed:
					return true;
			}
			return false;
		}

		#endregion
	}
}
