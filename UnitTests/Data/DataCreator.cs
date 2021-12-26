using NextGraphics.Models;

using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Drawing;
using System.Reflection;
using System.Text.RegularExpressions;

namespace UnitTests.Data
{
	/// <summary>
	/// Creates commonly used data for unit tests.
	/// </summary>
	public class DataCreator
	{
		#region MainModel

		public static MainModel LoadModel(XmlDocument document = null)
		{
			var result = new MainModel();
			var source = document != null ? document : XmlDocumentTilesTemplated();
			result.Load(source);

			return result;
		}

		public static XmlDocument XmlDocumentTiles()
		{
			var template = Properties.Resources.Project_Tiles;

			var result = new XmlDocument();
			result.LoadXml(template);
			return result;
		}

		public static XmlDocument XmlDocumentTilesTemplated(string outputType = "blocks", string imageFormat = "0", string paletteMapping = "Custom")
		{
			var template = Properties.Resources.Project_Tiles_Templated;

			var xml = String.Format(template, outputType, imageFormat, paletteMapping);

			var result = new XmlDocument();
			result.LoadXml(xml);
			return result;
		}

		#endregion

		#region Assembler

		public static string AssemblerTiles(DateTime time, CommentType comments, bool withImages = false)
		{
			// Note: assembler file doesn't differ in this case if images are also exported.
			var resource = comments == CommentType.Full ? Properties.Resources.Export_Tiles_Asm : Properties.Resources.Export_Tiles_Asm_NoComments;
			return String.Format(resource, time.ToString("F", CultureInfo.CurrentCulture));
		}

		public static string AssemblerTilesAndBinary(DateTime time, CommentType comments, bool withImages = false)
		{
			string resource = "";

			switch (comments)
			{
				case CommentType.Full:
					resource = withImages ? Properties.Resources.Export_Tiles_Asm_Binary_Images : Properties.Resources.Export_Tiles_Asm_Binary;
					break;

				case CommentType.None:
					resource = withImages ? Properties.Resources.Export_Tiles_Asm_Binary_Images : Properties.Resources.Export_Tiles_Asm_Binary_NoComments;
					break;
			}
			
			return String.Format(resource, time.ToString("F", CultureInfo.CurrentCulture));
		}

		public static string AssemblerTilesAndBinaryAndBlocks(DateTime time, CommentType comments, bool withImages = false)
		{
			string resource = "";

			switch (comments)
			{
				case CommentType.Full:
					resource = withImages ? Properties.Resources.Export_Tiles_Asm_BinaryBlocks_Images : Properties.Resources.Export_Tiles_Asm_BinaryBlocks;
					break;

				case CommentType.None:
					resource = withImages ? Properties.Resources.Export_Tiles_Asm_BinaryBlocks_Images : Properties.Resources.Export_Tiles_Asm_BinaryBlocks_NoComments;
					break;
			}

			return String.Format(resource, time.ToString("F", CultureInfo.CurrentCulture));
		}

		#endregion

		#region Binary

		public static byte[] TilesPal()
		{
			return Properties.Resources.Export_Tiles_Pal;
		}

		public static byte[] TilesBin()
		{
			return Properties.Resources.Export_Tiles_Bin;
		}

		public static byte[] TilesMap()
		{
			return Properties.Resources.Export_Tiles_Map;
		}

		public static byte[] TilesTil()
		{
			return Properties.Resources.Export_Tiles_Til;
		}

		public static byte[] TilesImageBlocks()
		{
			using (var stream = new MemoryStream())
			{
				Properties.Resources.Export_Tiles_Image_Blocks.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
				return stream.ToArray();
			}
		}

		public static byte[] TilesImageTiles()
		{
			using (var stream = new MemoryStream())
			{
				Properties.Resources.Export_Tiles_Image_Tiles.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
				return stream.ToArray();
			}
		}

		#endregion

		#region Images

		public static Bitmap TilesImageLevel1()
		{
			return new Bitmap(Properties.Resources.Project_Tiles_Image1);
		}

		#endregion
	}

	public static class Extensions
	{
		public static List<string> ToLines(this string source)
		{
			var result = new List<string>();

			using (var reader = new StringReader(source))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					result.Add(line);
				}
			}

			return result;
		}

		public static List<string> ToLines(this Stream stream)
		{
			using (var reader = new StreamReader(stream))
			{
				return reader.ReadToEnd().ToLines();
			}
		}
	}
}
