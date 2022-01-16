using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NextGraphics
{
	public partial class RebuildForm : Form
	{

		private OpenFileDialog reverseDialog = new OpenFileDialog();
		private SaveFileDialog reversedFilesDialog = new SaveFileDialog();

		#region Initialization & Disposal

		public RebuildForm()
		{
			InitializeComponent();
			rebuildButton.Enabled = false;
			outputFileTextBox.Enabled = false;
			blockFileTextBox.Enabled = false;
		}

		#endregion

		#region Events

		private void saveAsButton_Click(object sender, EventArgs e)
		{
			reversedFilesDialog.Filter = "Tile BMP Files (*.bmp)|*.bmp|Tile PNG Files (*.png)|*.png|Tile JPG Files (*.jpg)|*.jpg";
			reversedFilesDialog.FilterIndex = 2;
			reversedFilesDialog.RestoreDirectory = true;
			if (reversedFilesDialog.ShowDialog() == DialogResult.OK)
			{
				outputFileTextBox.Text = reversedFilesDialog.FileName;
				if (blockFileTextBox.Text != "")
				{
					rebuildButton.Enabled = true;
				}
			}
		}

		private void browseButton_Click(object sender, EventArgs e)
		{
			reverseDialog.Multiselect = false;
			reverseDialog.RestoreDirectory = true;
			reverseDialog.Filter = "Reversae Files (*.blk)|*.blk";
			if (reverseDialog.ShowDialog(this) == DialogResult.OK)
			{
				blockFileTextBox.Text = Path.ChangeExtension(reverseDialog.FileName, "blk");
				if (outputFileTextBox.Text != "")
				{
					rebuildButton.Enabled = true;
				}
			}
		}

		private void rebuildButton_Click(object sender, EventArgs e)
		{
			Int16 inInt = 0;
			using (BinaryReader reverseFile = new BinaryReader(File.Open(blockFileTextBox.Text, FileMode.Open)))
			{
				int blocksInRow = int.Parse(blocksInRowTextBox.Text);
				if (blocksInRow <= 0)
				{
					blocksInRow = 1;
				}
				if (blocksInRow >= 255)
				{
					blocksInRow = 255;
				}
				string charFile = reverseFile.ReadString();
				Bitmap charset = new Bitmap(charFile);
				byte charsWide = reverseFile.ReadByte();
				byte charsTall = reverseFile.ReadByte();
				byte numBlocks = reverseFile.ReadByte();
				Bitmap blocksOut = new Bitmap((charsWide * 8) * (blocksInRow), (charsTall * 8) * (int)(Math.Round((double)(numBlocks / (blocksInRow))) + 1), PixelFormat.Format24bppRgb);
				int strideChars = (charset.Width / 8);
				int blockX = 0;
				int blockY = 0;
				int startBlock = 0;
				if (zeroBlockCheckBox.Checked == false)
				{
					for (int yc = 0; yc < charsWide; yc++)
					{
						for (int xc = 0; xc < charsWide; xc++)
						{
							inInt = reverseFile.ReadInt16();
						}
					}
					startBlock = 1;
				}
				for (int b = startBlock; b < numBlocks; b++)
				{
					for (int yc = 0; yc < charsWide; yc++)
					{
						for (int xc = 0; xc < charsWide; xc++)
						{
							inInt = reverseFile.ReadInt16();
							bool flippedX = false;
							bool flippedY = false;
							bool rotated = false;
							Color readColour = new Color();
							if ((inInt & (1 << 15)) != 0)
							{
								flippedX = true;
							}
							if ((inInt & (1 << 14)) != 0)
							{
								flippedY = true;
							}
							if ((inInt & (1 << 13)) != 0)
							{
								rotated = true;
							}
							inInt = (short)(inInt & 0x07ff);
							int yIndex = inInt / strideChars;
							int xIndex = inInt % strideChars;
							int rx = 0;
							int ry = 0;
							int temp = 0;
							for (int y = 0; y < 8; y++)
							{
								for (int x = 0; x < 8; x++)
								{
									rx = x;
									ry = y;
									if (flippedY == true)
									{
										ry = (7 - y);
									}
									if (flippedX == true)
									{
										rx = (7 - x);
									}
									if (rotated == true)
									{
										temp = rx;
										rx = ry;
										ry = temp;
									}
									readColour = charset.GetPixel(rx + (xIndex * 8), ry + (yIndex * 8));
									blocksOut.SetPixel(x + (xc * 8) + (blockX * (charsWide * 8)), y + (yc * 8) + (blockY * (charsTall * 8)), readColour);
								}
							}
						}
					}
					blockX++;
					if (blockX >= blocksInRow)
					{
						blockX = 0;
						blockY++;
					}
				}
				string reversedPath = Path.GetDirectoryName(outputFileTextBox.Text);
				string reversedName = Path.GetFileNameWithoutExtension(outputFileTextBox.Text);
				switch (reversedFilesDialog.FilterIndex)
				{
					case 1: // bmp				
						blocksOut.Save(reversedPath + "\\" + reversedName + "-rebuilt.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
						break;
					case 2: // png
						blocksOut.Save(reversedPath + "\\" + reversedName + "-rebuilt.png", System.Drawing.Imaging.ImageFormat.Png);
						break;
					case 3: // jpeg
						blocksOut.Save(reversedPath + "\\" + reversedName + "-rebuilt.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
						break;
				}
			}
			this.Close();
		}

		#endregion
	}
}
