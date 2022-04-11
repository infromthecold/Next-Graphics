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
	public partial class InfoForm : Form
	{
		#region Initialization & Disposal
		
		public InfoForm()
		{
			InitializeComponent();	
		}

		#endregion

		#region Public

		public void Append(Color color, string text)
		{
			infoTextBox.SelectionColor = color;
			infoTextBox.AppendText(text);
		}

		public void Clear()
		{
			infoTextBox.Clear();
		}

		#endregion
	}
}
