using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using NextGraphics.Models;

namespace NextGraphics
{
	public partial class PaletteForm : Form
	{
		public MainModel Model {
			get => _model;
			set {
				_model = value;
				imageSelectForm.Model = value;
			}
		}
		private MainModel _model;

		private Palette Palette { get => Model.Palette; } // a shortcut instead of having to write Model.Palette

		private ImageSelectForm imageSelectForm = new ImageSelectForm();

		public Button[] colourButtons = null;
		public Button colourClicked;
		private Color selectedColor = Color.FromArgb(64, 64, 64);

		private bool isFormLoaded = false;
		private bool isLittleEndian = false;
		private bool isTransparentColourPickingActive = false;

		#region Initialization & Disposal

		public PaletteForm()
		{
			InitializeComponent();
			this.Width		=	520;
			this.Height		=	428;
			this.MinimumSize	=	new Size(this.Width, this.Height);
			this.MaximumSize	=	new Size(this.Width, this.Height);
			isLittleEndian		=	BitConverter.IsLittleEndian;
		}

		#endregion

		#region Events - Form

		private void PaletteForm_Load(object sender, EventArgs e)
		{
			isFormLoaded = false;

			CreatePaletteButtons();

			switch (Palette.Type)
			{
				case PaletteType.Next256:
					next256TypeRadioButton.Checked = true;
					messageLabel.Visible = false;
					break;

				case PaletteType.Next512:
					next512TypeRadioButton.Checked = true;
					messageLabel.Visible = true;
					break;
				default:
					customTypeRadioButton.Checked = true;
					messageLabel.Visible = false;
					break;
			}

			for (int c = 0; c < colourButtons.Length; c++)
			{
				colourButtons[c].BackColor = Palette[c].ToColor();
				colourButtons[c].Text = "";

				if (Palette.Type == PaletteType.Custom && c > Palette.StartIndex && c <= Palette.LastValidIndex)
				{
					colourButtons[c].FlatAppearance.BorderColor = Palette[c].ToColor();
				}
				else
				{
					colourButtons[c].FlatAppearance.BorderColor = SystemColors.ControlDark;
				}
			}

			colourButtons[Palette.TransparentIndex].Text = "X";
			tColourIndex1.Text = Palette.TransparentIndex.ToString();

			colourCountTextBox.Text = Palette.UsedCount.ToString();
			startIndexTextBox.Text = Palette.StartIndex.ToString();

			isFormLoaded = true;
		}

		private void PaletteForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (colourCountTextBox.Text == "255" && Palette.Type == PaletteType.Custom)
			{
				var result = MessageBox.Show("Have you forgotten to set the number of colours to use?", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
				if (result == DialogResult.Cancel)
				{
					e.Cancel = true;
				}
			}
		}

		#endregion

		#region Events - Colour Buttons

		private void ColourButtonClick(object sender, EventArgs e)
		{
			colourClicked = (Button)sender;
			if (isTransparentColourPickingActive)
			{
				var selectedColourIndex = int.Parse(colourClicked.Name);
				if (selectedColourIndex > Palette.LastValidIndex)
				{
					MessageBox.Show("Transparent Colour out of range of used colours", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
					isTransparentColourPickingActive = false;
					return;
				}

				if ((selectedColourIndex & 15) != 0)
				{
					if (MessageBox.Show("Move Transparent Colour to the first palette colour", "Move", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						int row = selectedColourIndex / 16;
						Color temp = colourButtons[row * 16].BackColor;
						colourButtons[row * 16].BackColor = colourButtons[(row * 16) + (selectedColourIndex & 15)].BackColor;
						colourButtons[(row * 16) + (selectedColourIndex & 15)].BackColor = temp;
						colourClicked = colourButtons[row * 16];
						Palette.ReplaceColours(row * 16, (row * 16) + (selectedColourIndex & 15));
					}
				}

				SetTransparentColour(ref colourClicked);
				isTransparentColourPickingActive = false;
			}
			else
			{
				if (Palette.Type == PaletteType.Custom)
				{
					Point lowerLeft = new Point(0, colourClicked.Height);
					lowerLeft = colourClicked.PointToScreen(lowerLeft);
					copyMenu.Show(lowerLeft);
				}
			}
		}

		private void ColourButtonMouseOver(object sender, EventArgs e)
		{
			Button thisButton = (Button)sender;
			hexColourTextBox.Text = "#" + thisButton.BackColor.R.ToString("X2") + thisButton.BackColor.G.ToString("X2") + thisButton.BackColor.B.ToString("X2");
		}

		private void ColourToolStripMenuItemOpenMixer(object sender, EventArgs e)
		{
			int colourIndex = 0;

			if (Palette.Type == PaletteType.Custom)
			{
				colorDialog1.Color = colourClicked.BackColor;
				if (colorDialog1.ShowDialog() == DialogResult.OK)
				{
					colourIndex = int.Parse(colourClicked.Name);
					colourClicked.BackColor = colorDialog1.Color;
					Palette[colourIndex].Red = colourClicked.BackColor.R;
					Palette[colourIndex].Green = colourClicked.BackColor.G;
					Palette[colourIndex].Blue = colourClicked.BackColor.B;
					SetTransparentColour(ref colourButtons[Palette.TransparentIndex]);
				}
			}
		}

		#endregion

		#region Events - Colour Mapping

		private void next256TypeRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (isFormLoaded && next256TypeRadioButton.Checked)
			{
				Palette.Type = PaletteType.Next256;
				messageLabel.Visible = false;
				for (int c = 0; c < 256; c++)
				{
					colourButtons[c].BackColor = Palette[c].ToColor();
					colourButtons[c].FlatAppearance.BorderColor = SystemColors.ControlDark;
				}
				SetTransparentColour(ref colourButtons[Palette.TransparentIndex]);
			}
		}

		private void next512TypeRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (isFormLoaded && next512TypeRadioButton.Checked)
			{
				Palette.Type = PaletteType.Next512;
				messageLabel.Visible = true;

				for (int c = 0; c < 256; c++)
				{
					colourButtons[c].BackColor = Palette[c].ToColor();
					colourButtons[c].FlatAppearance.BorderColor = SystemColors.ControlDark;
				}

				SetTransparentColour(ref colourButtons[Palette.TransparentIndex]);
			}
		}

		private void customTypeRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (isFormLoaded && customTypeRadioButton.Checked)
			{
				Palette.Type = PaletteType.Custom;
				messageLabel.Visible = false;
				for (int c = 0; c < 256; c++)
				{
					colourButtons[c].BackColor = SystemColors.Control;
					colourButtons[c].FlatAppearance.BorderColor = SystemColors.ControlDark;
				}
				for (int c = 0; c < Palette.UsedCount; c++)
				{
					colourButtons[c].BackColor = Palette[c].ToColor();
					colourButtons[c].FlatAppearance.BorderColor = selectedColor;
				}
				SetTransparentColour(ref colourButtons[Palette.TransparentIndex]);
			}
		}

		#endregion

		#region Events - Palette

		private void selectPaletteButton_Click(object sender, EventArgs e)
		{
			// select	
			imageSelectForm.IsUsingModel = true;
			imageSelectForm.StartPosition = FormStartPosition.CenterParent;
			imageSelectForm.FillFilenamesFromModel();
			imageSelectForm.ShowDialog();
			messageLabel.Visible = false;

			if (imageSelectForm.DialogResult == DialogResult.OK)
			{
				for (int c = 0; c < imageSelectForm.ColoursCount; c++)
				{
					var toIndex = imageSelectForm.ToIndex + c;
					var fromIndex = imageSelectForm.FromIndex + c;

					Palette[toIndex].Red = imageSelectForm.LoadedPalette[fromIndex, 0];
					Palette[toIndex].Green = imageSelectForm.LoadedPalette[fromIndex, 1];
					Palette[toIndex].Blue = imageSelectForm.LoadedPalette[fromIndex, 2];

					colourButtons[toIndex].BackColor = Palette[toIndex].ToColor();
				}

				customTypeRadioButton.Checked = true;
			}
		}

		private void loadPaletteButton_Click(object sender, EventArgs e)
		{
			OpenFileDialog loadPaletteDialog = new OpenFileDialog();
			loadPaletteDialog.Multiselect = true;
			loadPaletteDialog.RestoreDirectory = true;
			loadPaletteDialog.Filter = "Palette Files (*.act)|*.act|Mac Palette Files (*.8bct)|*.8bct|All Files (*.*)|*.*";
			loadPaletteDialog.FilterIndex = 1;

			if (loadPaletteDialog.ShowDialog(this) == DialogResult.OK)
			{
				imageSelectForm.IsUsingModel = false;
				imageSelectForm.StartPosition = FormStartPosition.CenterParent;
				imageSelectForm.FillFilenames(loadPaletteDialog.FileNames);
				imageSelectForm.ShowDialog();

				if (imageSelectForm.DialogResult == DialogResult.OK)
				{
					// open the palette import panel
					for (int c = 0; c < imageSelectForm.ColoursCount; c++)
					{
						var toIndex = imageSelectForm.ToIndex + c;
						var fromIndex = imageSelectForm.FromIndex + c;
						Palette[toIndex].Red = imageSelectForm.LoadedPalette[fromIndex, 0];
						Palette[toIndex].Green = imageSelectForm.LoadedPalette[fromIndex, 1];
						Palette[toIndex].Blue = imageSelectForm.LoadedPalette[fromIndex, 2];
						colourButtons[toIndex].BackColor = Palette[toIndex].ToColor();
					}
					customTypeRadioButton.Checked = true;
					messageLabel.Visible = false;

					Palette.Type = PaletteType.Custom;
					for (int c = 0; c < 256; c++)
					{
						colourButtons[c].BackColor = Palette[c].ToColor();
						colourButtons[c].FlatAppearance.BorderColor = SystemColors.ControlDark;
					}
					for (int c = 0; c < Palette.UsedCount; c++)
					{
						colourButtons[c].FlatAppearance.BorderColor = selectedColor;
					}
					SetTransparentColour(ref colourButtons[Palette.TransparentIndex]);
				}
			}
		}

		private void savePaletteButton_Click(object sender, EventArgs e)
		{
			int lineNumber = 1000;
			int lineStep = 10;
			SaveFileDialog savePaletteDialog = new SaveFileDialog();
			//savePaletteDialog.InitialDirectory		=	Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			savePaletteDialog.RestoreDirectory = true;
			savePaletteDialog.Filter = "Palette Files (*.act)|*.act|Mac Palette Files (*.8bct)|*.8bct|Spectrum Next (*.asm)|*.asm|Spectrum Next (*.bas)|*.asm|All Files (*.*)|*.*";
			savePaletteDialog.FilterIndex = 1;
			if (savePaletteDialog.ShowDialog(this) == DialogResult.OK)
			{
				if (savePaletteDialog.FilterIndex == 3 || savePaletteDialog.FilterIndex == 4)
				{
					using (StreamWriter outputFile = new StreamWriter(savePaletteDialog.FileName))
					{
						if (savePaletteDialog.FilterIndex == 4)
						{
							outputFile.WriteLine(lineNumber.ToString() + "\tREM");
							lineNumber += lineStep;
							outputFile.WriteLine(lineNumber.ToString() + "\tREM\tExported Palette starts here");
							lineNumber += lineStep;
							outputFile.WriteLine(lineNumber.ToString() + "\tREM");
							lineNumber += lineStep;
							outputFile.Write(lineNumber.ToString() + "\tDATA\t");
							lineNumber += lineStep;
						}
						else
						{
							outputFile.WriteLine("ExportedPalette:");
						}
						for (int j = 0; j < Palette.UsedCount; j++)
						{
							if (savePaletteDialog.FilterIndex == 4)
							{
								outputFile.Write(EightbitPalette(Palette[j + Palette.StartIndex, 0], Palette[j + Palette.StartIndex, 1], Palette[j + Palette.StartIndex, 2]).ToString());
								if (j < Palette.UsedCount - 1)
								{
									outputFile.Write(",");
								}
								else
								{
									outputFile.Write("\r\n");
								}
							}
							else
							{
								outputFile.WriteLine(
									"\t\t\tdb\t%" + ToBinary(EightbitPalette(Palette[j + Palette.StartIndex, 0],
									Palette[j + Palette.StartIndex, 1],
									Palette[j + Palette.StartIndex, 2])) +
									"\t//\t" + Palette[j + Palette.StartIndex, 0].ToString() +
									"," + Palette[j + Palette.StartIndex, 1].ToString() +
									"," + Palette[j + Palette.StartIndex, 2].ToString());
							}
						}
					}
				}
				else
				{
					using (FileStream fsSource = new FileStream(savePaletteDialog.FileName, FileMode.Create, FileAccess.Write))
					{
						for (int i = 0; i < 256; i++)
						{
							fsSource.WriteByte(Palette[i, 0]);
							fsSource.WriteByte(Palette[i, 1]);
							fsSource.WriteByte(Palette[i, 2]);
						}
						if (isLittleEndian)
						{
							fsSource.WriteByte((byte)(Palette.UsedCount & 255));
							fsSource.WriteByte((byte)(Palette.UsedCount >> 8));
						}
						else
						{
							fsSource.WriteByte((byte)(Palette.UsedCount >> 8));
							fsSource.WriteByte((byte)(Palette.UsedCount & 255));
						}
						if (isLittleEndian)
						{
							fsSource.WriteByte((byte)(Palette.TransparentIndex & 255));
							fsSource.WriteByte((byte)(Palette.TransparentIndex >> 8));
						}
						else
						{
							fsSource.WriteByte((byte)(Palette.TransparentIndex >> 8));
							fsSource.WriteByte((byte)(Palette.TransparentIndex & 255));
						}
					}
				}
			}
		}

		private void paletteRowUpButton_Click(object sender, EventArgs e)
		{
			byte[,] tempPalette = new byte[16, 3];

			for (int c = 0; c < 16; c++)
			{
				tempPalette[c, 0] = Palette[c, 0];
				tempPalette[c, 1] = Palette[c, 1];
				tempPalette[c, 2] = Palette[c, 2];
			}
			for (int c = 0; c < 256 - 16; c++)
			{
				Palette[c][0] = Palette[c + 16, 0];
				Palette[c][1] = Palette[c + 16, 1];
				Palette[c][2] = Palette[c + 16, 2];
			}
			for (int c = 0; c < 16; c++)
			{
				Palette[c + (256 - 16)][0] = tempPalette[c, 0];
				Palette[c + (256 - 16)][1] = tempPalette[c, 1];
				Palette[c + (256 - 16)][2] = tempPalette[c, 2];
			}
			for (int c = 0; c < 256; c++)
			{
				colourButtons[c].BackColor = Palette[c].ToColor();
			}
		}

		private void paletteRowDownButton_Click(object sender, EventArgs e)
		{
			byte[,] tempPalette = new byte[16, 3];

			for (int c = 0; c < 16; c++)
			{
				tempPalette[c, 0] = Palette[c + (256 - 16), 0];
				tempPalette[c, 1] = Palette[c + (256 - 16), 1];
				tempPalette[c, 2] = Palette[c + (256 - 16), 2];
			}
			for (int c = 255 - 16; c >= 0; c--)
			{
				Palette[c + 16][0] = Palette[c, 0];
				Palette[c + 16][1] = Palette[c, 1];
				Palette[c + 16][2] = Palette[c, 2];
			}
			for (int c = 0; c < 16; c++)
			{
				Palette[c][0] = tempPalette[c, 0];
				Palette[c][1] = tempPalette[c, 1];
				Palette[c][2] = tempPalette[c, 2];
			}
			for (int c = 0; c < 256; c++)
			{
				colourButtons[c].BackColor = Palette[c].ToColor();
			}
		}

		#endregion

		#region Events - Colours

		private void transparentColourButton_Click(object sender, EventArgs e)
		{
			isTransparentColourPickingActive = true;
		}

		private void startIndexTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			Palette.StartIndex = int.Parse(this.startIndexTextBox.Text);
		}

		private void colourCountTextBox_TextChanged(object sender, EventArgs e)
		{
			UpdateColoursCount();
		}

		private void colourCountTextBox_KeyUp(object sender, KeyEventArgs e)
		{
			UpdateColoursCount();
		}

		private void hexColourTextBox_TextChanged(object sender, EventArgs e)
		{
			SetFromHex();
		}

		private void hexColourTextBox_TextChanged(object sender, KeyEventArgs e)
		{
			SetFromHex();
		}

		#endregion

		#region Events - Colour ToolStripMenu

		private void cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int colourIndex = int.Parse(colourClicked.Name);
			colourClicked.BackColor = SystemColors.Control;
			Palette[colourIndex].Red = colourClicked.BackColor.R;
			Palette[colourIndex].Green = colourClicked.BackColor.G;
			Palette[colourIndex].Blue = colourClicked.BackColor.B;
			SetTransparentColour(ref colourButtons[Palette.TransparentIndex]);
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			imageSelectForm.CopiedColour = colourClicked.BackColor;
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int colourIndex = int.Parse(colourClicked.Name);
			colourClicked.BackColor = imageSelectForm.CopiedColour;
			Palette[colourIndex].Red = colourClicked.BackColor.R;
			Palette[colourIndex].Green = colourClicked.BackColor.G;
			Palette[colourIndex].Blue = colourClicked.BackColor.B;
			SetTransparentColour(ref colourButtons[Palette.TransparentIndex]);
		}

		private void hexValueToolStripMenuItem_Click(object sender, EventArgs e)
		{
			hexColourTextBox.Text = "#" + colourClicked.BackColor.R.ToString("X2") + colourClicked.BackColor.G.ToString("X2") + colourClicked.BackColor.B.ToString("X2");
		}

		#endregion

		#region Helpers

		private void CreatePaletteButtons()
		{
			// If already created, exit.
			if (colourButtons != null) return;

			colourButtons = new Button[256];

			int	across	=	0;
			int	down	=	0;
			for(int c=0;c<256;c++)
			{
				colourButtons[c]						=	new Button();
				this.Controls.Add(colourButtons[c]);
				colourButtons[c].Text						=	"";
				colourButtons[c].Location					=	new Point(160+(across*20),10+(down*20));
				colourButtons[c].Size						=	new Size(22, 22);
				colourButtons[c].BackColor				=	Palette[c].ToColor();
				colourButtons[c].Click					+=	ColourButtonClick;
				//colours[c].MouseHover					+=	ColourButtonMouseOver;
				//colours[c].Enabled					=	false;
				colourButtons[c].Name						=	c.ToString();				
				colourButtons[c].FlatStyle					=	FlatStyle.Flat;
				colourButtons[c].FlatAppearance.BorderColor			=	SystemColors.ControlDark;
				colourButtons[c].FlatAppearance.BorderSize			=	1;
				//colours[c].ForeColor					=	Color.Black;
				across++;
				if(across>15)
				{
					across	=	0;
					down++;
				}
			}
			startIndexTextBox.Text				=	Palette.StartIndex.ToString();
			tColourIndex1.Text			=	Palette.TransparentIndex.ToString();
			colourButtons[Palette.TransparentIndex].Text		=	"X";
			next256TypeRadioButton.Checked			=	true;

		}

		private void SetTransparentColour(ref Button button)
		{
			int colourIndex = int.Parse(tColourIndex1.Text);
			colourButtons[colourIndex].Text = "";
			tColourIndex1.Text = button.Name;
			Palette.TransparentIndex = int.Parse(tColourIndex1.Text);
			tColourIndex1.BackColor = button.BackColor;
			tColourIndex1.ForeColor = (384 - tColourIndex1.BackColor.R - tColourIndex1.BackColor.G - tColourIndex1.BackColor.B) > 0 ? Color.White : Color.Black;
			button.Text = "X";
			button.ForeColor = tColourIndex1.ForeColor;
		}

		private void SetFromHex()
		{
			if (colourClicked != null)
			{
				if (hexColourTextBox.Text.Substring(0, 1) == "#")
				{
					if (hexColourTextBox.Text.Length == 7)
					{
						colourClicked.BackColor = Color.FromArgb(
							Convert.ToInt32(hexColourTextBox.Text.Substring(1, 2), 16),
							Convert.ToInt32(hexColourTextBox.Text.Substring(3, 2), 16),
							Convert.ToInt32(hexColourTextBox.Text.Substring(5, 2), 16));
					}
				}
				else
				{
					if (hexColourTextBox.Text.Length == 6)
					{
						colourClicked.BackColor = Color.FromArgb(
							Convert.ToInt32(hexColourTextBox.Text.Substring(0, 2), 16),
							Convert.ToInt32(hexColourTextBox.Text.Substring(2, 2), 16),
							Convert.ToInt32(hexColourTextBox.Text.Substring(4, 2), 16));
					}
				}
			}
		}
		
		private void UpdateColoursCount()
		{
			int value;
			if(int.TryParse(colourCountTextBox.Text, out value))
			{
				Palette.UsedCount = value;
				if (Palette.UsedCount > 256)
				{
					Palette.UsedCount = 256;
					colourCountTextBox.Text = "256";
				}
				if (Palette.StartIndex > 256)
				{
					Palette.StartIndex = 0;
					startIndexTextBox.Text = "0";
				}
				for (int c = 0; c < 256; c++)
				{
					colourButtons[c].FlatAppearance.BorderColor = SystemColors.ControlDark;
				}
				for (int c = 0; c < Palette.UsedCount; c++)
				{
					colourButtons[c + Palette.StartIndex].FlatAppearance.BorderColor = selectedColor;
				}
			}
		}

		private byte EightbitPalette(decimal red, decimal green, decimal blue)
		{
			byte r = (byte)Math.Round(red / (255 / 7));
			byte g = (byte)Math.Round(green / (255 / 7));
			byte b = (byte)Math.Round(blue / (255 / 3));
			return (byte)((r << 5) | (g << 2) | b);

			//return	(red & 0x0E0) | ((green & 0x0E0)>>3) | (((blue & 0x0E0)>>6) | ((blue & 0x020)>>5));
		}

		private string ToBinary(byte num)
		{
			string outString = "";
			int bits = 0x080;
			for (int bit = 0; bit < 8; bit++)
			{
				if ((num & bits) == bits)
				{
					outString += "1";
				}
				else
				{
					outString += "0";
				}
				bits = bits >> 1;
			}
			return outString;
		}

		#endregion
	}
}
