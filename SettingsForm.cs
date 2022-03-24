using NextGraphics.Models;
using NextGraphics.Settings;
using NextGraphics.Utils;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NextGraphics
{
	public partial class SettingsForm : Form
	{
		public MainModel Model {
			get => _model;
			set
			{
				if (value != _model)
				{
					_model = value;

					EstablishDependantControls();
					EstablishModelEventHandlers();
					
					MapModelToCommonControls();
					MapModelToOptionControls();
					MapModelToTilemapControl();
					MapModelToSpriteControls();
					MapModelToFileExtensionControls();
					MapApplicationSettingsControls();
				}
			}
		}
		private MainModel _model;

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
		}

		#endregion

		#region Events

		private void Model_OutputTypeChanged(object sender, MainModel.OutputTypeChangedEventArgs e)
		{
			OutputTypeTextChangingControls.Update(Model.OutputType);
			OutputTypeEnabledControls.Update(Model.OutputType);
		}

		private void Model_GridWidthChanged(object sender, MainModel.SizeChangedEventArgs e)
		{
			commonWidthTextBox.Text = e.Size.ToString();
		}

		private void Model_GridHeightChanged(object sender, MainModel.SizeChangedEventArgs e)
		{
			commonHeightTextBox.Text = e.Size.ToString();
		}

		private void Model_BlocksAcrossChanged(object sender, MainModel.SizeChangedEventArgs e)
		{
			optionsTilesAcrossTextBox.Text = e.Size.ToString();
		}

		private void extensionsDefaultsButton_Click(object sender, EventArgs e)
		{
			Model.ResetFileExtensionsToDefaults();

			UpdateExtensionControls();
		}

		private void extensionsLastUsedButton_Click(object sender, EventArgs e)
		{
			Model.ExportAssemblerFileExtension = Properties.Settings.Default.ExportAssemblerFileExtension;
			Model.ExportBinaryDataFileExtension = Properties.Settings.Default.ExportBinaryDataFileExtension;
			Model.ExportBinaryPaletteFileExtension = Properties.Settings.Default.ExportBinaryPaletteFileExtension;

			Model.ExportBinaryTilesInfoFileExtension = Properties.Settings.Default.ExportBinaryTilesInfoFileExtension;
			Model.ExportBinaryTileAttributesFileExtension = Properties.Settings.Default.ExportBinaryTileAttributesFileExtension;
			Model.ExportBinaryTilemapFileExtension = Properties.Settings.Default.ExportBinaryTilemapFileExtension;

			Model.ExportSpriteAttributesFileExtension = Properties.Settings.Default.ExportSpriteAttributesFileExtension;

			UpdateExtensionControls();
		}

		#endregion

		#region Helpers

		private void EstablishDependantControls()
		{
			// Establish controls which text depends on sprites/tilemap selection.
			OutputTypeTextChangingControls.Add(builder =>
			{
				builder.Add(commonItemSizeLabel);
				builder.Add(optionsBinaryFramesAttributesCheckBox);
				builder.Add(optionsTilesAccrossLabel);
			});

			// Establish all controls which enabled state depends on options "export data as binary files" checkbox. These controls don't depend on any other circumstances.
			ConditionallyEnabledControls.Add(optionsBinaryDataCheckBox, builder =>
			{
				builder.Add(optionsBinaryFramesAttributesCheckBox);
			});

			// Establish controls which need to be enabled only when tilemap mode is selected.
			OutputTypeEnabledControls.Add(OutputType.Tiles, builder =>
			{
				builder.Add(tilemapTransparentFirstCheckBox);
				builder.Add(tilemapTilesAsImageCheckBox);
				builder.Add(tilemapTilesAsImageTransparentFirstCheckBox);
				builder.Add(tilemapExportTypeLabel);
				builder.Add(tilemapExportTypeComboBox);

				// If tilemap mode is selected, then the following controls need to be conditionally enabled based on "export tiles as images" checkbox.
				builder.ConditionallyEnabled(tilemapTilesAsImageCheckBox, conditions =>
				{
					conditions.Add(tilemapTilesAsImageTransparentFirstCheckBox);
				});
			});

			// Establish controls which need to be enabled only when sprites mode is selected.
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

			// Establish initial values.
			OutputTypeTextChangingControls.Update(Model.OutputType);
			OutputTypeEnabledControls.Update(Model.OutputType);
			ConditionallyEnabledControls.Update();
		}

		private void EstablishModelEventHandlers()
		{
			// We need to react to some model events so that changes from main form (or elsewhere in the future) will be reflected here too.
			Model.OutputTypeChanged += Model_OutputTypeChanged;
			Model.GridWidthChanged += Model_GridWidthChanged;
			Model.GridHeightChanged += Model_GridHeightChanged;
			Model.BlocksAcrossChanged += Model_BlocksAcrossChanged;
		}

		private void MapModelToCommonControls()
		{
			void SetupRadioButton(RadioButton radioButton, OutputType type)
			{
				if (Model.OutputType == type)
				{
					radioButton.Checked = true;
				}

				radioButton.CheckedChanged += (sender, e) =>
				{
					if (radioButton.Checked)
					{
						Model.OutputType = type;
					}
				};
			}

			SetupRadioButton(commonSpritesRadioButton, OutputType.Sprites);
			SetupRadioButton(commonTilemapRadioButton, OutputType.Tiles);

			commonWidthTextBox.MapIntTo(() => Model.GridWidth);
			commonHeightTextBox.MapIntTo(() => Model.GridHeight);
			commonIgnoreCopiesCheckBox.MapTo(() => Model.IgnoreCopies);
			commonIgnoreMirroredXCheckBox.MapTo(() => Model.IgnoreMirroredX);
			commonIgnoreMirroredYCheckBox.MapTo(() => Model.IgnoreMirroredY);
			commonIgnoreRotatedCheckBox.MapTo(() => Model.IgnoreRotated);
			commonIgnoreTransparentPixelsCheckBox.MapTo(() => Model.IgnoreTransparentPixels);
			commonAccuracyTextBox.MapIntTo(() => Model.Accuracy);
		}

		private void MapModelToOptionControls()
		{
			optionsCommentLevelComboBox.MapTo(() => Model.CommentType);
			optionsPaletteFormatComboBox.MapTo(() => Model.PaletteFormat);
			optionsPaletteParsingComboBox.MapTo(() => Model.PaletteParsingMethod);
			optionsBinaryDataCheckBox.MapTo(() => Model.BinaryOutput);
			optionsBinaryFramesAttributesCheckBox.MapTo(() => Model.BinaryFramesAttributesOutput);
			optionsFileFormatComboBox.MapTo(() => Model.ImageFormat);
			optionsTilesAcrossTextBox.MapIntTo(() => Model.BlocksAcross);
		}

		private void MapModelToTilemapControl()
		{
			tilemapTransparentFirstCheckBox.MapTo(() => Model.TransparentFirst);
			tilemapTilesAsImageCheckBox.MapTo(() => Model.TilesExportAsImage);
			tilemapTilesAsImageTransparentFirstCheckBox.MapTo(() => Model.TilesExportAsImageTransparent);
			tilemapExportTypeComboBox.MapTo(() => Model.TilemapExportType);
		}

		private void MapModelToSpriteControls()
		{
			void SetupRadioButton(RadioButton radioButton, int value)
			{
				if (Model.CenterPosition == value)
				{
					radioButton.Checked = true;
				}

				radioButton.CheckedChanged += (sender, e) =>
				{
					if (radioButton.Checked)
					{
						Model.CenterPosition = value;
					}
				};
			}

			SetupRadioButton(spriteCenterTLRadioButton, 0);
			SetupRadioButton(spriteCenterTCRadioButton, 1);
			SetupRadioButton(spriteCenterTRRadioButton, 2);
			SetupRadioButton(spriteCenterMLRadioButton, 3);
			SetupRadioButton(spriteCenterMCRadioButton, 4);
			SetupRadioButton(spriteCenterMRRadioButton, 5);
			SetupRadioButton(spriteCenterBLRadioButton, 6);
			SetupRadioButton(spriteCenterBCRadioButton, 7);
			SetupRadioButton(spriteCenterBRRadioButton, 8);

			sprites4BitsCheckBox.MapTo(() => Model.SpritesFourBit);
			spritesReducedCheckBox.MapTo(() => Model.SpritesReduced);
			spritesAttributesAsTextCheckBox.MapTo(() => Model.SpritesAttributesAsText);
			spritesAsImageCheckBox.MapTo(() => Model.SpritesExportAsImages);
		}

		private void MapModelToFileExtensionControls()
		{
			// Settings changes work on the fact that model properties are called exactly like the settings keys.
			extensionsAssemblerTextBox.MapTextTo(() => Model.ExportAssemblerFileExtension, (name, value) => Properties.Settings.Default.SetAndSave(name, value));
			extensionsBinaryDataTextBox.MapTextTo(() => Model.ExportBinaryDataFileExtension, (name, value) => Properties.Settings.Default.SetAndSave(name, value));
			extensionsPaletteTextBox.MapTextTo(() => Model.ExportBinaryPaletteFileExtension, (name, value) => Properties.Settings.Default.SetAndSave(name, value));
			
			extensionsTilemapInfoTextBox.MapTextTo(() => Model.ExportBinaryTilesInfoFileExtension, (name, value) => Properties.Settings.Default.SetAndSave(name, value));
			extensionsTilemapAttributesTextBox.MapTextTo(() => Model.ExportBinaryTileAttributesFileExtension, (name, value) => Properties.Settings.Default.SetAndSave(name, value));
			extensionsTilemapTextBox.MapTextTo(() => Model.ExportBinaryTilemapFileExtension, (name, value) => Properties.Settings.Default.SetAndSave(name, value));

			extensionsSpritesAttributesTextBox.MapTextTo(() => Model.ExportSpriteAttributesFileExtension, (name, value) => Properties.Settings.Default.SetAndSave(name, value));
		}

		private void MapApplicationSettingsControls()
		{
			applicationRenderTileIndexesCheckBox.MapCheckedTo(Properties.Settings.Default.TilemapRenderTileIndex, value =>
			{
				Properties.Settings.Default.TilemapRenderTileIndex = value;
				Properties.Settings.Default.Save();
			});

			applicationRenderTileAttributesCheckBox.MapCheckedTo(Properties.Settings.Default.TilemapRenderTileAttributes, value =>
			{
				Properties.Settings.Default.TilemapRenderTileAttributes = value;
				Properties.Settings.Default.Save();
			});

			applicationBehaviorPrintTilemapInfoCheckBox.MapCheckedTo(Properties.Settings.Default.InfoPrintTilemap, value =>
			{
				Properties.Settings.Default.InfoPrintTilemap = value;
				Properties.Settings.Default.Save();
			});
		}

		private void UpdateExtensionControls()
		{
			extensionsAssemblerTextBox.Text = Model.ExportAssemblerFileExtension;
			extensionsBinaryDataTextBox.Text = Model.ExportBinaryDataFileExtension;
			extensionsPaletteTextBox.Text = Model.ExportBinaryPaletteFileExtension;

			extensionsTilemapInfoTextBox.Text = Model.ExportBinaryTilesInfoFileExtension;
			extensionsTilemapAttributesTextBox.Text = Model.ExportBinaryTileAttributesFileExtension;
			extensionsTilemapTextBox.Text = Model.ExportBinaryTilemapFileExtension;

			extensionsSpritesAttributesTextBox.Text = Model.ExportSpriteAttributesFileExtension;
		}

		#endregion
	}

	// Namespace is to avoid messing up the main namespace with helper classes. Ideally we'd add them inside form class above, however static extension classes are not allowed to be embedded. And since other helper classes are needed with extensions, they must be made visible in there too.
	namespace Settings
	{
		public static class ControlExtensions
		{
			/// <summary>
			/// Maps given <see cref="CheckBox"/> to given boolean property. First updates check box with current value, and assigns event handler that updates property as check box value changes.
			/// </summary>
			public static void MapTo(
				this CheckBox checkBox, 
				Expression<Func<bool>> expression)
			{
				var lambda = expression.Compile();

				var memberExpression = expression.Body as MemberExpression;
				var memberName = memberExpression.Member.Name;
				var memberParent = expression.GetRootObject();

				checkBox.Checked = lambda();

				checkBox.CheckedChanged += (sender, e) =>
				{
					memberParent.GetType().GetProperty(memberName).SetValue(memberParent, checkBox.Checked);
				};
			}

			/// <summary>
			/// Maps given <see cref="ComboBox"/> to given enum or integer property. First updates combo box selection with current value, and assigns selection change event handler that updates the property as selection changes.
			/// </summary>
			public static void MapTo<T>(
				this ComboBox comboBox,
				Expression<Func<T>> expression) where T : IConvertible
			{
				var lambda = expression.Compile();

				var memberExpression = expression.Body as MemberExpression;
				var memberName = memberExpression.Member.Name;
				var memberParent = expression.GetRootObject();

				comboBox.SelectedIndex = lambda().ToInt32(null);

				comboBox.SelectedIndexChanged += (sender, e) =>
				{
					memberParent.GetType().GetProperty(memberName).SetValue(memberParent, comboBox.SelectedIndex);
				};
			}

			/// <summary>
			/// Maps given <see cref="TextBox"/> to given integer property. First updates text box with current vluae, and assigns selection change event handler that updates the property after text box looses focus.
			/// </summary>
			public static void MapIntTo(
				this TextBox textBox,
				Expression<Func<int>> expression)
			{
				var lambda = expression.Compile();

				var memberExpression = expression.Body as MemberExpression;
				var memberName = memberExpression.Member.Name;
				var memberParent = expression.GetRootObject();

				textBox.Text = lambda().ToString();

				textBox.Leave += (sender, e) =>
				{
					if (int.TryParse(textBox.Text, out var value))
					{
						// Update the model.
						memberParent.GetType().GetProperty(memberName).SetValue(memberParent, value);

						// Update the text box with new model value. This takes care of the case where model will adjust the value internally so it's not exactly the one just set.
						textBox.Text = lambda().ToString();
					}
					else
					{
						// If parsing the value fails, reset back to current model value.
						textBox.Text = lambda().ToString();
					}
				};
			}

			/// <summary>
			/// Maps given <see cref="TextBox"/> to given string property. First updates text box with current vluae, and assigns selection change event handler that updates the property after text box looses focus. Additionally, the given lambda is optionally called after updating the model, if provided. 2 parameters are passed, first is the name of the property that changed, the second is the new value.
			/// </summary>
			public static void MapTextTo(
				this TextBox textBox,
				Expression<Func<string>> expression,
				Action<string,string> customAction = null)
			{
				var lambda = expression.Compile();

				var memberExpression = expression.Body as MemberExpression;
				var memberName = memberExpression.Member.Name;
				var memberParent = expression.GetRootObject();

				textBox.Text = lambda();

				textBox.Leave += (sender, e) =>
				{
					var value = textBox.Text;

					// Update the model.
					memberParent.GetType().GetProperty(memberName).SetValue(memberParent, value);

					// Inform the caller.
					customAction?.Invoke(memberName, value);
				};
			}

			public static void MapCheckedTo(
				this CheckBox checkBox,
				bool currentValue,
				Action<bool> changed)
			{
				checkBox.Checked = currentValue;

				checkBox.CheckedChanged += (o, e) =>
				{
					changed(checkBox.Checked);
				}; 
			}

			private static object GetRootObject<T>(this Expression<Func<T>> expression)
			{
				// Convert Expression to MemberExpression. This should succeed in general, but let's be safe rather than sorry.
				var propertyAccessExpression = expression.Body as MemberExpression;
				if (propertyAccessExpression == null)
				{
					return null;
				}

				// Go up through members chain until we arrive to the root.
				while (propertyAccessExpression.Expression is MemberExpression)
				{
					propertyAccessExpression = (MemberExpression)propertyAccessExpression.Expression;
				}

				// The last expression is a constant expression that points to our form; the member is the MainModel assigned to as Model property.
				var rootObjectConstantExpression = propertyAccessExpression.Expression as ConstantExpression;
				if (rootObjectConstantExpression == null)
				{
					return null;
				}

				// We can use the constant expression to get access to Model property.
				var propertyName = propertyAccessExpression.Member.Name;
				var formObject = rootObjectConstantExpression.Value;
				return formObject.GetType().GetProperty(propertyName).GetValue(formObject, null);
			}
		}

		// This whole section below is a bit messy implementation wise (especially the builders), but it makes building the structures more readable with DSL it creates.

		public static class DataExtensions
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
