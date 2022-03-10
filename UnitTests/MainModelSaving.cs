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
			model.AddSource(@"File1");
			model.AddSource(@"\File\With\Backslashes");
			model.AddSource(@"/File/With/Slashes");
			model.AddSource(@"C:\Folder\Filename.xml");

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

			SettingsRunner.Setup(model => model.OutputType = OutputType.Tiles).Verify("true", "blocks");
			SettingsRunner.Setup(model => model.OutputType = OutputType.Tiles).VerifyNotExists("sprites");
		}

		[TestMethod]
		public void SettingsCommentTypeSaved()
		{
			SettingsRunner.Setup(model => model.CommentType = CommentType.Full).Verify("1", "comments");
			SettingsRunner.Setup(model => model.CommentType = CommentType.None).Verify("0", "comments");
		}

		[TestMethod]
		public void SettingsPaletteFormatSaved()
		{
			SettingsRunner.Setup(model => model.PaletteFormat = PaletteFormat.Next9Bit).Verify("1", "PaletteFormat");
			SettingsRunner.Setup(model => model.PaletteFormat = PaletteFormat.Next8Bit).Verify("0", "PaletteFormat");
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
		public void SettingsFourBitMethodSaved()
		{
			SettingsRunner.Setup(model => model.FourBitParsingMethod = FourBitParsingMethod.Manual).Verify("0", "FourBitParsing");
			SettingsRunner.Setup(model => model.FourBitParsingMethod = FourBitParsingMethod.DetectPaletteBanks).Verify("1", "FourBitParsing");
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
			// Note that grid is always constrained into multiple of default size!
			SettingsRunner.Setup(model => model.GridWidth = 1).Verify("16", "xSize");
			SettingsRunner.Setup(model => model.GridWidth = 17).Verify("32", "xSize");
		}

		[TestMethod]
		public void SettingsGridYSaved()
		{
			// Note that grid is always constrained into multiple of default size!
			SettingsRunner.Setup(model => model.GridHeight = 5).Verify("16", "ySize");
			SettingsRunner.Setup(model => model.GridHeight = 64).Verify("64", "ySize");
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
			SettingsRunner.Setup(model => model.SpritesFourBit = false).Verify("false", "fourBit");
			SettingsRunner.Setup(model => model.SpritesFourBit = true).Verify("true", "fourBit");
		}

		[TestMethod]
		public void SettingsReducedSaved()
		{
			SettingsRunner.Setup(model => model.SpritesReduced = false).Verify("false", "reduce");
			SettingsRunner.Setup(model => model.SpritesReduced = true).Verify("true", "reduce");
		}

		[TestMethod]
		public void SettingsTextFlipsSaved()
		{
			SettingsRunner.Setup(model => model.SpritesAttributesAsText = false).Verify("false", "textFlips");
			SettingsRunner.Setup(model => model.SpritesAttributesAsText = true).Verify("true", "textFlips");
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
			SettingsRunner.Setup(model => model.BinaryFramesAttributesOutput = false).Verify("false", "binaryBlocks");
			SettingsRunner.Setup(model => model.BinaryFramesAttributesOutput = true).Verify("true", "binaryBlocks");
		}

		[TestMethod]
		public void SettingsBlocksAsImageSaved()
		{
			SettingsRunner.Setup(model => model.TilesExportAsImage = false).Verify("false", "blocksImage");
			SettingsRunner.Setup(model => model.TilesExportAsImage = true).Verify("true", "blocksImage");
		}

		[TestMethod]
		public void SettingsTilesAsImageSaved()
		{
			SettingsRunner.Setup(model => model.SpritesExportAsImages = false).Verify("false", "tilesImage");
			SettingsRunner.Setup(model => model.SpritesExportAsImages = true).Verify("true", "tilesImage");
		}

		[TestMethod]
		public void SettingsTransparentBlocksSaved()
		{
			SettingsRunner.Setup(model => model.TilesExportAsImageTransparent = false).Verify("false", "transBlock");
			SettingsRunner.Setup(model => model.TilesExportAsImageTransparent = true).Verify("true", "transBlock");
		}

		[TestMethod]
		public void SettingsTransparentTilesSaved()
		{
			SettingsRunner.Setup(model => model.SpritesExportAsImageTransparent = false).Verify("false", "transTile");
			SettingsRunner.Setup(model => model.SpritesExportAsImageTransparent = true).Verify("true", "transTile");
		}

		[TestMethod]
		public void SettingsBlocksAccrossSaved()
		{
			SettingsRunner.Setup(model => model.BlocksAcross = 7).Verify("7", "across");
			SettingsRunner.Setup(model => model.BlocksAcross = 83).Verify("83", "across");
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

		[TestMethod]
		public void SettingsTilemapExportTypeSaved()
		{
			SettingsRunner.Setup(model => model.TilemapExportType = TilemapExportType.AttributesIndexAsWord).Verify("0", "TilemapExport");
			SettingsRunner.Setup(model => model.TilemapExportType = TilemapExportType.AttributesIndexAsTwoBytes).Verify("1", "TilemapExport");
			SettingsRunner.Setup(model => model.TilemapExportType = TilemapExportType.IndexOnly).Verify("2", "TilemapExport");
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

		private class ExportExtensionsRunner
		{
			private MainModel _model;

			private ExportExtensionsRunner(MainModel model)
			{
				_model = model;
			}

			public static ExportExtensionsRunner Setup(Action<MainModel> setup)
			{
				var model = new MainModel();

				setup(model);

				return new ExportExtensionsRunner(model);
			}

			public void Verify(string expected, string attributeName)
			{
				Assert.AreEqual(expected, _model.Save().ExportExtensionsNode().Attributes[attributeName].Value);
			}

			public void VerifyNotExists(string attributeName)
			{
				Assert.IsNull(_model.Save().ExportExtensionsNode().Attributes[attributeName]);
			}
		}

		#endregion

		#region Export extensions

		[TestMethod]
		public void SettingsExtensionAssemblerSaved()
		{
			ExportExtensionsRunner.Setup(model => model.ExportAssemblerFileExtension = "ex1").Verify("ex1", "Assembler");
			ExportExtensionsRunner.Setup(model => model.ExportAssemblerFileExtension = "ex2").Verify("ex2", "Assembler");
		}

		[TestMethod]
		public void SettingsExtensionPaletteSaved()
		{
			ExportExtensionsRunner.Setup(model => model.ExportBinaryPaletteFileExtension = "ex3").Verify("ex3", "Palette");
			ExportExtensionsRunner.Setup(model => model.ExportBinaryPaletteFileExtension = "ex4").Verify("ex4", "Palette");
		}

		[TestMethod]
		public void SettingsExtensionDataSaved()
		{
			ExportExtensionsRunner.Setup(model => model.ExportBinaryDataFileExtension = "ex5").Verify("ex5", "Data");
			ExportExtensionsRunner.Setup(model => model.ExportBinaryDataFileExtension = "ex6").Verify("ex6", "Data");
		}

		[TestMethod]
		public void SettingsExtensionTilesInfoSaved()
		{
			ExportExtensionsRunner.Setup(model => model.ExportBinaryTilesInfoFileExtension = "ex7").Verify("ex7", "TilesInfo");
			ExportExtensionsRunner.Setup(model => model.ExportBinaryTilesInfoFileExtension = "ex8").Verify("ex8", "TilesInfo");
		}

		[TestMethod]
		public void SettingsExtensionTileAttributesSaved()
		{
			ExportExtensionsRunner.Setup(model => model.ExportBinaryTileAttributesFileExtension = "ex9").Verify("ex9", "TileAttributes");
			ExportExtensionsRunner.Setup(model => model.ExportBinaryTileAttributesFileExtension = "ex10").Verify("ex10", "TileAttributes");
		}

		[TestMethod]
		public void SettingsExtensionTilemapSaved()
		{
			ExportExtensionsRunner.Setup(model => model.ExportBinaryTilemapFileExtension = "ex11").Verify("ex11", "Tilemap");
			ExportExtensionsRunner.Setup(model => model.ExportBinaryTilemapFileExtension = "ex12").Verify("ex12", "Tilemap");
		}

		[TestMethod]
		public void SettingsExtensionSpriteAttributesSaved()
		{
			ExportExtensionsRunner.Setup(model => model.ExportSpriteAttributesFileExtension = "ex13").Verify("ex13", "SpriteAttributes");
			ExportExtensionsRunner.Setup(model => model.ExportSpriteAttributesFileExtension = "ex14").Verify("ex14", "SpriteAttributes");
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

		[TestMethod]
		public void AddTilemapsFilterIndexSaved()
		{
			// setup
			var model = new MainModel();
			model.AddTilemapsFilterIndex = 42;

			// execute
			var document = model.Save();

			// verify
			Assert.AreEqual("42", document.DialogsNode().Attributes["TilemapIndex"].Value);
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

		internal static XmlNode ExportExtensionsNode(this XmlDocument document)
		{
			return document.SelectSingleNode("//Project/ExportExtensions");
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
