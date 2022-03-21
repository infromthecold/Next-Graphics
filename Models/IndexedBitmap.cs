using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Models
{
	/// <summary>
	/// Provides byte array like access to a <see cref="Bitmap"/>.
	/// </summary>
	public class IndexedBitmap : IDisposable
	{
		/// <summary>
		/// Bitmap width in pixels.
		/// </summary>
		public int Width { get; private set; }

		/// <summary>
		/// Bitmap height in pixels.
		/// </summary>
		public int Height { get; private set; }

		/// <summary>
		/// Indicates whether bitmap is transparent.
		/// </summary>
		public bool Transparent { get; set; }

		/// <summary>
		/// The index of 16-colour palette bank. Only used for 4-bit data and when auto banks are selected. If auto-banking is not enabled, or for 8-bit data, this value will be -1.
		/// </summary>
		public int PaletteBank { get; set; } = -1;

		/// <summary>
		/// Specifies whether <see cref="PaletteBank"/> is set and therefore we can automatically select 16	colour bank.
		/// </summary>
		public bool IsAutoBankingSupported { get => PaletteBank >= 0; }

		private short[] Colours { get; set; }

		#region Initialization & disposal

		public IndexedBitmap(int width, int height)
		{
			Width = width;
			Height = height;
			Colours = new short[width * height];
		}

		~IndexedBitmap()
		{
			Dispose();
		}

		public void Dispose()
		{
			// Nothing to do here right now; we used to have a Bitmap that we disposed here, and since the rest of the code already takes care of calling dispose, leaving the empty implementation in place if we need to add some of the functionality in the future.
		}

		#endregion

		#region Pixel access

		/// <summary>
		/// Indexer access to underlying pixels, same as <see cref="GetPixel(int, int)"/> and <see cref="SetPixel(int, int, short)"/>
		/// </summary>
		public short this[int x, int y]
		{
			get => GetPixel(x, y);
			set => SetPixel(x, y, value);
		}

		/// <summary>
		/// Sets the colour index of the pixel at coordinates (x, y).
		/// </summary>
		public void SetPixel(int x, int y, short colourIndex)
		{
			int index = x + (y * Width);
			Colours[index] = colourIndex;
		}

		/// <summary>
		/// Gets the pixel value at coordinates (x, y).
		/// </summary>
		public short GetPixel(int x, int y)
		{
			int index = x + (y * Width);
			if (y > Height || x > Width || index >= Colours.Length)
			{
				return 0;
			}
			return Colours[index];
		}

		/// <summary>
		/// Iterates gixen subset of pixels from this bitmap.
		/// </summary>
		/// <param name="xOffset">Optional x offset.</param>
		/// <param name="yOffset">Optional y offset.</param>
		/// <param name="handler">Called for each pixel, parameters are:
		/// - x (0 to width)
		/// - y (0 to height)
		/// - colour index
		/// Result is true to continue iterating, false otherwise.
		/// </param>
		public void PixelIterator(int xOffset, int yOffset, Func<int, int, short, bool> handler)
		{
			for (int y = 0; y < Width; y++)
			{
				for (int x = 0; x < Height; x++)
				{
					var pixel = GetPixel(x + xOffset, y + yOffset);
					if (!handler(x, y, pixel))
					{
						return;
					}
				}
			}
		}

		#endregion

		#region Pixel manipulation

		/// <summary>
		/// Copies data from given source image. No size checking is currently performed, so make sure source image is large enough!
		/// </summary>
		public void CopyFrom(IndexedBitmap source, int sourceX, int sourceY)
		{
			for (int y = 0; y < Width; y++)
			{
				for (int x = 0; x < Height; x++)
				{
					var pixel = source.GetPixel(x + sourceX, y + sourceY);
					SetPixel(x, y, pixel);
				}
			}
		}

		/// <summary>
		/// Copies a portion of this bitmap into the given frame of the destination bitmap.
		/// </summary>
		/// <param name="destination">Destination bitmap to copy to.</param>
		/// <param name="palette">Palette that defines colours for this bitmap.</param>
		/// <param name="position">The position into destination to copy to.</param>
		public void CopyTo(Bitmap destination, Palette palette, Point position, bool flippedX = false, bool flippedY = false, bool rotated = false)
		{
			Point Translated(int x, int y)
			{
				// (x,y) is expected to be a coordinate within this bitmap (0..width-1, 0..height-1)
				var result = new Point(x, y);

				if (rotated)
				{
					var temp = result.X;
					result.X = Width - result.Y - 1;
					result.Y = temp;
				}

				if (flippedX)
				{
					result.X = Width - result.X - 1;
				}

				if (flippedY)
				{
					result.Y = Height - result.Y - 1;
				}

				return result;
			}

			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					var colourIndex = GetPixel(x, y);
					var color = palette[colourIndex].ToColor();

					var translated = Translated(x, y);
					var tx = position.X + translated.X;
					var ty = position.Y + translated.Y;

					destination.SetPixel(tx, ty, color);
				}
			}
		}

		/// <summary>
		/// Remaps all colour indexes for 4-bit palette.
		/// </summary>
		public void RemapTo4Bit(Palette palette, PaletteParsingMethod parsingMethod, int width, int height, int objectSize)
		{
			int Map(int xCuts, int yCuts, int averagingIndex)
			{
				// If averaging index is less than 0, then exact colours are matched without remapping.
				var lowestNonTransparentIndex = int.MaxValue;

				for (int y = 0; y < objectSize; y++)
				{
					for (int x = 0; x < objectSize; x++)
					{
						var pixel = GetPixel(x + (xCuts * objectSize), y + (yCuts * objectSize));
						var pixelColor = palette[pixel].ToColor();
						var colourIndex = palette.ClosestColor(pixelColor, (short)averagingIndex, 0);

						if (colourIndex != palette.TransparentIndex && colourIndex < lowestNonTransparentIndex)
						{
							lowestNonTransparentIndex = colourIndex;
						}

						SetPixel(x + (xCuts * objectSize), y + (yCuts * objectSize), colourIndex);
					}
				}

				// For all-transparent pixels we simply return transparent colour index. Otherwise the index of the first colour.
				return lowestNonTransparentIndex == int.MaxValue ? palette.TransparentIndex : lowestNonTransparentIndex;
			}

			void MapDefault()
			{
				int averagingIndex = 0;

				for (int yCuts = 0; yCuts < height / objectSize; yCuts++)
				{
					for (int xCuts = 0; xCuts < width / objectSize; xCuts++)
					{
						for (int y = 0; y < objectSize; y++)
						{
							for (int x = 0; x < objectSize; x++)
							{
								averagingIndex += GetPixel(x + (xCuts * objectSize), y + (yCuts * objectSize));
							}
						}
						averagingIndex = (averagingIndex / (objectSize * objectSize)) & 0x0f0;

						Map(xCuts, yCuts, averagingIndex);

						// We don't support auto-banking in this mode.
						PaletteBank = -1;
					}
				}
			}

			void MapBanks()
			{
				// This method assumes palette is setup so that 16-colour banks are possible for objects. It will select the first bank for first non-transparent pixel.
				var firstColourIndex = Map(0, 0, -1);

				// We must assign palette bank in this mode.
				PaletteBank = firstColourIndex / 16;
			}

			switch (parsingMethod)
			{
				case PaletteParsingMethod.ByPixels:
					MapDefault();
					break;

				case PaletteParsingMethod.ByObjects:
					MapBanks();
					break;
			}
		}

		#endregion

		#region Helpers

		private int Size { get => Math.Min(Width, Height); }
		private short IdenticalPixelProvider(IndexedBitmap bitmap, int x, int y) => bitmap.GetPixel(x, y);
		private short FlippedXPixelProvider(IndexedBitmap bitmap, int x, int y) => bitmap.GetPixel((Size - 1) - x, y);
		private short FlippedYPixelProvider(IndexedBitmap bitmap, int x, int y) => bitmap.GetPixel(x, (Size - 1) - y);
		private short FlippedXYPixelProvider(IndexedBitmap bitmap, int x, int y) => bitmap.GetPixel((Size - 1) - x, (Size - 1) - y);

		/// <summary>
		/// Compares this bitmap to the given one and returns the comparison result taking into account current model settings.
		/// </summary>
		/// <param name="model">The <see cref="MainModel"/> that defines parameters for comparison.</param>
		/// <param name="compareTo"><see cref="IndexedBitmap"/> to compare to</param>
		/// <param name="xOffset">Optional x offset to start comparing from.</param>
		/// <param name="yOffset">Optional y offset to start comparing from.</param>
		/// <returns>Returns the comparison result in form of <see cref="BlockType"/>.</returns>
		public BlockType RepeatedBlockType(MainModel model, IndexedBitmap compareTo, int xOffset = 0, int yOffset = 0)
		{
			// Do we ignore all repeats?
			if (model.IgnoreCopies)
			{
				return BlockType.Original;
			}

			bool IsColourBlock()
			{
				bool result = true;
				short firstPixel = GetPixel(xOffset, yOffset);

				PixelIterator(xOffset, yOffset, (x, y, colour) =>
				{
					if (colour != firstPixel)
					{
						// As soon as we find first pixel that's different from first, we can stop.
						result = false;
						return false;
					}

					// Continue iterating otherwise.
					return true;
				});

				return result;
			}

			bool HasSomeTransparentPixels()
			{
				var result = false;

				PixelIterator(xOffset, yOffset, (x, y, colour) =>
				{
					if (colour == (short)model.Palette.TransparentIndex)
					{
						// As soon as we find transparent colour, we know the result so we can stop iterating...
						result = true;
						return false;
					}

					// Continue iterating otherwise.
					return true;
				});

				return result;
			}

			float DeterminePixelComparisonCountBase()
			{
				float result = Size * Size;

				// Determine same pixel "ratio" we'll compare with. If this bitmap is single colour block, the whole size, otherwise we use approximation with current accuracy setting.
				if (!IsColourBlock())
				{
					result *= (model.Accuracy / 100f);
				}

				return result;
			}

			var objectByteSize = Size * Size;
			var hasTransparentPixels = HasSomeTransparentPixels();
			var pixelComparisonBase = DeterminePixelComparisonCountBase();
			var bitmapToCompareTo = compareTo;	// this is used in `IsMatch`, we later re-assign to rotated bitmap

			IndexedBitmap CreateRotated()
			{
				var result = new IndexedBitmap(Size, Size);

				for (int y = 0; y < Size; y++)
				{
					for (int x = 0; x < Size; x++)
					{
						result.SetPixel((Size - 1) - y, x, compareTo.GetPixel(x, y));
					}
				}

				return result;
			}

			int DetermineSamePixelsCount(Func<IndexedBitmap, int, int, short> comparer)
			{
				int result = Size * Size;

				PixelIterator(xOffset, yOffset, (x, y, colour) =>
				{
					if (colour != comparer(bitmapToCompareTo, x, y))
					{
						result--;
					}

					return true;
				});

				return result;
			}

			bool IsMatch(Func<IndexedBitmap, int, int, short> comparer)
			{
				var matchedPixelsCount = DetermineSamePixelsCount(comparer);
				return matchedPixelsCount >= pixelComparisonBase;
			}

			// If fully transparent, take the block as such.
			if (!model.IgnoreTransparentPixels && hasTransparentPixels && IsTransparent(model.Palette.TransparentIndex, xOffset, yOffset))
			{
				return BlockType.Transparent;
			}

			// See how close to the original block it is. If it's not match, we need to continue searching.
			var identicalPixelsCount = DetermineSamePixelsCount(IdenticalPixelProvider);

			// If transparent pixels should be ignored and there are some present, take the block as either repeated or original.
			if (model.IgnoreTransparentPixels && hasTransparentPixels)
			{
				return identicalPixelsCount == objectByteSize ? BlockType.Repeated : BlockType.Original;
			}

			// If it's close to original % and not containing transparent, it's repeated block!
			if (identicalPixelsCount >= pixelComparisonBase)
			{
				return BlockType.Repeated;
			}

			// Compare with all flipped variants.
			if (!model.IgnoreMirroredX && IsMatch(FlippedXPixelProvider)) return BlockType.FlippedX;
			if (!model.IgnoreMirroredY && IsMatch(FlippedYPixelProvider)) return BlockType.FlippedY;
			if (!model.IgnoreMirroredX && !model.IgnoreMirroredY && IsMatch(FlippedXYPixelProvider)) return BlockType.FlippedXY;

			// Compare with all rotated variants. Note how we switch bitmap we're comparing against with rotated variant. Also note rotated variants don't include repeated check from above.
			if (!model.IgnoreRotated)
			{
				bitmapToCompareTo = CreateRotated();

				if (IsMatch(IdenticalPixelProvider)) return BlockType.Rotated;
				if (!model.IgnoreMirroredX && IsMatch(FlippedXPixelProvider)) return BlockType.FlippedXRotated;
				if (!model.IgnoreMirroredY && IsMatch(FlippedYPixelProvider)) return BlockType.FlippedYRotated;
				if (!model.IgnoreMirroredX && !model.IgnoreMirroredY && IsMatch(FlippedXYPixelProvider)) return BlockType.FlippedXYRotated;
			}

			// If we didn't find any match, assume this is original block.
			return BlockType.Original;
		}

		/// <summary>
		/// Determines if this image is transparent (aka all pixels are indexes to given transparent colour).
		/// </summary>
		public bool IsTransparent(int transparentColourIndex, int xOffset = 0, int yOffset = 0)
		{
			bool result = true;

			PixelIterator(xOffset, yOffset, (x, y, colour) =>
			{
				// As soon as we find non-transparent colour, the image is not transparent either.
				if (colour != (short)transparentColourIndex)
				{
					result = false;
					return false;
				}

				// Continue iterating otherwise.
				return true;
			});

			return result;
		}

		#endregion
	}
}
