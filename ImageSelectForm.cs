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
		public bool IsUsingModel { get; set; } = false;
		public int FromIndex { get; set; } = 0;
		public int ToIndex { get; set; } = 0;
		public int ColoursCount { get; set; } = 0;
		public Color CopiedColour { get; set; }

		private bool isLittleEndian = false;
		private int transparentColourIndex = 0;
		private int loadedColourStart = 0;
		private int loadedColourCount = 255;

		private Colour thatColour = new Colour();
		private Colour thisColour = new Colour();
		private Button[] colourButtons = new Button[256];
		private Button clickedColourButton;

		private List<string> fullNames = new List<string>();

		#region Initialization & Disposal

		public ImageSelectForm()
		{
			isLittleEndian = BitConverter.IsLittleEndian;
			InitializeComponent();
			CreatePalette();
		}

		#endregion

		#region Events

		private void openMixerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int colourIndex = 0;
			colorDialog1.Color = clickedColourButton.BackColor;
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{
				colourIndex = int.Parse(clickedColourButton.Name);
				clickedColourButton.BackColor = colorDialog1.Color;
				LoadedPalette[colourIndex, 0] = clickedColourButton.BackColor.R;
				LoadedPalette[colourIndex, 1] = clickedColourButton.BackColor.G;
				LoadedPalette[colourIndex, 2] = clickedColourButton.BackColor.B;
			}
		}

		private void cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int colourIndex = int.Parse(clickedColourButton.Name);
			clickedColourButton.BackColor = SystemColors.Control;
			LoadedPalette[colourIndex, 0] = clickedColourButton.BackColor.R;
			LoadedPalette[colourIndex, 1] = clickedColourButton.BackColor.G;
			LoadedPalette[colourIndex, 2] = clickedColourButton.BackColor.B;
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int colourIndex = int.Parse(clickedColourButton.Name);
			clickedColourButton.BackColor = CopiedColour;
			LoadedPalette[colourIndex, 0] = CopiedColour.R;
			LoadedPalette[colourIndex, 1] = CopiedColour.G;
			LoadedPalette[colourIndex, 2] = CopiedColour.B;
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CopiedColour = clickedColourButton.BackColor;
		}

		private void colourButton_Click(object sender, EventArgs e)
		{
			clickedColourButton = (Button)sender;
			Point lowerLeft = new Point(0, clickedColourButton.Height);
			lowerLeft = clickedColourButton.PointToScreen(lowerLeft);
			copyMenu.Show(lowerLeft);
		}

		private void imagesListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (imagesListBox.SelectedIndex >= 0)
			{
				Application.UseWaitCursor = true;
				Cursor.Current = Cursors.WaitCursor;
				
				if (IsUsingModel)
				{
					// use the image in the image window rather than the loaded file
					var images = Model.SourceImages().ToList();
					var srcBitmap = (Bitmap)images[imagesListBox.SelectedIndex].Data.Clone();

					// load all distinct colours from the image.
					int paletteIndex = 0;
					for (int y = 0; y < srcBitmap.Height; y++)
					{
						for (int x = 0; x < srcBitmap.Width; x++)
						{
							Color pixelColour = srcBitmap.GetPixel(x, y);
							thatColour.R = pixelColour.R;
							thatColour.G = pixelColour.G;
							thatColour.B = pixelColour.B;

							bool same = false;
							for (int c = 0; c < paletteIndex; c++)
							{
								if (LoadedPalette[c, 0] == thatColour.R && LoadedPalette[c, 1] == thatColour.G && LoadedPalette[c, 2] == thatColour.B)
								{
									same = true;
								}
							}

							if (!same)
							{
								if (paletteIndex < 256)
								{
									LoadedPalette[paletteIndex, 0] = thatColour.R;
									LoadedPalette[paletteIndex, 1] = thatColour.G;
									LoadedPalette[paletteIndex, 2] = thatColour.B;
									colourButtons[paletteIndex].BackColor = Color.FromArgb(thatColour.R, thatColour.G, thatColour.B);
									paletteIndex++;
								}
								else
								{
									goto NotSupported;
								}
							}
						}
					}

					// clear all subsequent colours and buttons.
					SetUnusedColour(paletteIndex, LoadedPalette.Length / 3, (i, r, g ,b) =>
					{
						colourButtons[i].BackColor = Color.FromArgb(r, g, b);
					});

					loadedColourCount = paletteIndex;
					countTextBox.Text = loadedColourCount.ToString();
					fromTextBox.Text = "0";
					toTextBox.Text = "0";
				}
				else
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

								if (isLittleEndian == true)
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
									if (isLittleEndian == true)
									{
										transparentColourIndex = bytesBuffer[1] << 8 | bytesBuffer[0];
									}
									else
									{
										transparentColourIndex = bytesBuffer[0] << 8 | bytesBuffer[1];
									}

									if (transparentColourIndex > 255 || transparentColourIndex < 0)
									{
										transparentColourIndex = 0;
									}
								}
							}

							for (int c = 0; c < 256; c++)
							{
								colourButtons[c].BackColor = Color.FromArgb(LoadedPalette[c, 0], LoadedPalette[c, 1], LoadedPalette[c, 2]);
							}
						}
					}

					loadedColourCount = 255;
					loadedColourStart = 0;
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
			FillFilenames(Model.SourceImages().Select(image => $" {image.Filename}"));
		}

		public void FillFilenames(IEnumerable<string> filenames)
		{
			fullNames.Clear();
			fullNames.AddRange(filenames);

			imagesListBox.Items.Clear();
			imagesListBox.Items.AddRange(fullNames.Select(n => Path.GetFileName(n)).ToArray());

			UpdateImportButton();
		}

		#endregion

		#region Helpers

		private void CreatePalette()
		{
			int across = 0;
			int down = 0;

			for (int c = 0; c < LoadedPalette.Length / 3; c++)
			{
				SetUnusedColour(c);

				colourButtons[c] = new Button();
				colourButtons[c].Text = "";
				colourButtons[c].Location = new Point(10 + (across * 20), 15 + (down * 20));
				colourButtons[c].Size = new Size(22, 22);
				colourButtons[c].Click += colourButton_Click;
				colourButtons[c].Name = c.ToString();
				colourButtons[c].FlatStyle = FlatStyle.Flat;
				colourButtons[c].FlatAppearance.BorderColor = SystemColors.ControlDark;
				colourButtons[c].FlatAppearance.BorderSize = 1;
				Controls.Add(colourButtons[c]);

				colourButtons[c].BackColor = Color.FromArgb(LoadedPalette[c, 0], LoadedPalette[c, 1], LoadedPalette[c, 2]);

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

		private void SetUnusedColour(int index, int count = 1, Action<int, byte, byte, byte> handler = null) {
			var max = Math.Min(index + count, LoadedPalette.Length / 3);
			var colour = SystemColors.Control;

			for (int i = index; i < max; i++)
			{
				LoadedPalette[i, 0] = colour.R;
				LoadedPalette[i, 1] = colour.G;
				LoadedPalette[i, 2] = colour.B;

				if (handler != null)
				{
					handler(i, colour.R, colour.G, colour.B);
				}
			}
		}

		#endregion
	}
}
