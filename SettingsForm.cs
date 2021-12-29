using NextGraphics.Models;

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
	public partial class SettingsForm : Form
	{
		public MainModel Model { get; set; }

		#region Initialization & Disposal

		public SettingsForm()
		{
			InitializeComponent();
			MC.Checked		=	true;
		}

		#endregion

		#region Events

		private void settingsPanel_Load(object sender, EventArgs e)
		{
			ApplyCenterPosition(Model.CenterPosition);
			FourBit.Checked = Model.FourBit;
			reduce.Checked = Model.Reduced;
			textFlips.Checked = Model.TextFlips;
			binaryOut.Checked = Model.BinaryOutput;
			binaryBlocks.Enabled = binaryOut.Checked;
			Repeats.Checked = Model.IgnoreCopies;
			mirrorX.Checked = Model.IgnoreMirroredX;
			mirrorY.Checked = Model.IgnoreMirroredY;
			rotations.Checked = Model.IgnoreRotated;
			Transparent.Checked = Model.IgnoreTransparentPixels;
			sortTransparent.Checked = Model.TransparentFirst;
			blocksOut.Checked = Model.BlocksAsImage;
			tilesOut.Checked = Model.TilesAsImage;
			transBlock.Checked = Model.TransparentBlocks;
			transTile.Checked = Model.TransparentTiles;
			tilesAcross.Text = Model.BlocsAcross.ToString();
			blocksFormat.SelectedIndex = (int)Model.ImageFormat;
			Accuracy.Text = Model.Accuracy.ToString();
		}

		private void settingsPanel_FormClosing(object sender, FormClosingEventArgs e)
		{
			Model.CenterPosition = GetCenterPosition();
			Model.FourBit = FourBit.Checked;
			Model.Reduced = reduce.Checked;
			Model.TextFlips = textFlips.Checked;
			Model.BinaryOutput = binaryOut.Checked;
			Model.IgnoreCopies = Repeats.Checked;
			Model.IgnoreMirroredX = mirrorX.Checked;
			Model.IgnoreMirroredY = mirrorY.Checked;
			Model.IgnoreRotated = rotations.Checked;
			Model.IgnoreTransparentPixels = Transparent.Checked;
			Model.TransparentFirst = sortTransparent.Checked;
			Model.BlocksAsImage = blocksOut.Checked;
			Model.TilesAsImage = tilesOut.Checked;
			Model.TransparentBlocks = transBlock.Checked;
			Model.TransparentTiles = transTile.Checked;
			Model.BlocsAcross = int.Parse(tilesAcross.Text);
			Model.ImageFormat = (ImageFormat)blocksFormat.SelectedIndex;
			Model.Accuracy = int.Parse(Accuracy.Text);
		}

		private void binaryOut_CheckedChanged(object sender, EventArgs e)
		{
			// Binary blocks option is only enabled if binary out is checked
			binaryBlocks.Enabled = binaryOut.Checked;
		}

		#endregion

		#region Helpers

		private void ApplyCenterPosition(int center)
		{
			switch (center)
			{
				case 0:
					TL.Checked = true;
					break;
				case 1:
					TC.Checked = true;
					break;
				case 2:
					TR.Checked = true;
					break;
				case 3:
					ML.Checked = true;
					break;
				case 4:
					MC.Checked = true;
					break;
				case 5:
					MR.Checked = true;
					break;
				case 6:
					BL.Checked = true;
					break;
				case 7:
					BC.Checked = true;
					break;
				case 8:
					BR.Checked = true;
					break;
			}
		}

		private int GetCenterPosition()
		{
			if (TL.Checked)
			{
				return 0;
			}
			else if (TC.Checked)
			{
				return 1;
			}
			else if (TR.Checked)
			{
				return 2;
			}
			else if (ML.Checked)
			{
				return 3;
			}
			else if (MC.Checked)
			{
				return 4;
			}
			else if (MR.Checked)
			{
				return 5;
			}
			else if (BL.Checked)
			{
				return 6;
			}
			else if (BC.Checked)
			{
				return 7;
			}
			else //if(BR.Checked)
			{
				return 8;
			}
		}

		#endregion
	}
}
