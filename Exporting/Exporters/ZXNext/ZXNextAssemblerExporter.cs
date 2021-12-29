using NextGraphics.Exporting.Common;
using NextGraphics.Exporting.Exporters.Base;
using NextGraphics.Models;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
			var labelPrefix = Regex.Replace(Model.Name, @"\s+", ""); // Strip all whitespace from model name
			var blockLabelName = DetermineBlockLabelName();

			using (var writer = new StreamWriter(Parameters.SourceStream()))
			{
				writer.WriteLine($"// {Model.Name}.asm");
				writer.WriteLine($"// Created on {Parameters.Time.ToString("F", CultureInfo.CurrentCulture)} by the NextGraphics tool from");
				writer.WriteLine($"// patricia curtis at luckyredfish dot com");
				writer.WriteLine();
				writer.WriteLine($"{$"{labelPrefix}_Colours".ToUpper()}:\t\tequ\t{Model.Palette.UsedCount}");
				writer.WriteLine();

				switch (Model.Palette.Type)
				{
					case PaletteType.Custom:
						writer.WriteLine($"{labelPrefix}Palette:");

						var startColour = Model.Palette.StartIndex;
						for (int j = 0; j < Model.Palette.UsedCount; j++)
						{
							writer.Write("\t\t\tdb\t%");
							writer.Write(AsBinary(AsPalette8Bit(Model.Palette[startColour + j])));

							if (Model.CommentType == CommentType.Full)
							{
								writer.Write($"\t//\t{Model.Palette[startColour + j].Red},{Model.Palette[startColour + j].Green},{Model.Palette[startColour + j].Blue}");
							}

							writer.WriteLine();
						}
						break;

					case PaletteType.Next256:
						if (Model.CommentType == CommentType.Full)
						{
							writer.WriteLine("// Mapped to the spectrum next 256 palette");
						}
						break;

					case PaletteType.Next512:
						if (Model.CommentType == CommentType.Full)
						{
							writer.WriteLine("// Mapped to the spectrum next 512 palette");
						}
						break;
				}

				writer.WriteLine();
				writer.WriteLine($"{$"{labelPrefix}_{blockLabelName}".ToUpper()}_SIZE:\t\tequ\t{ExportData.BlockSize}");
				writer.WriteLine();
				writer.WriteLine($"{$"{labelPrefix}_{blockLabelName}".ToUpper()}S:\t\tequ\t{ExportData.CharactersCount}");
				writer.WriteLine();

				if (!Model.BinaryOutput)
				{
					for (int s = 0; s < ExportData.CharactersCount; s++)
					{
						void WriteCharacterByte(byte value, int x)
						{
							writer.Write("${0:x2}", value);
							if (x < (ExportData.Chars[s].Width - 1))
							{
								writer.Write(",");
							}
						}

						writer.WriteLine($"{labelPrefix}{blockLabelName}{s}:");
						for (int y = 0; y < ExportData.Chars[s].Height; y++)
						{
							writer.Write("\t\t\t\t.db\t");

							byte value = 0;
							for (int x = 0; x < ExportData.Chars[s].Width; x++)
							{
								if (Model.FourBit || Model.OutputType == OutputType.Tiles)
								{
									if ((x & 1) == 0)
									{
										// In this case we only assign high nibble, we'll actually write it in the next iteration when we prepare low nibble part.
										value = (byte)((ExportData.Chars[s].GetPixel(x, y) & 0x0f) << 4);
									}
									else
									{
										// For second colour, we add it as low nibble to previously prepared value and then write them both out as a single byte.
										value = (byte)(value | (ExportData.Chars[s].GetPixel(x, y) & 0x0f));

										WriteCharacterByte(value, x);
									}
								}
								else
								{
									// For 8-bit palette, each colour takes full byte.
									WriteCharacterByte((byte)ExportData.Chars[s].GetPixel(x, y), x);
								}
							}

							// New line after each line.
							writer.WriteLine();
						}
					}
				}

				if (Model.CommentType == CommentType.Full)
				{
					if (Model.OutputType == OutputType.Sprites)
					{
						writer.WriteLine("\r\n\t\t\t\t// number of sprites");
						writer.WriteLine();
						writer.WriteLine("\t\t\t\t\t// x offset from center of sprite");
						writer.WriteLine("\t\t\t\t\t// y offset from center of sprite");
						writer.WriteLine("\t\t\t\t\t// Palette offset with the X mirror,Y mirror, Rotate bits if set");
						writer.WriteLine("\t\t\t\t\t// 4 bit colour bit and pattern offset bit");
						writer.WriteLine("\t\t\t\t\t// index of the sprite at this position that makes up the frame");
						writer.WriteLine();
						writer.WriteLine("\t\t\t\t//...... repeated wide x tall times");
						writer.WriteLine();
					}
					else
					{
						writer.WriteLine("\r\n\t\t\t\t// block data");
						writer.WriteLine("\t\t\t\t// number of tiles (characters) tall");
						writer.WriteLine("\t\t\t\t// number of tiles (characters) wide");
						writer.WriteLine("\t\t\t\t\t// Palette offset with the X mirror,Y mirror, Rotate bits if set");
						writer.WriteLine("\t\t\t\t\t// index of the character at this position that makes up the block");
						writer.WriteLine("\t\t\t\t//...... repeated wide x tall times");
						writer.WriteLine();

						writer.WriteLine("\t\t\t\t//Note: Blocks/Tiles/characters output block 0 and tile 0 is blank.");
						writer.WriteLine();
						writer.WriteLine($"{labelPrefix}{ blockLabelName}Width:\tequ\t{ExportData.Sprites[0].Width}");
						writer.WriteLine($"{labelPrefix}{blockLabelName}Height:\tequ\t{ExportData.Sprites[0].Height}");
					}
				}

				if (!Model.BinaryOutput || !Model.BinaryBlocksOutput)
				{
					writer.WriteLine("// Collisions Left Width Top Height");

					for (int s = 0; s < ExportData.BlocksCount; s++)
					{
						if (Model.OutputType == OutputType.Sprites)
						{
							Rectangle collision = ExportData.Sprites[s].Frame;
							writer.Write($"{labelPrefix}Collision{s}:");
							writer.Write("\t\t.db\t");
							writer.Write($"{collision.X},");
							writer.Write($"{collision.Width},");
							writer.Write($"{collision.Y},");
							writer.Write($"{collision.Height}");
							writer.WriteLine();
						}
					}

					var spriteCount = 0;

					writer.WriteLine();
					for (int s = 0; s < ExportData.BlocksCount; s++)
					{
						if (Model.OutputType == OutputType.Sprites)
						{
							spriteCount = ExportData.Sprites[s].Size;

							for (int y = 0; y < ExportData.Sprites[s].Height; y++)
							{
								for (int x = 0; x < ExportData.Sprites[s].Width; x++)
								{
									if (ExportData.Sprites[s].GetTransparent(x, y) == true)
									{
										spriteCount--;
									}
								}
							}

							writer.Write($"{labelPrefix}Frame{s}:");
							writer.Write("\t\t.db\t");
							//outputFile.Write(ExportData.Sprites[s].Width.ToString() + ",");
							//outputFile.Write(ExportData.Sprites[s].Height.ToString() + ",\t");
							writer.Write(spriteCount.ToString() + ",\t");
						}
						else
						{
							writer.Write($"{labelPrefix}Block{s}:");
							writer.Write("\t\t.db\t");
						}

						// adjust x y pos based on the centerPanel setting
						for (int y = 0; y < ExportData.Sprites[s].Height; y++)
						{
							int writtenWidth = ExportData.Sprites[s].Width;

							for (int x = 0; x < ExportData.Sprites[s].Width; x++)
							{
								if (Model.OutputType == OutputType.Sprites)
								{
									if (ExportData.Sprites[s].GetTransparent(x, y) == true)
									{
										writtenWidth--;
										continue;
									}

									writer.Write($"{ExportData.Sprites[s].OffsetX + (ExportData.ImageOffset.X + (ExportData.Sprites[s].GetXPos(x, y) * ExportData.ObjectSize))},");
									writer.Write($"{ExportData.Sprites[s].OffsetY + (ExportData.ImageOffset.Y + (ExportData.Sprites[s].GetYpos(x, y) * ExportData.ObjectSize))},");

									string textFlips = "";
									byte writeByte = 0;
									
									if (ExportData.Sprites[s].GetPaletteOffset(x, y) != 0)
									{
										writeByte = (byte)ExportData.Sprites[s].GetPaletteOffset(x, y);
									}
									
									if (ExportData.Sprites[s].GetFlippedX(x, y) == true)
									{
										writeByte = (byte)(writeByte | 8);
										textFlips = "+XFLIP";

									}
									
									if (ExportData.Sprites[s].GetFlippedY(x, y) == true)
									{
										writeByte = (byte)(writeByte | 4);
										textFlips += "+YFLIP";
									}
									
									if (ExportData.Sprites[s].GetRotated(x, y) == true)
									{
										writeByte = (byte)(writeByte | 2);
										textFlips += "+ROT";
									}
									
									if (Model.TextFlips)
									{
										writer.Write($"\t{labelPrefix.ToUpper()}_OFFSET+{textFlips},\t");
									}
									else
									{
										writer.Write($"\t{labelPrefix.ToUpper()}_OFFSET+{writeByte},\t");
									}

									writeByte = 0;

									if (Model.FourBit)
									{
										writeByte = (byte)(writeByte | 128);
										if (((ExportData.Sprites[s].GetId(x, y) - IdReduction) & 1) == 1)
										{
											writeByte = (byte)(writeByte | 64);
										}
									}
									writer.Write($"{writeByte},");

									if (Model.FourBit)
									{
										writer.Write($"{(ExportData.Sprites[s].GetId(x, y) - IdReduction) / 2}");
									}
									else
									{
										writer.Write($"{ExportData.Sprites[s].GetId(x, y)}");
									}

									if (x < (writtenWidth) - 1)
									{
										writer.Write(",\t");
									}
								}
								else
								{
									byte writeByte = 0;

									if (ExportData.Sprites[s].GetPaletteOffset(x, y) != 0)
									{
										writeByte = (byte)ExportData.Sprites[s].GetPaletteOffset(x, y);
									}

									if (ExportData.Sprites[s].GetFlippedX(x, y) == true)
									{
										writeByte = (byte)(writeByte | 8);
									}

									if (ExportData.Sprites[s].GetFlippedY(x, y) == true)
									{
										writeByte = (byte)(writeByte | 4);
									}

									if (ExportData.Sprites[s].GetRotated(x, y) == true)
									{
										writeByte = (byte)(writeByte | 2);
									}

									writer.Write($"{writeByte},");

									writeByte = 0;
									writer.Write($"{ExportData.SortIndexes[ExportData.Sprites[s].GetId(x, y)]}");

									if (x < (ExportData.Sprites[s].Width) - 1)
									{
										writer.Write(",\t");
									}
								}
							}

							if (y < (ExportData.Sprites[s].Height) - 1)
							{
								if (!ExportData.Sprites[s].GetTransparent(ExportData.Sprites[s].Width - 1, y) || Model.OutputType == OutputType.Tiles)
								{
									writer.Write(",\t");
								}

								if (spriteCount > 10 && Model.OutputType == OutputType.Sprites)
								{
									writer.Write("\r\n\t\t\t\t.db\t\t");
								}
							}
							else
							{
								writer.Write("\r\n");
							}
						}
					}
				}

				writer.WriteLine();
				if (Model.OutputType == OutputType.Sprites)
				{
					writer.Write($"{labelPrefix}Frames:\t\t.dw\t");
					for (int s = 0; s < ExportData.BlocksCount; s++)
					{
						writer.Write($"{labelPrefix}Frame{s}");
						if (s < (ExportData.BlocksCount) - 1)
						{
							writer.Write(",");
						}
						else
						{
							writer.WriteLine();
						}
					}
				}

				if (Model.BinaryOutput)
				{
					writer.Write($"{labelPrefix.ToUpper()}_FILE_SIZE\tequ\t{ExportData.BinarySize}");
					writer.WriteLine();
					writer.WriteLine($"{labelPrefix}File:\t\t\tdw\t{Model.Name.ToUpper()}_FILE_SIZE");
					writer.WriteLine($"\t\t\tdb\tPATH,\"game/level1/{labelPrefix.ToLower()}.bin\",0");
				}
			}
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Converts a number into a binary string.
		/// </summary>
		private string AsBinary(byte num)
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
				bits = bits >> 1;
			}
			return outString;
		}

		private string DetermineBlockLabelName()
		{
			switch (Model.OutputType)
			{
				case OutputType.Tiles: return "Tile";
				default: return "Sprite";
			}
		}

		#endregion
	}
}
