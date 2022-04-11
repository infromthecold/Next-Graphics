using NextGraphics.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Exporting.Common
{
	public static class ModelExtensions
	{
		public static System.Drawing.Imaging.ImageFormat ToSystemImageFormat(this ImageFormat format)
		{
			switch (format)
			{
				case ImageFormat.BMP: return System.Drawing.Imaging.ImageFormat.Bmp;
				case ImageFormat.PNG: return System.Drawing.Imaging.ImageFormat.Png;
				case ImageFormat.JPG: return System.Drawing.Imaging.ImageFormat.Jpeg;
			}

			throw new ArgumentException($"Unsupported image format {format}");
		}
	}
}
