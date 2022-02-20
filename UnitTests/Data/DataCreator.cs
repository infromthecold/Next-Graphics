using NextGraphics.Models;

using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Drawing;

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

		public static XmlDocument XmlDocumentTilesTemplated(
			string outputType = "blocks", 
			string imageFormat = "0", 
			string paletteMapping = "Custom",
			string paletteFormat = "0")
		{
			var template = Properties.Resources.Project_Tiles_Templated;

			var xml = string.Format(template, outputType, imageFormat, paletteMapping, paletteFormat);

			var result = new XmlDocument();
			result.LoadXml(xml);
			return result;
		}

		#endregion

		#region Assembler

		public static string AssemblerTilemaps(
			DateTime time,
			TilemapExportType tilemapExportType,
			bool binary)
		{
			string resource;

			if (binary)
			{
				resource = Properties.Resources.Export_Tilemaps_Asm_Binary;
			}
			else
			{
				switch (tilemapExportType)
				{
					case TilemapExportType.AttributesIndexAsWord:
						resource = Properties.Resources.Export_Tilemaps_Asm;
						break;
					case TilemapExportType.AttributesIndexAsTwoBytes:
						resource = Properties.Resources.Export_Tilemaps_Asm_2_Bytes;
						break;
					default:
						resource = Properties.Resources.Export_Tilemaps_Asm_1_Byte;
						break;
				}
			}

			return string.Format(resource, time.ToString("F", CultureInfo.CurrentCulture));
		}

		public static string AssemblerTiles(
			DateTime time, 
			CommentType comments, 
			PaletteFormat paletteFormat = PaletteFormat.Next8Bit, 
			bool withImages = false)
		{
			// Note: at the moment there's no difference in output between images or no images option. Leaving the option in so unit tests are ready in case this will change in the future.
			string resource;

			switch (comments)
			{
				case CommentType.Full:
					switch (paletteFormat)
					{
						case PaletteFormat.Next9Bit:
							resource = Properties.Resources.Export_Tiles_Asm_PaletteNext9Bit;
							break;
						default:
							resource = Properties.Resources.Export_Tiles_Asm;
							break;
					}
					break;

				default:
					switch (paletteFormat)
					{
						case PaletteFormat.Next9Bit:
							resource = Properties.Resources.Export_Tiles_Asm_NoComments_PaletteNext9Bit;
							break;
						default:
							resource = Properties.Resources.Export_Tiles_Asm_NoComments;
							break;
					}
					break;
			}

			return string.Format(resource, time.ToString("F", CultureInfo.CurrentCulture));
		}

		public static string AssemblerTilesAndBinary(
			DateTime time, 
			CommentType comments, 
			PaletteFormat paletteFormat = PaletteFormat.Next8Bit, 
			bool withImages = false)
		{
			// Note: at the moment there's no difference in output between images or no images option. Leaving the option in so unit tests are ready in case this will change in the future.
			string resource = "";

			switch (comments)
			{
				case CommentType.Full:
					switch (paletteFormat)
					{
						case PaletteFormat.Next9Bit:
							resource = Properties.Resources.Export_Tiles_Asm_Binary_PaletteNext9Bit;
							break;
						default:
							resource = Properties.Resources.Export_Tiles_Asm_Binary;
							break;
					}
					break;

				case CommentType.None:
					switch (paletteFormat)
					{
						case PaletteFormat.Next9Bit:
							resource = Properties.Resources.Export_Tiles_Asm_Binary_NoComments_PaletteNext9Bit;
							break;
						default:
							resource = Properties.Resources.Export_Tiles_Asm_Binary_NoComments;
							break;
					}
					break;
			}
			
			return string.Format(resource, time.ToString("F", CultureInfo.CurrentCulture));
		}

		public static string AssemblerTilesAndBinaryAndBlocks(
			DateTime time, 
			CommentType comments, 
			PaletteFormat paletteFormat = PaletteFormat.Next8Bit, 
			bool withImages = false)
		{
			// Note: at the moment there's no difference in output between images or no images option. Leaving the option in so unit tests are ready in case this will change in the future.
			string resource = "";

			switch (comments)
			{
				case CommentType.Full:
					switch (paletteFormat)
					{
						case PaletteFormat.Next9Bit:
							resource = Properties.Resources.Export_Tiles_Asm_BinaryBlocks_PaletteNext9Bit;
							break;
						default:
							resource = Properties.Resources.Export_Tiles_Asm_BinaryBlocks;
							break;
					}
					break;

				case CommentType.None:
					switch (paletteFormat)
					{
						case PaletteFormat.Next9Bit:
							resource = Properties.Resources.Export_Tiles_Asm_BinaryBlocks_NoComments_PaletteNext9Bit;
							break;
						default:
							resource = Properties.Resources.Export_Tiles_Asm_BinaryBlocks_NoComments;
							break;
					}
					break;
			}

			return string.Format(resource, time.ToString("F", CultureInfo.CurrentCulture));
		}

		public static string AssemblerSprites(
			DateTime time, 
			CommentType comments, 
			PaletteFormat paletteFormat = PaletteFormat.Next8Bit, 
			bool withImages = false)
		{
			// Note: at the moment there's no difference in output between images or no images option. Leaving the option in so unit tests are ready in case this will change in the future.
			string resource;

			switch (comments)
			{
				case CommentType.Full:
					switch (paletteFormat)
					{
						case PaletteFormat.Next9Bit:
							resource = Properties.Resources.Export_Sprites_Asm_PaletteNext9Bit;
							break;
						default:
							resource = Properties.Resources.Export_Sprites_Asm;
							break;
					}
					break;

				default:
					switch (paletteFormat)
					{
						case PaletteFormat.Next9Bit:
							resource = Properties.Resources.Export_Sprites_Asm_NoComments_PaletteNext9Bit;
							break;
						default:
							resource = Properties.Resources.Export_Sprites_Asm_NoComments;
							break;
					}
					break;
			}

			return string.Format(resource, time.ToString("F", CultureInfo.CurrentCulture));
		}

		public static string AssemblerSpritesAndBinary(
			DateTime time, 
			CommentType comments, 
			PaletteFormat paletteFormat = PaletteFormat.Next8Bit,
			bool withImages = false)
		{
			// Note: at the moment there's no difference in output between images or no images option. Leaving the option in so unit tests are ready in case this will change in the future.
			string resource = "";

			switch (comments)
			{
				case CommentType.Full:
					switch (paletteFormat)
					{
						case PaletteFormat.Next9Bit:
							resource = Properties.Resources.Export_Sprites_Asm_Binary_PaletteNext9Bit;
							break;
						default:
							resource = Properties.Resources.Export_Sprites_Asm_Binary;
							break;
					}
					break;

				case CommentType.None:
					switch (paletteFormat)
					{
						case PaletteFormat.Next9Bit:
							resource = Properties.Resources.Export_Sprites_Asm_Binary_NoComments_PaletteNext9Bit;
							break;
						default:
							resource = Properties.Resources.Export_Sprites_Asm_Binary_NoComments;
							break;
					}
					break;
			}

			return string.Format(resource, time.ToString("F", CultureInfo.CurrentCulture));
		}

		public static string AssemblerSpritesAndBinaryAndBlocks(
			DateTime time, 
			CommentType comments, 
			PaletteFormat paletteFormat = PaletteFormat.Next8Bit, 
			bool withImages = false)
		{
			// Note: at the moment there's no difference in output between images or no images option. Leaving the option in so unit tests are ready in case this will change in the future.
			string resource = "";

			switch (comments)
			{
				case CommentType.Full:
					switch (paletteFormat)
					{
						case PaletteFormat.Next9Bit:
							resource = Properties.Resources.Export_Sprites_Asm_BinaryBlocks_PaletteNext9Bit;
							break;
						default:
							resource = Properties.Resources.Export_Sprites_Asm_BinaryBlocks;
							break;
					}
					break;

				case CommentType.None:
					switch (paletteFormat)
					{
						case PaletteFormat.Next9Bit:
							resource = Properties.Resources.Export_Sprites_Asm_BinaryBlocks_NoComments_PaletteNext9Bit;
							break;
						default:
							resource = Properties.Resources.Export_Sprites_Asm_BinaryBlocks_NoComments;
							break;
					}
					break;
			}

			return string.Format(resource, time.ToString("F", CultureInfo.CurrentCulture));
		}

		#endregion

		#region Binary Tiles

		public static byte[] TilesPal(PaletteFormat paletteFormat = PaletteFormat.Next8Bit)
		{
			switch (paletteFormat)
			{
				case PaletteFormat.Next9Bit: return Properties.Resources.Export_Tiles_Pal_PaletteNext9Bit;
				default: return Properties.Resources.Export_Tiles_Pal;
			}
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

		public static byte[] TilesBlk()
		{
			return Properties.Resources.Export_Tiles_Blk;
		}

		public static string TilesBlkFilename()
		{
			var result = "";

			// We read the value from blk resource itself so tests will always pass, even if the data changes.
			var data = TilesBlk();
			var stringLength = data[0];
			for (int i = 1; i <= stringLength; i++)
			{
				result += (char)data[i];
			}

			return result;
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

		public static byte[] SpritesPal(PaletteFormat paletteFormat = PaletteFormat.Next8Bit)
		{
			switch (paletteFormat)
			{
				case PaletteFormat.Next9Bit:  return Properties.Resources.Export_Sprites_Pal_PaletteNext9Bit;
				default: return Properties.Resources.Export_Sprites_Pal;
			}
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

		#region Binary Tilemaps

		public static byte[] TilemapGeneratedData2x2()
		{
			return Properties.Resources.Export_Tilemaps_Tilemap0;
		}

		#endregion

		#region Sources

		public static Bitmap ImageTiles1()
		{
			return new Bitmap(Properties.Resources.Project_Tiles_Image1);
		}

		public static Bitmap ImageSprites1()
		{
			return new Bitmap(Properties.Resources.Project_Sprites_Image1);
		}

		public static TilemapData TilemapData2x2()
		{
			var width = 2;
			var height = 2;
			
			var result = new TilemapData(width, height);
			var data = Properties.Resources.Project_Tilemap_2x2;

			var i = 8;
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					var index = data[i++];
					
					var attributes = data[i++];
					var flippedX = (attributes & (1 << 3)) > 0;
					var flippedY = (attributes & (1 << 2)) > 0;
					var rotated = (attributes & (1 << 1)) > 0;

					result.Tiles[y, x] = new TilemapData.Tile(index, flippedX, flippedY, rotated);
				}
			}

			return result;
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
