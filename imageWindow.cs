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
	public partial class imageWindow : Form
	{
		
		public	Bitmap	inputImage;
		public	int	blockXSize	=	32;
		public	int	blockYSize	=	32;
		public	double	pictureWidth;		
		public	double	pictureHeight;		
		public	double	pictureRatio;		
		public	double	imageRatio;		
		public	double	MagicNumber;		
		public	double	realPictureHeight;
		public	double	realPictureWidth;		
		public	double	windowScale;			
		public	double	windowScaleX;			
		public	double	windowScaleY;		
		public	double	windowLeftOffset;	
		public	double	windowTopOffset;
		private	float	yscaleAdjust		=	1.0f;
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
			using (var fs = new System.IO.FileStream(fullPath, System.IO.FileMode.Open))
			{
				var bmp		=	new Bitmap(fs);
				inputImage	=	new Bitmap(bmp.Width,bmp.Height,PixelFormat.Format24bppRgb);
				for(int y=0;y<bmp.Height;y++)
				{ 
					for(int x=0;x<bmp.Width;x++)
					{
						inputImage.SetPixel(x,y,bmp.GetPixel(x,y));
					}
				}
				//inputImage	= new Bitmap(bmp.Width,bmp.Height);
				//inputImage	= (Bitmap) bmp.Clone();				
			}	
		
			this.Width		=	inputImage.Width+50;
			this.Height		=	inputImage.Height+75;
			this.srcPicture.Image	=	inputImage;
			yscaleAdjust		=	0.03f;
			this.srcPicture.Width	=	Math.Max(inputImage.Width-32,inputImage.Width);
			this.srcPicture.Height	=	Math.Max(inputImage.Height-32,inputImage.Height);
			blockXSize		=	gridXSize;
			blockYSize		=	gridYSize;
			this.Invalidate(true);
			this.Update();
		}

		public	void	copyImage(Bitmap sourceImage,int gridXSize,int gridYSize,bool blocksWindow)
		{
			if(blocksWindow==true)
			{ 
				yscaleAdjust	=	0.3f;
			}
			else
			{
				yscaleAdjust	=	1.0f;
			}

			this.Width		=	sourceImage.Width+50;
			this.Height		=	sourceImage.Height+75;
			this.Invalidate(true);
			this.Update();
			this.srcPicture.Image	=	sourceImage;

			this.srcPicture.Width	=	sourceImage.Width-32;
			this.srcPicture.Height	=	sourceImage.Height-32;
			blockXSize		=	gridXSize;
			blockYSize		=	gridYSize;
			this.Invalidate(true);
			this.Update();
		}

		private	void  updateWindowChange() 
		{
			pictureWidth			=	this.srcPicture.Width;
			pictureHeight			=	this.srcPicture.Height;
			pictureRatio			=	(float)pictureWidth/pictureHeight;
			imageRatio			=	(float)((float)this.srcPicture.Image.Width/(float)this.srcPicture.Image.Height);
			MagicNumber			=	imageRatio/pictureRatio;	
			realPictureHeight		=	this.srcPicture.Image.Height/MagicNumber;
			realPictureWidth		=	this.srcPicture.Image.Width/MagicNumber;
			windowScaleX			=	(pictureWidth/(float)this.srcPicture.Image.Width);
			windowScaleY			=	(pictureHeight/(float)this.srcPicture.Image.Height);
			if((pictureHeight/(float)this.srcPicture.Image.Height)<(pictureWidth/(float)this.srcPicture.Image.Width))
			{ 
				windowScale		=	windowScaleX;
			}
			else
			{ 
				windowScale		=	windowScaleY;
			}
			windowLeftOffset		=	(pictureWidth-(this.srcPicture.Image.Width*windowScale))*0.5f;
			windowTopOffset			=	(pictureHeight-(this.srcPicture.Image.Height*windowScale))*0.5f;
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// paint the grig over the image
		//
		//-------------------------------------------------------------------------------------------------------------------
		private void srcWindowPaint(object sender, PaintEventArgs e)
		{
			Graphics g		=	e.Graphics; 	
			g.InterpolationMode	=	InterpolationMode.NearestNeighbor;
			float	xscale		=	scaleBar.Value/50.0f;
			float	yscale		=	scaleBar.Value*yscaleAdjust;
			this.srcPicture.Width	=	(int)((float)panel1.Width*xscale)-32;
			this.srcPicture.Height	=	(int)((float)panel1.Height*yscale)-32;

			g.DrawImage(	this.srcPicture.Image,
					new Rectangle(0, 0, this.srcPicture.Width, this.srcPicture.Height),
					// destination rectangle
					0,
					0,           // upper-left corner of source rectangle
					this.srcPicture.Image.Width,       // width of source rectangle
					this.srcPicture.Image.Height,      // height of source rectangle
					GraphicsUnit.Pixel);

			Pen pen		=	new Pen(Color.Black);
			float[]		dashValues = { 4, 2};			
			pen.DashPattern = dashValues;
			// horizontal lines
			Point	fromLine	=	new	Point();
			Point	tooLine		=	new	Point();
			int	xScale		=	(int)(blockXSize *windowScaleX);
			int	yScale		=	(int)(blockYSize *windowScaleY);
			for (int y = 0; y < this.srcPicture.Image.Height / blockYSize; ++y)
			{
				fromLine.X = 0;
				fromLine.Y = y *  yScale;
				tooLine.X = this.srcPicture.Image.Width*xScale;
				tooLine.Y = y * yScale;

				g.DrawLine(pen,fromLine.X,fromLine.Y ,tooLine.X ,tooLine.Y );
			}			
			// verticle lines
			for (int x = 0; x < this.srcPicture.Image.Width/blockXSize; ++x)
			{
				fromLine.X =	x * xScale;
				fromLine.Y =	0;
				tooLine.X =	x * xScale;
				tooLine.Y =	this.srcPicture.Image.Height*yScale;					
				g.DrawLine(pen,fromLine.X,fromLine.Y ,tooLine.X ,tooLine.Y );
			}	
		}
		
		//-------------------------------------------------------------------------------------------------------------------
		//
		// load an image into the window
		//
		//-------------------------------------------------------------------------------------------------------------------
		private void resize(object sender, EventArgs e)
		{			
			if(this.Visible==true)
			{ 
				updateWindowChange();				
			}
			this.Invalidate();
			this.Update();
			this.Refresh();
		}

		private void scroll(object sender, ScrollEventArgs e)
		{
			if(this.Visible==true)
			{ 
				updateWindowChange();
				
			}
			this.Invalidate(true);
			this.Update();
			this.Refresh();
		}
	}
}
