using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using NextGraphics.Models;
using NextGraphics.Exporting;
using NextGraphics.Utils;
using System.Linq;

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

		public Exporter Exporter
		{
			get => _exporter;
			set
			{
				_exporter = value;
				imageSelectForm.Exporter = value;
			}
		}
		private Exporter _exporter;

		private Palette Palette { get => Model.Palette; } // a shortcut instead of having to write Model.Palette

		private bool IsFourBitMode
		{
			get => Model.IsFourBitData && Model.PaletteParsingMethod == PaletteParsingMethod.ByObjects;
		}

		private int ValidatedTransparentIndex
		{
			get => IsFourBitMode ? Palette.TransparentIndex % 16 : Palette.TransparentIndex;
		}

		private ImageSelectForm imageSelectForm = new ImageSelectForm();

		public Button[] colourButtons = null;
		public Button[,] colourButtonBanks = null;
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

			// Update title to indicate whether we have 8-bit or 4-bit palette.
			Text = Model.IsFourBitData ? "Palette (4-bit)" : "Palette";

			CreateColourButtons();

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

			colourCountTextBox.Text = Palette.UsedCount.ToString();
			startIndexTextBox.Text = Palette.StartIndex.ToString();

			UpdateColourButtons();
			IndicateTransparentButtons();

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

		private void ColourButton_MouseUp(object sender, MouseEventArgs e)
		{
			colourClicked = (Button)sender;

			if (isTransparentColourPickingActive)
			{
				var selectedColourIndex = int.Parse(colourClicked.Name);
				if (selectedColourIndex > Palette.LastValidIndex)
				{
					MessageBox.Show("Transparent colour out of range of used colours", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
					isTransparentColourPickingActive = false;
					return;
				}

				bool ShowAlert()
				{
					return MessageBox.Show("Move transparent colour to the first palette colour?", "Move", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
				}

				if (IsFourBitMode)
				{
					var validatedIndex = selectedColourIndex % 16;

					if (validatedIndex > 0 && ShowAlert())
					{
						for (int row = 0; row < 16; row++)
						{
							var to = (row * 16);
							var from = to + validatedIndex;

							Palette.SwapColours(from, to);
						}

						UpdateColourButtons();

						selectedColourIndex = 0;
					}
				}
				else
				{
					if (selectedColourIndex > 0 && ShowAlert())
					{
						var to = 0;
						var from = selectedColourIndex;

						Palette.SwapColours(from, to);
						UpdateColourButtons();

						selectedColourIndex = 0;
					}
				}

				Palette.TransparentIndex = selectedColourIndex;
				IndicateTransparentButtons();

				isTransparentColourPickingActive = false;
			}
			else if (e.Button == MouseButtons.Right && Palette.Type == PaletteType.Custom)
			{
				Point lowerLeft = new Point(0, colourClicked.Height);
				lowerLeft = colourClicked.PointToScreen(lowerLeft);
				copyMenu.Show(lowerLeft);
				ShowHexValue();
			}
			else if (e.Button == MouseButtons.Left)
			{
				ShowHexValue();
			}
		}

		private void ColourButtonMouseOver(object sender, EventArgs e)
		{
			Button thisButton = (Button)sender;
			hexColourTextBox.Text = "#" + thisButton.BackColor.R.ToString("X2") + thisButton.BackColor.G.ToString("X2") + thisButton.BackColor.B.ToString("X2");
		}

		#endregion

		#region Events - Colour Mapping

		private void next256TypeRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (isFormLoaded && next256TypeRadioButton.Checked)
			{
				Palette.Type = PaletteType.Next256;
				messageLabel.Visible = false;
				
				UpdateColourButtons();
				IndicateTransparentButtons();
			}
		}

		private void next512TypeRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (isFormLoaded && next512TypeRadioButton.Checked)
			{
				Palette.Type = PaletteType.Next512;
				messageLabel.Visible = true;

				UpdateColourButtons();
				IndicateTransparentButtons();
			}
		}

		private void customTypeRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (isFormLoaded && customTypeRadioButton.Checked)
			{
				Palette.Type = PaletteType.Custom;
				messageLabel.Visible = false;

				UpdateColourButtons();
				IndicateTransparentButtons();
			}
		}

		#endregion

		#region Events - Palette

		private void selectPaletteButton_Click(object sender, EventArgs e)
		{
			imageSelectForm.IsUsingModel = true;
			imageSelectForm.StartPosition = FormStartPosition.CenterParent;
			imageSelectForm.FillFilenamesFromModel();
			imageSelectForm.ShowDialog();
			messageLabel.Visible = false;

			if (imageSelectForm.DialogResult == DialogResult.OK)
			{
				ResetColourButtons();

				for (int c = 0; c < imageSelectForm.ColoursCount; c++)
				{
					var toIndex = imageSelectForm.ToIndex + c;
					var fromIndex = imageSelectForm.FromIndex + c;

					Palette[toIndex].Red = imageSelectForm.LoadedPalette[fromIndex].R;
					Palette[toIndex].Green = imageSelectForm.LoadedPalette[fromIndex].G;
					Palette[toIndex].Blue = imageSelectForm.LoadedPalette[fromIndex].B;

					colourButtons[toIndex].BackColor = Palette[toIndex].ToColor();
				}

				startIndexTextBox.Text = imageSelectForm.FromIndex.ToString();
				colourCountTextBox.Text = imageSelectForm.ColoursCount.ToString();

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
					ResetColourButtons();

					for (int c = 0; c < imageSelectForm.ColoursCount; c++)
					{
						var toIndex = imageSelectForm.ToIndex + c;
						var fromIndex = imageSelectForm.FromIndex + c;

						Palette[toIndex].Red = imageSelectForm.LoadedPalette[fromIndex].R;
						Palette[toIndex].Green = imageSelectForm.LoadedPalette[fromIndex].G;
						Palette[toIndex].Blue = imageSelectForm.LoadedPalette[fromIndex].B;

						colourButtons[toIndex].BackColor = Palette[toIndex].ToColor();
					}

					customTypeRadioButton.Checked = true;
					messageLabel.Visible = false;

					Palette.Type = PaletteType.Custom;

					for (int c = 0; c < colourButtons.Length; c++)
					{
						colourButtons[c].BackColor = Palette[c].ToColor();
						colourButtons[c].FlatAppearance.BorderColor = SystemColors.ControlDark;
					}

					for (int c = 0; c < Palette.UsedCount; c++)
					{
						colourButtons[c].FlatAppearance.BorderColor = selectedColor;
					}
					
					IndicateTransparentButtons();
				}
			}
		}

		private void savePaletteButton_Click(object sender, EventArgs e)
		{
			int lineNumber = 1000;
			int lineStep = 10;

			var filterBuilder = new DialogFilterBuilder();
			filterBuilder.Add("Palette Files", "*.act", ExportAsPaletteFile);
			filterBuilder.Add("Mac Palette Files", "*.8bct", ExportAsMacPaletteFile);
			filterBuilder.Add("Spectrum Next Assembler", $"*.{Model.ExportAssemblerFileExtension}", ExportAsSpectrumNextAssemblerFile);
			filterBuilder.Add("Spectrum Next Basic", "*.bas", ExportAsSpectrumNextBasicFile);
			filterBuilder.Add("Spectrum Next Binary Palette", "*.pal", ExportAsSpectrumNextBinaryPaletteFile);

			SaveFileDialog savePaletteDialog = new SaveFileDialog();
			savePaletteDialog.Filter = filterBuilder.Filters;
			savePaletteDialog.FilterIndex = Properties.Settings.Default.PaletteFormExportFilterIndex;
			savePaletteDialog.RestoreDirectory = true;

			if (savePaletteDialog.ShowDialog(this) == DialogResult.OK)
			{
				Properties.Settings.Default.PaletteFormExportFilterIndex = savePaletteDialog.FilterIndex;
				Properties.Settings.Default.Save();

				filterBuilder.Handle(savePaletteDialog.FilterIndex - 1, savePaletteDialog.FileName);
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
		}

		private void openMixerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (Palette.Type == PaletteType.Custom)
			{
				colorDialog.Color = colourClicked.BackColor;

				if (colorDialog.ShowDialog() == DialogResult.OK)
				{
					int colourIndex = int.Parse(colourClicked.Name);
					colourClicked.BackColor = colorDialog.Color;

					Palette[colourIndex].Red = colourClicked.BackColor.R;
					Palette[colourIndex].Green = colourClicked.BackColor.G;
					Palette[colourIndex].Blue = colourClicked.BackColor.B;
				}
			}
		}

		#endregion

		#region Exporting

		private void ExportAsPaletteFile(string filename)
		{
			using (FileStream output = new FileStream(filename, FileMode.Create, FileAccess.Write))
			{
				void WriteInt(int value)
				{
					if (isLittleEndian)
					{
						output.WriteByte((byte)(value & 0xFF));
						output.WriteByte((byte)(value >> 8));
					}
					else
					{
						output.WriteByte((byte)(value >> 8));
						output.WriteByte((byte)(value & 0xFF));
					}
				}

				for (int i = 0; i < colourButtons.Length; i++)
				{
					output.WriteByte(Palette[i].Red);
					output.WriteByte(Palette[i].Green);
					output.WriteByte(Palette[i].Blue);
				}

				WriteInt(Palette.UsedCount);
				WriteInt(Palette.TransparentIndex);
			}
		}

		private void ExportAsMacPaletteFile(string filename)
		{
			// We share the output, but keep separate method.
			ExportAsPaletteFile(filename);
		}

		private void ExportAsSpectrumNextAssemblerFile(string filename)
		{
			using (StreamWriter outputFile = new StreamWriter(filename))
			{
				outputFile.WriteLine("ExportedPalette:");

				for (int i = 0; i < Palette.UsedCount; i++)
				{
					var colour = Palette[i + Palette.StartIndex];
					var bytes = colour.ToRawBytes(Model.PaletteFormat);
					var binaries = bytes.Select(x => $"%{x.ToBinaryString()}");

					outputFile.Write("\t\t\tdb\t");
					outputFile.WriteLine(string.Join(",", binaries));
				}
			}
		}

		private void ExportAsSpectrumNextBasicFile(string filename)
		{
			int lineNumber = 1000;
			int lineStep = 10;

			using (StreamWriter outputFile = new StreamWriter(filename))
			{
				outputFile.WriteLine(lineNumber.ToString() + "\tREM");
				lineNumber += lineStep;

				outputFile.WriteLine(lineNumber.ToString() + "\tREM\tExported Palette starts here");
				lineNumber += lineStep;

				outputFile.WriteLine(lineNumber.ToString() + "\tREM");
				lineNumber += lineStep;

				outputFile.Write(lineNumber.ToString() + "\tDATA\t");
				lineNumber += lineStep;

				for (int i = 0; i < Palette.UsedCount; i++)
				{
					// Basic always exports as 8-bit colours.
					var colour = Palette[i + Palette.StartIndex];
					var colourBytes = colour.ToRawBytes(PaletteFormat.Next8Bit);

					outputFile.Write(string.Join(",", colourBytes));

					if (i < Palette.UsedCount - 1)
					{
						outputFile.Write(",");
					}
					else
					{
						outputFile.WriteLine();
					}
				}
			}
		}

		private void ExportAsSpectrumNextBinaryPaletteFile(string filename)
		{
			using (FileStream output = new FileStream(filename, FileMode.Create, FileAccess.Write))
			{
				for (int i = 0; i < Palette.UsedCount; i++)
				{
					var colour = Palette[i];
					var bytes = colour.ToRawBytes(Model.PaletteFormat);

					foreach (var b in bytes)
					{
						output.WriteByte(b);
					}
				}
			}
		}

		#endregion

		#region Helpers

		private void CreateColourButtons()
		{
			// If already created, exit.
			if (colourButtons != null) return;

			colourButtons = new Button[256];
			colourButtonBanks = new Button[16, 16];

			int across = 0;
			int down = 0;
			for (int c = 0; c < 256; c++)
			{
				var button = new Button
				{
					Text = "",
					Name = c.ToString(),
					BackColor = Palette[c].ToColor(),
					FlatStyle = FlatStyle.Flat,
					Location = new Point(160 + (across * 20), 10 + (down * 20)),
					Size = new Size(22, 22),
				};

				button.FlatAppearance.BorderColor = SystemColors.ControlDark;
				button.FlatAppearance.BorderSize = 1;
				button.MouseUp += ColourButton_MouseUp;

				colourButtons[c] = button;
				colourButtonBanks[down, across] = button;

				Controls.Add(colourButtons[c]);

				across++;
				if (across > 15)
				{
					across = 0;
					down++;
				}
			}

			startIndexTextBox.Text = Palette.StartIndex.ToString();
			next256TypeRadioButton.Checked = true;
		}

		private void UpdateColourButtons()
		{
			ResetColourButtons();

			for (int i = 0; i < colourButtons.Length; i++)
			{
				if (i >= Palette.StartIndex && i <= Palette.LastValidIndex)
				{
					colourButtons[i].BackColor = Palette[i].ToColor();
					colourButtons[i].ForeColor = colourButtons[i].BackColor.FittingBlackOrWhite();
				}
			}
		}

		private void ResetColourButtons()
		{
			for (int i = 0; i < colourButtons.Length; i++)
			{
				colourButtons[i].BackColor = SystemColors.Control;
				colourButtons[i].FlatAppearance.BorderColor = SystemColors.ControlDark;
			}
		}

		private void IndicateTransparentButtons()
		{
			var transparentIndex = ValidatedTransparentIndex;

			void IndicateTransparent(Button button, bool isTransparent)
			{
				if (isTransparent)
				{
					button.Text = "X";
					button.ForeColor = button.BackColor.FittingBlackOrWhite();
				}
				else
				{
					button.Text = "";
				}
			}

			if (Model.IsFourBitData && Model.PaletteParsingMethod == PaletteParsingMethod.ByObjects)
			{
				// For 4-bit palette, we must indicate transparency in each bank. But only if auto-banking is set. Note we should already have transparent index trimmed within 16-colours, but let's be proactive just in case (maybe the index is invalid after loading the data for example).
				for (int y = 0; y < 16; y++)
				{
					for (int x = 0; x < 16; x++)
					{
						int totalIndex = y * 16 + x;

						IndicateTransparent(colourButtonBanks[y, x], x == transparentIndex && totalIndex < Palette.UsedColours);
					}
				}
			}
			else
			{
				// For 8-bit palette, only 1 colour must be shown as transparent.
				for (int i = 0; i < colourButtons.Length; i++)
				{
					IndicateTransparent(colourButtons[i], i == transparentIndex);
				}
			}

			transparentColourIndexLabel.BackColor = Palette.TransparentColour.ToColor();
			transparentColourIndexLabel.ForeColor = transparentColourIndexLabel.BackColor.FittingBlackOrWhite();
		}

		private void ShowHexValue()
		{
			if (colourClicked == null) return;

			hexColourTextBox.Text = "#" + colourClicked.BackColor.R.ToString("X2") + colourClicked.BackColor.G.ToString("X2") + colourClicked.BackColor.B.ToString("X2");
		}

		private void SetFromHex()
		{
			if (colourClicked == null) return;
			
			var value = hexColourTextBox.Text;
			if (value.Length >= 7 && value[0] == '#') value = value.Substring(1);
			if (value.Length < 6) return;

			var r = Convert.ToInt32(value.Substring(0, 2), 16);
			var g = Convert.ToInt32(value.Substring(2, 2), 16);
			var b = Convert.ToInt32(value.Substring(4, 2), 16);

			colourClicked.BackColor = Color.FromArgb(r, g, b);
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

		#endregion
	}
}
