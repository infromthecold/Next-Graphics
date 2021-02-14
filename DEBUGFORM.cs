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
	public partial class DEBUGFORM : Form
	{
		public	Bitmap	DEBUG_IMAGE;
		public	int	windowSale		=	2;
		public DEBUGFORM()
		{
			InitializeComponent();
			DEBUG_IMAGE				=	new	Bitmap(16*256,16*128, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			DEBUG_PICTURE.Image			=	DEBUG_IMAGE;			
			DEBUG_PICTURE.Height			=	DEBUG_IMAGE.Height*windowSale;
			DEBUG_PICTURE.Width			=	DEBUG_IMAGE.Width*windowSale;
		}
		public	void	SetScale(int scale)
		{		
			windowSale				=	scale;
			DEBUG_PICTURE.Height			=	DEBUG_IMAGE.Height*windowSale;
			DEBUG_PICTURE.Width			=	DEBUG_IMAGE.Width*windowSale;
		}
		private void DEBUGDisplay_Paint(object sender, PaintEventArgs e)
		{
			Graphics g	=	e.Graphics; 
			Pen pen = new Pen(Color.Black);
			float[] dashValues = { 1, 1};
			int	divLines	=	8*windowSale;			
			pen.DashPattern = dashValues;
			// horizontal lines
			for (int y = 0; y < (DEBUG_PICTURE.Image.Height / divLines)+1; ++y)
			{
				g.DrawLine(pen, 0, y * divLines, DEBUG_PICTURE.Image.Width, y * divLines);
			}			
			// verticle lines
			for (int x = 0; x < (DEBUG_PICTURE.Image.Width/divLines)+1; ++x)
			{
				g.DrawLine(pen, x * divLines, 0, x * divLines, DEBUG_PICTURE.Image.Height);
			}	
		}
	}
}
