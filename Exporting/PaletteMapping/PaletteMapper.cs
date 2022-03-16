using NextGraphics.Exporting.Common;
using NextGraphics.Models;
using NextGraphics.Utils;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace NextGraphics.Exporting.PaletteMapping
{
	/// <summary>
	/// Helpers for preparing palette from source files.
	/// </summary>
	public class PaletteMapper
	{
		private ExportData Data { get; }
		private MainModel Model { get => Data.Model; }

		#region Initialization & disposal

		public PaletteMapper(ExportData data)
		{
			Data = data;
		}

		#endregion

		#region Loading

		/// <summary>
		/// Prepares palette from the given bitmap. This will inteligently map all distinct colours based on model parameters. Resulting colours may be different from the palette used in preferred image editor and with more complex images it can happen that mapping will fail. In such case palette should be exported from image editor into separate file and then <see cref="Load"/> should be used.
		/// </summary>
		public Palette Map(Bitmap bitmap)
		{
			var result = new Palette();

			void Map4BitColoursToBanks()
			{
				// Mapping 4 bit colours is quite complex operation composed of several steps. In a nutshell: we need to parse inteligently attempting to fit all objects into 16-colour banks. One of the 16 colours must be transparent colour.

				Palette NewBank()
				{
					return new Palette
					{
						MaxCount = 16
					};
				}

				// The first step is to scan the source bitmap object by object and remember all distinct colours for each object.
				List<Palette> ParsePaletteForObjects()
				{
					var coloursPerObject = new List<Palette>();

					int blockY = 0;
					while (blockY < bitmap.Height)
					{
						int blockX = 0;
						while (blockX < bitmap.Width)
						{
							var objectPalette = NewBank();

							for (int y = 0; y < Model.GridHeight; y++)
							{
								for (int x = 0; x < Model.GridWidth; x++)
								{
									var color = bitmap.GetPixel(x + blockX, y + blockY);
									var colour = new Colour(color);

									// If colour is already represented, nothing else to do.
									if (objectPalette.HasColour(colour))
									{
										continue;
									}

									// If adding a new colour will result in more than 16 colours, then throw exception - this object can't be represented by 4-bit palette. Note: alternatively we could find the best fit, however this may result in unwanted output without any warning for the user.
									if (!objectPalette.Add(colour))
									{
										throw new Exception($"Found object with more than 16-colours at ({blockX},{blockY})");
									}
								}
							}

							coloursPerObject.Add(objectPalette);

							blockX += Model.GridWidth;
						}

						blockY += Model.GridHeight;
					}

					return coloursPerObject;
				}

				// Then we generate 16-colour banks from object palettes. This method tries to find the best fit for each object. It will attempt to reuse a bank if possible. But algorithm is not perfect, it may result in unused gaps or duplicated colours. Or in worse case, for very complex images with lots of objects and colours, it will outright not be able to fit all objects. In such case, palette should be exported from image editor and loaded manually, provided the image editor is able to maintain 16-colour banks. If not, then the only way is to simplify the image...
				List<Palette> ParseObjectPalettesIntoBanks(List<Palette> objectPalettes)
				{
					var banks = new List<Palette>();

					foreach (var objectPalette in objectPalettes)
					{
						// Find the best fit within existing banks.
						Palette bestBank = null;
						int bestBankAddedColours = int.MaxValue;
						foreach (var bank in banks)
						{
							var addedColours = bank.CanFitPalette(objectPalette);
							if (addedColours < bestBankAddedColours)
							{
								bestBankAddedColours = addedColours;
								bestBank = bank;

								// A small optimization; if we find a bank where all colours are already present, we don't have to search anymore.
								if (bestBankAddedColours == 0) break;
							}
						}

						// If we found a bank that doesn't require adding any new colour, continue with next object.
						if (bestBankAddedColours == 0)
						{
							continue;
						}

						// If we found the bank that can fit all the colours, do it.
						if (bestBank != null)
						{
							bestBank.AddDistinctColoursFromPalette(objectPalette);
							continue;
						}

						// If we didn't find a bank, we need to create a new one. However if this means we'll create more banks than can fit 256 colour palette, that's an error and we throw exception.
						if (banks.Count == 16)
						{
							throw new InvalidOperationException("Unable to fit all objects within 4-bit pallette. Try to import palette from your image editor.");
						}

						// If we can fit the new bank, create one and fill it with colours.
						var newBank = NewBank();
						newBank.AddDistinctColoursFromPalette(objectPalette);
						banks.Add(newBank);
					}

					return banks;
				}

				// At this point we should have optimized palette so we can establish transparent colour. We do it by searching all banks and see if there is a colour that's repeated in all of them. Most often than not, this is the correct transparent colour. If we find it, we must make sure it's present at the same index in all banks by moving it if necessary. But otherwise we fall down to simply assume each bank will use different transparent colour.
				void EstablishTransparentColour(List<Palette> banks)
				{
					void ResetTransparentIndexes(int assumedFirstBankIndex)
					{
						for (int i = 0; i < banks.Count; i++)
						{
							banks[i].TransparentColourIndex = i == 0 ? assumedFirstBankIndex : -1;
						}
					}

					int MatchingTransparentColourBanksCount(Palette bank)
					{
						// We take the transparent colour from the given bank and count how many other banks have this same colour. This is our result.
						var matchingBanksCount = 1; // we assume given bank is a match...
						var colour = bank[bank.TransparentColourIndex];

						for (int b = 0; b < banks.Count; b++)
						{
							// Skip given bank, we already counted it.
							var otherBank = banks[b];
							if (otherBank == bank) continue;

							for (int c = 0; c < otherBank.Colours.Count; c++)
							{
								var bankColour = otherBank[c];
								if (bankColour.IsSameColour(colour))
								{
									otherBank.TransparentColourIndex = c;
									matchingBanksCount++;
								}
							}
						}

						return matchingBanksCount;
					}

					int BestTransparentColourMatch(Dictionary<int, int> banksCountPerColourIndex)
					{
						var bestCount = 0;
						var bestIndex = 0;

						for (int i = 0; i < banksCountPerColourIndex.Count; i++)
						{
							var count = banksCountPerColourIndex[i];
							if (count > bestCount)
							{
								bestCount = count;
								bestIndex = i;
							}
						}

						return bestIndex;
					}

					void MoveTransparentColour()
					{
						// We always respect user's choice of transparent index. Note that this code assumes we already trimmed it within 16-colour bank (which we do as first thing).
						var transparentColourIndex = Model.Palette.TransparentIndex;

						foreach (var bank in banks)
						{
							// If this bank doesn't have transparent colour (aka the corresponding tile/sprite is fully opaque), we must insert it to the appropriate place. If the bank is already full, insert will fail, but we simply assume the existing colour at the requested index is transparent for this tile/sprite.
							if (bank.TransparentColourIndex < 0)
							{
								var colour = banks[0].TransparentColour;
								bank.Insert(colour, transparentColourIndex);
								bank.TransparentColourIndex = transparentColourIndex;
								continue;
							}

							// If bank contains transparent colour, we must move it to new index...
							bank.MoveColour(bank.TransparentColourIndex, transparentColourIndex);

							// ...and update transparent colour index.
							bank.TransparentColourIndex = transparentColourIndex;
						}
					}

					// Regardless of where the data will lead us, transparent colour for 4-bit palettes must be contained within 16 colours.
					Model.Palette.TransparentIndex %= 16;

					// If there is just a single bank, assume the index from palette is the one to use.
					if (banks.Count == 1) return;

					// Note: line below will throw exception if there is no bank. This shouldn't really happen, but even so, we'll catch it and inform user anyway.
					var firstBank = banks.First();
					var matchingTransparencyCounts = new Dictionary<int, int>();	// key = transparent index, value = number of banks that contain that colour

					for (int i = 0; i < firstBank.Colours.Count; i++)
					{
						// Assume current colour is transparent. Set transparent index for first bank and reset all others.
						ResetTransparentIndexes(i);

						// Find the colour in other banks, adjust transparency indexes and count how many other banks contain this colour. If we find a colour that's matched in all banks, we take it as the result. Since matching method adjusts transparent colour index for all banks, we simply need to move them all to the same index as set in model (which we did at the start).
						var matchedBanks = MatchingTransparentColourBanksCount(firstBank);
						if (matchedBanks == banks.Count)
						{
							MoveTransparentColour();
							return;
						}

						// If some banks don't contain the colour, just remember how many matched for this colour index.
						matchingTransparencyCounts[i] = matchedBanks;
					}

					// If we didn't find a match in all banks (for example some tiles/sprites may be fully opaque), take the first colour that's matched with the most banks. Note that we changed transparent index for the first bank while searching for best match in the above loop, so we need to reset it to the given colour before moving. In the same manner we must also adjust all other banks to point to the best match in their own palette.
					var transparentColour = BestTransparentColourMatch(matchingTransparencyCounts);
					firstBank.TransparentColourIndex = transparentColour;
					MatchingTransparentColourBanksCount(firstBank);
					MoveTransparentColour();
				}

				// Now we can "fuse" all 16-colour banks into single 256 colour palette.
				void FillPaletteFromBanks(List<Palette> banks)
				{
					for (int i = 0; i < banks.Count; i++)
					{
						var bank = banks[i];

						// Add all colours from the bank.
						foreach (var colour in bank.Colours)
						{
							result.Add(colour);
						}

						// If there are additional banks, fill in magenta colour until the whole bank is filled.
						if (i < banks.Count - 1)
						{
							for (int c = bank.Colours.Count; c < bank.MaxCount; c++)
							{
								result.Add(new Colour(255, 0, 255));
							}
						}
					}
				}

				var palettes = ParsePaletteForObjects();
				var paletteBanks = ParseObjectPalettesIntoBanks(palettes);
				EstablishTransparentColour(paletteBanks);
				FillPaletteFromBanks(paletteBanks);
			}

			void MapColoursByObjects()
			{
				int blockY = 0;
				while (blockY < bitmap.Height)
				{
					int blockX = 0;
					while (blockX < bitmap.Width)
					{
						for (int y = 0; y < Model.GridHeight; y++)
						{
							for (int x = 0; x < Model.GridWidth; x++)
							{
								var color = bitmap.GetPixel(x + blockX, y + blockY);

								result.AddIfDistinct(color);
							}
						}

						blockX += Model.GridWidth;
					}

					blockY += Model.GridHeight;
				}
			}

			void MapColoursByPixelOrder()
			{
				for (int y = 0; y < bitmap.Height; y++)
				{
					for (int x = 0; x < bitmap.Width; x++)
					{
						var color = bitmap.GetPixel(x, y);

						result.AddIfDistinct(color);
					}
				}
			}

			if (Model.IsFourBitData && Model.PaletteParsingMethod == PaletteParsingMethod.ByObjects)
			{
				// With 4-bit palette, we also attempt to auto map all palette banks to automate multiple objects palettes as much as possible.
				Map4BitColoursToBanks();
			}
			else if (Model.PaletteParsingMethod == PaletteParsingMethod.ByObjects)
			{
				// When 8-bit palette and object grouped colour mapping is selected, group colours by objects but without having to think of mapping colour banks.
				MapColoursByObjects();
			}
			else
			{
				// Otherwise 8-bit mapping will be used for all cases. If this is 4-bit palette, then user will need to select the bank manually.
				MapColoursByPixelOrder();
			}

			return result;
		}

		/// <summary>
		/// Loads palette from the given file. This will use colours exactly as defined in the file without any additional preprocessing. This means the colours will remain exactly as exported from preferred image editor.
		/// </summary>
		public Palette Load(string filename)
		{
			var expectedColoursCount = 256;
			var expectedComponentsCount = 3;
			var isLittleEndian = BitConverter.IsLittleEndian;

			var result = new Palette();

			int ReadInt(FileStream stream)
			{
				byte[] bytesBuffer = new byte[2];
				stream.Read(bytesBuffer, 0, 2);

				if (isLittleEndian)
				{
					return bytesBuffer[1] << 8 | bytesBuffer[0];
				}
				else
				{
					return bytesBuffer[0] << 8 | bytesBuffer[1];
				}
			}

			using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
			{
				int numBytesToRead = (int)stream.Length;
				int numBytesRead = 0;

				// If file doesn't contain all data, throw exception.
				if (stream.Length < expectedColoursCount * expectedComponentsCount)
				{
					throw new InvalidDataException($"Not enough bytes in the file, expected {expectedColoursCount * expectedComponentsCount}, found {stream.Length}.");
				}

				for (int i = 0; i < 256; i++)
				{
					var r = (byte)stream.ReadByte();
					var g = (byte)stream.ReadByte();
					var b = (byte)stream.ReadByte();
					result.Add(new Colour(r, g, b));
					numBytesRead += 3;
				}

				if (numBytesToRead > numBytesRead)
				{
					// Probably got 2 bytes for colours count. In such case store custom used colours count.
					result.UsedCount = ReadInt(stream);

					// Probably got 2 bytes for transparent colour. If so, store transparent colour index. If not, first colour will be taken as transparent.
					if (numBytesToRead > numBytesRead)
					{
						var index = ReadInt(stream);
						result.TransparentColourIndex = index >= 0 ? index : 0;
					}
				}
			}

			return result;
		}

		#endregion

		#region Declarations

		public class Colour
		{
			public byte R { get; set; }
			public byte G { get; set; }
			public byte B { get; set; }

			public Colour(byte r, byte g, byte b)
			{
				R = r;
				G = g;
				B = b;
			}

			public Colour(Color source) : this(source.R, source.G, source.B)
			{
			}

			public bool IsSameColour(Colour source)
			{
				return R == source.R && G == source.G && B == source.B;
			}

			public void CopyFromColor(Color source)
			{
				R = source.R;
				G = source.G;
				B = source.B;
			}

			public Color ToColor()
			{
				return Color.FromArgb(R, G, B);
			}

			public override string ToString()
			{
				return $"{R} {G} {B}";
			}
		}

		public class Palette
		{
			public List<Colour> Colours { get; private set; } = new List<Colour>();

			public Colour this[int index] => Colours[index];

			public int MaxCount { get; set; } = 256;

			public int UsedCount
			{
				get => _usedCount;
				set
				{
					if (value < 0)
					{
						_usedCount = 0;
					}
					else if (value > Colours.Count)
					{
						_usedCount = Colours.Count;
					}
					else
					{
						_usedCount = value;
					}
				}
			}
			private int _usedCount = 0;

			public int TransparentColourIndex
			{
				get => _transparentColourIndex;
				set
				{
					// Note we use -1 in some parts when parsing, so we don't check for it here.
					_transparentColourIndex = value >= Colours.Count ? Colours.Count - 1 : value;
				}
			}
			private int _transparentColourIndex = 0;

			public Colour TransparentColour { get => Colours[TransparentColourIndex]; }

			#region Initialization & disposal

			public Palette()
			{
			}

			public Palette(Palette source)
			{
				foreach (var colour in source.Colours)
				{
					Colours.Add(colour);
				}

				MaxCount = source.MaxCount;
				UsedCount = source.UsedCount;
				TransparentColourIndex = source.TransparentColourIndex;
			}

			#endregion

			#region Overrides

			public override string ToString()
			{
				return $"{Colours.Count} (transparent={TransparentColourIndex} used={UsedCount} max={MaxCount})";
			}

			#endregion

			#region Colours handling

			/// <summary>
			/// Adds the given <see cref="Colour"/> and returns true. If <see cref="MaxCount"/> is used and palette is already full, colour is not added and false is returned.
			/// </summary>
			public bool Add(Colour colour)
			{
				if (Colours.Count >= MaxCount) return false;

				Colours.Add(colour);
				UsedCount = Colours.Count;

				return true;
			}

			/// <summary>
			/// Inserts the given <see cref="Colour"/> into the given index and returns true. If <see cref="MaxCount"/> is used and palette is already full, colour is not added and false is returned.
			/// </summary>
			public bool Insert(Colour colour, int index)
			{
				if (Colours.Count >= MaxCount) return false;

				Colours.Insert(index, colour);
				UsedCount = Colours.Count;

				return true;
			}

			/// <summary>
			/// If the given <see cref="Colour"/> is distinct from all other colours in the palette it's added, then true returned. If same color is already represented, colour is not added and false is returned. If <see cref="MaxCount"/> is used and palette is already full, colour is not added and false is returned.
			/// </summary>
			public bool AddIfDistinct(Colour colour)
			{
				if (HasColour(colour)) return false;

				return Add(colour);
			}

			/// <summary>
			/// If the given <see cref="Color"/> is distinct from all other colours in the palette, it's converted to <see cref="Colour"/> and added, then true returned. If same color is already represented, color is not added and false is returned. If <see cref="MaxCount"/> is used and palette is already full, colour is not added and false is returned.
			/// </summary>
			public bool AddIfDistinct(Color color)
			{
				return AddIfDistinct(new Colour(color));
			}

			/// <summary>
			/// Determines if the given <see cref="Colour"/> is already represented within the palette or not.
			/// </summary>
			/// <param name="colour"></param>
			/// <returns></returns>
			public bool HasColour(Colour colour)
			{
				foreach (var existingColour in Colours)
				{
					if (existingColour.IsSameColour(colour))
					{
						return true;
					}
				}

				return false;

			}

			#endregion

			#region Helpers

			/// <summary>
			/// Moves the colour from source index to destination index, updating all other colours as needed.
			/// </summary>
			public void MoveColour(int fromIndex, int toIndex)
			{
				Colours.Move(fromIndex, toIndex);
			}

			/// <summary>
			/// A helper for copying all colours from this palette to the model.
			/// </summary>
			public void CopyTo(Models.Palette palette)
			{
				palette.Clear();
				palette.Type = PaletteType.Custom;

				for (int c = 0; c < UsedCount; c++)
				{
					var colour = Colours[c];

					palette[c].Red = colour.R;
					palette[c].Green = colour.G;
					palette[c].Blue = colour.B;
				}

				palette.StartIndex = 0;
				palette.UsedCount = UsedCount;
				palette.TransparentIndex = TransparentColourIndex;
			}

			/// <summary>
			/// Determines if the given <see cref="Palette"/> can be fitted into this one or not. Distinct colours are taken into account, existing ones will be "reused" if possible. This palette is not modified though, the function only checks. The result is the number of new colours, IF given palette would be added to this one.
			/// </summary>
			public int CanFitPalette(Palette palette)
			{
				// We try to add into a copy so we don't mess up this palette.
				return new Palette(this).AddDistinctColoursFromPalette(palette);
			}

			/// <summary>
			/// Adds all distinct colours from given <see cref="Palette"/> and returns the number of added colours if succeeded, false if all colours could not fit (based on <see cref="MaxCount"/>). <see cref="CanFitPalette(Palette)"/> should be used before this to make sure all colours can fit, otherwise we can result in only a subset of colours added. In such case <see cref="int.MaxValue"/> will be returned to indicate the failure.
			/// </summary>
			public int AddDistinctColoursFromPalette(Palette palette)
			{
				var startingColoursCount = Colours.Count;

				// Add all distinct colours from given palette. Note we don't use `AddIfDistinctColour` because we need to differentiate between "not added because already exists" and "not added because running out of colours".
				foreach (var colour in palette.Colours)
				{
					// If the colour is already represented, skip it. This won't increment the colours count, so object can fit.
					if (HasColour(colour)) continue;

					// If colour is distinct, attempt to add it. However if palette is already full, bail out, this means we can't fit the whole of the source palette. The result in such case is the largest possible value to indicate a "impossible" result.
					if (!Add(colour)) return int.MaxValue;
				}

				// If we were able to fit all colours, return the number of added.
				var endingColourCount = Colours.Count;
				return endingColourCount - startingColoursCount;
			}

			#endregion
		}

		#endregion
	}
}
