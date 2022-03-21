﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

using NextGraphics.Models;
using NextGraphics.Exporting;

using System;
using System.IO;
using System.Linq;
using System.Xml;

using UnitTests.Data;
using NextGraphics.Exporting.Common;
using System.Drawing;
using System.Text.RegularExpressions;
using UnitTests.ExporterTestingExtensions;

namespace UnitTests
{
	/*
	 * Note: this test class was created for purposes of extracting exporting code out of main.cs so it doesn't necessarily cover all future exporting capabilities (would be nice to do so though).
	 * 
	 * Test data was generated with original codebase (prior to extracting export from main.cs). Each exported file was added to unit test project as resource and tested against with data generated by exporter class. Assembler output file header was additionally tweaked for each resource so that the date code can be tested - files in resources use `{0}` template which is replaced with the date used on test run. Other files are used exactly as generated by the original codebase.
	 */

	[TestClass]
	public class ExporterTesting
	{
		#region Sprites

		#region Assembler

		[DataTestMethod]
		[DataRow(CommentType.Full, PaletteFormat.Next8Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.Full, PaletteFormat.Next8Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.Full, PaletteFormat.Next9Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.Full, PaletteFormat.Next9Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.None, PaletteFormat.Next8Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.None, PaletteFormat.Next8Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.None, PaletteFormat.Next9Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.None, PaletteFormat.Next9Bit, PaletteParsingMethod.ByObjects)]
		public void Sprites_Assembler(CommentType commentType, PaletteFormat paletteFormat, PaletteParsingMethod parsingMethod)
		{
			TestSprites((model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Sprites;
				model.CommentType = commentType;
				model.PaletteFormat = paletteFormat;
				model.PaletteParsingMethod = parsingMethod;
				model.BinaryOutput = false;
				model.BinaryFramesAttributesOutput = false;
				model.SpritesFourBit = false;
				model.SpritesExportAsImages = false;

				// execute
				exporter.MapPaletteFromFirstImage();
				exporter.Remap();
				exporter.Export();

				// verify
				VerifyBinaryIsEmpty(parameters.PaletteStream, "palette");
				VerifyBinaryIsEmpty(parameters.BinaryStream, "binary data");
				VerifyBinaryIsEmpty(parameters.TileAttributesStream, "tile attributes");
				VerifyBinaryIsEmpty(parameters.TilesInfoStream, "tiles info");
				VerifyBinaryIsEmpty(parameters.TilesImageStream, "tiles image");
				VerifyBinaryIsEmpty(parameters.SpriteAttributesStream, "sprite attributes");
				VerifyBinaryIsEmpty(parameters.SpritesImageStream, "sprites image");
				VerifyAssembler(parameters, DataCreator.AssemblerSprites(parameters.Time, commentType, paletteFormat, parsingMethod));
			});
		}

		[DataTestMethod]
		[DataRow(CommentType.Full, PaletteFormat.Next8Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.Full, PaletteFormat.Next8Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.Full, PaletteFormat.Next9Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.Full, PaletteFormat.Next9Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.None, PaletteFormat.Next8Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.None, PaletteFormat.Next8Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.None, PaletteFormat.Next9Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.None, PaletteFormat.Next9Bit, PaletteParsingMethod.ByObjects)]
		public void Sprites_Assembler_SpritesAsImages(CommentType commentType, PaletteFormat paletteFormat, PaletteParsingMethod parsingMethod)
		{
			TestSprites((model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Sprites;
				model.CommentType = commentType;
				model.PaletteFormat = paletteFormat;
				model.PaletteParsingMethod = parsingMethod;
				model.BinaryOutput = false;
				model.BinaryFramesAttributesOutput = false;
				model.SpritesFourBit = false;
				model.SpritesExportAsImages = true;

				// execute
				exporter.MapPaletteFromFirstImage();
				exporter.Remap();
				exporter.Export();

				// verify
				VerifyBinaryIsEmpty(parameters.PaletteStream, "palette");
				VerifyBinaryIsEmpty(parameters.BinaryStream, "binary data");
				VerifyBinaryIsEmpty(parameters.TileAttributesStream, "tile attributes");
				VerifyBinaryIsEmpty(parameters.TilesInfoStream, "tiles info");
				VerifyBinaryIsEmpty(parameters.TilesImageStream, "tiles image");
				VerifyBinaryIsEmpty(parameters.SpriteAttributesStream, "sprite attributes");
				VerifyBinary(parameters.SpritesImageStream, DataCreator.SpritesImage(), "sprites image");
				VerifyBinaryArray(10, (i) => DataCreator.SpritesSpriteImage(i), parameters.SpriteImageStream, "block image");
				VerifyAssembler(parameters, DataCreator.AssemblerSprites(parameters.Time, commentType, paletteFormat, parsingMethod));
			});
		}

		#endregion

		#region Assembler+Binary

		[DataTestMethod]
		[DataRow(CommentType.Full, PaletteFormat.Next8Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.Full, PaletteFormat.Next8Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.Full, PaletteFormat.Next9Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.Full, PaletteFormat.Next9Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.None, PaletteFormat.Next8Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.None, PaletteFormat.Next8Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.None, PaletteFormat.Next9Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.None, PaletteFormat.Next9Bit, PaletteParsingMethod.ByObjects)]
		public void Sprites_Assembler_Binary(CommentType commentType, PaletteFormat paletteFormat, PaletteParsingMethod parsingMethod)
		{
			TestSprites((model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Sprites;
				model.CommentType = commentType;
				model.PaletteFormat = paletteFormat;
				model.PaletteParsingMethod = parsingMethod;
				model.BinaryOutput = true;
				model.BinaryFramesAttributesOutput = false;
				model.SpritesFourBit = false;
				model.SpritesExportAsImages = false;

				// execute
				exporter.MapPaletteFromFirstImage();
				exporter.Remap();
				exporter.Export();

				// verify
				VerifyBinary(parameters.PaletteStream, DataCreator.SpritesPalette(paletteFormat, parsingMethod), "palette");
				VerifyBinary(parameters.BinaryStream, DataCreator.SpritesBinary(parsingMethod), "binary data");
				VerifyBinaryIsEmpty(parameters.TileAttributesStream, "tile attributes");
				VerifyBinaryIsEmpty(parameters.TilesInfoStream, "tiles info");
				VerifyBinaryIsEmpty(parameters.TilesImageStream, "tiles image");
				VerifyBinaryIsEmpty(parameters.SpriteAttributesStream, "sprite attributes");
				VerifyBinaryIsEmpty(parameters.SpritesImageStream, "sprites image");
				VerifyAssembler(parameters, DataCreator.AssemblerSpritesBinary(parameters.Time, commentType));
			});
		}

		[DataTestMethod]
		[DataRow(CommentType.Full, PaletteFormat.Next8Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.Full, PaletteFormat.Next8Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.Full, PaletteFormat.Next9Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.Full, PaletteFormat.Next9Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.None, PaletteFormat.Next8Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.None, PaletteFormat.Next8Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.None, PaletteFormat.Next9Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.None, PaletteFormat.Next9Bit, PaletteParsingMethod.ByObjects)]
		public void Sprites_Assembler_Binary_SpritesAsImages(CommentType commentType, PaletteFormat paletteFormat, PaletteParsingMethod parsingMethod)
		{
			TestSprites((model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Sprites;
				model.CommentType = commentType;
				model.PaletteFormat = paletteFormat;
				model.PaletteParsingMethod = parsingMethod;
				model.BinaryOutput = true;
				model.BinaryFramesAttributesOutput = false;
				model.SpritesFourBit = false;
				model.SpritesExportAsImages = true;

				// execute
				exporter.MapPaletteFromFirstImage();
				exporter.Remap();
				exporter.Export();

				// verify
				VerifyBinary(parameters.PaletteStream, DataCreator.SpritesPalette(paletteFormat, parsingMethod), "palette");
				VerifyBinary(parameters.BinaryStream, DataCreator.SpritesBinary(parsingMethod), "binary data");
				VerifyBinaryIsEmpty(parameters.TileAttributesStream, "tile attributes");
				VerifyBinaryIsEmpty(parameters.TilesInfoStream, "tiles info");
				VerifyBinaryIsEmpty(parameters.TilesImageStream, "tiles image");
				VerifyBinaryIsEmpty(parameters.SpriteAttributesStream, "sprite attributes");
				VerifyBinary(parameters.SpritesImageStream, DataCreator.SpritesImage(), "sprites image");
				VerifyBinaryArray(10, (i) => DataCreator.SpritesSpriteImage(i), parameters.SpriteImageStream, "block image");
				VerifyAssembler(parameters, DataCreator.AssemblerSpritesBinary(parameters.Time, commentType));
			});
		}

		#endregion

		#region Assembler+Binary+Attributes

		[DataTestMethod]
		[DataRow(CommentType.Full, PaletteFormat.Next8Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.Full, PaletteFormat.Next8Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.Full, PaletteFormat.Next9Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.Full, PaletteFormat.Next9Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.None, PaletteFormat.Next8Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.None, PaletteFormat.Next8Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.None, PaletteFormat.Next9Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.None, PaletteFormat.Next9Bit, PaletteParsingMethod.ByObjects)]
		public void Sprites_Assembler_BinaryAttributes(CommentType commentType, PaletteFormat paletteFormat, PaletteParsingMethod parsingMethod)
		{
			TestSprites((model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Sprites;
				model.CommentType = commentType;
				model.PaletteFormat = paletteFormat;
				model.PaletteParsingMethod = parsingMethod;
				model.BinaryOutput = true;
				model.BinaryFramesAttributesOutput = true;
				model.SpritesFourBit = false;
				model.SpritesExportAsImages = false;

				// execute
				exporter.MapPaletteFromFirstImage();
				exporter.Remap();
				exporter.Export();

				// verify
				VerifyBinary(parameters.PaletteStream, DataCreator.SpritesPalette(paletteFormat, parsingMethod), "palette");
				VerifyBinary(parameters.BinaryStream, DataCreator.SpritesBinary(parsingMethod), "binary data");
				VerifyBinaryIsEmpty(parameters.TileAttributesStream, "tile attributes");
				VerifyBinaryIsEmpty(parameters.TilesInfoStream, "tiles info");
				VerifyBinaryIsEmpty(parameters.TilesImageStream, "tiles image");
				VerifyBinary(parameters.SpriteAttributesStream, DataCreator.SpritesAttributes(), "sprite attributes");
				VerifyBinaryIsEmpty(parameters.SpritesImageStream, "sprites image");
				VerifyAssembler(parameters, DataCreator.AssemblerSpritesBinaryAttributes(parameters.Time, commentType));
			});
		}

		[DataTestMethod]
		[DataRow(CommentType.Full, PaletteFormat.Next8Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.Full, PaletteFormat.Next8Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.Full, PaletteFormat.Next9Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.Full, PaletteFormat.Next9Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.None, PaletteFormat.Next8Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.None, PaletteFormat.Next8Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.None, PaletteFormat.Next9Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.None, PaletteFormat.Next9Bit, PaletteParsingMethod.ByObjects)]
		public void Sprites_Assembler_BinaryAttributes_SpritesAsImages(CommentType commentType, PaletteFormat paletteFormat, PaletteParsingMethod parsingMethod)
		{
			TestSprites((model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Sprites;
				model.CommentType = commentType;
				model.PaletteFormat = paletteFormat;
				model.PaletteParsingMethod = parsingMethod;
				model.BinaryOutput = true;
				model.BinaryFramesAttributesOutput = true;
				model.SpritesFourBit = false;
				model.SpritesExportAsImages = true;

				// execute
				exporter.MapPaletteFromFirstImage();
				exporter.Remap();
				exporter.Export();

				// verify
				VerifyBinary(parameters.PaletteStream, DataCreator.SpritesPalette(paletteFormat, parsingMethod), "palette");
				VerifyBinary(parameters.BinaryStream, DataCreator.SpritesBinary(parsingMethod), "binary data");
				VerifyBinaryIsEmpty(parameters.TileAttributesStream, "tile attributes");
				VerifyBinaryIsEmpty(parameters.TilesInfoStream, "tiles info");
				VerifyBinaryIsEmpty(parameters.TilesImageStream, "tiles image");
				VerifyBinary(parameters.SpriteAttributesStream, DataCreator.SpritesAttributes(), "sprite attributes");
				VerifyBinary(parameters.SpritesImageStream, DataCreator.SpritesImage(), "sprites image");
				VerifyBinaryArray(10, (i) => DataCreator.SpritesSpriteImage(i), parameters.SpriteImageStream, "block image");
				VerifyAssembler(parameters, DataCreator.AssemblerSpritesBinaryAttributes(parameters.Time, commentType));
			});
		}

		#endregion

		#region 4-bit

		// note: we don't repeat the whole set of tests here, just run couple variants to ensure the most common scenarios of 4-bit parsing are correctly handled
		// note: expected exports were created with full comments option

		[DataTestMethod]
		[DataRow(PaletteFormat.Next8Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(PaletteFormat.Next8Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(PaletteFormat.Next9Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(PaletteFormat.Next9Bit, PaletteParsingMethod.ByObjects)]
		public void Sprites_Assembler_4bit(PaletteFormat paletteFormat, PaletteParsingMethod parsingMethod)
		{
			TestSprites((model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Sprites;
				model.CommentType = CommentType.Full;
				model.PaletteFormat = paletteFormat;
				model.PaletteParsingMethod = parsingMethod;
				model.BinaryOutput = false;
				model.BinaryFramesAttributesOutput = false;
				model.SpritesFourBit = true;
				model.SpritesExportAsImages = false;

				// execute
				exporter.MapPaletteFromFirstImage();
				exporter.Remap();
				exporter.Export();

				// verify
				VerifyBinaryIsEmpty(parameters.PaletteStream, "palette");
				VerifyBinaryIsEmpty(parameters.BinaryStream, "binary data");
				VerifyBinaryIsEmpty(parameters.TileAttributesStream, "tile attributes");
				VerifyBinaryIsEmpty(parameters.TilesInfoStream, "tiles info");
				VerifyBinaryIsEmpty(parameters.TilesImageStream, "tiles image");
				VerifyBinaryIsEmpty(parameters.SpriteAttributesStream, "sprite attributes");
				VerifyBinaryIsEmpty(parameters.SpritesImageStream, "sprites image");
				VerifyAssembler(parameters, DataCreator.AssemblerSprites4Bit(parameters.Time, paletteFormat, parsingMethod));
			});
		}

		#endregion

		#endregion

		#region Tiles

		#region Assembler

		[DataTestMethod]
		[DataRow(CommentType.Full, PaletteFormat.Next8Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.Full, PaletteFormat.Next8Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.Full, PaletteFormat.Next9Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.Full, PaletteFormat.Next9Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.None, PaletteFormat.Next8Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.None, PaletteFormat.Next8Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.None, PaletteFormat.Next9Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.None, PaletteFormat.Next9Bit, PaletteParsingMethod.ByObjects)]
		public void Tiles_Assembler(CommentType commentType, PaletteFormat paletteFormat, PaletteParsingMethod parsingMethod)
		{
			TestTiles((model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Tiles;
				model.CommentType = commentType;
				model.PaletteFormat = paletteFormat;
				model.PaletteParsingMethod = parsingMethod;
				model.BinaryOutput = false;
				model.BinaryFramesAttributesOutput = false;
				model.TilesExportAsImage = false;

				// execute
				exporter.MapPaletteFromFirstImage();
				exporter.Remap();
				exporter.Export();

				// verify
				VerifyBinaryIsEmpty(parameters.PaletteStream, "palette");
				VerifyBinaryIsEmpty(parameters.BinaryStream, "binary data");
				VerifyBinaryIsEmpty(parameters.TileAttributesStream, "tile attributes");
				VerifyBinaryIsEmpty(parameters.TilesInfoStream, "tiles info");
				VerifyBinaryIsEmpty(parameters.TilesImageStream, "tiles image");
				VerifyBinaryIsEmpty(parameters.SpriteAttributesStream, "sprite attributes");
				VerifyBinaryIsEmpty(parameters.SpritesImageStream, "sprites image");
				VerifyAssembler(parameters, DataCreator.AssemblerTiles(parameters.Time, commentType, paletteFormat, parsingMethod));
			});
		}

		[DataTestMethod]
		[DataRow(CommentType.Full, PaletteFormat.Next8Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.Full, PaletteFormat.Next8Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.Full, PaletteFormat.Next9Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.Full, PaletteFormat.Next9Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.None, PaletteFormat.Next8Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.None, PaletteFormat.Next8Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.None, PaletteFormat.Next9Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.None, PaletteFormat.Next9Bit, PaletteParsingMethod.ByObjects)]
		public void Tiles_Assembler_TilesAsImage(CommentType commentType, PaletteFormat paletteFormat, PaletteParsingMethod parsingMethod)
		{
			TestTiles((model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Tiles;
				model.CommentType = commentType;
				model.PaletteFormat = paletteFormat;
				model.PaletteParsingMethod = parsingMethod;
				model.BinaryOutput = false;
				model.BinaryFramesAttributesOutput = false;
				model.TilesExportAsImage = true;

				// execute
				exporter.MapPaletteFromFirstImage();
				exporter.Remap();
				exporter.Export();

				// verify
				VerifyBinaryIsEmpty(parameters.PaletteStream, "palette");
				VerifyBinaryIsEmpty(parameters.BinaryStream, "binary data");
				VerifyBinaryIsEmpty(parameters.TileAttributesStream, "tile attributes");
				VerifyBinary(parameters.TilesInfoStream, DataCreator.TilesInfo(parsingMethod), "tiles info");
				VerifyBinary(parameters.TilesImageStream, DataCreator.TilesImage(parsingMethod), "tiles image");
				VerifyBinaryIsEmpty(parameters.SpriteAttributesStream, "sprite attributes");
				VerifyBinaryIsEmpty(parameters.SpritesImageStream, "sprites image");
				VerifyAssembler(parameters, DataCreator.AssemblerTiles(parameters.Time, commentType, paletteFormat, parsingMethod));
			});
		}

		#endregion

		#region Assembler+Binary

		[DataTestMethod]
		[DataRow(CommentType.Full, PaletteFormat.Next8Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.Full, PaletteFormat.Next8Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.Full, PaletteFormat.Next9Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.Full, PaletteFormat.Next9Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.None, PaletteFormat.Next8Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.None, PaletteFormat.Next8Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.None, PaletteFormat.Next9Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.None, PaletteFormat.Next9Bit, PaletteParsingMethod.ByObjects)]
		public void Tiles_Assembler_Binary(CommentType commentType, PaletteFormat paletteFormat, PaletteParsingMethod parsingMethod)
		{
			TestTiles((model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Tiles;
				model.CommentType = commentType;
				model.PaletteFormat = paletteFormat;
				model.PaletteParsingMethod = parsingMethod;
				model.BinaryOutput = true;
				model.BinaryFramesAttributesOutput = false;
				model.TilesExportAsImage = false;

				// execute
				exporter.MapPaletteFromFirstImage();
				exporter.Remap();
				exporter.Export();

				// verify
				VerifyBinary(parameters.PaletteStream, DataCreator.TilesPalette(paletteFormat, parsingMethod), "palette");
				VerifyBinary(parameters.BinaryStream, DataCreator.TilesBinary(parsingMethod), "binary data");
				VerifyBinaryIsEmpty(parameters.TileAttributesStream, "tile attributes");
				VerifyBinaryIsEmpty(parameters.TilesInfoStream, "tiles info");
				VerifyBinaryIsEmpty(parameters.TilesImageStream, "tiles image");
				VerifyBinaryIsEmpty(parameters.SpriteAttributesStream, "sprite attributes");
				VerifyBinaryIsEmpty(parameters.SpritesImageStream, "sprites image");
				VerifyAssembler(parameters, DataCreator.AssemblerTilesBinary(parameters.Time, commentType, parsingMethod));
			});
		}

		[DataTestMethod]
		[DataRow(CommentType.Full, PaletteFormat.Next8Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.Full, PaletteFormat.Next8Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.Full, PaletteFormat.Next9Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.Full, PaletteFormat.Next9Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.None, PaletteFormat.Next8Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.None, PaletteFormat.Next8Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.None, PaletteFormat.Next9Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.None, PaletteFormat.Next9Bit, PaletteParsingMethod.ByObjects)]
		public void Tiles_Assembler_Binary_TilesAsImage(CommentType commentType, PaletteFormat paletteFormat, PaletteParsingMethod parsingMethod)
		{
			TestTiles((model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Tiles;
				model.CommentType = commentType;
				model.PaletteFormat = paletteFormat;
				model.PaletteParsingMethod = parsingMethod;
				model.BinaryOutput = true;
				model.BinaryFramesAttributesOutput = false;
				model.TilesExportAsImage = true;

				// execute
				exporter.MapPaletteFromFirstImage();
				exporter.Remap();
				exporter.Export();

				// verify
				VerifyBinary(parameters.PaletteStream, DataCreator.TilesPalette(paletteFormat, parsingMethod), "palette");
				VerifyBinary(parameters.BinaryStream, DataCreator.TilesBinary(parsingMethod), "binary data");
				VerifyBinaryIsEmpty(parameters.TileAttributesStream, "tile attributes");
				VerifyBinary(parameters.TilesInfoStream, DataCreator.TilesInfo(parsingMethod), "tiles info");
				VerifyBinary(parameters.TilesImageStream, DataCreator.TilesImage(parsingMethod), "tiles image");
				VerifyBinaryIsEmpty(parameters.SpriteAttributesStream, "sprite attributes");
				VerifyBinaryIsEmpty(parameters.SpritesImageStream, "sprites image");
				VerifyAssembler(parameters, DataCreator.AssemblerTilesBinary(parameters.Time, commentType, parsingMethod));
			});
		}

		#endregion

		#region Assembler+Binary+Attributes

		[DataTestMethod]
		[DataRow(CommentType.Full, PaletteFormat.Next8Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.Full, PaletteFormat.Next8Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.Full, PaletteFormat.Next9Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.Full, PaletteFormat.Next9Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.None, PaletteFormat.Next8Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.None, PaletteFormat.Next8Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.None, PaletteFormat.Next9Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.None, PaletteFormat.Next9Bit, PaletteParsingMethod.ByObjects)]
		public void Tiles_Assembler_BinaryAttributes(CommentType commentType, PaletteFormat paletteFormat, PaletteParsingMethod parsingMethod)
		{
			TestTiles((model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Tiles;
				model.CommentType = commentType;
				model.PaletteFormat = paletteFormat;
				model.PaletteParsingMethod = parsingMethod;
				model.BinaryOutput = true;
				model.BinaryFramesAttributesOutput = true;
				model.TilesExportAsImage = false;

				// execute
				exporter.MapPaletteFromFirstImage();
				exporter.Remap();
				exporter.Export();

				// verify
				VerifyBinary(parameters.PaletteStream, DataCreator.TilesPalette(paletteFormat, parsingMethod), "palette");
				VerifyBinary(parameters.BinaryStream, DataCreator.TilesBinary(parsingMethod), "binary data");
				VerifyBinary(parameters.TileAttributesStream, DataCreator.TilesAttributes(parsingMethod), "tile attributes");
				VerifyBinaryIsEmpty(parameters.TilesInfoStream, "tiles info");
				VerifyBinaryIsEmpty(parameters.TilesImageStream, "tiles image");
				VerifyBinaryIsEmpty(parameters.SpriteAttributesStream, "sprite attributes");
				VerifyBinaryIsEmpty(parameters.SpritesImageStream, "sprites image");
				VerifyAssembler(parameters, DataCreator.AssemblerTilesBinaryAttributes(parameters.Time, commentType, parsingMethod));
			});
		}

		[DataTestMethod]
		[DataRow(CommentType.Full, PaletteFormat.Next8Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.Full, PaletteFormat.Next8Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.Full, PaletteFormat.Next9Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.Full, PaletteFormat.Next9Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.None, PaletteFormat.Next8Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.None, PaletteFormat.Next8Bit, PaletteParsingMethod.ByObjects)]
		[DataRow(CommentType.None, PaletteFormat.Next9Bit, PaletteParsingMethod.ByPixels)]
		[DataRow(CommentType.None, PaletteFormat.Next9Bit, PaletteParsingMethod.ByObjects)]
		public void Tiles_Assembler_BinaryAttributes_TilesAsImage(CommentType commentType, PaletteFormat paletteFormat, PaletteParsingMethod parsingMethod)
		{
			TestTiles((model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Tiles;
				model.CommentType = commentType;
				model.PaletteFormat = paletteFormat;
				model.PaletteParsingMethod = parsingMethod;
				model.BinaryOutput = true;
				model.BinaryFramesAttributesOutput = true;
				model.TilesExportAsImage = true;

				// execute
				exporter.MapPaletteFromFirstImage();
				exporter.Remap();
				exporter.Export();

				// verify
				VerifyBinary(parameters.PaletteStream, DataCreator.TilesPalette(paletteFormat, parsingMethod), "palette");
				VerifyBinary(parameters.BinaryStream, DataCreator.TilesBinary(parsingMethod), "binary data");
				VerifyBinary(parameters.TileAttributesStream, DataCreator.TilesAttributes(parsingMethod), "tile attributes");
				VerifyBinary(parameters.TilesInfoStream, DataCreator.TilesInfo(parsingMethod), "tiles info");
				VerifyBinary(parameters.TilesImageStream, DataCreator.TilesImage(parsingMethod), "tiles image");
				VerifyBinaryIsEmpty(parameters.SpriteAttributesStream, "sprite attributes");
				VerifyBinaryIsEmpty(parameters.SpritesImageStream, "sprites image");
				VerifyAssembler(parameters, DataCreator.AssemblerTilesBinaryAttributes(parameters.Time, commentType, parsingMethod));
			});
		}

		#endregion

		#endregion

		#region Tilemaps

		// note: we only test different tile formats here, repeated for assembler and binary output. Also: we don't include an image, therefore we don't have to map colours, we simply take the ones loaded from project file.

		[TestMethod]
		[DataRow(TilemapExportType.AttributesIndexAsWord)]
		[DataRow(TilemapExportType.AttributesIndexAsTwoBytes)]
		[DataRow(TilemapExportType.IndexOnly)]
		public void Tilemap_Assembler(TilemapExportType tilemapType)
		{
			TestTilemaps((model, parameters, exporter) =>
			{
				// setup
				model.TilemapExportType = tilemapType;
				model.BinaryOutput = false;

				// execute
				exporter.Remap();
				exporter.Export();

				// verify
				VerifyBinaryIsEmpty(parameters.PaletteStream, "pal");
				VerifyBinaryIsEmpty(parameters.BinaryStream, "bin");
				VerifyBinaryIsEmpty(parameters.TileAttributesStream, "map");
				VerifyBinaryIsEmpty(parameters.SpriteAttributesStream, "til");
				VerifyBinaryIsEmpty(parameters.TilesInfoStream, "blk");
				VerifyBinaryIsEmpty(parameters.TilesImageStream, "blocks image");
				VerifyBinaryIsEmpty(parameters.SpritesImageStream, "tiles image");
				VerifyBinaryArrayIsEmpty(10, parameters.TilemapsStream, "tilemaps");
				VerifyAssembler(parameters, DataCreator.AssemblerTilemaps(parameters.Time, tilemapType));
			});
		}

		[TestMethod]
		[DataRow(TilemapExportType.AttributesIndexAsWord)]
		[DataRow(TilemapExportType.AttributesIndexAsTwoBytes)]
		[DataRow(TilemapExportType.IndexOnly)]
		public void Tilemap_Assembler_Binary(TilemapExportType tilemapType)
		{
			TestTilemaps((model, parameters, exporter) =>
			{
				// setup
				model.TilemapExportType = tilemapType;
				model.BinaryOutput = true;

				// execute
				exporter.Remap();
				exporter.Export();

				// verify
				VerifyBinary(parameters.PaletteStream, DataCreator.TilemapsPalette(), "pal");
				VerifyBinaryIsEmpty(parameters.BinaryStream, "bin");
				VerifyBinaryIsEmpty(parameters.TileAttributesStream, "map");
				VerifyBinaryIsEmpty(parameters.SpriteAttributesStream, "til");
				VerifyBinaryIsEmpty(parameters.TilesInfoStream, "blk");
				VerifyBinaryIsEmpty(parameters.TilesImageStream, "blocks image");
				VerifyBinaryIsEmpty(parameters.SpritesImageStream, "tiles image");
				VerifyBinaryArray(1, (i) => DataCreator.TilemapsBinary(tilemapType), parameters.TilemapsStream, "tilemaps");
				VerifyAssembler(parameters, DataCreator.AssemblerTilemapsBinary(parameters.Time, tilemapType));
			});
		}

		#endregion

		#region Creating

		private void TestTilemaps(Action<MainModel, ExportParameters, Exporter> tester)
		{
			Test(
				DataCreator.XmlDocumentTiles(),	// we can reuse tiles document
				null,							// no source image needed
				DataCreator.ProjectTilemapData2x2(),
				(model, parameters, exporter) =>
				{
					// For tilemaps we only test a subset since the rest of the export is exactly like tiles/sprites. So we can setup common values here.
					model.OutputType = OutputType.Tiles;
					model.CommentType = CommentType.Full;
					model.PaletteFormat = PaletteFormat.Next8Bit;

					// After common values are set, we can call out to tester to further setup or run the test.
					tester(model, parameters, exporter);
				});
		}

		private void TestTiles(Action<MainModel, ExportParameters, Exporter> tester)
		{
			Test(
				DataCreator.XmlDocumentTiles(),
				DataCreator.ProjectImageTiles(),
				null,
				tester);
		}

		private void TestSprites(Action<MainModel, ExportParameters, Exporter> tester)
		{
			Test(
				DataCreator.XmlDocumentSprites(),
				DataCreator.ProjectImageSprites(),
				null,
				tester);
		}

		private void Test(
			XmlDocument sourceDocument, 
			Bitmap sourceBitmap, 
			TilemapData sourceTilemap,
			Action<MainModel, ExportParameters, Exporter> tester)
		{
			// We use memory streams so that we can later on examine the results without writing out files - faster and more predictable. 
			using (
				MemoryStream sourceStream = new MemoryStream(),
				binaryStream = new MemoryStream(),
				paletteStream = new MemoryStream(),
				mapStream = new MemoryStream(),
				tilesStream = new MemoryStream(),
				tilesImageStream = new MemoryStream(),
				tilesInfoStream = new MemoryStream(),
				blocksImageStream = new MemoryStream())
			{
				var tilemapStreams = new MemoryStream[10];
				var blockStreams = new MemoryStream[40];

				// We want streams to be "constant", created only once per test and then reused whenever anyone calls out the closures.
				var parameters = new ExportParameters
				{
					Time = DateTime.Now,

					ExportCallbacks = new ExportCallbacksImpl(),

					SourceStream = () => sourceStream,
					PaletteStream = () => paletteStream,
					BinaryStream = () => binaryStream,
					TilesImageStream = () => blocksImageStream,
					TileAttributesStream = () => mapStream,
					TilesInfoStream = () => tilesInfoStream,
					SpritesImageStream = () => tilesImageStream,
					SpriteAttributesStream = () => tilesStream,
					TilemapsStream = (i) =>
					{
						if (tilemapStreams[i] == null)
						{
							tilemapStreams[i] = new MemoryStream();
						}
						return tilemapStreams[i];
					},
					SpriteImageStream = (i) =>
					{
						if (blockStreams[i] == null)
						{
							blockStreams[i] = new MemoryStream();
						}
						return blockStreams[i];
					},
				};

				// We load default data and rely on each test to set it up as needed. This sets up model with default parameters, but we can later change them as needed in each specific test.
				var model = DataCreator.LoadModel(sourceDocument);

				if (sourceBitmap != null)
				{
					model.AddSource(new SourceImage("image1", sourceBitmap));
				}

				if (sourceTilemap != null)
				{
					model.AddSource(new SourceTilemapMap("tilemap1", model, sourceTilemap));
				}

				// Prepare exporter.
				var exporter = new Exporter(model, parameters);

				// Call out tester closure to actually perform the test.
				tester(model, parameters, exporter);

				foreach (var stream in blockStreams)
				{
					if (stream != null)
					{
						stream.Dispose();
					}
				}
			}
		}

		#endregion

		#region Verifying

		private void VerifyAssembler(ExportParameters parameters, string expected) 
		{
			// Prepare new memory stream as original one was already closed after writing the output.
			using (var actual = new MemoryStream(((MemoryStream)parameters.SourceStream()).ToArray()))
			{
				// This code trims all empty lines so we can compare just data.
				var expectedLines = expected.ToLines().Where(line => line.Trim().Length > 0).ToList();
				var actualLines = actual.ToLines().Where(line => line.Trim().Length > 0).ToList();

				// Note: we could assert on both lists direclty, but then assertion errors are not very helpful...
				Assert.AreEqual(expectedLines.Count, actualLines.Count, $"assembler lines count is different");

				string SanitizedLine(string line)
				{
					return Regex.Replace(line, @"\s+", " ");
				}

				for (var i=0; i<expectedLines.Count; i++)
				{
					// We replace all whitespace with single space to match only on actual data.
					var expectedLine = SanitizedLine(expectedLines[i]);
					var actualLine = SanitizedLine(actualLines[i]);
					Assert.AreEqual(expectedLine, actualLine, $"assembler line {i + 1} different");
				}
			}
		}

		private void VerifyBinary(Func<Stream> stream, byte[] expected, string explanation = "")
		{
			var actual = ((MemoryStream)stream()).ToArray();
			Assert.AreEqual(expected.Length, actual.Length, $"{explanation} size is different");
				
			for (var i=0; i<expected.Length; i++)
			{
				var expectedByte = expected[i];
				var actualByte = actual[i];

				Assert.AreEqual(expectedByte, actualByte, $"{explanation} byte {i} is different");
			}
		}

		private void VerifyBinaryArray(int count, Func<int, byte[]> expected, Func<int, Stream> streams, string explanation = "")
		{
			// If no stream was produced, we assume it's indeed emtpy.
			if (streams == null) return;

			for (int i = 0; i < count; i++)
			{
				var actualStream = streams(i);
				var expectedData = expected(i);

				VerifyBinary(() => actualStream, expectedData, $"{explanation}[{i}]");
			}
		}

		private void VerifyBinaryIsEmpty(Func<Stream> stream, string explanation = "")
		{
			// If no stream  was produces, we assume it's indeed empty...
			if (stream == null) return;

			using (var actual = new MemoryStream(((MemoryStream)stream()).ToArray()))
			{
				Assert.AreEqual(0, actual.Length, $"{explanation} size is not zero");
			}
		}

		private void VerifyBinaryArrayIsEmpty(int count, Func<int, Stream> streams, string explanation = "")
		{
			// If no stream was produced, we assume it's indeed emtpy.
			if (streams == null) return;

			for (int i=0; i<count; i++)
			{
				using (var actual = new MemoryStream(((MemoryStream)streams(i)).ToArray()))
				{
					Assert.AreEqual(0, actual.Length, $"{explanation}[{i}] size is not zero");
				}
			}
		}

		#endregion

		#region Declarations

		private class ExportCallbacksImpl : ExportCallbacks
		{
			public void OnExportStarted()
			{
			}

			public void OnExportCompleted()
			{
			}

			public string OnExportTilesInfoFilename()
			{
				return DataCreator.TilesInfoFilename();
			}

			public byte OnExportFourBitColourConverter(byte proposed)
			{
				return proposed;
			}

			public byte OnExportPaletteOffsetMapper(byte proposed)
			{
				return proposed;
			}
		}

		#endregion
	}

	namespace ExporterTestingExtensions
	{
		public static class Extensions
		{
			public static void MapPaletteFromFirstImage(this Exporter exporter)
			{
				var model = exporter.Data.Model;
				var image = model.SourceImages().First();

				var palette = exporter.MapPalette(image.Data);

				palette.CopyTo(model.Palette);
			}
		}
	}
}
