using NextGraphics.Models;
using NextGraphics.Utils;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NextGraphics
{
	public partial class DebugForm : Form
	{
		private Bitmap debugImage;
		private int windowScale = 2;

		public DebugForm()
		{
			InitializeComponent();
			debugImage = new Bitmap(16 * 256, 16 * 128, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			debugPictureBox.Image = debugImage;
			debugPictureBox.Height = debugImage.Height * windowScale;
			debugPictureBox.Width = debugImage.Width * windowScale;
		}

		public void ClearImage()
		{
			debugImage.Clear();
		}

		public void CopyImage(Palette palette, Point position, IndexedBitmap source)
		{
			source.CopyTo(debugImage, palette, position);
		}

		public void SetScale(int scale)
		{
			windowScale = scale;
			debugPictureBox.Height = debugImage.Height * windowScale;
			debugPictureBox.Width = debugImage.Width * windowScale;
		}

		private void debugPictureBox_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawGrid(debugPictureBox.Image, 8, 8, windowScale, windowScale);
		}
	}
}
