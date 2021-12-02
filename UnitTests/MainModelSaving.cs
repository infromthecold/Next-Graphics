using Microsoft.VisualStudio.TestTools.UnitTesting;

using NextGraphics.Models;

using System;
using System.Xml;

namespace UnitTests
{
	[TestClass]
	public class MainModelSaving
	{
		#region Common

		[TestMethod]
		public void RootNodesSaved()
		{
			// Setup
			var model = new MainModel();

			// Execute
			var document = model.Save();

			// Verify
			Assert.AreEqual("XML", document.DocumentElement.Name);
			Assert.AreEqual(1, document.DocumentElement.ChildNodes.Count);
			Assert.AreEqual("Project", document.DocumentElement.FirstChild.Name);
		}

		[TestMethod]
		public void ProjectNameSaved()
		{
			// Setup
			var model = new MainModel();
			model.Name = "Test";

			// Execute
			var document = model.Save();

			// Verify
			Assert.AreEqual("Test", document.ProjectNameNode().Attributes["Projectname"].Value);
		}

		#endregion

		#region Filenames

		[TestMethod]
		public void AllFilenamesSaved()
		{
			// Setup
			var model = new MainModel();	
			model.Filenames.Add(@"File1");
			model.Filenames.Add(@"\File\With\Backslashes");
			model.Filenames.Add(@"/File/With/Slashes");
			model.Filenames.Add(@"C:\Folder\Filename.xml");

			// Execute
			var document = model.Save();

			// Verify
			var nodes = document.FileNodes();
			Assert.AreEqual(4, nodes.Count);
		}

		#endregion

		#region Settings

		[TestMethod]
		public void SettingsSaved()
		{
			// Setup
			var model = new MainModel();

			// Execute
			var document = model.Save();

			// Verify
			var settingsNode = document.SettingsNode();
			Assert.AreEqual("Settings", settingsNode.Name);
		}

		[TestMethod]
		public void SettingsOutputTypeSaved()
		{
			SettingsRunner.Setup(model => model.OutputType = OutputType.Sprites).Verify("true", "sprites");
			SettingsRunner.Setup(model => model.OutputType = OutputType.Sprites).VerifyNotExists("blocks");

			SettingsRunner.Setup(model => model.OutputType = OutputType.Blocks).Verify("true", "blocks");
			SettingsRunner.Setup(model => model.OutputType = OutputType.Blocks).VerifyNotExists("sprites");
		}

		[TestMethod]
		public void SettingsIgnoreCopiesSaved()
		{
			SettingsRunner.Setup(model => model.IgnoreCopies = false).Verify("false", "Repeats");
			SettingsRunner.Setup(model => model.IgnoreCopies = true).Verify("true", "Repeats");
		}

		[TestMethod]
		public void SettingsIgnoreMirroredXSaved()
		{
			SettingsRunner.Setup(model => model.IgnoreMirroredX = false).Verify("false", "MirrorX");
			SettingsRunner.Setup(model => model.IgnoreMirroredX = true).Verify("true", "MirrorX");
		}

		[TestMethod]
		public void SettingsIgnoreMirroredYSaved()
		{
			SettingsRunner.Setup(model => model.IgnoreMirroredY = false).Verify("false", "MirrorY");
			SettingsRunner.Setup(model => model.IgnoreMirroredY = true).Verify("true", "MirrorY");
		}

		[TestMethod]
		public void SettingsIgnoreRotatedSaved()
		{
			SettingsRunner.Setup(model => model.IgnoreRotated = false).Verify("false", "Rotations");
			SettingsRunner.Setup(model => model.IgnoreRotated = true).Verify("true", "Rotations");
		}

		[TestMethod]
		public void SettingsIgnoreTransparentSaved()
		{
			SettingsRunner.Setup(model => model.IgnoreTransparentPixels = false).Verify("false", "Transparent");
			SettingsRunner.Setup(model => model.IgnoreTransparentPixels = true).Verify("true", "Transparent");
		}

		[TestMethod]
		public void SettingsCenterPositionSaved()
		{
			SettingsRunner.Setup(model => model.CenterPosition = 0).Verify("0", "center");
			SettingsRunner.Setup(model => model.CenterPosition = 42).Verify("42", "center");
		}

		[TestMethod]
		public void SettingsGridXSaved()
		{
			SettingsRunner.Setup(model => model.GridXSize = 1).Verify("1", "xSize");
			SettingsRunner.Setup(model => model.GridXSize = 123).Verify("123", "xSize");
		}

		[TestMethod]
		public void SettingsGridYSaved()
		{
			SettingsRunner.Setup(model => model.GridYSize = 5).Verify("5", "ySize");
			SettingsRunner.Setup(model => model.GridYSize = 64).Verify("64", "ySize");
		}

		[TestMethod]
		public void SettingsTransparentFirstSaved()
		{
			SettingsRunner.Setup(model => model.TransparentFirst = false).Verify("false", "Sort");
			SettingsRunner.Setup(model => model.TransparentFirst = true).Verify("true", "Sort");
		}

		[TestMethod]
		public void SettingsFourBitSaved()
		{
			SettingsRunner.Setup(model => model.FourBit = false).Verify("false", "fourBit");
			SettingsRunner.Setup(model => model.FourBit = true).Verify("true", "fourBit");
		}

		[TestMethod]
		public void SettingsReducedSaved()
		{
			SettingsRunner.Setup(model => model.Reduced = false).Verify("false", "reduce");
			SettingsRunner.Setup(model => model.Reduced = true).Verify("true", "reduce");
		}

		[TestMethod]
		public void SettingsTextFlipsSaved()
		{
			SettingsRunner.Setup(model => model.TextFlips = false).Verify("false", "textFlips");
			SettingsRunner.Setup(model => model.TextFlips = true).Verify("true", "textFlips");
		}

		[TestMethod]
		public void SettingsBinaryOutputSaved()
		{
			SettingsRunner.Setup(model => model.BinaryOutput = false).Verify("false", "binary");
			SettingsRunner.Setup(model => model.BinaryOutput = true).Verify("true", "binary");
		}

		[TestMethod]
		public void SettingsBinaryBlocksOutputSaved()
		{
			SettingsRunner.Setup(model => model.BinaryBlocksOutput = false).Verify("false", "binaryBlocks");
			SettingsRunner.Setup(model => model.BinaryBlocksOutput = true).Verify("true", "binaryBlocks");
		}

		[TestMethod]
		public void SettingsBlocksAsImageSaved()
		{
			SettingsRunner.Setup(model => model.BlocksAsImage = false).Verify("false", "blocksImage");
			SettingsRunner.Setup(model => model.BlocksAsImage = true).Verify("true", "blocksImage");
		}

		[TestMethod]
		public void SettingsTilesAsImageSaved()
		{
			SettingsRunner.Setup(model => model.TilesAsImage = false).Verify("false", "tilesImage");
			SettingsRunner.Setup(model => model.TilesAsImage = true).Verify("true", "tilesImage");
		}

		[TestMethod]
		public void SettingsTransparentBlocksSaved()
		{
			SettingsRunner.Setup(model => model.TransparentBlocks = false).Verify("false", "transBlock");
			SettingsRunner.Setup(model => model.TransparentBlocks = true).Verify("true", "transBlock");
		}

		[TestMethod]
		public void SettingsTransparentTilesSaved()
		{
			SettingsRunner.Setup(model => model.TransparentTiles = false).Verify("false", "transTile");
			SettingsRunner.Setup(model => model.TransparentTiles = true).Verify("true", "transTile");
		}

		[TestMethod]
		public void SettingsBlocksAccrossSaved()
		{
			SettingsRunner.Setup(model => model.BlocksAccross = 7).Verify("7", "across");
			SettingsRunner.Setup(model => model.BlocksAccross = 83).Verify("83", "across");
		}

		[TestMethod]
		public void SettingsAccuracySaved()
		{
			SettingsRunner.Setup(model => model.Accuracy = 93).Verify("93", "accurate");
			SettingsRunner.Setup(model => model.Accuracy = 105).Verify("105", "accurate");
		}

		[TestMethod]
		public void SettingsImageFormatSaved()
		{
			SettingsRunner.Setup(model => model.ImageFormat = ImageFormat.BMP).Verify("0", "format");
			SettingsRunner.Setup(model => model.ImageFormat = ImageFormat.PNG).Verify("1", "format");
			SettingsRunner.Setup(model => model.ImageFormat = ImageFormat.JPG).Verify("2", "format");
		}

		private class SettingsRunner
		{
			private MainModel _model;

			private SettingsRunner(MainModel model)
			{
				_model = model;
			}

			public static SettingsRunner Setup(Action<MainModel> setup)
			{
				var model = new MainModel();

				setup(model);

				return new SettingsRunner(model);
			}

			public void Verify(string expected, string attributeName)
			{
				Assert.AreEqual(expected, _model.Save().SettingsNode().Attributes[attributeName].Value);
			}

			public void VerifyNotExists(string attributeName)
			{
				Assert.IsNull(_model.Save().SettingsNode().Attributes[attributeName]);
			}
		}

		#endregion

		#region Palette

		[TestMethod]
		public void PaletteMappingSaved()
		{
			PaletteRunner.Setup(palette => palette.Type = PaletteType.Next256).Verify("Next256", "Mapping");
			PaletteRunner.Setup(palette => palette.Type = PaletteType.Next512).Verify("Next512", "Mapping");
			PaletteRunner.Setup(palette => palette.Type = PaletteType.Custom).Verify("Custom", "Mapping");
		}

		[TestMethod]
		public void PaletteTransparentIndexSaved()
		{
			PaletteRunner.Setup(palette => palette.TransparentIndex = 0).Verify("0", "Transparent");
			PaletteRunner.Setup(palette => palette.TransparentIndex = 25).Verify("25", "Transparent");
		}

		[TestMethod]
		public void PaletteUsedCountSaved()
		{
			PaletteRunner.Setup(palette => palette.UsedCount = 12).Verify("12", "Used");
			PaletteRunner.Setup(palette => palette.UsedCount = 823).Verify("823", "Used");
		}

		[TestMethod]
		public void PaletteColoursSaved()
		{
			PaletteRunner
				.Setup(palette =>
				{
					byte value = 0;
					foreach (var colour in palette.Colours)
					{
						colour.Red = value++;
						colour.Green = value++;
						colour.Blue = value++;
					}
				})
				.Verify(document =>
				{
					Assert.AreEqual(256, document.PaletteNode().ChildNodes.Count);

					byte value = 0;
					for (int i = 0; i<256; i++)
					{
						var colourNode = document.ColourNode(i);
						Assert.AreEqual(value++, int.Parse(colourNode.Attributes["Red"].Value));
						Assert.AreEqual(value++, int.Parse(colourNode.Attributes["Green"].Value));
						Assert.AreEqual(value++, int.Parse(colourNode.Attributes["Blue"].Value));
					}
				});
		}

		private class PaletteRunner
		{
			private MainModel _model;

			private PaletteRunner(MainModel model)
			{
				_model = model;
			}

			public static PaletteRunner Setup(Action<Palette> setup)
			{
				var model = new MainModel();

				setup(model.Palette);

				return new PaletteRunner(model);
			}

			public void Verify(string expected, string attributeName)
			{
				Assert.AreEqual(expected, _model.Save().PaletteNode().Attributes[attributeName].Value);
			}

			public void Verify(Action<XmlDocument> tester)
			{
				var document = _model.Save();

				tester(document);
			}
		}

		#endregion

		#region Dialogs

		[TestMethod]
		public void OutputFilesFilterIndexSaved()
		{
			// setup
			var model = new MainModel();
			model.OutputFilesFilterIndex = 12;

			// execute
			var document = model.Save();

			// verify
			Assert.AreEqual("12", document.DialogsNode().Attributes["OutputIndex"].Value);
		}

		[TestMethod]
		public void AddImagesFilterIndexSaved()
		{
			// setup
			var model = new MainModel();
			model.AddImagesFilterIndex = 51;

			// execute
			var document = model.Save();

			// verify
			Assert.AreEqual("51", document.DialogsNode().Attributes["ImageIndex"].Value);
		}

		#endregion
	}

	internal static class MainModelExtensions
	{
		internal static XmlNode ProjectNameNode(this XmlDocument document)
		{
			return document.SelectSingleNode("//Project/Name");
		}

		internal static XmlNodeList FileNodes(this XmlDocument document)
		{
			return document.SelectNodes("//Project/File");
		}

		internal static XmlNode SettingsNode(this XmlDocument document)
		{
			return document.SelectSingleNode("//Project/Settings");
		}

		internal static XmlNode PaletteNode(this XmlDocument document)
		{
			return document.SelectSingleNode("//Project/Palette");
		}

		internal static XmlNode ColourNode(this XmlDocument document, int index)
		{
			return document.SelectSingleNode($"//Project/Palette/Colour{index}");
		}

		internal static XmlNode DialogsNode(this XmlDocument document)
		{
			return document.SelectSingleNode("//Project/Dialogs");
		}
	}
}
