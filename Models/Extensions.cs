using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Models
{
	public static class ModelExtensions
	{
		public static string Extension(this ImageFormat format)
		{
			switch (format)
			{
				case ImageFormat.PNG: return "png";
				case ImageFormat.JPG: return "jpg";
				default: return "bmp";
			}
		}
	}
}
