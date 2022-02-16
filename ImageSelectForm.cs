using NextGraphics.Models;

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;


namespace NextGraphics
{
	public partial class ImageSelectForm : Form
	{
		public MainModel Model { get; set; }
		public byte[,] LoadedPalette { get; set; } = new byte[256, 3];
		public bool PaletteFiles { get; set; } = false;
		public int FromIndex { get; set; } = 0;
		public int ToIndex { get; set; } = 0;
		public int ColoursCount { get; set; } = 0;
		public Color CopiedColour { get; set; }

		private bool littleEndian = false;
		//		private	int		thisPixel		=	0;
		//		private	int		bytesPerPixel		=	3;
		private int transIndex = 0;
		private Colour thatColour = new Colour();
		private Colour thisColour = new Colour();
		//		private	Color		thisColor		=	new Color();	
		private Button[] colours = new Button[256];
		private int loadedColourCount = 255;
		private int loadedColourStart = 0;
		private List<string> fullNames = new List<string>();
		private Button colourClicked;

		#region Initialization & Disposal

		public ImageSelectForm()
		{
			littleEndian = BitConverter.IsLittleEndian;
			InitializeComponent();
			CreatePalette();
		}

		#endregion

		#region Events

		private void openMixerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int colourIndex = 0;
			colorDialog1.Color = colourClicked.BackColor;
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{
				colourIndex = int.Parse(colourClicked.Name);
				colourClicked.BackColor = colorDialog1.Color;
				LoadedPalette[colourIndex, 0] = colourClicked.BackColor.R;
				LoadedPalette[colourIndex, 1] = colourClicked.BackColor.G;
				LoadedPalette[colourIndex, 2] = colourClicked.BackColor.B;
			}
		}

		private void cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int colourIndex = int.Parse(colourClicked.Name);
			colourClicked.BackColor = SystemColors.Control;
			LoadedPalette[colourIndex, 0] = colourClicked.BackColor.R;
			LoadedPalette[colourIndex, 1] = colourClicked.BackColor.G;
			LoadedPalette[colourIndex, 2] = colourClicked.BackColor.B;
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int colourIndex = int.Parse(colourClicked.Name);
			colourClicked.BackColor = CopiedColour;
			LoadedPalette[colourIndex, 0] = CopiedColour.R;
			LoadedPalette[colourIndex, 1] = CopiedColour.G;
			LoadedPalette[colourIndex, 2] = CopiedColour.B;
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CopiedColour = colourClicked.BackColor;
		}

		private void colourButton_Click(object sender, EventArgs e)
		{
			colourClicked = (Button)sender;
			Point lowerLeft = new Point(0, colourClicked.Height);
			lowerLeft = colourClicked.PointToScreen(lowerLeft);
			copyMenu.Show(lowerLeft);
		}

		private void imagesListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (imagesListBox.SelectedIndex >= 0)
			{
				Application.UseWaitCursor = true;
				Cursor.Current = Cursors.WaitCursor;
				if (PaletteFiles)
				{
					byte[] bytesBuffer = new byte[2];
					using (FileStream fsSource = new FileStream(fullNames[imagesListBox.SelectedIndex], FileMode.Open, FileAccess.Read))
					{
						int numBytesToRead = (int)fsSource.Length;
						int numBytesRead = 0;

						// Read and verify the data.
						if (fsSource.Length < 256 * 3)
						{
							// dodgy file 
							MessageBox.Show("Not enough bytes in the file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
						}
						else
						{
							for (int i = 0; i < 256; i++)
							{
								LoadedPalette[i, 0] = (byte)fsSource.ReadByte();
								LoadedPalette[i, 1] = (byte)fsSource.ReadByte();
								LoadedPalette[i, 2] = (byte)fsSource.ReadByte();
								numBytesRead += 3;
							}
							if (numBytesToRead > numBytesRead)
							{
								// probably got 2 bytes of num colours and 2 transparent colour
								fsSource.Read(bytesBuffer, 0, 2);
								numBytesRead += 2;
								//BigEndian	= $1234 = $12 $34
								//LittleEndian	= $1234 = $34 $12

								if (littleEndian == true)
								{
									loadedColourCount = bytesBuffer[1] << 8 | bytesBuffer[0];
								}
								else
								{
									loadedColourCount = bytesBuffer[0] << 8 | bytesBuffer[1];
								}
								if (loadedColourCount > 255 || loadedColourCount < 0)
								{
									loadedColourCount = 0;
								}
								if (numBytesToRead > numBytesRead)
								{
									fsSource.Read(bytesBuffer, 0, 2);
									if (littleEndian == true)
									{
										transIndex = bytesBuffer[1] << 8 | bytesBuffer[0];
									}
									else
									{
										transIndex = bytesBuffer[0] << 8 | bytesBuffer[1];
									}
									if (transIndex > 255 || transIndex < 0)
									{
										transIndex = 0;
									}
								}
							}
							for (int c = 0; c < 256; c++)
							{
								colours[c].BackColor = Color.FromArgb(LoadedPalette[c, 0], LoadedPalette[c, 1], LoadedPalette[c, 2]);
							}
						}
					}

					loadedColourCount = 255;
					loadedColourStart = 0;
					countTextBox.Text = loadedColourCount.ToString();
					fromTextBox.Text = "0";
					toTextBox.Text = "0";

				}
				else
				{
					Bitmap srcBitmap;
					// use the image in the image window rather than the loaded file
					if (Model.Sources[imagesListBox.SelectedIndex] is SourceImage image)
					{
						srcBitmap = (Bitmap)image.Data.Clone();
					}
					else
					{
						using (var fs = new FileStream(fullNames[imagesListBox.SelectedIndex], FileMode.Open))
						{
							var bmp = new Bitmap(fs);
							srcBitmap = (Bitmap)bmp.Clone();
						}
					}
					int PaletteIndex = 0;
					for (int y = 0; y < srcBitmap.Height; y++)
					{
						for (int x = 0; x < srcBitmap.Width; x++)
						{

							Color pixelColour = srcBitmap.GetPixel(x, y);
							thatColour.R = pixelColour.R;
							thatColour.G = pixelColour.G;
							thatColour.B = pixelColour.B;

							bool same = false;
							for (int c = 0; c < PaletteIndex; c++)
							{
								if (LoadedPalette[c, 0] == thatColour.R && LoadedPalette[c, 1] == thatColour.G && LoadedPalette[c, 2] == thatColour.B)
								{
									same = true;
								}
							}
							if (same == false)
							{
								if (PaletteIndex < 256)
								{

									LoadedPalette[PaletteIndex, 0] = thatColour.R;
									LoadedPalette[PaletteIndex, 1] = thatColour.G;
									LoadedPalette[PaletteIndex, 2] = thatColour.B;
									colours[PaletteIndex].BackColor = Color.FromArgb(thatColour.R, thatColour.G, thatColour.B);
									PaletteIndex++;
								}
								else
								{
									goto NotSupported;
								}
							}
						}
					}
					loadedColourCount = PaletteIndex;
					countTextBox.Text = loadedColourCount.ToString();
					fromTextBox.Text = "0";
					toTextBox.Text = "0";
				}
			NotSupported:
				Cursor.Current = Cursors.Default;
				Application.UseWaitCursor = false;
			}

			UpdateImportButton();
		}

		private void fromTextBox_TextChanged(object sender, EventArgs e)
		{
			FromIndex = IntegerFromControl(sender);
		}

		private void toTextBox_TextChanged(object sender, EventArgs e)
		{
			ToIndex = IntegerFromControl(sender);
		}

		private void countTextBox_TextChanged(object sender, EventArgs e)
		{
			ColoursCount = IntegerFromControl(sender);
		}

		private void importButton_Click(object sender, EventArgs e)
		{
			ToIndex = IntegerFromControl(toTextBox);
			ColoursCount = IntegerFromControl(countTextBox);
			FromIndex = IntegerFromControl(fromTextBox);
		}

		#endregion

		#region Public

		public void FillFilenamesFromModel()
		{
			FillFilenames(Model.Sources.Select(image => $" {image.Filename}"));
		}

		public void FillFilenames(IEnumerable<string> filenames)
		{
			imagesListBox.Items.Clear();
			imagesListBox.Items.AddRange(filenames.Select(n => Path.GetFileName(n)).ToArray());

			UpdateImportButton();
		}

		#endregion

		#region Helpers

		private void CreatePalette()
		{
			int across = 0;
			int down = 0;
			for (int c = 0; c < 256; c++)
			{
				colours[c] = new Button();
				Controls.Add(colours[c]);
				colours[c].Text = "";
				colours[c].Location = new Point(10 + (across * 20), 15 + (down * 20));
				colours[c].Size = new Size(22, 22);
				//colours[c].Enabled			=	false;
				colours[c].Click += colourButton_Click;
				colours[c].Name = c.ToString();
				colours[c].FlatStyle = FlatStyle.Flat;
				colours[c].FlatAppearance.BorderColor = SystemColors.ControlDark;
				colours[c].FlatAppearance.BorderSize = 1;
				LoadedPalette[c, 0] = SystemColors.Control.R;
				LoadedPalette[c, 1] = SystemColors.Control.G;
				LoadedPalette[c, 2] = SystemColors.Control.B;
				colours[c].BackColor = Color.FromArgb(LoadedPalette[c, 0], LoadedPalette[c, 1], LoadedPalette[c, 2]);
				across++;
				if (across > 15)
				{
					across = 0;
					down++;
				}
			}
		}

		private void UpdateImportButton()
		{
			importButton.Enabled = (imagesListBox.Items.Count > 0 && imagesListBox.SelectedIndex >= 0);
		}

		private int IntegerFromControl(object control)
		{
			TextBox textBox = (TextBox)control;
			try
			{
				return int.Parse(textBox.Text);
			}
			catch
			{
				return 0;
			}
		}

		#endregion
	}
}
