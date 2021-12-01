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
	public enum centers
	{
		TopLeft,
		TopCenter,
		TopRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		BottomLeft,
		BottomCenter,
		BottomRight
	}
	public partial class settingsPanel : Form
	{
		
		public	int		centerPosition	=	4;
		public settingsPanel()
		{
			InitializeComponent();
			MC.Checked		=	true;
			centerPosition		=	4;
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// ok
		//
		//-------------------------------------------------------------------------------------------------------------------

		private void okButonClick(object sender, EventArgs e)
		{

			if(TL.Checked==true)
			{
				centerPosition	=	0;
			}
			else if(TC.Checked==true)
			{
				centerPosition	=	1;
			}
			else if(TR.Checked==true)
			{
				centerPosition	=	2;
			}
			else if(ML.Checked==true)
			{
				centerPosition	=	3;
			}
			else if(MC.Checked==true)
			{
				centerPosition	=	4;
			}
			else if(MR.Checked==true)
			{
				centerPosition	=	5;
			}
			else if(BL.Checked==true)
			{
				centerPosition	=	6;
			}
			else if(BC.Checked==true)
			{
				centerPosition	=	7;
			}
			else //if(BR.Checked==true)
			{
				centerPosition	=	8;
			}
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// set from settings
		//
		//-------------------------------------------------------------------------------------------------------------------

		public void setCenter(int center)
		{
			switch(center)
			{
				case	0:
					TL.Checked	=	true;
				break;
				case	1:
					TC.Checked	=	true;
				break;
				case	2:
					TR.Checked	=	true;
				break;
				case	3:
					ML.Checked	=	true;
				break;
				case	4:
					MC.Checked	=	true;
				break;
				case	5:
					MR.Checked	=	true;
				break;
				case	6:
					BL.Checked	=	true;
				break;
				case	7:
					BC.Checked	=	true;
				break;
				case	8:
					BR.Checked	=	true;
				break;
			}			
		}

		private void binaryOut_CheckedChanged(object sender, EventArgs e)
		{
			binaryBlocks.Enabled	=	 binaryOut.Checked;
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{

		}
    }
}
