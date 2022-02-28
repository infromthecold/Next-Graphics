using NextGraphics.Exporting.Common;
using NextGraphics.Exporting.Exporters.Base;
using NextGraphics.Models;

using Scriban;

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NextGraphics.Exporting.Exporters.ZXNext
{
	/// <summary>
	/// Exports data as Z80 Next assembler format.
	/// </summary>
	public class ZXNextAssemblerExporter : BaseExporter
	{
		#region Overrides

		protected override void OnExport()
		{
			string templateContents = ExportTemplate();
			var template = Template.Parse(templateContents);

			using (var writer = new StreamWriter(Parameters.SourceStream()))
			{
				var isFullBinaryExport = Model.BinaryOutput && Model.BinaryFramesAttributesOutput;
				var sanitizedModelName = Regex.Replace(Model.Name, @"\s+", ""); // Strips all whitespace from model name

				var characterWidth = ExportData.Sprites[0] != null ? ExportData.Sprites[0].Width : 0;
				var characterHeight = ExportData.Sprites[0] != null ? ExportData.Sprites[0].Height : 0;

				var tilemaps = Model.SourceTilemaps().ToList();

				// Note: scriban (template engine) transforms capitalized names (for example ProjectName) into lowercase and underscore (project_name), so we simply use expected syntax here to avoid confusion.
				// Note: in general, all data is available even if corresponding "x_can_export" flag is false; it's up to template file to determine whether to respect flag or not. This given template writer more versatility.
				var result = template.Render(new
				{
					// By providing model and export data, we give full access to all of the available data to template file, however not all is easily digestable. If at all possible use (or add) specific values below.
					model_data = Model,
					export_data = ExportData,

					project_name = Model.Name,

					parameter_block_size = ExportData.BlockSize,

					export_date = Parameters.Time.ToString("F", CultureInfo.CurrentCulture),
					export_is_sprites = Model.OutputType == OutputType.Sprites,
					export_is_tiles = Model.OutputType == OutputType.Tiles,

					label_name = sanitizedModelName,
					label_suffix = CreateTemplateLabelNameSuffix(),

					palette_is_custom = Model.Palette.Type == PaletteType.Custom,
					palette_is_next_256 = Model.Palette.Type == PaletteType.Next256,
					palette_is_next_512 = Model.Palette.Type == PaletteType.Next512,
					palette = CreateTemplatePalette(),

					characters_can_export = !Model.BinaryOutput,
					characters_size_can_export = Model.OutputType == OutputType.Tiles,
					character_width = characterWidth,
					character_height = characterHeight,
					characters = CreateTemplateCharacters(),

					collisions_can_export = !isFullBinaryExport && Model.OutputType == OutputType.Sprites,
					collisions = CreateTemplateCollisions(),

					frames_can_export = !isFullBinaryExport && Model.OutputType == OutputType.Sprites,
					frames_lookup_can_export = Model.OutputType == OutputType.Sprites,
					frames = CreateTemplateFrames(),

					tiles_can_export = !isFullBinaryExport && Model.OutputType == OutputType.Tiles,
					tiles = CreateTemplateTiles(),

					tilemap_can_export = Model.OutputType == OutputType.Tiles && tilemaps.Count > 0,
					tilemap_data_can_export = !Model.BinaryOutput && Model.OutputType == OutputType.Tiles && tilemaps.Count > 0,
					tilemaps = CreateTemplateTilemaps(),

					binary_can_export = Model.BinaryOutput,
					binary_file_size = ExportData.BinarySize,
					binary_file_path = $"{sanitizedModelName}.bin",

					comments_can_export = Model.CommentType == CommentType.Full,
				});

				writer.Write(result);
			}
		}

		#endregion

		#region Exporting

		private string ExportTemplate()
		{
			// There are several options when choosing the template. We check each by priority and select the first one found:
			string GetTemplate(string filename)
			{
				var templatePath = Path.GetDirectoryName(filename);
				var templateFilename = Path.GetFileName(filename);
				var path = Path.Combine(templatePath, Path.ChangeExtension(templateFilename, "template"));

				if (!File.Exists(path)) throw new FileLoadException();

				return File.ReadAllText(path);
			}

			// Highest priority has the template stored with the project file. This allows user to use different template for each project. The file name must be the same as project name but with .template extension.
			try
			{
				return GetTemplate(Model.Filename);
			}
			catch
			{
			}

			// If project file template is missing, or cannot be read, check for template named the same as the executable but with .template extension.
			try
			{
				return GetTemplate(Assembly.GetExecutingAssembly().Location);
			}
			catch
			{
			}

			// Finally, if none of above works, get embedded template.
			return Properties.Resources.DefaultAssemblerExportTemplate;
		}

		private string CreateTemplateLabelNameSuffix()
		{
			switch (Model.OutputType)
			{
				case OutputType.Tiles: return "Tile";
				default: return "Sprite";
			}
		}

		private IEnumerable<TemplateColour> CreateTemplatePalette()
		{
			var result = new List<TemplateColour>();

			for (int i = Model.Palette.StartIndex; i < Model.Palette.UsedCount; i++)
			{
				var palette = Model.Palette[i];

				var colour = new TemplateColour
				{
					Red = palette.Red,
					Green = palette.Green,
					Blue = palette.Blue,
				};

				palette.ToRawBytes(Model.PaletteFormat).ForEach(c =>
				{
					colour.Values.Add(new ByteValue(c));
				});

				colour.Values.SetupListItems();

				result.Add(colour);
			}

			return result.SetupListItems();
		}

		private IEnumerable<TemplateCharacter> CreateTemplateCharacters()
		{
			List<TemplateCharacter.Row> CreateRows(IndexedBitmap character)
			{
				var rows = new List<TemplateCharacter.Row>();

				for (int y = 0; y < character.Height; y++)
				{
					var row = new TemplateCharacter.Row { Y = y };

					byte value = 0;
					for (int x = 0; x < character.Width; x++)
					{
						void AddColumn()
						{
							row.Columns.Add(new TemplateCharacter.Column
							{
								X = x,
								Y = y
							});
						}

						void AddValue(byte v)
						{
							var column = row.Columns.Last();
							column.Values.Add(new ByteValue(v));
							column.Values.SetupListItems();
						}

						if (Model.SpritesFourBit || Model.OutputType == OutputType.Tiles)
						{
							if ((x & 1) == 0)
							{
								// In this case we only assign high nibble, we'll actually write it in the next iteration when we prepare low nibble part.
								value = (byte)((character.GetPixel(x, y) & 0x0f) << 4);
							}
							else
							{
								// For second colour, we add it as low nibble to previously prepared value and then write them both out as a single byte.
								value = (byte)(value | (character.GetPixel(x, y) & 0x0f));
								AddColumn();
								AddValue(value);
							}
						}
						else
						{
							AddColumn();
							AddValue((byte)character.GetPixel(x, y));
						}
					}

					row.Columns.SetupListItems();

					rows.Add(row);
				}

				rows.SetupListItems();

				return rows;
			}

			var result = new List<TemplateCharacter>();

			for (int i = 0; i < ExportData.CharactersCount; i++)
			{
				var character = ExportData.Chars[i];

				result.Add(new TemplateCharacter
				{
					Rows = CreateRows(character)
				});
			}

			return result.SetupListItems();
		}

		private IEnumerable<TemplateCollision> CreateTemplateCollisions()
		{
			var result = new List<TemplateCollision>();

			for (int i = 0; i < ExportData.BlocksCount; i++)
			{
				var sprite = ExportData.Sprites[i];

				result.Add(new TemplateCollision
				{
					X = sprite.Frame.X,
					Y = sprite.Frame.Y,
					Width = sprite.Frame.Width,
					Height = sprite.Frame.Height,
				});
			}

			return result.SetupListItems();
		}

		private IEnumerable<TemplateFrame> CreateTemplateFrames()
		{
			var result = new List<TemplateFrame>();

			for (int i = 0; i < ExportData.BlocksCount; i++)
			{
				var sprite = ExportData.Sprites[i];
				var frame = new TemplateFrame();

				for (int y = 0; y < sprite.Height; y++)
				{
					int writtenWidth = sprite.Width;

					for (int x = 0; x < sprite.Width; x++)
					{
						if (sprite.GetTransparent(x, y))
						{
							writtenWidth--;
							continue;
						}

						// Create new data for this iteration.
						var item = new TemplateFrame.Item
						{
							IsTextAttributes = Model.SpritesAttributesAsText,
							IsFourBit = Model.SpritesFourBit,
						};

						// Note: the main reason for using inline functions it to be able to collapse them to avoid code clutter.

						void AssignId()
						{
							item.Id = sprite.GetId(x, y);
							if (Model.SpritesFourBit)
							{
								item.Id = (short)((item.Id - IdReduction) / 2);
							}
						}

						void AssignOffset()
						{
							item.OffsetX = sprite.OffsetX + (ExportData.ImageOffset.X + (sprite.GetXPos(x, y) * ExportData.ObjectSize));
							item.OffsetY = sprite.OffsetY + (ExportData.ImageOffset.Y + (sprite.GetYpos(x, y) * ExportData.ObjectSize));
						}

						void AssignAttributes()
						{
							item.Attributes.Value = 0;

							if (sprite.GetPaletteOffset(x, y) != 0)
							{
								item.Attributes.Value = (byte)sprite.GetPaletteOffset(x, y);
							}

							if (sprite.GetFlippedX(x, y) == true)
							{
								item.Attributes.Value |= (byte)8;
								item.IsAttributeFlipX = true;
							}

							if (sprite.GetFlippedY(x, y) == true)
							{
								item.Attributes.Value |= (byte)4;
								item.IsAttributeFlipY = true;
							}

							if (sprite.GetRotated(x, y) == true)
							{
								item.Attributes.Value |= (byte)2;
								item.IsAttributeRotate = true;
							}
						}

						void AssignFourBitAttributes()
						{
							item.FourBitAttributes.Value = 0;
							if (Model.SpritesFourBit)
							{
								item.FourBitAttributes.Value |= (byte)128;
								if (((sprite.GetId(x, y) - IdReduction) & 1) == 1)
								{
									item.FourBitAttributes.Value |= (byte)64;
								}
							}
						}

						AssignId();
						AssignOffset();
						AssignAttributes();
						AssignFourBitAttributes();

						frame.Items.Add(item);
					}
				}

				frame.Items.SetupListItems();

				result.Add(frame);
			}

			return result.SetupListItems();
		}

		private IEnumerable<TemplateTile> CreateTemplateTiles()
		{
			var result = new List<TemplateTile>();

			for (int i = 0; i < ExportData.BlocksCount; i++)
			{
				var sprite = ExportData.Sprites[i];
				var tile = new TemplateTile();

				for (int y = 0; y < sprite.Height; y++)
				{
					for (int x = 0; x < sprite.Width; x++)
					{
						var item = new TemplateTile.Item();

						// Note: the main reason for using inline functions it to be able to collapse them to avoid code clutter.

						void AssignAttributes()
						{
							item.Attributes.Value = 0;

							if (sprite.GetPaletteOffset(x, y) != 0)
							{
								item.Attributes.Value = (byte)sprite.GetPaletteOffset(x, y);
							}

							if (sprite.GetFlippedX(x, y) == true)
							{
								item.Attributes.Value |= (byte)8;
							}

							if (sprite.GetFlippedY(x, y) == true)
							{
								item.Attributes.Value |= (byte)4;
							}

							if (sprite.GetRotated(x, y) == true)
							{
								item.Attributes.Value |= (byte)2;
							}
						}

						void AssignTileIndex()
						{
							item.TileIndex = new ByteValue((byte)ExportData.SortIndexes[sprite.GetId(x, y)]);
						}

						AssignAttributes();
						AssignTileIndex();

						tile.Items.Add(item);
					}
				}

				tile.Items.SetupListItems();

				result.Add(tile);
			}

			return result.SetupListItems();
		}

		private IEnumerable<TemplateTilemap> CreateTemplateTilemaps()
		{
			var result = new List<TemplateTilemap>();

			foreach (var tilemap in Model.SourceTilemaps())
			{
				var templateTilemap = new TemplateTilemap();

				switch (Model.TilemapExportType)
				{
					case TilemapExportType.AttributesIndexAsWord:
						templateTilemap.IsAttributesDesired = true;
						templateTilemap.IsWordOutput = true;
						break;
					case TilemapExportType.AttributesIndexAsTwoBytes:
						templateTilemap.IsAttributesDesired = true;
						templateTilemap.IsWordOutput = false;
						break;
					case TilemapExportType.IndexOnly:
						templateTilemap.IsAttributesDesired = false;
						templateTilemap.IsWordOutput = false;	// ignored in this case, but makes it explicit
						break;
				}

				templateTilemap.Width = tilemap.Data.Width;
				templateTilemap.Height = tilemap.Data.Height;

				for (int y = 0; y < tilemap.Data.Height; y++)
				{
					var templateRow = new TemplateTilemap.Row();

					for (int x = 0; x < tilemap.Data.Width; x++)
					{
						var tile = tilemap.Data.Tiles[y, x];
						var index = (byte)tile.Index;
						var attributes = tile.ZXNextTileAttributes();
						var templateTile = new TemplateTilemap.Tile(attributes, index);
						
						templateRow.Tiles.Add(templateTile);
					}

					templateRow.Tiles.SetupListItems();
					templateTilemap.Rows.Add(templateRow);
				}

				templateTilemap.Rows.SetupListItems();
				result.Add(templateTilemap);
			}

			return result.SetupListItems();
		}

		#endregion

		#region Declarations

		private class TemplateColour : ListItem
		{
			public int Red { get; set; }
			public int Green { get; set; }
			public int Blue { get; set; }

			public List<ByteValue> Values { get; set; } = new List<ByteValue>();
		}

		private class TemplateCharacter : ListItem
		{
			public List<Row> Rows { get; set; }

			public class Row : ListItem
			{
				public int Y { get; set; }
				public List<Column> Columns { get; set; } = new List<Column>();
			}

			public class Column : ListItem
			{
				public int X { get; set; }
				public int Y { get; set; }
				public List<ByteValue> Values { get; set; } = new List<ByteValue>();
			}
		}

		private class TemplateCollision : ListItem
		{
			public int X { get; set; }
			public int Y { get; set; }
			public int Width { get; set; }
			public int Height { get; set; }
		}

		private class TemplateFrame : ListItem
		{
			public List<Item> Items { get; set; } = new List<Item>();

			public class Item : ListItem
			{
				public short Id { get; set; } = 0;

				public int OffsetX { get; set; } = 0;
				public int OffsetY { get; set; } = 0;

				public ByteValue Attributes { get; set; } = new ByteValue();
				public bool IsTextAttributes { get; set; } = false;
				public bool IsAttributeFlipX { get; set; } = false;
				public bool IsAttributeFlipY { get; set; } = false;
				public bool IsAttributeRotate { get; set; } = false;

				public bool IsFourBit { get; set; } = false;
				public ByteValue FourBitAttributes { get; set; } = new ByteValue();    //< only provided if 4-bit output is selected (`IsFourBit` is true), otherwise 0
			}
		}

		private class TemplateTile : ListItem
		{
			public List<Item> Items { get; set; } = new List<Item>();

			public class Item : ListItem
			{
				public ByteValue Attributes { get; set; } = new ByteValue();
				public ByteValue TileIndex { get; set; } = new ByteValue();
			}
		}

		private class TemplateTilemap : ListItem
		{
			public string Name { get; set; } = "";
			public string NameOrIndex { get => Name.Length > 0 ? Name : Index.ToString(); }

			public int Width { get; set; } = 0;
			public int Height { get; set; } = 0;
			public int Size { get => IsAttributesDesired ? Width * Height * 2 : Width * Height; }

			public bool IsAttributesDesired { get; set; } = true;
			public bool IsWordOutput { get; set; } = true;

			public List<Row> Rows { get; set; } = new List<Row>();

			public class Row : ListItem
			{
				public List<Tile> Tiles { get; set; } = new List<Tile>();
			}

			public class Tile : ListItem
			{
				public ByteValue Attributes { get; set; } = new ByteValue();
				public ByteValue TileIndex { get; set; } = new ByteValue();
				public WordValue AsWord { get => new WordValue(Attributes, TileIndex); }

				public Tile(byte attributes, byte index)
				{
					Attributes.Value = attributes;
					TileIndex.Value = index;
				}
			}
		}

		private class ByteValue : ListItem
		{
			public byte Value { get; set; } = 0;
			public string AsBinary { get => Value.ToBinaryString(); }
			public string AsHex { get => Value.ToHexString(); }

			public ByteValue(byte value = 0)
			{
				Value = value;
			}
		}

		private class WordValue : ListItem
		{
			public short Value {
				get {
					short high = HighByte.Value;
					short low = LowByte.Value;

					return (short)(((short)(high << 8)) | low);
				}
			}

			public string AsBinary { get => $"{LowByte.AsBinary}{HighByte.AsBinary}"; }
			public string AsHex { get => $"{LowByte.AsHex}{HighByte.AsHex}"; }

			public ByteValue HighByte { get; set; }
			public ByteValue LowByte { get; set; }

			public WordValue(ByteValue high, ByteValue low)
			{
				HighByte = high;
				LowByte = low;
			}

			public WordValue(byte high = 0, byte low = 0) : this(new ByteValue(high), new ByteValue(low))
			{
			}

		}

		#endregion
	}

	#region Declarations

	abstract class ListItem
	{
		public int Index { get; set; } = 0;
		public bool IsLast { get; set; } = true;
	}

	static class ZXNextAssemblerExtensions
	{
		/// <summary>
		/// Updates all items of the list to have <see cref="ListItem.Index"/> and <see cref="ListItem.IsLast" represent item position in the list. Returns the source list as convenience so we can use it as a sort of "builder pattern".
		/// </summary>
		public static IEnumerable<T> SetupListItems<T>(this IEnumerable<T> enumerable) where T : ListItem
		{
			ListItem lastItem = null;
			int index = 0;

			foreach (var item in enumerable)
			{
				item.Index = index;
				item.IsLast = false;

				lastItem = item;

				index++;
			}

			if (lastItem != null)
			{
				lastItem.IsLast = true;
			}

			return enumerable;
		}

		/// <summary>
		/// Calculates binary attributes for given tile suitable for ZX Next export. Format is:
		/// 
		/// Bit
		/// 76543210
		/// --------
		///     |||
		///     ||+--- rotated clockwise
		///     |+---- X mirror
		///     +----- Y mirror
		/// </summary>
		public static byte ZXNextTileAttributes(this TilemapData.Tile tile)
		{
			byte result = 0;

			if (tile.RotatedClockwise)
			{
				result += 1 << 1;
			}

			if (tile.FlippedY)
			{
				result += 1 << 2;
			}

			if (tile.FlippedX)
			{
				result += 1 << 3;
			}

			return result;
		}
	}

	#endregion
}
