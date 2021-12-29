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
		/// Underlying <see cref="Bitmap"/>.
		/// </summary>
		public Bitmap Bitmap { get; private set; }

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

		private short[] Colours { get; set; }
		private GCHandle ColourHandler { get; set; }

		#region Initialization & disposal

		public IndexedBitmap(int width, int height)
		{
			Width = width;
			Height = height;
			Colours = new short[width * height];
			ColourHandler = GCHandle.Alloc(Colours, GCHandleType.Pinned);
			Bitmap = new Bitmap(width, height, width, PixelFormat.Format8bppIndexed, ColourHandler.AddrOfPinnedObject());
		}

		~IndexedBitmap()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (Bitmap != null)
			{
				ColourHandler.Free();
				Bitmap.Dispose();
				Bitmap = null;
			}
		}

		#endregion

		#region Helpers

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
		/// <param name="palette">Palette that defines colours for this bitmap.</param>
		/// <param name="rectangle">Rectangle that defines: width and height for this (source) bitmap and destination (position and size) of the given destination bitmap.</param>
		/// <param name="destination">Destination bitmap to copy to.</param>
		public void CopyTo(Palette palette, Rectangle rectangle, Bitmap destination)
		{
			for (int y = 0; y < rectangle.Height; y++)
			{
				for (int x = 0; x < rectangle.Width; x++)
				{
					var colourIndex = GetPixel(x, y);
					var color = palette[colourIndex].ToColor();
					destination.SetPixel(rectangle.X + x, rectangle.Y + y, color);
				}
			}
		}

		/// <summary>
		/// Remaps all colour indexes for 4-bit palette.
		/// </summary>
		public void RemapTo4Bit(Palette palette, int width, int height, int objectSize)
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

					for (int y = 0; y < objectSize; y++)
					{
						for (int x = 0; x < objectSize; x++)
						{
							var pixel = GetPixel(x + (xCuts * objectSize), y + (yCuts * objectSize));
							var pixelColor = palette[pixel].ToColor();
							var colourIndex = palette.ClosestColor(pixelColor, (short)averagingIndex, 0);
							SetPixel(x + (xCuts * objectSize), y + (yCuts * objectSize), colourIndex);
						}
					}
				}
			}
		}

		/// <summary>
		/// Determines if at least one pixel is transparent and returns true if so. Returns false if no transparent pixel is present.
		/// </summary>
		public bool HasTransparentPixels(int transparentColourIndex, int xOffset = 0, int yOffset = 0, int size = 0)
		{
			var result = false;

			PixelIterator(xOffset, yOffset, size, (x, y, colour) =>
			{
				if (colour == (short)transparentColourIndex)
				{
					// As soon as we find transparent colour, we know the result...
					result = true;
					return false;
				}

				// Continue iterating otherwise.
				return true;
			});

			return result;
		}

		/// <summary>
		/// Determines if this image is transparent (aka all pixels are indexes to given transparent colour).
		/// </summary>
		public bool IsTransparent(int transparentColourIndex, int xOffset = 0, int yOffset = 0, int size = 0)
		{
			bool result = true;

			PixelIterator(xOffset, yOffset, size, (x, y, colour) =>
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

		/// <summary>
		/// Determines if all pixels are the same (note this is also true if all pixels are transparent).
		/// </summary>
		public bool IsColourBlock(int xOffset = 0, int yOffset = 0, int size = 0)
		{
			bool result = true;
			short firstPixel = GetPixel(xOffset, yOffset);

			PixelIterator(xOffset, yOffset, size, (x, y, colour) =>
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

		/// <summary>
		/// Iterates gixen subset of pixels from this bitmap.
		/// </summary>
		/// <param name="xOffset">Optional x offset.</param>
		/// <param name="yOffset">Optional y offset.</param>
		/// <param name="size">Optiona size, if 0, then <see cref="Width"/> and <see cref="Height"/> are used (taking into account both offsets).</param>
		/// <param name="handler">Called for each pixel, parameters are:
		/// - x (0 to width)
		/// - y (0 to height)
		/// - colour index
		/// Result is true to continue iterating, false otherwise.
		/// </param>
		public void PixelIterator(int xOffset, int yOffset, int size, Func<int, int, short, bool> handler)
		{
			var width = size > 0 ? size : (Width - xOffset);
			var height = size > 0 ? size : (Height - yOffset);

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
	}
}
