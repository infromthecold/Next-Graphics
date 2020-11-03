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
	public partial class imageWindow : Form
	{
		
		public	Bitmap	inputImage;
		public	int	blockXSize	=	32;
		public	int	blockYSize	=	32;

		//-------------------------------------------------------------------------------------------------------------------
		//
		// new window 
		//
		//-------------------------------------------------------------------------------------------------------------------
		public imageWindow()
		{
			InitializeComponent();
		}	

		//-------------------------------------------------------------------------------------------------------------------
		//
		// load an image into the window
		//
		//-------------------------------------------------------------------------------------------------------------------

		public	void	loadImage(string fullPath,int gridXSize,int gridYSize)
		{
			inputImage		=	new Bitmap(fullPath);
			this.srcPicture.Image	=	inputImage;
			this.srcPicture.Width	=	this.srcPicture.Image.Width;
			this.srcPicture.Height	=	this.srcPicture.Image.Height;	
			this.Width		=	this.srcPicture.Image.Width+50;
			this.Height		=	this.srcPicture.Image.Height+75;	
			blockXSize		=	gridXSize;
			blockYSize		=	gridYSize;
			this.Invalidate(true);
			this.Update();
		}
		
		//-------------------------------------------------------------------------------------------------------------------
		//
		// paint the grig over the image
		//
		//-------------------------------------------------------------------------------------------------------------------
		private void srcWindowPaint(object sender, PaintEventArgs e)
		{
			Graphics g	=	e.Graphics; 
			Pen pen = new Pen(Color.Black);
			float[] dashValues = { 4, 2};

			pen.DashPattern = dashValues;
			// horizontal lines
			for (int y = 0; y < this.srcPicture.Image.Height / blockYSize; ++y)
			{
				g.DrawLine(pen, 0, y * blockYSize, this.srcPicture.Image.Width, y * blockYSize);
			}			
			// verticle lines
			for (int x = 0; x < this.srcPicture.Image.Width/blockXSize; ++x)
			{
				g.DrawLine(pen, x * blockXSize, 0, x * blockXSize, this.srcPicture.Image.Height);
			}	
		}
		
		//-------------------------------------------------------------------------------------------------------------------
		//
		// load an image into the window
		//
		//-------------------------------------------------------------------------------------------------------------------
		private void resize(object sender, EventArgs e)
		{			
			this.Invalidate();
			this.Update();
		}
	}
}
