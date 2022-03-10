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
		/// <summary>
		/// Changes settings value and saves.
		/// </summary>
		internal static void SetAndSave(this Properties.Settings settings, string name, object value)
		{
			settings[name] = value;
			settings.Save();
		}

		/// <summary>
		/// Fills the given <see cref="Bitmap"/> with magenta.
		/// </summary>
		public static void Clear(this Bitmap bitmap)
		{
			using (Graphics g = Graphics.FromImage(bitmap))
			{
				using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 255, 0, 255)))
				{
					g.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
				}
			}
		}

		/// <summary>
		/// Returns either black or white <see cref="Color"/> so it will be visible on the given color.
		/// </summary>
		public static Color FittingBlackOrWhite(this Color color)
		{
			return color.GetBrightness() > 0.4f ? Color.Black : Color.White;
		}

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

		/// <summary>
		/// Renders a grid into the given <see cref="Image"/>.
		/// </summary>
		public static void Render(
			this Image image,
			Graphics g,
			int gridX,
			int gridY,
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
	}
}
