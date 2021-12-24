using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Models
{
	public class SourceImage
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

		#endregion

		#region Helpers

		public void ReloadImage()
		{
			DisposeImage();

			Image = LoadBitmapFromFile(_filename);
		}

		public void DisposeImage()
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
