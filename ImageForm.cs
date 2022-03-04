using NextGraphics.Utils;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NextGraphics
{
	public partial class ImageForm : Form
	{
		private Bitmap inputImage;
		private int blockXSize = 32;
		private int blockYSize = 32;
		private double pictureWidth;
		private double pictureHeight;
		private double pictureRatio;
		private double imageRatio;
		private double magicNumber;
		private double realPictureHeight;
		private double realPictureWidth;
		private double windowScale;
		private double windowScaleX;
		private double windowScaleY;
		private double windowLeftOffset;
		private double windowTopOffset;
		private float yscaleAdjust = 1.0f;

		#region Initialization & Disposal

		public ImageForm()
		{
			InitializeComponent();
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}

			if (disposing && inputImage != null)
			{
				inputImage.Dispose();
				inputImage = null;
			}

			base.Dispose(disposing);
		}

		#endregion

		#region Events

		private void sourcePictureBox_Paint(object sender, PaintEventArgs e)
		{
			// Paints a grid on top of the image.
			Graphics g = e.Graphics;
			g.InterpolationMode = InterpolationMode.NearestNeighbor;
			float xscale = scaleHScrollBar.Value / 50.0f;
			float yscale = scaleHScrollBar.Value * yscaleAdjust;
			sourcePictureBox.Width = (int)((float)sourceImagePanel.Width * xscale) - 32;
			sourcePictureBox.Height = (int)((float)sourceImagePanel.Height * yscale) - 32;

			g.DrawImage(sourcePictureBox.Image,
				new Rectangle(0, 0, sourcePictureBox.Width, sourcePictureBox.Height),	// destination rectangle
				0, 0,								// upper-left corner of source rectangle
				sourcePictureBox.Image.Width,		// width of source rectangle
				sourcePictureBox.Image.Height,		// height of source rectangle
				GraphicsUnit.Pixel);

			sourcePictureBox.Image.Render(e.Graphics, blockXSize, blockYSize, windowScaleX, windowScaleY);
		}

		private void sourceImagePanel_Resize(object sender, EventArgs e)
		{
			if (Visible)
			{
				UpdateVariables();
			}

			Invalidate();
			Update();
			Refresh();
		}

		private void scaleHScrollBar_Scroll(object sender, ScrollEventArgs e)
		{
			if (Visible)
			{
				UpdateVariables();
			}

			Invalidate(true);
			Update();
			Refresh();
		}

		#endregion

		#region Public

		public void LoadImage(string filename, int gridXSize, int gridYSize)
		{
			using (var fs = new System.IO.FileStream(filename, System.IO.FileMode.Open))
			{
				var bmp = new Bitmap(fs);
				inputImage = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format24bppRgb);
				for (int y = 0; y < bmp.Height; y++)
				{
					for (int x = 0; x < bmp.Width; x++)
					{
						inputImage.SetPixel(x, y, bmp.GetPixel(x, y));
					}
				}		
			}

			Width = inputImage.Width + 50;
			Height = inputImage.Height + 75;

			sourcePictureBox.Image = inputImage;
			sourcePictureBox.Width = Math.Max(inputImage.Width - 32, inputImage.Width);
			sourcePictureBox.Height = Math.Max(inputImage.Height - 32, inputImage.Height);

			yscaleAdjust = 0.03f;
			blockXSize = gridXSize;
			blockYSize = gridYSize;

			Invalidate(true);
			Update();
		}

		public void CopyImage(Bitmap image, int gridXSize, int gridYSize, float yAdjust = 0f)
		{
			if (yAdjust > 0f)
			{
				yscaleAdjust = yAdjust;
			}
			else
			{
				yscaleAdjust = (float)image.Width / (float)image.Height;
			}

			Width = image.Width + 50;
			Height = image.Height + 75;

			sourcePictureBox.Image = image;
			sourcePictureBox.Width = image.Width - 32;
			sourcePictureBox.Height = image.Height - 32;

			blockXSize = gridXSize;
			blockYSize = gridYSize;

			Invalidate(true);
			Update();
		}

		public void SetImage(Bitmap image)
		{
			inputImage = image;
			sourcePictureBox.Image = inputImage;
		}

		public void SetPixel(int x, int y, Color color)
		{
			inputImage.SetPixel(x, y, color);
		}

		#endregion

		#region Helpers

		private void UpdateVariables()
		{
			pictureWidth = sourcePictureBox.Width;
			pictureHeight = sourcePictureBox.Height;

			pictureRatio = (float)pictureWidth / pictureHeight;
			imageRatio = (float)((float)sourcePictureBox.Image.Width / (float)sourcePictureBox.Image.Height);
			magicNumber = imageRatio / pictureRatio;

			realPictureHeight = sourcePictureBox.Image.Height / magicNumber;
			realPictureWidth = sourcePictureBox.Image.Width / magicNumber;

			windowScaleX = (pictureWidth / (float)sourcePictureBox.Image.Width);
			windowScaleY = (pictureHeight / (float)sourcePictureBox.Image.Height);

			if ((pictureHeight / (float)sourcePictureBox.Image.Height) < (pictureWidth / (float)sourcePictureBox.Image.Width))
			{
				windowScale = windowScaleX;
			}
			else
			{
				windowScale = windowScaleY;
			}

			windowLeftOffset = (pictureWidth - (sourcePictureBox.Image.Width * windowScale)) * 0.5f;
			windowTopOffset = (pictureHeight - (sourcePictureBox.Image.Height * windowScale)) * 0.5f;
		}

		#endregion
	}
}
