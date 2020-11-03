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
	public partial class Progress : Form
	{
		bool	progressState	=	true;
		public Progress()
		{
			InitializeComponent();
		}
		public	bool	progress(bool steps)
		{
			if(this.progressBar1.Value == progressBar1.Maximum)
			{
				this.progressBar1.Value		=	0;
			}
			else if(steps==true)
			{ 
				this.progressBar1.PerformStep();
			}			
			this.Validate(true);
			this.Update();
			Application.DoEvents();
			return	progressState;
		}
		public	void	setProgress()
		{			
			this.progressBar1.Value	=	0;
		}
		private void button1_Click(object sender, EventArgs e)
		{
			progressState	=	false;
		}
	}
}