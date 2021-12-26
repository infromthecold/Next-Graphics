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

		public static XmlDocument XmlDocumentSprites()
		{
			var template = Properties.Resources.Project_Sprites;

			var result = new XmlDocument();
			result.LoadXml(template);
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
			var resource = comments == CommentType.Full ? 
				Properties.Resources.Export_Tiles_Asm : 
				Properties.Resources.Export_Tiles_Asm_NoComments;
			return String.Format(resource, time.ToString("F", CultureInfo.CurrentCulture));
		}

		public static string AssemblerTilesAndBinary(DateTime time, CommentType comments, bool withImages = false)
		{
			string resource = "";

			switch (comments)
			{
				case CommentType.Full:
					resource = withImages ? 
						Properties.Resources.Export_Tiles_Asm_Binary_Images : 
						Properties.Resources.Export_Tiles_Asm_Binary;
					break;

				case CommentType.None:
					resource = withImages ? 
						Properties.Resources.Export_Tiles_Asm_Binary_Images : 
						Properties.Resources.Export_Tiles_Asm_Binary_NoComments;
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
					resource = withImages ? 
						Properties.Resources.Export_Tiles_Asm_BinaryBlocks_Images : 
						Properties.Resources.Export_Tiles_Asm_BinaryBlocks;
					break;

				case CommentType.None:
					resource = withImages ? 
						Properties.Resources.Export_Tiles_Asm_BinaryBlocks_Images : 
						Properties.Resources.Export_Tiles_Asm_BinaryBlocks_NoComments;
					break;
			}

			return String.Format(resource, time.ToString("F", CultureInfo.CurrentCulture));
		}

		public static string AssemblerSprites(DateTime time, CommentType comments, bool withImages = false)
		{
			// Note: assembler file doesn't differ in this case if images are also exported.
			var resource = comments == CommentType.Full ? 
				Properties.Resources.Export_Sprites_Asm : 
				Properties.Resources.Export_Sprites_Asm_NoComments;
			return String.Format(resource, time.ToString("F", CultureInfo.CurrentCulture));
		}

		public static string AssemblerSpritesAndBinary(DateTime time, CommentType comments, bool withImages = false)
		{
			string resource = "";

			switch (comments)
			{
				case CommentType.Full:
					resource = withImages ?
						Properties.Resources.Export_Sprites_Asm_Binary_Images :
						Properties.Resources.Export_Sprites_Asm_Binary;
					break;

				case CommentType.None:
					resource = withImages ?
						Properties.Resources.Export_Sprites_Asm_Binary_Images :
						Properties.Resources.Export_Sprites_Asm_Binary_NoComments;
					break;
			}

			return String.Format(resource, time.ToString("F", CultureInfo.CurrentCulture));
		}

		public static string AssemblerSpritesAndBinaryAndBlocks(DateTime time, CommentType comments, bool withImages = false)
		{
			string resource = "";

			switch (comments)
			{
				case CommentType.Full:
					resource = withImages ?
						Properties.Resources.Export_Sprites_Asm_BinaryBlocks_Images :
						Properties.Resources.Export_Sprites_Asm_BinaryBlocks;
					break;

				case CommentType.None:
					resource = withImages ?
						Properties.Resources.Export_Sprites_Asm_BinaryBlocks_Images :
						Properties.Resources.Export_Sprites_Asm_BinaryBlocks_NoComments;
					break;
			}

			return String.Format(resource, time.ToString("F", CultureInfo.CurrentCulture));
		}

		#endregion

		#region Binary Tiles

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

		#region Binary Sprites

		public static byte[] SpritesPal()
		{
			return Properties.Resources.Export_Sprites_Pal;
		}

		public static byte[] SpritesBin()
		{
			return Properties.Resources.Export_Sprites_Bin;
		}

		public static byte[] SpritesTil()
		{
			return Properties.Resources.Export_Sprites_Til;
		}

		public static byte[] SpritesImageTiles()
		{
			using (var stream = new MemoryStream())
			{
				Properties.Resources.Export_Sprites_Image_Tiles.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
				return stream.ToArray();
			}
		}

		public static byte[] SpritesImageBlock(int index)
		{
			Bitmap BitmapResource()
			{
				switch (index)
				{
					case 0: return Properties.Resources.Export_Sprites_Image_Block0;
					case 1: return Properties.Resources.Export_Sprites_Image_Block1;
					case 2: return Properties.Resources.Export_Sprites_Image_Block2;
					case 3: return Properties.Resources.Export_Sprites_Image_Block3;
					case 4: return Properties.Resources.Export_Sprites_Image_Block4;
					case 5: return Properties.Resources.Export_Sprites_Image_Block5;
					case 6: return Properties.Resources.Export_Sprites_Image_Block6;
					case 7: return Properties.Resources.Export_Sprites_Image_Block7;
					case 8: return Properties.Resources.Export_Sprites_Image_Block8;
					case 9: return Properties.Resources.Export_Sprites_Image_Block9;
					case 10: return Properties.Resources.Export_Sprites_Image_Block10;
					case 11: return Properties.Resources.Export_Sprites_Image_Block11;
					case 12:  return Properties.Resources.Export_Sprites_Image_Block12;
					case 13:  return Properties.Resources.Export_Sprites_Image_Block13;
					default: return null;
				}
			}

			var bitmap = BitmapResource();
			if (bitmap == null)
			{
				return new byte[0];
			}

			using (var stream = new MemoryStream())
			{
				bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
				return stream.ToArray();
			}
		}

		#endregion

		#region Images

		public static Bitmap ImageTiles1()
		{
			return new Bitmap(Properties.Resources.Project_Tiles_Image1);
		}

		public static Bitmap ImageSprites1()
		{
			return new Bitmap(Properties.Resources.Project_Sprites_Image1);
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
