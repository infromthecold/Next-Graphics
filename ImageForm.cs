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
		private double ImageScaleFactor { get => scaleTrackBar.Value / ScaleFactor; }

		private Bitmap image;
		private Parameters parameters;

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

			if (disposing && image != null)
			{
				image.Dispose();
				image = null;
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

			var scaleFactor = ImageScaleFactor;

			// If we have dynamic image provider set, update the image. Ask for size that would fit our current view (but we will stretch the image if different size is returned).
			if (parameters != null && parameters.ImageProvider != null)
			{
				if (image != null) image.Dispose();

				image = parameters.ImageProvider(scaleFactor);
			}

			// Render the image. Since we always constrain picture box aspect ratio to our image, we simply stretch the image into the whole control. Note how we dynamically setup top-left coordinate based on scale. This ensures the image is aligned with the grid.
			if (image != null)
			{
				var topLeft = (int)(ImageScaleFactor / 2.0);

				g.DrawImage(image,
					new Rectangle(topLeft, topLeft, sourcePictureBox.Width, sourcePictureBox.Height),
					new Rectangle(0, 0, image.Width, image.Height),
					GraphicsUnit.Pixel);
			}

			// Render overlay if needed.
			if (parameters != null && parameters.OverlayProvider != null)
			{
				parameters.OverlayProvider(g, sourcePictureBox.Size, scaleFactor);
			}

			// Render grid if required.
			if (parameters != null && parameters.GridWidth != null && parameters.GridHeight != null && ImageScaleFactor >= 1)
			{
				double gridWidth = parameters.GridWidth();
				double gridHeight = parameters.GridHeight();
				g.DrawGrid(image, gridWidth, gridHeight, scaleFactor, scaleFactor);
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
				UpdateScale();
			}

			Invalidate(true);
			Update();
		}

		public void CopyImage(Bitmap image, Parameters parameters = null)
		{
			this.parameters = parameters;

			AssignImage(image);
			UpdateScale();

			Invalidate(true);
			Update();
		}

		public void SetImage(Bitmap image, Parameters parameters = null)
		{
			this.parameters = parameters;

			AssignImage(image);
			UpdateScale();
		}

		public void SetPixel(int x, int y, Color color)
		{
			image.SetPixel(x, y, color);
		}

		#endregion

		#region Helpers

		private void AssignImage(Bitmap image)
		{
			this.image = image;

			sourcePictureBox.Width = image.Width;
			sourcePictureBox.Height = image.Height;
		}

		private void UpdateScale()
		{
			var scaledWidth = (int)(image.Width * ImageScaleFactor);
			var scaledHeight = (int)(image.Height * ImageScaleFactor);

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

			/// <summary>
			/// Optional, if non-null, it will be called on every reload to dynamically update the image. The parameter is current scale factor in case implementor want to do scale-sensitive drawing.
			/// </summary>
			public Func<double, Bitmap> ImageProvider { get; set; } = null;

			/// <summary>
			/// Optional, if non-null, the image is first rendered and scaled, then this method is called passing it the scaling factor, size of the rendered image (taking into account scaling factor) and <see cref="Graphics"/> that can be used to render additional data. After this call completes, the grid is rendered on top.
			/// </summary>
			public Action<Graphics, Size, double> OverlayProvider { get; set; } = null;
		}

		#endregion
	}
}
