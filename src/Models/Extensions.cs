using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Models
{
	public static class ModelExtensions
	{
		/// <summary>
		/// Converts <see cref="ImageFormat"/> into corresponding file extension.
		/// </summary>
		public static string Extension(this ImageFormat format)
		{
			switch (format)
			{
				case ImageFormat.PNG: return "png";
				case ImageFormat.JPG: return "jpg";
				default: return "bmp";
			}
		}

		/// <summary>
		/// Converts a byte into a hex string. Result is padded so it's always 2 characters long and doesn't include prefix.
		/// </summary>
		public static string ToHexString(this byte num)
		{
			return num.ToString("x2");
		}

		/// <summary>
		/// Converts a byte into a binary string. Result is padded so it's always 8 characters long and doesn't include prefix.
		/// </summary>
		public static string ToBinaryString(this byte num)
		{
			string outString = "";
			int bits = 0x080;
			for (int bit = 0; bit < 8; bit++)
			{
				if ((num & bits) == bits)
				{
					outString += "1";
				}
				else
				{
					outString += "0";
				}
				bits >>= 1;
			}
			return outString;
		}
	}

	public static class ReaderExtensions
	{
		public static void Skip(this BinaryReader reader, int bytes)
		{
			for (int i = 0; i < bytes; i++)
			{
				reader.ReadByte();
			}
		}
	}
}
