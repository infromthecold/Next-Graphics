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
