using NextGraphics.Models;

using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Drawing;
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

		#region Assembler Tiles

		public static string AssemblerTiles(
			DateTime time, 
			CommentType comments, 
			PaletteFormat paletteFormat,
			FourBitParsingMethod parsingMethod)
		{
			var results = new[]
			{
				new { comment = CommentType.Full, palette = PaletteFormat.Next8Bit, parsing = FourBitParsingMethod.Manual,				result = Properties.Resources.export_tiles_asm_fullcomments_8bit_manual },
				new { comment = CommentType.Full, palette = PaletteFormat.Next8Bit, parsing = FourBitParsingMethod.DetectPaletteBanks,	result = Properties.Resources.export_tiles_asm_fullcomments_8bit_detect },
				new { comment = CommentType.Full, palette = PaletteFormat.Next9Bit, parsing = FourBitParsingMethod.Manual,				result = Properties.Resources.export_tiles_asm_fullcomments_9bit_manual },
				new { comment = CommentType.Full, palette = PaletteFormat.Next9Bit, parsing = FourBitParsingMethod.DetectPaletteBanks,	result = Properties.Resources.export_tiles_asm_fullcomments_9bit_detect },
				new { comment = CommentType.None, palette = PaletteFormat.Next8Bit, parsing = FourBitParsingMethod.Manual,				result = Properties.Resources.export_tiles_asm_nocomments_8bit_manual },
				new { comment = CommentType.None, palette = PaletteFormat.Next8Bit, parsing = FourBitParsingMethod.DetectPaletteBanks,	result = Properties.Resources.export_tiles_asm_nocomments_8bit_detect },
				new { comment = CommentType.None, palette = PaletteFormat.Next9Bit, parsing = FourBitParsingMethod.Manual,				result = Properties.Resources.export_tiles_asm_nocomments_9bit_manual },
				new { comment = CommentType.None, palette = PaletteFormat.Next9Bit, parsing = FourBitParsingMethod.DetectPaletteBanks,	result = Properties.Resources.export_tiles_asm_nocomments_9bit_detect },
			};

			foreach (var option in results)
			{
				if (option.comment == comments && option.palette == paletteFormat && option.parsing == parsingMethod)
				{
					return FormattedAssembler(option.result, time);
				}
			}

			throw new ArgumentException($"No export export assembler associated with {comments} {paletteFormat} {parsingMethod}");
		}

		public static string AssemblerTilesBinary(
			DateTime time,
			CommentType comments,
			FourBitParsingMethod parsingMethod)
		{
			var results = new[]
			{
				new { comment = CommentType.Full, parsing = FourBitParsingMethod.Manual,				result = Properties.Resources.export_tiles_asmbinary_fullcomments_manual },
				new { comment = CommentType.Full, parsing = FourBitParsingMethod.DetectPaletteBanks,	result = Properties.Resources.export_tiles_asmbinary_fullcomments_detect },
				new { comment = CommentType.None, parsing = FourBitParsingMethod.Manual,				result = Properties.Resources.export_tiles_asmbinary_nocomments_manual },
				new { comment = CommentType.None, parsing = FourBitParsingMethod.DetectPaletteBanks,	result = Properties.Resources.export_tiles_asmbinary_nocomments_detect }
			};

			foreach (var option in results)
			{
				if (option.comment == comments && option.parsing == parsingMethod)
				{
					return FormattedAssembler(option.result, time);
				}
			}

			throw new ArgumentException($"No export tiles assembler+binary associated with {comments} {parsingMethod}");
		}

		public static string AssemblerTilesBinaryAttributes(
			DateTime time,
			CommentType comments,
			FourBitParsingMethod parsingMethod)
		{
			var results = new[]
			{
				new { comment = CommentType.Full, parsing = FourBitParsingMethod.Manual,				result = Properties.Resources.export_tiles_asmbinaryattrs_fullcomments_manual },
				new { comment = CommentType.Full, parsing = FourBitParsingMethod.DetectPaletteBanks,	result = Properties.Resources.export_tiles_asmbinaryattrs_fullcomments_detect },
				new { comment = CommentType.None, parsing = FourBitParsingMethod.Manual,				result = Properties.Resources.export_tiles_asmbinaryattrs_nocomments_manual },
				new { comment = CommentType.None, parsing = FourBitParsingMethod.DetectPaletteBanks,	result = Properties.Resources.export_tiles_asmbinaryattrs_nocomments_detect }
			};

			foreach (var option in results)
			{
				if (option.comment == comments && option.parsing == parsingMethod)
				{
					return FormattedAssembler(option.result, time);
				}
			}

			throw new ArgumentException($"No export tiles assembler+binary associated with {comments} {parsingMethod}");
		}

		#endregion

		#region Assembler Sprites

		public static string AssemblerSprites(
			DateTime time,
			CommentType comments,
			PaletteFormat paletteFormat)
		{
			var results = new[]
			{
				new { comment = CommentType.Full, palette = PaletteFormat.Next8Bit, result = Properties.Resources.export_sprites_asm_fullcomments_8bit },
				new { comment = CommentType.Full, palette = PaletteFormat.Next9Bit, result = Properties.Resources.export_sprites_asm_fullcomments_9bit },
				new { comment = CommentType.None, palette = PaletteFormat.Next8Bit, result = Properties.Resources.export_sprites_asm_nocomments_8bit },
				new { comment = CommentType.None, palette = PaletteFormat.Next9Bit, result = Properties.Resources.export_sprites_asm_nocomments_9bit }
			};

			foreach (var option in results)
			{
				if (option.comment == comments && option.palette == paletteFormat)
				{
					return FormattedAssembler(option.result, time);
				}
			}

			throw new ArgumentException($"No export export assembler associated with {comments} {paletteFormat}");
		}

		public static string AssemblerSpritesBinary(
			DateTime time,
			CommentType comments)
		{
			var results = new[]
			{
				new { comment = CommentType.Full, result = Properties.Resources.export_sprites_asmbinary_fullcomments },
				new { comment = CommentType.None, result = Properties.Resources.export_sprites_asmbinary_nocomments },
			};

			foreach (var option in results)
			{
				if (option.comment == comments)
				{
					return FormattedAssembler(option.result, time);
				}
			}

			throw new ArgumentException($"No export export assembler associated with {comments}");
		}

		public static string AssemblerSpritesBinaryAttributes(
			DateTime time,
			CommentType comments)
		{
			var results = new[]
			{
				new { comment = CommentType.Full, result = Properties.Resources.export_sprites_asmbinaryattrs_fullcomments },
				new { comment = CommentType.None, result = Properties.Resources.export_sprites_asmbinaryattrs_nocomments },
			};

			foreach (var option in results)
			{
				if (option.comment == comments)
				{
					return FormattedAssembler(option.result, time);
				}
			}

			throw new ArgumentException($"No export export assembler associated with {comments}");
		}

		public static string AssemblerSprites4Bit(
			DateTime time,
			PaletteFormat paletteFormat,
			FourBitParsingMethod parsingMethod)
		{
			var results = new[]
			{
				new { palette = PaletteFormat.Next8Bit, parsing = FourBitParsingMethod.Manual,				result = Properties.Resources.export_sprites_asm4bit_8bit_manual },
				new { palette = PaletteFormat.Next8Bit, parsing = FourBitParsingMethod.DetectPaletteBanks,	result = Properties.Resources.export_sprites_asm4bit_8bit_detect },
				new { palette = PaletteFormat.Next9Bit, parsing = FourBitParsingMethod.Manual,				result = Properties.Resources.export_sprites_asm4bit_9bit_manual },
				new { palette = PaletteFormat.Next9Bit, parsing = FourBitParsingMethod.DetectPaletteBanks,	result = Properties.Resources.export_sprites_asm4bit_9bit_detect },
			};

			foreach (var option in results)
			{
				if (option.palette == paletteFormat && option.parsing == parsingMethod)
				{
					return FormattedAssembler(option.result, time);
				}
			}

			throw new ArgumentException($"No export export assembler associated with {parsingMethod}");
		}

		#endregion

		#region Assembler Tilemap

		public static string AssemblerTilemaps(
			DateTime time,
			TilemapExportType tilemapExportType)
		{
			var results = new[]
			{
				new { type = TilemapExportType.AttributesIndexAsWord,		result = Properties.Resources.export_tilemaps_asm_words },
				new { type = TilemapExportType.AttributesIndexAsTwoBytes,	result = Properties.Resources.export_tilemaps_asm_bytes },
				new { type = TilemapExportType.IndexOnly,					result = Properties.Resources.export_tilemaps_asm_index }
			};

			foreach (var option in results)
			{
				if (option.type == tilemapExportType)
				{
					return FormattedAssembler(option.result, time);
				}
			}

			throw new ArgumentException($"No export export assembler associated with {tilemapExportType}");
		}

		public static string AssemblerTilemapsBinary(
			DateTime time,
			TilemapExportType tilemapExportType)
		{
			// Assembler-wise the output is exactly the same for words and two-bytes, the only difference with index-only is smaller tilemap size.
			var results = new[]
			{
				new { type = TilemapExportType.AttributesIndexAsWord,		result = Properties.Resources.export_tilemaps_asmbinary_word },
				new { type = TilemapExportType.AttributesIndexAsTwoBytes,	result = Properties.Resources.export_tilemaps_asmbinary_word },
				new { type = TilemapExportType.IndexOnly,					result = Properties.Resources.export_tilemaps_asmbinary_index }
			};

			foreach (var option in results)
			{
				if (option.type == tilemapExportType)
				{
					return FormattedAssembler(option.result, time);
				}
			}

			throw new ArgumentException($"No export export assembler associated with {tilemapExportType}");

		}

		#endregion

		#region Binary Tiles

		public static byte[] TilesPalette(PaletteFormat paletteFormat, FourBitParsingMethod parsingMethod)
		{
			var options = new[]
			{
				new { palette = PaletteFormat.Next8Bit, parsing = FourBitParsingMethod.Manual, result = Properties.Resources.export_tiles_palette_8bit_manual },
				new { palette = PaletteFormat.Next8Bit, parsing = FourBitParsingMethod.DetectPaletteBanks, result = Properties.Resources.export_tiles_palette_8bit_detect },
				new { palette = PaletteFormat.Next9Bit, parsing = FourBitParsingMethod.Manual, result = Properties.Resources.export_tiles_palette_9bit_manual },
				new { palette = PaletteFormat.Next9Bit, parsing = FourBitParsingMethod.DetectPaletteBanks, result = Properties.Resources.export_tiles_palette_9bit_detect }
			};

			foreach (var option in options)
			{
				if (option.palette == paletteFormat && option.parsing == parsingMethod)
				{
					return option.result;
				}
			}

			throw new ArgumentException($"No export tiles palette associated with {paletteFormat} {parsingMethod}");
		}

		public static byte[] TilesBinary(FourBitParsingMethod parsingMethod)
		{
			switch (parsingMethod)
			{
				case FourBitParsingMethod.DetectPaletteBanks:
					return Properties.Resources.export_tiles_binary_detect;
				default:
					return Properties.Resources.export_tiles_binary_manual;
			}
		}

		public static byte[] TilesAttributes(FourBitParsingMethod parsingMethod)
		{
			switch (parsingMethod)
			{
				case FourBitParsingMethod.DetectPaletteBanks:
					return Properties.Resources.export_tiles_attributes_detect;
				default:
					return Properties.Resources.export_tiles_attributes_manual;
			}
		}

		public static byte[] TilesInfo(FourBitParsingMethod parsingMethod)
		{
			switch (parsingMethod) {
				case FourBitParsingMethod.DetectPaletteBanks:
					return Properties.Resources.export_tiles_info_detect;
				default:
					return Properties.Resources.export_tiles_info_manual;
			}
		}

		public static byte[] TilesImage(FourBitParsingMethod parsingMethod)
		{
			using (var stream = new MemoryStream())
			{
				Bitmap bitmap;

				switch (parsingMethod)
				{
					case FourBitParsingMethod.DetectPaletteBanks:
						bitmap = Properties.Resources.export_tiles_image_detect;
						break;
					default:
						bitmap = Properties.Resources.export_tiles_image_manual;
						break;
				}

				bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);

				return stream.ToArray();
			}
		}

		public static string TilesInfoFilename()
		{
			var result = "";

			// We read the value from blk resource itself so tests will always pass, even if the data changes.
			var data = TilesInfo(FourBitParsingMethod.Manual);
			var stringLength = data[0];
			for (int i = 1; i <= stringLength; i++)
			{
				result += (char)data[i];
			}

			return result;
		}

		#endregion

		#region Binary Sprites

		public static byte[] SpritesPalette(PaletteFormat paletteFormat)
		{
			switch (paletteFormat)
			{
				case PaletteFormat.Next9Bit: return Properties.Resources.export_sprites_palette_9bit;
				default: return Properties.Resources.export_sprites_palette_8bit;
			}
	}

		public static byte[] SpritesBinary()
		{
			return Properties.Resources.export_sprites_binary;
		}

		public static byte[] SpritesAttributes()
		{
			return Properties.Resources.export_sprites_attributes;
		}

		public static byte[] SpritesImage()
		{
			using (var stream = new MemoryStream())
			{
				Properties.Resources.export_sprites_image.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
				return stream.ToArray();
			}
		}

		public static byte[] SpritesSpriteImage(int index)
		{
			Bitmap BitmapResource()
			{
				switch (index)
				{
					case 0: return Properties.Resources.export_sprites_image0;
					case 1: return Properties.Resources.export_sprites_image1;
					case 2: return Properties.Resources.export_sprites_image2;
					case 3: return Properties.Resources.export_sprites_image3;
					case 4: return Properties.Resources.export_sprites_image4;
					case 5: return Properties.Resources.export_sprites_image5;
					case 6: return Properties.Resources.export_sprites_image6;
					case 7: return Properties.Resources.export_sprites_image7;
					case 8: return Properties.Resources.export_sprites_image8;
					case 9: return Properties.Resources.export_sprites_image9;
					case 10: return Properties.Resources.export_sprites_image10;
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

		public static byte[] TilemapsPalette()
		{
			return Properties.Resources.export_tilemaps_palette;
		}
		
		public static byte[] TilemapsBinary(TilemapExportType tilemapExportType)
		{
			// Note: binary wise words and two-bytes are exactly the same, the difference is only when exporting as assembler (dw vs db).
			var results = new[]
			{
				new { type = TilemapExportType.AttributesIndexAsWord,       result = Properties.Resources.export_tilemaps_binary_word },
				new { type = TilemapExportType.AttributesIndexAsTwoBytes,   result = Properties.Resources.export_tilemaps_binary_word },
				new { type = TilemapExportType.IndexOnly,                   result = Properties.Resources.export_tilemaps_binary_index }
			};

			foreach (var option in results)
			{
				if (option.type == tilemapExportType)
				{
					return option.result;
				}
			}

			throw new ArgumentException($"No export export assembler associated with {tilemapExportType}");
		}

		#endregion

		#region Helpers

		private static string FormattedAssembler(string source, DateTime time)
		{
			// Sources may either include `{0}` placeholder, or full formatted time. This regex will match both and replace with formatted time.
			var formattedTime = time.ToString("F", CultureInfo.CurrentCulture);
			return Regex.Replace(source, "(Created on )(.*)( by the)", match => $"{match.Groups[1]}{formattedTime}{match.Groups[3]}");
		}

		#endregion

		#region Sources

		public static Bitmap ProjectImageTiles()
		{
			return new Bitmap(Properties.Resources.Project_Tiles_Image);
		}

		public static Bitmap ProjectImageSprites()
		{
			return new Bitmap(Properties.Resources.Project_Sprites_Image);
		}

		public static TilemapData ProjectTilemapData2x2()
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
					var flippedX = (attributes & 0b00000100) > 0;
					var flippedY = (attributes & 0b00001000) > 0;

					 result.Tiles[y, x] = new TilemapData.Tile(index, flippedX, flippedY, false);
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
