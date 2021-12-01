﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

using NextGraphics.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
			Assert.AreEqual(3, model.Filenames.Count);
			Assert.AreEqual(@"C:\tiles.bmp", model.Filenames[0]);
			Assert.AreEqual(@"/a/file/with/slashes", model.Filenames[1]);
			Assert.AreEqual(@"\the\file\with\backslashes", model.Filenames[2]);
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
			Assert.AreEqual(OutputType.Blocks, model.OutputType);

			// execute & verify
			model.Load(TestDocument("sprites"));
			Assert.AreEqual(OutputType.Sprites, model.OutputType);
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
			Assert.AreEqual(9, model.GridXSize);
			Assert.AreEqual(10, model.GridYSize);
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
			Assert.AreEqual(5, model.BlocksAccross);
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

		private XmlDocument TestDocument(string outputType = "blocks", string imageFormat = "0", string paletteMapping="Custom")
		{
			var template = @"
<XML>
  <Project>
	<Name Projectname=""Level1"" />
	<File Path=""C:\tiles.bmp"" />
	<File Path=""/a/file/with/slashes"" />
	<File Path=""\the\file\with\backslashes"" />
	<Settings {0}=""true"" center=""4"" xSize=""9"" ySize=""10"" fourBit=""true"" binary=""false"" binaryBlocks=""true"" Repeats=""false"" MirrorX=""false"" MirrorY=""true"" Rotations=""false"" Transparent=""false"" Sort=""true"" blocksImage=""false"" tilesImage=""true"" transBlock=""true"" transTile=""false"" across=""5"" accurate=""983"" format=""{1}"" />
	<Dialogs OutputIndex=""1"" ImageIndex=""1"" />
	<Palette Mapping=""{2}"" Transparent=""0"" Used=""6"">
	  <Colour0 Red=""10"" Green=""11"" Blue=""12"" />
	  <Colour1 Red=""255"" Green=""255"" Blue=""255"" />
	  <Colour2 Red=""218"" Green=""218"" Blue=""218"" />
	  <Colour3 Red=""182"" Green=""182"" Blue=""182"" />
	  <Colour4 Red=""145"" Green=""145"" Blue=""145"" />
	  <Colour5 Red=""109"" Green=""109"" Blue=""109"" />
	  <Colour6 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour7 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour8 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour9 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour10 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour11 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour12 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour13 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour14 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour15 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour16 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour17 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour18 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour19 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour20 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour21 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour22 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour23 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour24 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour25 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour26 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour27 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour28 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour29 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour30 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour31 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour32 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour33 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour34 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour35 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour36 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour37 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour38 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour39 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour40 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour41 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour42 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour43 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour44 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour45 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour46 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour47 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour48 Red=""1"" Green=""2"" Blue=""3"" />
	  <Colour49 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour50 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour51 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour52 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour53 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour54 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour55 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour56 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour57 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour58 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour59 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour60 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour61 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour62 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour63 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour64 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour65 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour66 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour67 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour68 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour69 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour70 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour71 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour72 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour73 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour74 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour75 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour76 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour77 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour78 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour79 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour80 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour81 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour82 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour83 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour84 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour85 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour86 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour87 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour88 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour89 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour90 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour91 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour92 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour93 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour94 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour95 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour96 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour97 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour98 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour99 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour100 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour101 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour102 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour103 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour104 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour105 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour106 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour107 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour108 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour109 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour110 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour111 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour112 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour113 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour114 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour115 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour116 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour117 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour118 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour119 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour120 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour121 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour122 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour123 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour124 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour125 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour126 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour127 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour128 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour129 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour130 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour131 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour132 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour133 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour134 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour135 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour136 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour137 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour138 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour139 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour140 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour141 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour142 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour143 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour144 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour145 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour146 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour147 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour148 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour149 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour150 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour151 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour152 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour153 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour154 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour155 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour156 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour157 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour158 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour159 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour160 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour161 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour162 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour163 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour164 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour165 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour166 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour167 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour168 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour169 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour170 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour171 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour172 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour173 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour174 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour175 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour176 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour177 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour178 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour179 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour180 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour181 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour182 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour183 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour184 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour185 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour186 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour187 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour188 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour189 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour190 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour191 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour192 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour193 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour194 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour195 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour196 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour197 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour198 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour199 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour200 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour201 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour202 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour203 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour204 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour205 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour206 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour207 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour208 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour209 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour210 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour211 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour212 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour213 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour214 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour215 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour216 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour217 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour218 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour219 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour220 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour221 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour222 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour223 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour224 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour225 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour226 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour227 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour228 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour229 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour230 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour231 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour232 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour233 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour234 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour235 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour236 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour237 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour238 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour239 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour240 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour241 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour242 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour243 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour244 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour245 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour246 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour247 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour248 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour249 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour250 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour251 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour252 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour253 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour254 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour255 Red=""5"" Green=""6"" Blue=""7"" />
	</Palette>
  </Project>
</XML>
";

			var xml = String.Format(template, outputType, imageFormat, paletteMapping);

			var result = new XmlDocument();
			result.LoadXml(xml);
			return result;
		}

		#endregion
	}
}
