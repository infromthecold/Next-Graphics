using NextGraphics.Models;
using NextGraphics.Settings;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NextGraphics
{
	public partial class SettingsForm : Form
	{
		public MainModel Model { get; set; }

		private List<Settings.OutputTypeNamingData> OutputTypeTextChangingControls { get; set; }
			= new List<Settings.OutputTypeNamingData>();

		private List<Settings.OutputTypeEnabledData> OutputTypeEnabledControls { get; set; }
			= new List<Settings.OutputTypeEnabledData>();

		private List<Settings.ConditionallyEnabledData> ConditionallyEnabledControls { get; set; }
			= new List<Settings.ConditionallyEnabledData>();

		#region Initialization & Disposal

		public SettingsForm()
		{
			InitializeComponent();

			OutputTypeTextChangingControls.Add(builder =>
			{
				builder.Add(commonItemSizeLabel);
				builder.Add(optionsBinaryFramesAttributesCheckBox);
				builder.Add(optionsTilesAccrossLabel);
			});

			ConditionallyEnabledControls.Add(optionsBinaryDataCheckBox, conditionals =>
			{
				conditionals.Add(optionsBinaryFramesAttributesCheckBox);
			});

			OutputTypeEnabledControls.Add(OutputType.Tiles, builder =>
			{
				builder.Add(tilemapTransparentFirstCheckBox);
				builder.Add(tilemapTilesAsImageCheckBox);
				builder.Add(tilemapTilesAsImageTransparentFirstCheckBox);
				builder.Add(tilemapExportTypeLabel);
				builder.Add(tilemapExportTypeComboBox);

				builder.ConditionallyEnabled(tilemapTilesAsImageCheckBox, conditions =>
				{
					conditions.Add(tilemapTilesAsImageTransparentFirstCheckBox);
				});
			});

			OutputTypeEnabledControls.Add(OutputType.Sprites, builder =>
			{
				builder.Add(spriteCenterTLRadioButton);
				builder.Add(spriteCenterTCRadioButton);
				builder.Add(spriteCenterTRRadioButton);
				builder.Add(spriteCenterMLRadioButton);
				builder.Add(spriteCenterMCRadioButton);
				builder.Add(spriteCenterMRRadioButton);
				builder.Add(spriteCenterBLRadioButton);
				builder.Add(spriteCenterBCRadioButton);
				builder.Add(spriteCenterBRRadioButton);
				builder.Add(sprites4BitsCheckBox);
				builder.Add(spritesReducedCheckBox);
				builder.Add(spritesAttributesAsTextCheckBox);
				builder.Add(spritesAsImageCheckBox);
			});
		}

		#endregion

		#region Events

		#region Form

		private void settingsPanel_Load(object sender, EventArgs e)
		{
			CommonUpdateOutputTypeControls(Model.OutputType);
			commonWidthTextBox.Text = Model.GridWidth.ToString();
			commonHeightTextBox.Text = Model.GridHeight.ToString();
			commonIgnoreCopiesCheckBox.Checked = Model.IgnoreCopies;
			commonIgnoreMirroredXCheckBox.Checked = Model.IgnoreMirroredX;
			commonIgnoreMirroredYCheckBox.Checked = Model.IgnoreMirroredY;
			commonIgnoreRotatedCheckBox.Checked = Model.IgnoreRotated;
			commonIgnoreTransparentPixelsCheckBox.Checked = Model.IgnoreTransparentPixels;
			commonAccuracyTextBox.Text = Model.Accuracy.ToString();

			optionsCommentLevelComboBox.SelectedIndex = (int)Model.CommentType;
			optionsPaletteFormatComboBox.SelectedIndex = (int)Model.PaletteFormat;
			optionsBinaryDataCheckBox.Checked = Model.BinaryOutput;
			optionsBinaryFramesAttributesCheckBox.Checked = Model.BinaryFramesAttributesOutput;
			optionsFileFormatComboBox.SelectedIndex = (int)Model.ImageFormat;
			optionsTilesAcrossTextBox.Text = Model.BlocsAcross.ToString();

			tilemapTransparentFirstCheckBox.Checked = Model.TransparentFirst;
			tilemapTilesAsImageCheckBox.Checked = Model.TilesExportAsImage;
			tilemapTilesAsImageTransparentFirstCheckBox.Checked = Model.TilesExportAsImageTransparent;
			tilemapExportTypeComboBox.SelectedIndex = (int)Model.TilemapExportType;

			SpritesUpdateCenterPositionControls(Model.CenterPosition);
			sprites4BitsCheckBox.Checked = Model.SpritesFourBit;
			spritesReducedCheckBox.Checked = Model.SpritesReduced;
			spritesAttributesAsTextCheckBox.Checked = Model.SpritesAttributesAsText;
			spritesAsImageCheckBox.Checked = Model.SpritesExportAsImages;

			OutputTypeTextChangingControls.Update(Model.OutputType);
			OutputTypeEnabledControls.Update(Model.OutputType);
			ConditionallyEnabledControls.Update();

			Model.OutputTypeChanged += Model_OutputTypeChanged;
			Model.GridWidthChanged += Model_GridWidthChanged;
			Model.GridHeightChanged += Model_GridHeightChanged;
		}

		private void settingsPanel_FormClosing(object sender, FormClosingEventArgs e)
		{
			Model.OutputType = commonSpritesRadioButton.Checked ? OutputType.Sprites : OutputType.Tiles;
			Model.IgnoreCopies = commonIgnoreCopiesCheckBox.Checked;
			Model.IgnoreMirroredX = commonIgnoreMirroredXCheckBox.Checked;
			Model.IgnoreMirroredY = commonIgnoreMirroredYCheckBox.Checked;
			Model.IgnoreRotated = commonIgnoreRotatedCheckBox.Checked;
			Model.IgnoreTransparentPixels = commonIgnoreTransparentPixelsCheckBox.Checked;
			Model.Accuracy = int.Parse(commonAccuracyTextBox.Text);

			Model.CommentType = (CommentType)optionsCommentLevelComboBox.SelectedIndex;
			Model.PaletteFormat = (PaletteFormat)optionsPaletteFormatComboBox.SelectedIndex;
			Model.BinaryOutput = optionsBinaryDataCheckBox.Checked;
			Model.BinaryFramesAttributesOutput = optionsBinaryFramesAttributesCheckBox.Checked;
			Model.ImageFormat = (ImageFormat)optionsFileFormatComboBox.SelectedIndex;
			Model.BlocsAcross = int.Parse(optionsTilesAcrossTextBox.Text);

			Model.TransparentFirst = tilemapTransparentFirstCheckBox.Checked;
			Model.TilesExportAsImage = tilemapTilesAsImageCheckBox.Checked;
			Model.TilesExportAsImageTransparent = tilemapTilesAsImageTransparentFirstCheckBox.Checked;
			Model.TilemapExportType = (TilemapExportType)tilemapExportTypeComboBox.SelectedIndex;

			Model.CenterPosition = SpritesGetCenterPosition();
			Model.SpritesFourBit = sprites4BitsCheckBox.Checked;
			Model.SpritesReduced = spritesReducedCheckBox.Checked;
			Model.SpritesAttributesAsText = spritesAttributesAsTextCheckBox.Checked;
			Model.SpritesExportAsImages = spritesAsImageCheckBox.Checked;
		}

		#endregion

		#region Common

		private void commonSpritesRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			var control = sender as RadioButton;
			if (control.Checked)
			{
				Model.OutputType = OutputType.Sprites;
				OutputTypeTextChangingControls.Update(Model.OutputType);
				OutputTypeEnabledControls.Update(Model.OutputType);
			}
		}

		private void commonTilemapRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			var control = sender as RadioButton;
			if (control.Checked) {
				Model.OutputType = OutputType.Tiles;
				OutputTypeTextChangingControls.Update(Model.OutputType);
				OutputTypeEnabledControls.Update(Model.OutputType);
			}
		}

		private void commonWidthTextBox_Leave(object sender, EventArgs e)
		{
			if (int.TryParse(commonWidthTextBox.Text, out int size))
			{
				// Make sure arbitrary value is constrained within allowed range.
				Model.GridWidth = Model.ConstrainItemWidth(size);

				// Make sure UI reflects the actual value, in case it was constrained.
				commonWidthTextBox.Text = Model.GridWidth.ToString();
			}
			else
			{
				// When unable to parse, leave original value.
				commonWidthTextBox.Text = Model.GridWidth.ToString();
			}
		}

		private void commonHeightTextBox_Leave(object sender, EventArgs e)
		{
			if (int.TryParse(commonHeightTextBox.Text, out int size))
			{
				// Make sure arbitrary value is constrained within allowed range.
				Model.GridHeight = Model.ConstrainItemHeight(size);

				// Make sure UI reflects the actual value.
				commonHeightTextBox.Text = Model.GridHeight.ToString();
			}
			else
			{
				// When unable to parse, leave original value.
				commonHeightTextBox.Text = Model.GridHeight.ToString();
			}
		}

		#endregion

		#region Model

		private void Model_OutputTypeChanged(object sender, MainModel.OutputTypeChangedEventArgs e)
		{
			CommonUpdateOutputTypeControls(e.OutputType);
		}

		private void Model_GridWidthChanged(object sender, MainModel.SizeChangedEventArgs e)
		{
			// When model width changes, we should simply apply it to UI without any further handling
			commonWidthTextBox.Text = e.Size.ToString();
		}

		private void Model_GridHeightChanged(object sender, MainModel.SizeChangedEventArgs e)
		{
			// When model height changes, we should simply apply it to UI without any further handling.
			commonHeightTextBox.Text = e.Size.ToString();
		}

		#endregion

		#endregion

		#region Helpers

		private void CommonUpdateOutputTypeControls(OutputType type)
		{
			switch (type)
			{
				case OutputType.Sprites:
					commonSpritesRadioButton.Checked = true;
					break;
				case OutputType.Tiles:
					commonTilemapRadioButton.Checked = true;
					break;
			}
		}

		private void SpritesUpdateCenterPositionControls(int center)
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

		private int SpritesGetCenterPosition()
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

	// Namespace is to avoid messing up the main namespace with helper classes. Ideally we'd add them inside form class above, however static extension classes are not allowed to be embedded. And since other helper classes are needed with extensions, they must be made visible in there too.
	namespace Settings
	{
		// This whole section is a bit messy implementation wise (especially the builders), but it makes building the structures more readable with DSL it creates.

		public static class Extensions
		{
			public static void Add(
				this List<OutputTypeNamingData> list, 
				Action<OutputTypeNamingData.Builder> builder)
			{
				var handler = new OutputTypeNamingData.Builder(list);

				builder(handler);
			}

			public static void Add(
				this List<OutputTypeEnabledData> list, 
				OutputType type, 
				Action<OutputTypeEnabledData.Builder> builder)
			{
				var item = new OutputTypeEnabledData(type);

				var handler = new OutputTypeEnabledData.Builder(item);
				builder(handler);

				list.Add(item);
			}

			public static void Add(
				this List<ConditionallyEnabledData> list, 
				CheckBox parent, Action<ConditionallyEnabledData.Builder> builder)
			{
				var item = new ConditionallyEnabledData(parent);

				var handler = new ConditionallyEnabledData.Builder(item);
				builder(handler);

				list.Add(item);
			}

			public static void Update(
				this List<OutputTypeNamingData> list, 
				OutputType type)
			{
				foreach (var item in list)
				{
					item.Update(type);
				}
			}

			public static void Update(
				this List<OutputTypeEnabledData> list,
				OutputType type)
			{
				foreach (var item in list)
				{
					item.Update(type);
				}
			}

			public static void Update(
				this List<ConditionallyEnabledData> list)
			{
				foreach (var item in list)
				{
					item.Update();
				}
			}
		}

		public class OutputTypeNamingData
		{
			private object Control { get; set; }

			private string OriginalName { get; set; }

			public OutputTypeNamingData(object control)
			{
				switch (control)
				{
					case ButtonBase button:
						OriginalName = button.Text;
						break;

					case Label label:
						OriginalName = label.Text;
						break;

					default:
						throw new NotSupportedException($"Control {control.GetType().Name} is not supported yet!");
				}

				Control = control;
			}

			public void Update(OutputType type)
			{
				switch (Control)
				{
					case ButtonBase button:
						button.Text = GenerateText(type);
						break;

					case Label label:
						label.Text = GenerateText(type);
						break;
				}
			}

			private string GenerateText(OutputType type)
			{
				var regex = new Regex("([^\\{]*)\\{([^\\|]+)\\|([^}]+)\\}(.*)");
				var match = regex.Match(OriginalName);
				if (!match.Success) return OriginalName;

				string Generate(string value)
				{
					return $"{match.Groups[1].Value}{value}{match.Groups[4].Value}";
				}

				switch (type)
				{
					case OutputType.Sprites:
						return Generate(match.Groups[2].Value);
					case OutputType.Tiles:
						return Generate(match.Groups[3].Value);
				}

				return OriginalName;
			}

			// Builder is only needed to make registration less verbose. It could
			public class Builder
			{
				private List<OutputTypeNamingData> List;
					
				public Builder(List<OutputTypeNamingData> list)
				{
					this.List = list;
				}

				public void Add(Control control)
				{
					List.Add(new OutputTypeNamingData(control));
				}
			}
		}

		public class OutputTypeEnabledData
		{
			private OutputType EnabledWhenType { get; set; }
			private List<Control> UnconditionalControls { get; set; } = new List<Control>();
			private List<ConditionallyEnabledData> ConditionalControls { get; set; } = new List<ConditionallyEnabledData>();

			public OutputTypeEnabledData(OutputType enabledWhenType)
			{
				this.EnabledWhenType = enabledWhenType;
			}

			public void Update(OutputType type)
			{
				var isTypeMatched = (type == EnabledWhenType);

				UnconditionalControls.ForEach(c => c.Enabled = isTypeMatched);

				ConditionalControls.ForEach(c =>
				{
					if (isTypeMatched)
					{
						// If this is our type, then update all conditionals according to their parent.
						c.Update();
					}
					else
					{
						// If this is not our type, then disable all conditionals.
						c.Disable();
					}
				});
			}

			public class Builder
			{
				private List<Control> UnconditionalControls { get; set; } = new List<Control>();
				private List<ConditionallyEnabledData> ConditionalControls { get; set; } = new List<ConditionallyEnabledData>();

				public Builder(OutputTypeEnabledData item) : this(item.UnconditionalControls, item.ConditionalControls)
				{
				}

				public Builder(List<Control> unconditionals, List<ConditionallyEnabledData> conditionals)
				{
					this.UnconditionalControls = unconditionals;
					this.ConditionalControls = conditionals;
				}

				public void Add(Control control)
				{
					UnconditionalControls.Add(control);
				}

				public void ConditionallyEnabled(CheckBox parent, Action<ConditionallyEnabledData.Builder> builder)
				{
					ConditionalControls.Add(parent, builder);
				}
			}
		}

		public class ConditionallyEnabledData
		{
			private CheckBox Parent { get; set; }
			private List<ControlData> ConditionalControls { get; set; } = new List<ControlData>();

			public ConditionallyEnabledData(CheckBox parent)
			{
				this.Parent = parent;

				parent.CheckedChanged += Parent_CheckedChanged;
			}

			public void Update()
			{
				ConditionalControls.ForEach(c => c.Update(Parent.Checked));
			}

			public void Disable()
			{
				ConditionalControls.ForEach(c => c.Update(false));
			}

			private void Parent_CheckedChanged(object sender, EventArgs e)
			{
				Update();
			}

			private class ControlData
			{
				private Control Control { get; set; }
				private bool EnableIfChecked { get; set; }

				public ControlData(Control control, bool enableIfChecked = true)
				{
					this.Control = control;
					this.EnableIfChecked = enableIfChecked;
				}

				public void Update(bool isParentChecked)
				{
					if (EnableIfChecked)
					{
						Control.Enabled = isParentChecked;
					}
					else
					{
						Control.Enabled = !isParentChecked;
					}
				}
			}

			public class Builder
			{
				private ConditionallyEnabledData Item { get; set; }

				public Builder(ConditionallyEnabledData item)
				{
					this.Item = item;
				}

				public void Add(CheckBox parent, bool enableIfChecked = true)
				{
					var item = new ControlData(parent, enableIfChecked);

					Item.ConditionalControls.Add(item);
				}
			}
		}
	}
}
