using NextGraphics.Utils;

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace NextGraphics
{
	public partial class ImageForm : Form
	{
		private static double ScaleFactor = 10.0;

		private Bitmap inputImage;
		private Parameters parameters;

		private double ImageScaleFactor { get => scaleTrackBar.Value / ScaleFactor; }

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
			// Draw pixel perfect images (as much as possible, depending on image scale factor).
			var g = e.Graphics;
			g.InterpolationMode = InterpolationMode.NearestNeighbor;
			g.Clear();

			// Render the image. Since we always constrain picture box aspect ratio to our image, we simply stretch the image into the whole control. Note how we dynamically setup top-left coordinate based on scale. This ensures the image is aligned with the grid.
			if (inputImage != null)
			{
				var topLeft = (int)(ImageScaleFactor / 2.0);

				g.DrawImage(inputImage,
					new Rectangle(topLeft, topLeft, sourcePictureBox.Width, sourcePictureBox.Height),
					new Rectangle(0, 0, inputImage.Width, inputImage.Height),
					GraphicsUnit.Pixel);
			}

			// Render grid if required.
			if (parameters != null)
			{
				var gridWidth = parameters.GridWidth != null ? parameters.GridWidth() : 32;
				var gridHeight = parameters.GridHeight != null ? parameters.GridHeight() : 32;
				sourcePictureBox.Image.RenderGrid(e.Graphics, gridWidth, gridHeight, ImageScaleFactor, ImageScaleFactor);
			}
		}

		private void sourceImagePanel_Resize(object sender, EventArgs e)
		{
			Invalidate();
			Update();
			Refresh();
		}

		private void scaleTrackBar_ValueChanged(object sender, EventArgs e)
		{
			UpdateScale();

			scaleNumericUpDown.ValueChanged -= scaleNumericUpDown_ValueChanged;
			scaleNumericUpDown.Value = (decimal)ImageScaleFactor;
			scaleNumericUpDown.ValueChanged += scaleNumericUpDown_ValueChanged;
		}

		private void scaleNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			scaleTrackBar.Value = (int)((double)scaleNumericUpDown.Value * ScaleFactor);
		}

		#endregion

		#region Public

		public void LoadImage(string filename, int gridWidth, int gridHeight)
		{
			using (var stream = new System.IO.FileStream(filename, System.IO.FileMode.Open))
			{
				parameters = new Parameters
				{
					GridWidth = () => gridWidth,
					GridHeight = () => gridHeight,
				};

				AssignImage(new Bitmap(stream));
			}

			Invalidate(true);
			Update();
		}

		public void CopyImage(Bitmap image, Parameters parameters)
		{
			this.parameters = parameters;

			AssignImage(image);

			Invalidate(true);
			Update();
		}

		public void SetImage(Bitmap image)
		{
			AssignImage(image);
		}

		public void SetPixel(int x, int y, Color color)
		{
			inputImage.SetPixel(x, y, color);
		}

		#endregion

		#region Helpers

		private void AssignImage(Bitmap image)
		{
			inputImage = image;

			sourcePictureBox.Image = image;
			sourcePictureBox.Width = image.Width;
			sourcePictureBox.Height = image.Height;

			UpdateScale();
		}

		private void UpdateScale()
		{
			var scaledWidth = (int)(inputImage.Width * ImageScaleFactor);
			var scaledHeight = (int)(inputImage.Height * ImageScaleFactor);

			sourcePictureBox.Width = scaledWidth;
			sourcePictureBox.Height = scaledHeight;
			sourcePictureBox.Invalidate();
			sourcePictureBox.Update();
		}

		#endregion

		#region Declarations

		public class Parameters
		{
			public Func<int> GridWidth { get; set; } = null;
			public Func<int> GridHeight { get; set; } = null;
		}

		#endregion
	}
}
