using Microsoft.VisualStudio.TestTools.UnitTesting;

using NextGraphics.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using UnitTests.Data;

namespace UnitTests
{
	[TestClass]
	public class MainModelLoading
	{
		#region Common

		[TestMethod]
		public void ProjectNameLoaded()
		{
			// setup
			var model = new MainModel();

			// execute
			model.Load(TestDocument());

			// verify
			Assert.AreEqual("Level1", model.Name);
		}

		[TestMethod]
		public void FilesLoaded()
		{
			// setup
			var model = new MainModel();

			// execute
			model.Load(TestDocument());

			// verify
			Assert.AreEqual(3, model.Sources.Count);
			Assert.AreEqual(@"C:\tiles.bmp", model.Sources[0].Filename);
			Assert.AreEqual(@"/a/file/with/slashes", model.Sources[1].Filename);
			Assert.AreEqual(@"\the\file\with\backslashes", model.Sources[2].Filename);
		}

		#endregion

		#region Dialogs

		[TestMethod]
		public void DialogsOutputIndexLoaded()
		{
			// setup
			var model = new MainModel();

			// execute
			model.Load(TestDocument());

			// verify
			Assert.AreEqual(1, model.OutputFilesFilterIndex);
		}

		[TestMethod]
		public void DialogsImageIndexLoaded()
		{
			// setup
			var model = new MainModel();

			// execute
			model.Load(TestDocument());

			// verify
			Assert.AreEqual(2, model.AddImagesFilterIndex);
		}

		[TestMethod]
		public void DialogsTilemapIndexLoaded()
		{
			// setup
			var model = new MainModel();

			// execute
			model.Load(TestDocument());

			// verify
			Assert.AreEqual(3, model.AddTilemapsFilterIndex);
		}

		#endregion

		#region Settings

		[TestMethod]
		public void SettingsOutputTypeLoaded()
		{
			// setup
			var model = new MainModel();

			// execute & verify
			model.Load(TestDocument("blocks"));
			Assert.AreEqual(OutputType.Tiles, model.OutputType);

			// execute & verify
			model.Load(TestDocument("sprites"));
			Assert.AreEqual(OutputType.Sprites, model.OutputType);
		}

		[TestMethod]
		public void SettingsCommentTypeLoaded()
		{
			// setup
			var model = new MainModel();

			// execute & verify (note we only test for none variant since full is the default - would be nice to add both in the future)
			model.Load(TestDocument());
			Assert.AreEqual(CommentType.None, model.CommentType);
		}

		[TestMethod]
		public void SettingsIgnoreCopiesLoaded()
		{
			// setup
			var model = new MainModel();

			// execute
			model.Load(TestDocument());

			// verify
			Assert.AreEqual(false, model.IgnoreCopies);
		}

		[TestMethod]
		public void SettingsIgnoreMirroredXYLoaded()
		{
			// setup
			var model = new MainModel();

			// execute
			model.Load(TestDocument());

			// verify
			Assert.AreEqual(false, model.IgnoreMirroredX);
			Assert.AreEqual(true, model.IgnoreMirroredY);
		}

		[TestMethod]
		public void SettingsIgnoreRotatedLoaded()
		{
			// setup
			var model = new MainModel();

			// execute
			model.Load(TestDocument());

			// verify
			Assert.AreEqual(false, model.IgnoreRotated);
		}

		[TestMethod]
		public void SettingsIgnoreTransparentLoaded()
		{
			// setup
			var model = new MainModel();

			// execute
			model.Load(TestDocument());

			// verify
			Assert.AreEqual(false, model.IgnoreTransparentPixels);
		}

		[TestMethod]
		public void SettingsCenterPositionLoaded()
		{
			// setup
			var model = new MainModel();

			// execute
			model.Load(TestDocument());

			// verify
			Assert.AreEqual(4, model.CenterPosition);
		}

		[TestMethod]
		public void SettingsGridXYLoaded()
		{
			// setup
			var model = new MainModel();

			// execute
			model.Load(TestDocument());

			// verify
			Assert.AreEqual(9, model.GridWidth);
			Assert.AreEqual(10, model.GridHeight);
		}

		[TestMethod]
		public void SettingsTransparentFirstLoaded()
		{
			// setup
			var model = new MainModel();

			// execute
			model.Load(TestDocument());

			// verify
			Assert.AreEqual(true, model.TransparentFirst);
		}

		[TestMethod]
		public void SettingsFourBitLoaded()
		{
			// setup
			var model = new MainModel();

			// execute
			model.Load(TestDocument());

			// verify
			Assert.AreEqual(true, model.FourBit);
		}

		[TestMethod]
		public void SettingsReducedLoaded()
		{
			// setup
			var model = new MainModel();

			// execute
			model.Load(TestDocument());

			// verify
			Assert.AreEqual(true, model.Reduced);
		}

		[TestMethod]
		public void SettingsTextFlipsLoaded()
		{
			// setup
			var model = new MainModel();

			// execute
			model.Load(TestDocument());

			// verify
			Assert.AreEqual(true, model.AttributesAsText);
		}

		[TestMethod]
		public void SettingsBinaryOutputLoaded()
		{
			// setup
			var model = new MainModel();

			// execute
			model.Load(TestDocument());

			// verify
			Assert.AreEqual(false, model.BinaryOutput);
		}

		[TestMethod]
		public void SettingsBinaryBlocksOutputLoaded()
		{
			// setup
			var model = new MainModel();

			// execute
			model.Load(TestDocument());

			// verify
			Assert.AreEqual(true, model.BinaryBlocksOutput);
		}

		[TestMethod]
		public void SettingsBlocksTilesLoaded()
		{
			// setup
			var model = new MainModel();

			// execute
			model.Load(TestDocument());

			// verify
			Assert.AreEqual(false, model.BlocksAsImage);
			Assert.AreEqual(true, model.TilesAsImage);
		}

		[TestMethod]
		public void SettingsTransparentBlocksTilesLoaded()
		{
			// setup
			var model = new MainModel();

			// execute
			model.Load(TestDocument());

			// verify
			Assert.AreEqual(true, model.TransparentBlocks);
			Assert.AreEqual(false, model.TransparentTiles);
		}

		[TestMethod]
		public void SettingsBlocksAccrossLoaded()
		{
			// setup
			var model = new MainModel();

			// execute
			model.Load(TestDocument());

			// verify
			Assert.AreEqual(5, model.BlocsAcross);
		}

		[TestMethod]
		public void SettingsAccuracyLoaded()
		{
			// setup
			var model = new MainModel();

			// execute
			model.Load(TestDocument());

			// verify
			Assert.AreEqual(983, model.Accuracy);
		}

		[TestMethod]
		public void SettingsImageFormatLoaded()
		{
			// setup
			var model = new MainModel();

			// execute & verify
			model.Load(TestDocument(imageFormat: "0"));
			Assert.AreEqual(ImageFormat.BMP, model.ImageFormat);

			// execute & verify
			model.Load(TestDocument(imageFormat: "1"));
			Assert.AreEqual(ImageFormat.PNG, model.ImageFormat);

			// execute & verify
			model.Load(TestDocument(imageFormat: "2"));
			Assert.AreEqual(ImageFormat.JPG, model.ImageFormat);
		}

		[TestMethod]
		public void SettingsTilemapExportTypeLoaded()
		{
			// setup
			var model = new MainModel();

			// execute & verify
			model.Load(TestDocument());
			Assert.AreEqual(TilemapExportType.AttributesIndexAsTwoBytes, model.TilemapExportType);
		}

		[TestMethod]
		public void SettingsPaletteFormatLoaded()
		{
			// setup
			var model = new MainModel();

			// execute & verify
			model.Load(TestDocument(paletteFormat: "0"));
			Assert.AreEqual(PaletteFormat.Next8Bit, model.PaletteFormat);

			// execute & verify
			model.Load(TestDocument(paletteFormat: "1"));
			Assert.AreEqual(PaletteFormat.Next9Bit, model.PaletteFormat);
		}

		#endregion

		#region Palette

		[TestMethod]
		public void SettingsPaletteMappingLoaded()
		{
			// setup
			var model = new MainModel();

			// execute & verify
			model.Load(TestDocument(paletteMapping: "Custom"));
			Assert.AreEqual(PaletteType.Custom, model.Palette.Type);

			// execute & verify
			model.Load(TestDocument(paletteMapping: "Next256"));
			Assert.AreEqual(PaletteType.Next256, model.Palette.Type);

			// execute & verify
			model.Load(TestDocument(paletteMapping: "Next512"));
			Assert.AreEqual(PaletteType.Next512, model.Palette.Type);
		}

		[TestMethod]
		public void SettingsPaletteTransparentIndexLoaded()
		{
			// setup
			var model = new MainModel();

			// execute
			model.Load(TestDocument());

			// verify
			Assert.AreEqual(0, model.Palette.TransparentIndex);
		}

		[TestMethod]
		public void SettingsPaletteUsedCountLoaded()
		{
			// setup
			var model = new MainModel();

			// execute
			model.Load(TestDocument());

			// verify
			Assert.AreEqual(6, model.Palette.UsedCount);
		}

		[TestMethod]
		public void SettingsPaletteColoursLoaded()
		{
			// setup
			var model = new MainModel();

			// execute
			model.Load(TestDocument());

			// verify (note we just sample couple colours here)
			Assert.AreEqual(256, model.Palette.Colours.Count);

			Assert.AreEqual(10, model.Palette[0].Red);
			Assert.AreEqual(11, model.Palette[0].Green);
			Assert.AreEqual(12, model.Palette[0].Blue);

			Assert.AreEqual(1, model.Palette[48].Red);
			Assert.AreEqual(2, model.Palette[48].Green);
			Assert.AreEqual(3, model.Palette[48].Blue);

			Assert.AreEqual(5, model.Palette[255].Red);
			Assert.AreEqual(6, model.Palette[255].Green);
			Assert.AreEqual(7, model.Palette[255].Blue);
		}

		#endregion

		#region Creation

		private XmlDocument TestDocument(
			string outputType = "blocks", 
			string imageFormat = "0", 
			string paletteMapping="Custom",
			string paletteFormat="0")
		{
			return DataCreator.XmlDocumentTilesTemplated(
				outputType, 
				imageFormat, 
				paletteMapping,
				paletteFormat);
		}

		#endregion
	}
}
