using NextGraphics.Models.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Xml;

namespace NextGraphics.Models
{
	public class MainModel
	{
		public string Filename { get; set; } = "";
		public string Name { get; set; } = "";
		public Palette Palette { get; } = new Palette();
		public List<ISourceFile> Sources { get; private set; } = new List<ISourceFile>();

		public CommentType CommentType { get; set; } = CommentType.Full;
		public ImageFormat ImageFormat { get; set; } = ImageFormat.BMP;
		public PaletteFormat PaletteFormat { get; set; } = PaletteFormat.Next8Bit;
		public TilemapExportType TilemapExportType { get; set; } = TilemapExportType.AttributesIndexAsWord;

		public bool BinaryOutput { get; set; } = false;
		public bool TilesExportAsImage { get; set; } = false;

		public bool SpritesAttributesAsText { get; set; } = false;
		public bool SpritesExportAsImages { get; set; } = false;
		public bool SpritesExportAsImageTransparent { get; set; } = false;

		public int OutputFilesFilterIndex { get; set; } = 0;
		public int AddImagesFilterIndex { get; set; } = 0;
		public int AddTilemapsFilterIndex { get; set; } = 0;

		public string ExportAssemblerFileExtension { get; set; } = "asm";
		public string ExportBinaryDataFileExtension { get; set; } = "bin";
		public string ExportBinaryPaletteFileExtension { get; set; } = "pal";
		public string ExportBinaryTilesInfoFileExtension { get; set; } = "blk";
		public string ExportBinaryTileAttributesFileExtension { get; set; } = "map";
		public string ExportBinaryTilemapFileExtension { get; set; } = "tilemap";
		public string ExportSpriteAttributesFileExtension { get; set; } = "til";

		#region Not saved properties

		public Bitmap BlocksBitmap { get; private set; } = null;
		public Bitmap CharsBitmap { get; private set; } = null;

		#endregion

		#region Properties raising events

		public OutputType OutputType
		{
			get => _outputType;
			set => RaiseRemapRequired(value, ref _outputType, constrainedValue =>
			{
				if (OutputTypeChanged != null)
				{
					OutputTypeChanged(this, new OutputTypeChangedEventArgs(constrainedValue));
				}

				GridWidth = ConstrainItemWidth(GridWidth);
				GridHeight = ConstrainItemHeight(GridHeight);
			});
		}
		private OutputType _outputType = OutputType.Sprites;

		public int GridWidth
		{
			get => _gridWidth;
			set => RaiseRemapRequired(ConstrainItemWidth(value), ref _gridWidth, constrainedValue =>
			{
				if (GridWidthChanged != null)
				{
					GridWidthChanged(this, new SizeChangedEventArgs(constrainedValue));
				}

				if (BlocksAcrossWidthProvider != null && constrainedValue > 0)
				{
					var width = BlocksAcrossWidthProvider();
					BlocksAcross = (int)Math.Floor((float)width / (float)constrainedValue);
				}
				else
				{
					BlocksAcross = 0;
				}
			});
		}
		private int _gridWidth = 32;

		public int GridHeight
		{
			get => _gridHeight;
			set => RaiseRemapRequired(ConstrainItemHeight(value), ref _gridHeight, actualValue =>
			{
				if (GridHeightChanged != null)
				{
					GridHeightChanged(this, new SizeChangedEventArgs(actualValue));
				}
			});
		}
		private int _gridHeight = 3232;

		public int BlocksAcross
		{
			get => _blocksAcross;
			set => RaiseRemapRequired(value > 0 ? value : 1, ref _blocksAcross, actualValue =>
			{
				// Note: we don't allow value of 0 to be written. It happens when grid width/height are large enough and will mess up exporter.
				if (BlocksAcrossChanged != null)
				{
					BlocksAcrossChanged(this, new SizeChangedEventArgs(actualValue));
				}
			});
		}
		private int _blocksAcross = 1;

		public bool IgnoreCopies
		{
			get => _ignoreCopies;
			set => RaiseRemapRequired(value, ref _ignoreCopies);
		}
		private bool _ignoreCopies = false;

		public bool IgnoreMirroredX
		{
			get => _ignoreMirroredX;
			set => RaiseRemapRequired(value, ref _ignoreMirroredX);
		}
		private bool _ignoreMirroredX = false;

		public bool IgnoreMirroredY
		{
			get => _ignoreMirroredY;
			set => RaiseRemapRequired(value, ref _ignoreMirroredY);
		}
		private bool _ignoreMirroredY = false;

		public bool IgnoreRotated
		{
			get => _ignoreRotated;
			set => RaiseRemapRequired(value, ref _ignoreRotated);
		}
		private bool _ignoreRotated = false;

		public bool IgnoreTransparentPixels
		{
			get => _ignoreTransparentPixels;
			set => RaiseRemapRequired<bool>(value, ref _ignoreTransparentPixels);
		}
		private bool _ignoreTransparentPixels = false;

		public int Accuracy
		{
			get => _accuracy;
			set => RaiseRemapRequired(value, ref _accuracy);
		}
		private int _accuracy = 100;

		public int CenterPosition
		{
			get => _centerPosition;
			set => RaiseRemapRequired(value, ref _centerPosition);
		}
		private int _centerPosition = 4;

		public bool TransparentFirst
		{
			get => _transparentFirst;
			set => RaiseRemapRequired(value, ref _transparentFirst);
		}
		private bool _transparentFirst = false;

		public bool SpritesFourBit
		{
			get => _spritesFourBit;
			set => RaiseRemapRequired(value, ref _spritesFourBit);
		}
		private bool _spritesFourBit = false;

		public bool SpritesReduced
		{
			get => _spritesReduced;
			set => RaiseRemapRequired(value, ref _spritesReduced);
		}
		private bool _spritesReduced = false;

		public bool BinaryFramesAttributesOutput
		{
			get => _binaryFramesAttributesOutput;
			set => RaiseRemapRequired<bool>(value, ref _binaryFramesAttributesOutput);
		}
		private bool _binaryFramesAttributesOutput = false;

		public bool TilesExportAsImageTransparent
		{
			get => _tilesExportAsImageTransparent;
			set => RaiseRemapRequired(value, ref _tilesExportAsImageTransparent);
		}
		private bool _tilesExportAsImageTransparent = false;

		#endregion

		#region Events

		/// <summary>
		/// This is not event per-se, but it is a callback for just a single object that's responsible for providing number of blocks across value. It's called when relevant data changes. Implementor is expected to return the width of the bitmap into which blocks will be layed. The model will update <see cref="BlocksAcross"/> property as result and raise events as needed.
		/// </summary>
		public Func<int> BlocksAcrossWidthProvider { get; set; } = null;

		/// <summary>
		/// Raised when <see cref="OutputType"/> changes.
		/// </summary>
		public event EventHandler<OutputTypeChangedEventArgs> OutputTypeChanged;

		/// <summary>
		/// Raised when <see cref="GridWidth"/> value changes.
		/// </summary>
		public event EventHandler<SizeChangedEventArgs> GridWidthChanged;

		/// <summary>
		/// Raised when <see cref="GridHeight"/> value changes.
		/// </summary>
		public event EventHandler<SizeChangedEventArgs> GridHeightChanged;

		/// <summary>
		/// Raised when <see cref="BlocksAcross"/> value changes.
		/// </summary>
		public event EventHandler<SizeChangedEventArgs> BlocksAcrossChanged;

		/// <summary>
		/// Raised when any property that requires remap is changed. Note: this event may be raised very frequently, multiple times in succession (for example when loading data), so don't perform resource intensive operations in event handlers!
		/// </summary>
		public event EventHandler RemapRequired;

		#endregion

		#region Initialization & Disposal

		public MainModel()
		{
			CreateBitmaps();

			// The idea with loading the values from settings is to allow across projects changes. These are the types of values that user is likely to change in every project, so we make it simpler. User can still change on per-project basis (though last change will be persisted in settings). Loading from settings works on the fact that settings keys are exactly the same as our properties.
			ExportAssemblerFileExtension = Properties.Settings.Default[nameof(ExportAssemblerFileExtension)].ToString();
			ExportBinaryDataFileExtension = Properties.Settings.Default[nameof(ExportBinaryDataFileExtension)].ToString();
			ExportBinaryPaletteFileExtension = Properties.Settings.Default[nameof(ExportBinaryPaletteFileExtension)].ToString();
			ExportBinaryTilesInfoFileExtension = Properties.Settings.Default[nameof(ExportBinaryTilesInfoFileExtension)].ToString();
			ExportBinaryTileAttributesFileExtension = Properties.Settings.Default[nameof(ExportBinaryTileAttributesFileExtension)].ToString();
			ExportBinaryTilemapFileExtension = Properties.Settings.Default[nameof(ExportBinaryTilemapFileExtension)].ToString();
			ExportSpriteAttributesFileExtension = Properties.Settings.Default[nameof(ExportSpriteAttributesFileExtension)].ToString();
		}

		#endregion

		#region Serialization

		/// <summary>
		/// Loads the data from the given filename. This automatically assigns <see cref="Filename"/> as well.
		/// </summary>
		public void Load(string filename)
		{
			// Load and parse the XML.
			XmlDocument document = new XmlDocument();
			document.Load(filename);

			// Load the data.
			Load(document);

			// If all is fine, assign the filename.
			Filename = filename;
		}

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
				Sources = new List<ISourceFile>();
				foreach (XmlNode node in nodes)
				{
					var path = node.Attributes["Path"].Value;
					var type = node.Attribute("Type", "");

					switch (type)
					{
						case "Tilemap":
							AddSource(SourceTilemap.Create(path));
							break;
						default:
							AddSource(new SourceImage(path));
							break;
					}
				}
			});

			// Settings
			document.WithNode("//Project/Settings", (node) =>
			{
				OutputType = node.Attributes["sprites"] != null ? OutputType.Sprites : OutputType.Tiles;

				node.WithAttribute("comments", value => CommentType = (CommentType)int.Parse(value));
				node.WithAttribute("center", value => CenterPosition = int.Parse(value));
				node.WithAttribute("xSize", value => GridWidth = int.Parse(value));
				node.WithAttribute("ySize", value => GridHeight = int.Parse(value));
				node.WithAttribute("binary", value => BinaryOutput = bool.Parse(value));
				node.WithAttribute("binaryBlocks", value => BinaryFramesAttributesOutput = bool.Parse(value));
				node.WithAttribute("MirrorX", value => IgnoreMirroredX = bool.Parse(value));
				node.WithAttribute("MirrorY", value => IgnoreMirroredY = bool.Parse(value));
				node.WithAttribute("Rotations", value => IgnoreRotated = bool.Parse(value));
				node.WithAttribute("Transparent", value => IgnoreTransparentPixels = bool.Parse(value));
				node.WithAttribute("xSize", value => GridWidth = int.Parse(value));
				node.WithAttribute("ySize", value => GridHeight = int.Parse(value));
				node.WithAttribute("Sort", value => TransparentFirst = bool.Parse(value));
				node.WithAttribute("fourBit", value => SpritesFourBit = bool.Parse(value));
				node.WithAttribute("binary", value => BinaryOutput = bool.Parse(value));
				node.WithAttribute("binaryBlocks", value => BinaryFramesAttributesOutput = bool.Parse(value));
				node.WithAttribute("blocksImage", value => TilesExportAsImage = bool.Parse(value));
				node.WithAttribute("tilesImage", value => SpritesExportAsImages = bool.Parse(value));
				node.WithAttribute("transBlock", value => TilesExportAsImageTransparent = bool.Parse(value));
				node.WithAttribute("transTile", value => SpritesExportAsImageTransparent = bool.Parse(value));
				node.WithAttribute("across", value => BlocksAcross = int.Parse(value));
				node.WithAttribute("accurate", value => Accuracy = int.Parse(value));
				node.WithAttribute("format", value => ImageFormat = (ImageFormat)int.Parse(value));
				node.WithAttribute("PaletteFormat", value => PaletteFormat = (PaletteFormat)int.Parse(value));
				node.WithAttribute("TilemapExport", value => TilemapExportType = (TilemapExportType)int.Parse(value));
				node.WithAttribute("textFlips", value => SpritesAttributesAsText = bool.Parse(value));
				node.WithAttribute("reduce", value => SpritesReduced = bool.Parse(value));
			});

			// Export extensions

			document.WithNode("//Project/ExportExtensions", node =>
			{
				node.WithAttribute("Assembler", value => { ExportAssemblerFileExtension = value; });
				node.WithAttribute("Palette", value => { ExportBinaryPaletteFileExtension = value; });
				node.WithAttribute("Data", value => { ExportBinaryDataFileExtension = value; });
				node.WithAttribute("TilesInfo", value => { ExportBinaryTilesInfoFileExtension = value; });
				node.WithAttribute("TileAttributes", value => { ExportBinaryTileAttributesFileExtension = value; });
				node.WithAttribute("Tilemap", value => { ExportBinaryTilemapFileExtension = value; });
				node.WithAttribute("SpriteAttributes", value => { ExportSpriteAttributesFileExtension = value; });
			});

			// Dialogs
			document.WithNode("//Project/Dialogs", node =>
			{
				node.WithAttribute("OutputIndex", value => { OutputFilesFilterIndex = int.Parse(value); });
				node.WithAttribute("ImageIndex", value => { AddImagesFilterIndex = int.Parse(value); });
				node.WithAttribute("TilemapIndex", value => { AddTilemapsFilterIndex = int.Parse(value); });
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
			foreach (var source in Sources)
			{
				var fileNode = projectNode.AddNode("File");
				fileNode.AddAttribute("Path", source.Filename);

				if (source is SourceTilemap)
				{
					fileNode.AddAttribute("Type", "Tilemap");
				}
			}

			// Settings
			var settingsNode = projectNode.AddNode("Settings");
			switch (OutputType)
			{
				case OutputType.Sprites: settingsNode.AddAttribute("sprites", "true"); break;
				case OutputType.Tiles: settingsNode.AddAttribute("blocks", "true"); break;
			}
			settingsNode.AddAttribute("comments", (int)CommentType);
			settingsNode.AddAttribute("center", CenterPosition);
			settingsNode.AddAttribute("xSize", GridWidth);
			settingsNode.AddAttribute("ySize", GridHeight);
			settingsNode.AddAttribute("fourBit", SpritesFourBit);
			settingsNode.AddAttribute("binary", BinaryOutput);
			settingsNode.AddAttribute("binaryBlocks", BinaryFramesAttributesOutput);
			settingsNode.AddAttribute("Repeats", IgnoreCopies);
			settingsNode.AddAttribute("MirrorX", IgnoreMirroredX);
			settingsNode.AddAttribute("MirrorY", IgnoreMirroredY);
			settingsNode.AddAttribute("Rotations", IgnoreRotated);
			settingsNode.AddAttribute("Transparent", IgnoreTransparentPixels);
			settingsNode.AddAttribute("Sort", TransparentFirst);
			settingsNode.AddAttribute("blocksImage", TilesExportAsImage);
			settingsNode.AddAttribute("tilesImage", SpritesExportAsImages);
			settingsNode.AddAttribute("transBlock", TilesExportAsImageTransparent);
			settingsNode.AddAttribute("transTile", SpritesExportAsImageTransparent);
			settingsNode.AddAttribute("across", BlocksAcross.ToString());
			settingsNode.AddAttribute("accurate", Accuracy.ToString());
			settingsNode.AddAttribute("format", (int)ImageFormat);
			settingsNode.AddAttribute("PaletteFormat", (int)PaletteFormat);
			settingsNode.AddAttribute("TilemapExport", (int)TilemapExportType);
			settingsNode.AddAttribute("textFlips", SpritesAttributesAsText);
			settingsNode.AddAttribute("reduce", SpritesReduced);

			// Export extensions
			var exportExtensionsNode = projectNode.AddNode("ExportExtensions");
			exportExtensionsNode.AddAttribute("Assembler", ExportAssemblerFileExtension);
			exportExtensionsNode.AddAttribute("Palette", ExportBinaryPaletteFileExtension);
			exportExtensionsNode.AddAttribute("Data", ExportBinaryDataFileExtension);
			exportExtensionsNode.AddAttribute("TilesInfo", ExportBinaryTilesInfoFileExtension);
			exportExtensionsNode.AddAttribute("TileAttributes", ExportBinaryTileAttributesFileExtension);
			exportExtensionsNode.AddAttribute("Tilemap", ExportBinaryTilemapFileExtension);
			exportExtensionsNode.AddAttribute("SpriteAttributes", ExportSpriteAttributesFileExtension);

			// Dialogs
			var dialogsNode = projectNode.AddNode("Dialogs");
			dialogsNode.AddAttribute("OutputIndex", OutputFilesFilterIndex.ToString());
			dialogsNode.AddAttribute("ImageIndex", AddImagesFilterIndex.ToString());
			dialogsNode.AddAttribute("TilemapIndex", AddTilemapsFilterIndex.ToString());

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

			// After all data is saved, pass project node to closure so additional data can be appended.
			if (projectNodeHandler != null)
			{
				projectNodeHandler(projectNode);
			}

			return document;
		}

		#endregion

		#region Data handling

		/// <summary>
		/// Simpler variant for adding an image to the <see cref="Sources"/> list.
		/// </summary>
		public void AddSource(ISourceFile source)
		{
			Sources.Add(source);
			RaiseRemapRequired();
		}

		/// <summary>
		/// Simpler variant for adding an image to the <see cref="Sources"/> list. Note that while this will attempt to load the bitmap as well, loading may fail in which case the <see cref="SourceImage.Data"/> will be null!
		/// </summary>
		public void AddSource(string filename)
		{
			AddSource(new SourceImage(filename));
		}

		/// <summary>
		/// Removes the image by its filename. Note: it's preferred to use this function than removing images directly from the list as this also takes care of disposing underlying bitmaps.
		/// </summary>
		public void RemoveSource(string filename) {
			int index = Sources.FindIndex(source => source.Filename == filename);
			if (index < 0) return;
			
			var item = Sources[index];
			item.Dispose();

			Sources.RemoveAt(index);
			RaiseRemapRequired();
		}

		/// <summary>
		/// Disposes all allocated resources and clears the data.
		/// </summary>
		public void Clear()
		{
			Sources.ForEach(source =>
			{
				source.Dispose();
			});

			Sources.Clear();
			Palette.Clear();

			CreateBitmaps();
			RaiseRemapRequired();
		}

		#endregion

		#region Data enquiry

		/// <summary>
		/// Returns enumerable of all <see cref="Sources"/> which represent an image with either tiles or sprites.
		/// </summary>
		public IEnumerable<SourceImage> SourceImages()
		{
			return Sources.Where(source => source is SourceImage).Select(source => source as SourceImage);
		}

		/// <summary>
		/// Returns enumerable of all <see cref="Sources"/> which represent a tilemap.
		/// </summary>
		public IEnumerable<SourceTilemap> SourceTilemaps()
		{
			return Sources.Where(source => source is SourceTilemap).Select(source => source as SourceTilemap);
		}

		/// <summary>
		/// Enumerates <see cref="Sources"/> and calls given closure for each source that represents an image.
		/// </summary>
		public void ForEachSourceImage(Action<SourceImage, int> imageHandler)
		{
			for (int i = 0; i < Sources.Count; i++) {
				var source = Sources[i];

				if (source is SourceImage image)
				{
					imageHandler(image, i);
				}
			}
		}

		/// <summary>
		/// Enumerates <see cref="Sources"/> and calls given closure for each source that represents a tilemap.
		/// </summary>
		public void ForEachSourceTilemap(Action<SourceTilemap, int> tilemapHandler)
		{
			for (int i = 0; i < Sources.Count; i++)
			{
				var source = Sources[i];

				if (source is SourceTilemap tilemap)
				{
					tilemapHandler(tilemap, i);
				}
			}
		}

		/// <summary>
		/// Returns the item from the <see cref="Sources"/> that corresponds to the given filename or null if none found.
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public ISourceFile SourceWithFilename(string filename)
		{
			foreach (var source in Sources)
			{
				if (source.Filename == filename)
				{
					return source;
				}
			}

			return null;
		}

		#endregion

		#region Default sizes

		/// <summary>
		/// Constraints the given proposed width to be within acceptable range based on project parameters.
		/// </summary>
		public int ConstrainItemWidth(int proposedWidth)
		{
			var defaultWidth = DefaultItemWidth();
			var maxWidth = MaximumItemWidth();

			if (proposedWidth < defaultWidth)
			{
				return defaultWidth;
			}
			else if (proposedWidth > maxWidth)
			{
				return maxWidth;
			}
			else
			{
				var referenceWidth = defaultWidth - 1;
				return (proposedWidth + referenceWidth) & ~referenceWidth;
			}
		}

		/// <summary>
		/// Constraints the given proposed height to be within acceptable range based on project parameters.
		/// </summary>
		public int ConstrainItemHeight(int proposedHeight)
		{
			var defaultHeight = DefaultItemHeight();
			var maxHeight = MaximumItemHeight();

			if (proposedHeight < defaultHeight)
			{
				return defaultHeight;
			}
			else if (proposedHeight > maxHeight)
			{
				return maxHeight;
			}
			else
			{
				var referenceHeight = defaultHeight - 1;
				return (proposedHeight + referenceHeight) & ~referenceHeight;
			}
		}

		/// <summary>
		/// Returns maximum item width (based on <see cref="OutputType"/> and possibly other values).
		/// </summary>
		public int MaximumItemWidth()
		{
			switch (OutputType)
			{
				case OutputType.Sprites: return 320;
				case OutputType.Tiles: return 128;
				default: return 16;
			}
		}

		/// <summary>
		/// Returns maximum item height (based on <see cref="OutputType"/> and possibly other values).
		/// </summary>
		public int MaximumItemHeight()
		{
			return MaximumItemWidth();
		}

		/// <summary>
		/// Determines item width (based on <see cref="OutputType"/> and possibly other values).
		/// </summary>
		public int DefaultItemWidth()
		{
			// Note: atm this code is suited for ZX Spectrum Next.
			switch (OutputType)
			{
				case OutputType.Sprites: return 16;
				default: return 8;
			}
		}

		/// <summary>
		/// Determines item width (based on <see cref="OutputType"/> and possibly other values).
		/// </summary>
		public int DefaultItemHeight()
		{
			// Note: atm this code is suited for ZX Spectrum Next.
			return DefaultItemWidth();
		}

		#endregion

		#region Helpers

		private void CreateBitmaps()
		{
			BlocksBitmap = new Bitmap(256, 512, PixelFormat.Format24bppRgb);
			CharsBitmap = new Bitmap(256, 256 * 16, PixelFormat.Format24bppRgb);
		}

		/// <summary>
		/// Unconditionally raises <see cref="RemapRequired"/> event.
		/// </summary>
		private void RaiseRemapRequired()
		{
			if (RemapRequired != null)
			{
				RemapRequired(this, new EventArgs());
			}
		}

		/// <summary>
		/// If the given value is different from current one (stored in given field), then the following happens in given order:
		/// 
		/// 1. the new value is assigned to the given field
		/// 2. <see cref="onChange"/> is called (optional, only if not null)
		/// 3. <see cref="RemapRequired"/> event is raised
		/// 
		/// If the value is the same, then nothing happens.
		/// </summary>
		private void RaiseRemapRequired<T>(T value, ref T field, Action<T> onChange = null)
		{
			if (!Equals(value, field))
			{
				// First assign the new value to the field.
				field = value;

				// If on change action is provided, call it. This gives caller a chance to perform additional logic before `RemapRequired` event is raised. For example adjust some dependant values or raise another event.
				if (onChange != null)
				{
					onChange(value);
				}

				// Finally raise the `RemapRequired` event.
				RaiseRemapRequired();
			}
		}

		#endregion

		#region Declarations

		public class OutputTypeChangedEventArgs : EventArgs
		{
			public OutputTypeChangedEventArgs(OutputType type)
			{
				this.OutputType = type;
			}

			public OutputType OutputType { get; private set; }
		}

		public class SizeChangedEventArgs : EventArgs
		{
			public SizeChangedEventArgs(int size)
			{
				this.Size = size;
			}

			public int Size { get; private set; }
		}

		#endregion
	}

	namespace Model
	{
		static class Extensions
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

			public static string Attribute(this XmlNode node, string name, string defaultValue)
			{
				string result = defaultValue;

				node.WithAttribute(name, value =>
				{
					result = value;
				});

				return result;
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

			public static XmlAttribute AddAttribute(this XmlNode node, string name, int value)
			{
				return AddAttribute(node, name, value.ToString());
			}

			public static XmlAttribute AddAttribute(this XmlNode node, string name, bool value)
			{
				return AddAttribute(node, name, value ? "true" : "false");
			}
		}
	}
}
