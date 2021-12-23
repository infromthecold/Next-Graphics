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
			var source = document != null ? document : ModelDummyDocument();
			result.Load(source);

			return result;
		}

		public static XmlDocument ModelTilesDocument()
		{
			var template = Properties.Resources.ProjectTiles;

			var result = new XmlDocument();
			result.LoadXml(template);
			return result;
		}

		public static XmlDocument ModelDummyDocument(string outputType = "blocks", string imageFormat = "0", string paletteMapping = "Custom")
		{
			var template = Properties.Resources.ProjectTemplated;

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
			var resource = comments == CommentType.Full ? Properties.Resources.ExportTilesAsm : Properties.Resources.ExportTilesNoCommentAsm;
			return String.Format(resource, time.ToString("F", CultureInfo.CurrentCulture));
		}

		public static string AssemblerTilesAndBinary(DateTime time, CommentType comments, bool withImages = false)
		{
			string resource = "";

			switch (comments)
			{
				case CommentType.Full:
					resource = withImages ? Properties.Resources.ExportTilesBinaryImagesAsm : Properties.Resources.ExportTilesBinaryAsm;
					break;

				case CommentType.None:
					resource = withImages ? Properties.Resources.ExportTilesBinaryImagesAsm : Properties.Resources.ExportTilesBinaryNoCommentAsm;
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
					resource = withImages ? Properties.Resources.ExportTilesBinaryBlocksImagesAsm : Properties.Resources.ExportTilesBinaryBlocksAsm;
					break;

				case CommentType.None:
					resource = withImages ? Properties.Resources.ExportTilesBinaryBlocksImagesAsm : Properties.Resources.ExportTilesBinaryBlocksNoCommentAsm;
					break;
			}

			return String.Format(resource, time.ToString("F", CultureInfo.CurrentCulture));
		}

		#endregion

		#region Binary

		public static byte[] BinaryTilesPal()
		{
			return Properties.Resources.ExportTilesBinaryPal;
		}

		public static byte[] BinaryTilesBin()
		{
			return Properties.Resources.ExportTilesBinaryBin;
		}

		public static byte[] BinaryTilesMap()
		{
			return Properties.Resources.ExportTilesBinaryMap;
		}

		public static byte[] BinaryTilesTil()
		{
			return Properties.Resources.ExportTilesBinaryTil;
		}

		public static byte[] TilesBlocksImage()
		{
			using (var stream = new MemoryStream())
			{
				Properties.Resources.ExportTiles_BlocksAsImage.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
				return stream.ToArray();
			}
		}

		public static byte[] TilesTilesImage()
		{
			using (var stream = new MemoryStream())
			{
				Properties.Resources.ExportTiles_TilesImage.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
				return stream.ToArray();
			}
		}

		#endregion

		#region Images

		public static Bitmap Level1Bitmap()
		{
			return new Bitmap(Properties.Resources.Level1Bitmap);
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
