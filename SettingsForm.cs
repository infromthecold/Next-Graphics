using NextGraphics.Models;

using System;
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
			spriteCenterMCRadioButton.Checked		=	true;
		}

		#endregion

		#region Events

		private void settingsPanel_Load(object sender, EventArgs e)
		{
			ignoreAccuracyTextBox.Text = Model.Accuracy.ToString();
			ignoreCopiesCheckBox.Checked = Model.IgnoreCopies;
			ignoreMirroredXCheckBox.Checked = Model.IgnoreMirroredX;
			ignoreMirroredYCheckBox.Checked = Model.IgnoreMirroredY;
			ignoreRotatedCheckBox.Checked = Model.IgnoreRotated;
			ignoreTransparentPixelsCheckBox.Checked = Model.IgnoreTransparentPixels;

			ApplyCenterPosition(Model.CenterPosition);

			output4BitsCheckBox.Checked = Model.FourBit;
			outputReducedCheckBox.Checked = Model.Reduced;
			outputTextFlipsCheckBox.Checked = Model.AttributesAsText;
			outputBinaryCheckBox.Checked = Model.BinaryOutput;
			outputBinaryBlocksCheckBox.Enabled = outputBinaryCheckBox.Checked;
			outputTransparentFirstCheckBox.Checked = Model.TransparentFirst;
			outputCommentLevelComboBox.SelectedIndex = (int)Model.CommentType;
			outputPaletteFormatComboBox.SelectedIndex = (int)Model.PaletteFormat;

			mappingBlocksAsImageCheckBox.Checked = Model.BlocksAsImage;
			mappingTilesAsImageCheckBox.Checked = Model.TilesAsImage;
			mappingTransparentBlocksCheckBox.Checked = Model.TransparentBlocks;
			mappingTransparentTilesCheckBox.Checked = Model.TransparentTiles;
			mappingTilesAcrossTextBox.Text = Model.BlocsAcross.ToString();
			mappingFileFormatComboBox.SelectedIndex = (int)Model.ImageFormat;
		}

		private void settingsPanel_FormClosing(object sender, FormClosingEventArgs e)
		{
			Model.IgnoreCopies = ignoreCopiesCheckBox.Checked;
			Model.IgnoreMirroredX = ignoreMirroredXCheckBox.Checked;
			Model.IgnoreMirroredY = ignoreMirroredYCheckBox.Checked;
			Model.IgnoreRotated = ignoreRotatedCheckBox.Checked;
			Model.IgnoreTransparentPixels = ignoreTransparentPixelsCheckBox.Checked;
			Model.Accuracy = int.Parse(ignoreAccuracyTextBox.Text);

			Model.CenterPosition = GetCenterPosition();

			Model.FourBit = output4BitsCheckBox.Checked;
			Model.Reduced = outputReducedCheckBox.Checked;
			Model.AttributesAsText = outputTextFlipsCheckBox.Checked;
			Model.BinaryOutput = outputBinaryCheckBox.Checked;
			Model.TransparentFirst = outputTransparentFirstCheckBox.Checked;
			Model.CommentType = (CommentType)outputCommentLevelComboBox.SelectedIndex;
			Model.PaletteFormat = (PaletteFormat)outputPaletteFormatComboBox.SelectedIndex;

			Model.BlocksAsImage = mappingBlocksAsImageCheckBox.Checked;
			Model.TilesAsImage = mappingTilesAsImageCheckBox.Checked;
			Model.TransparentBlocks = mappingTransparentBlocksCheckBox.Checked;
			Model.TransparentTiles = mappingTransparentTilesCheckBox.Checked;
			Model.BlocsAcross = int.Parse(mappingTilesAcrossTextBox.Text);
			Model.ImageFormat = (ImageFormat)mappingFileFormatComboBox.SelectedIndex;
		}

		private void binaryOut_CheckedChanged(object sender, EventArgs e)
		{
			// Binary blocks option is only enabled if binary out is checked
			outputBinaryBlocksCheckBox.Enabled = outputBinaryCheckBox.Checked;
		}

		#endregion

		#region Helpers

		private void ApplyCenterPosition(int center)
		{
			switch (center)
			{
				case 0:
					spriteCenterTLRadioButton.Checked = true;
					break;
				case 1:
					spriteCenterTCRadioButton.Checked = true;
					break;
				case 2:
					spriteCenterTRRadioButton.Checked = true;
					break;
				case 3:
					spriteCenterMLRadioButton.Checked = true;
					break;
				case 4:
					spriteCenterMCRadioButton.Checked = true;
					break;
				case 5:
					spriteCenterMRRadioButton.Checked = true;
					break;
				case 6:
					spriteCenterBLRadioButton.Checked = true;
					break;
				case 7:
					spriteCenterBCRadioButton.Checked = true;
					break;
				case 8:
					spriteCenterBRRadioButton.Checked = true;
					break;
			}
		}

		private int GetCenterPosition()
		{
			if (spriteCenterTLRadioButton.Checked)
			{
				return 0;
			}
			else if (spriteCenterTCRadioButton.Checked)
			{
				return 1;
			}
			else if (spriteCenterTRRadioButton.Checked)
			{
				return 2;
			}
			else if (spriteCenterMLRadioButton.Checked)
			{
				return 3;
			}
			else if (spriteCenterMCRadioButton.Checked)
			{
				return 4;
			}
			else if (spriteCenterMRRadioButton.Checked)
			{
				return 5;
			}
			else if (spriteCenterBLRadioButton.Checked)
			{
				return 6;
			}
			else if (spriteCenterBCRadioButton.Checked)
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
