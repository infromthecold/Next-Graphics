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
	public partial class infoWindow : Form
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
		//private	float	yscaleAdjust		=	1.0f;
		//-------------------------------------------------------------------------------------------------------------------
		//
		// new window 
		//
		//-------------------------------------------------------------------------------------------------------------------
		public infoWindow()
		{

			InitializeComponent();	
		}	

	}
}
