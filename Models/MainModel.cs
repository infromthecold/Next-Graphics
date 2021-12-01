using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NextGraphics.Models
{
	public class MainModel
	{
		public string Name { get; set; } = "";
		public List<string> Filenames { get; private set; } = new List<string>();
		public Palette Palette { get; } = new Palette();

		public OutputType OutputType { get; set; } = OutputType.Sprites;
		public ImageFormat ImageFormat { get; set; } = ImageFormat.BMP;

		public bool IgnoreCopies { get; set; } = false;
		public bool IgnoreMirroredX { get; set; } = false;
		public bool IgnoreMirroredY { get; set; } = false;
		public bool IgnoreRotated { get; set; } = false;
		public bool IgnoreTransparentPixels { get; set; } = false;

		public int CenterPosition { get; set; } = 4;
		public int GridXSize { get; set; } = 32;
		public int GridYSize { get; set; } = 32;
		public int BlocksAccross { get; set; } = 1;
		public int Accuracy { get; set; } = 100;

		public bool TransparentFirst { get; set; } = false;
		public bool FourBit { get; set; } = false;
		public bool Reduced { get; set; } = false;
		public bool TextFlips { get; set; } = false;
		public bool BinaryOutput { get; set; } = false;
		public bool BinaryBlocksOutput { get; set; } = false;

		public bool BlocksAsImage { get; set; } = false;
		public bool TilesAsImage { get; set; } = false;
		public bool TransparentBlocks { get; set; } = false;
		public bool TransparentTiles { get; set; } = false;

		public int OutputFilesFilterIndex { get; set; } = 0;
		public int AddImagesFilterIndex { get; set; } = 0;

		// This is not saved!
		public CommentType CommentType { get; set; } = CommentType.Full;

		#region Serialization

		/// <summary>
		/// Loads the data from the given <see cref="XmlDocument"/>. In case of failure, an exception will be thrown.
		/// </summary>
		public void Load(XmlDocument document)
		{
			// Project
			document.WithNode("//Project/Name", node =>
			{
				node.WithAttribute("Projectname", value => Name = value);
			});

			// Files
			document.WithNodes("//Project/File", nodes =>
			{
				Filenames = nodes.Cast<XmlNode>().Select(node => node.Attributes["Path"].Value).ToList();
			});

			// Settings
			document.WithNode("//Project/Settings", (node) =>
			{
				OutputType = node.Attributes["sprites"] != null ? OutputType.Sprites : OutputType.Blocks;

				node.WithAttribute("center", value => CenterPosition = int.Parse(value));
				node.WithAttribute("xSize", value => GridXSize = int.Parse(value));
				node.WithAttribute("ySize", value => GridYSize = int.Parse(value));
				node.WithAttribute("binary", value => BinaryOutput = bool.Parse(value));
				node.WithAttribute("binaryBlocks", value => BinaryBlocksOutput = bool.Parse(value));
				node.WithAttribute("MirrorX", value => IgnoreMirroredX = bool.Parse(value));
				node.WithAttribute("MirrorY", value => IgnoreMirroredY = bool.Parse(value));
				node.WithAttribute("Rotations", value => IgnoreRotated = bool.Parse(value));
				node.WithAttribute("Transparent", value => IgnoreTransparentPixels = bool.Parse(value));
				node.WithAttribute("xSize", value => GridXSize = int.Parse(value));
				node.WithAttribute("ySize", value => GridYSize = int.Parse(value));
				node.WithAttribute("Sort", value => TransparentFirst = bool.Parse(value));
				node.WithAttribute("fourBit", value => FourBit = bool.Parse(value));
				node.WithAttribute("binary", value => BinaryOutput = bool.Parse(value));
				node.WithAttribute("binaryBlocks", value => BinaryBlocksOutput = bool.Parse(value));
				node.WithAttribute("blocksImage", value => BlocksAsImage = bool.Parse(value));
				node.WithAttribute("tilesImage", value => TilesAsImage = bool.Parse(value));
				node.WithAttribute("transBlock", value => TransparentBlocks = bool.Parse(value));
				node.WithAttribute("transTile", value => TransparentTiles = bool.Parse(value));
				node.WithAttribute("across", value => BlocksAccross = int.Parse(value));
				node.WithAttribute("accurate", value => Accuracy = int.Parse(value));
				node.WithAttribute("format", value => ImageFormat = (ImageFormat)int.Parse(value));
				node.WithAttribute("textFlips", value => TextFlips = bool.Parse(value));
				node.WithAttribute("reduce", value => Reduced = bool.Parse(value));
			});

			// Palette
			document.WithNode("//Project/Palette", node =>
			{
				node.WithAttribute("Mapping", value =>
				{
					switch (value)
					{
						case "Next256": Palette.Type = PaletteType.Next256; break;
						case "Next512": Palette.Type = PaletteType.Next512; break;
						default: Palette.Type = PaletteType.Custom; break;
					}
				});

				node.WithAttribute("Transparent", value => Palette.TransparentIndex = int.Parse(value));
				node.WithAttribute("Used", value => Palette.UsedCount = int.Parse(value));

				int colourIndex = 0;
				while (document.WithNode($"//Project/Palette/Colour{colourIndex}", colourNode =>
				{
					if (colourIndex >= Palette.Colours.Count)
					{
						Palette.Colours.Add(new Palette.Colour(0, 0, 0));
					}

					colourNode.WithAttribute("Red", value => Palette[colourIndex].Red = byte.Parse(value));
					colourNode.WithAttribute("Green", value => Palette[colourIndex].Green = byte.Parse(value));
					colourNode.WithAttribute("Blue", value => Palette[colourIndex].Blue = byte.Parse(value));
					
					colourIndex++;
				}));
			});
		}

		/// <summary>
		/// Saves the data into a <see cref="XmlDocument"/> and returns it. It's up to caller to actually save the XML into a file. Optionally a closure can be assigned that will be called with the main project node - in case additional data needs to be saved.
		/// </summary>
		public XmlDocument Save(Action<XmlNode> projectNodeHandler = null)
		{
			var document = new XmlDocument();

			// Root node
			var rootNode = document.CreateElement("", "XML", "");
			document.AppendChild(rootNode);

			// Project root node
			var projectNode = rootNode.AddNode("Project");

			// Project name node
			var nameNode = projectNode.AddNode("Name");
			nameNode.AddAttribute("Projectname", Name);

			// Filenames
			foreach (string filename in Filenames)
			{
				var fileNode = projectNode.AddNode("File");
				fileNode.AddAttribute("Path", filename);
			}

			// Settings
			var settingsNode = projectNode.AddNode("Settings");
			switch (OutputType)
			{
				case OutputType.Sprites: settingsNode.AddAttribute("sprites", "true"); break;
				case OutputType.Blocks: settingsNode.AddAttribute("blocks", "true"); break;
			}
			settingsNode.AddAttribute("center", CenterPosition.ToString());
			settingsNode.AddAttribute("xSize", GridXSize.ToString());
			settingsNode.AddAttribute("ySize", GridYSize.ToString());
			settingsNode.AddAttribute("fourBit", FourBit);
			settingsNode.AddAttribute("binary", BinaryOutput);
			settingsNode.AddAttribute("binaryBlocks", BinaryBlocksOutput);
			settingsNode.AddAttribute("Repeats", IgnoreCopies);
			settingsNode.AddAttribute("MirrorX", IgnoreMirroredX);
			settingsNode.AddAttribute("MirrorY", IgnoreMirroredY);
			settingsNode.AddAttribute("Rotations", IgnoreRotated);
			settingsNode.AddAttribute("Transparent", IgnoreTransparentPixels);
			settingsNode.AddAttribute("Sort", TransparentFirst);
			settingsNode.AddAttribute("blocksImage", BlocksAsImage);
			settingsNode.AddAttribute("tilesImage", TilesAsImage);
			settingsNode.AddAttribute("transBlock", TransparentBlocks);
			settingsNode.AddAttribute("transTile", TransparentTiles);
			settingsNode.AddAttribute("across", BlocksAccross.ToString());
			settingsNode.AddAttribute("accurate", Accuracy.ToString());
			settingsNode.AddAttribute("format", ((int)ImageFormat).ToString());
			settingsNode.AddAttribute("textFlips", TextFlips);
			settingsNode.AddAttribute("reduce", Reduced);

			// Palette
			var paletteNode = projectNode.AddNode("Palette");
			paletteNode.AddAttribute("Mapping", Enum.GetName(typeof(PaletteType), Palette.Type));
			paletteNode.AddAttribute("Transparent", Palette.TransparentIndex.ToString());
			paletteNode.AddAttribute("Used", Palette.UsedCount.ToString());
			for (int i = 0; i < Palette.Colours.Count; i++)
			{
				var colourNode = paletteNode.AddNode($"Colour{i}");
				var colour = Palette.Colours[i];
				colourNode.AddAttribute("Red", colour.Red.ToString());
				colourNode.AddAttribute("Green", colour.Green.ToString());
				colourNode.AddAttribute("Blue", colour.Blue.ToString());
			}

			// Dialogs
			var dialogsNode = projectNode.AddNode("Dialogs");
			dialogsNode.AddAttribute("OutputIndex", OutputFilesFilterIndex.ToString());
			dialogsNode.AddAttribute("ImageIndex", AddImagesFilterIndex.ToString());

			// After all data is saved, pass project node to closure so additional data can be appended.
			if (projectNodeHandler != null)
			{
				projectNodeHandler(projectNode);
			}

			return document;
		}

		#endregion

		#region Helpers

		public void Clear()
		{
			Filenames.Clear();
		}

		#endregion
	}

	static class XmlExtensions
	{
		public static bool WithNodes(this XmlDocument document, string path, Action<XmlNodeList> handler)
		{
			var nodes = document.SelectNodes(path);
			if (nodes == null || nodes.Count == 0) return false;

			handler(nodes);
			return true;
		}

		public static bool WithNode(this XmlDocument document, string path, Action<XmlNode> handler)
		{
			var node = document.SelectSingleNode(path);
			if (node == null) return false;

			handler(node);
			return true;
		}

		public static bool WithAttribute(this XmlNode node, string name, Action<string> handler)
		{
			var attribute = node.Attributes[name];
			if (attribute == null) return false;

			handler(attribute.Value);
			return true;
		}

		public static XmlNode AddNode(this XmlNode node, string name)
		{
			XmlNode result = node.OwnerDocument.CreateElement(name);

			node.AppendChild(result);

			return result;
		}

		public static XmlAttribute AddAttribute(this XmlNode node, string name, string value)
		{
			XmlAttribute attr = node.OwnerDocument.CreateAttribute(name);
			attr.Value = value;

			node.Attributes.Append(attr);

			return attr;
		}

		public static XmlAttribute AddAttribute(this XmlNode node, string name, bool value)
		{
			return AddAttribute(node, name, value ? "true" : "false");
		}
	}
}
