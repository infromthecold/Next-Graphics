using NextGraphics.Models;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Utils
{
	public static class CommonExtensions
	{
		#region Settings

		/// <summary>
		/// Changes settings value and saves.
		/// </summary>
		internal static void SetAndSave(this Properties.Settings settings, string name, object value)
		{
			settings[name] = value;
			settings.Save();
		}

		#endregion

		#region Lists

		/// <summary>
		/// Moves the given item within the list, effectively changing positions of all affected items in between from and to index. Will throw exception if either index is invalid.
		/// </summary>
		public static void Move<T>(this List<T> list, int fromIndex, int toIndex)
		{
			if (fromIndex == toIndex) return;

			var item = list[fromIndex];

			list.RemoveAt(fromIndex);

			if (toIndex > fromIndex)
			{
				toIndex--;
			}

			list.Insert(toIndex, item);
		}

		/// <summary>
		/// Swaps the positions of the given 2 elements of this list.
		/// </summary>
		public static void Swap<T>(this List<T> list, int index1, int index2)
		{
			if (index1 == index2) return;

			var item = list[index1];

			list[index1] = list[index2];
			list[index2] = item;
		}

		#endregion

		#region Colours

		/// <summary>
		/// Returns either black or white <see cref="Color"/> so it will be visible on the given color.
		/// </summary>
		public static Color FittingBlackOrWhite(this Color color)
		{
			return color.GetBrightness() > 0.4f ? Color.Black : Color.White;
		}

		#endregion

		#region Bitmaps

		public static void CopyRegionIntoBlock(
			this Bitmap bitmap,
			MainModel model,
			Rectangle region,
			IndexedBitmap destBlock,
			SpriteInfo destSpriteInfo = null)
		{
			var palette = model.Palette;

			var spriteWidth = Math.Max(1, model.GridWidth / model.ObjectSize);
			var spriteHeight = Math.Max(1, model.GridHeight / model.ObjectSize);

			var spriteInfo = destSpriteInfo ?? new SpriteInfo(spriteWidth, spriteHeight);

			// Clip because images may not be in blocks size 
			if (region.Y + region.Height > bitmap.Height)
			{
				region.Height = bitmap.Height - region.Y;
			}

			if (region.X + region.Width > bitmap.Width)
			{
				region.Width = bitmap.Width - region.X;
			}

			if (model.SpritesReduced && model.OutputType == OutputType.Sprites)
			{
				// so we make the output block all transparent
				for (int y = 0; y < region.Height; y++)
				{
					for (int x = 0; x < region.Width; x++)
					{
						destBlock.SetPixel(x, y, (short)palette.TransparentIndex);
					}
				}

				for (int y = 0; y < region.Height; y++)
				{
					for (int x = 0; x < region.Width; x++)
					{
						var pixel = bitmap.GetPixel(region.X + x, region.Y + y);
						var colour = palette.ClosestColor(pixel, -1, palette.StartIndex);

						if (colour != (short)palette.TransparentIndex)
						{
							spriteInfo.OffsetY = (short)y;
							goto checkLeft;
						}
					}
				}

			checkLeft:
				for (int x = 0; x < region.Width; x++)
				{
					for (int y = 0; y < region.Height; y++)
					{
						var pixel = bitmap.GetPixel(region.X + x, region.Y + y);
						var colour = palette.ClosestColor(pixel, -1, palette.StartIndex);

						if (colour != (short)palette.TransparentIndex)
						{
							spriteInfo.OffsetX = (short)x;
							goto xYDone;
						}
					}
				}

			xYDone:
				for (int y = spriteInfo.OffsetY; y < region.Height; y++)
				{
					for (int x = spriteInfo.OffsetX; x < region.Width; x++)
					{
						var pixel = bitmap.GetPixel(region.X + x, region.Y + y);
						var colour = palette.ClosestColor(pixel, -1, palette.StartIndex);
						destBlock.SetPixel(x - spriteInfo.OffsetX, y - spriteInfo.OffsetY, colour);
					}
				}
			}
			else
			{
				spriteInfo.ClearOffset();

				for (int y = 0; y < region.Height; y++)
				{
					for (int x = 0; x < region.Width; x++)
					{
						var pixel = bitmap.GetPixel(region.X + x, region.Y + y);
						var colour = palette.ClosestColor(pixel, -1, palette.StartIndex);
						destBlock.SetPixel(x, y, colour);
					}
				}
			}
		}

		#endregion

		#region Drawing

		/// <summary>
		/// Fills the given <see cref="Bitmap"/> with magenta and returns the same image (so it can be used in builder like flow pattern).
		/// </summary>
		public static Bitmap Clear(this Bitmap bitmap)
		{
			using (Graphics g = Graphics.FromImage(bitmap))
			{
				using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 255, 0, 255)))
				{
					g.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
				}
			}

			return bitmap;
		}

		/// <summary>
		/// Clears the given <see cref="Graphics"/> with magenta.
		/// </summary>
		public static void Clear(this Graphics graphics)
		{
			graphics.Clear(Color.Magenta);
		}

		/// <summary>
		/// Renders a grid with the given <see cref="Graphics"/> using given <see cref="Image"/> width and height.
		/// </summary>
		public static void DrawGrid(
			this Graphics g,
			Image image,
			double gridX,
			double gridY,
			double scaleX = 1,
			double scaleY = 1)
		{
			Pen pen = new Pen(Color.Black)
			{
				DashPattern = new float[] { 1, 1 }
			};

			// Horizontal lines.
			double y = 0;
			while (y < image.Height * scaleY)
			{
				g.DrawLine(pen, 0, (int)y, (int)(image.Width * scaleX), (int)y);

				y += gridY * scaleY;
			}

			// Vertical lines.
			double x = 0;
			while (x < image.Width * scaleY)
			{
				g.DrawLine(pen, (int)x, 0, (int)x, (int)(image.Height * scaleY));

				x += gridX * scaleX;
			}
		}

		#endregion
	}
}
