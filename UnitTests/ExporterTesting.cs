using Microsoft.VisualStudio.TestTools.UnitTesting;

using NextGraphics.Models;
using NextGraphics.Exporting;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using UnitTests.Data;
using NextGraphics.Exporting.Common;

namespace UnitTests
{
	// Note: this test class was creater for purposes of extracting exporting code out of main.cs so it doesn't necessarily cover all future exporting capabilities (would be nice to do so though)

	[TestClass]
	public class ExporterTesting
	{
		#region Tiles

		#region Assembler

		[TestMethod]
		public void ShouldExport_TilesAssembler_FullComments()
		{
			Test(DataCreator.XmlDocumentTiles(), (model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Tiles;
				model.CommentType = CommentType.Full;
				model.BinaryOutput = false;
				model.BinaryBlocksOutput = false;
				model.BlocksAsImage = false;
				model.TilesAsImage = false;

				// execute
				exporter.Export(parameters);

				// verify
				VerifyBinaryIsEmpty(parameters.PaletteStream, "pal");
				VerifyBinaryIsEmpty(parameters.BinaryStream, "bin");
				VerifyBinaryIsEmpty(parameters.TilesStream, "til");
				VerifyBinaryIsEmpty(parameters.MapStream, "map");
				VerifyBinaryIsEmpty(parameters.BlocksImageStream, "blocks image");
				VerifyBinaryIsEmpty(parameters.TilesImageStream, "tiles image");
				VerifyAssembler(parameters, DataCreator.AssemblerTiles(parameters.Time, CommentType.Full));
			});
		}

		[TestMethod]
		public void ShouldExport_TilesAssembler_NoComments()
		{
			Test(DataCreator.XmlDocumentTiles(), (model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Tiles;
				model.CommentType = CommentType.None;
				model.BinaryOutput = false;
				model.BinaryBlocksOutput = false;
				model.BlocksAsImage = false;
				model.TilesAsImage = false;

				// execute
				exporter.Export(parameters);

				// verify
				VerifyBinaryIsEmpty(parameters.PaletteStream, "pal");
				VerifyBinaryIsEmpty(parameters.BinaryStream, "bin");
				VerifyBinaryIsEmpty(parameters.TilesStream, "til");
				VerifyBinaryIsEmpty(parameters.MapStream, "map");
				VerifyBinaryIsEmpty(parameters.BlocksImageStream, "blocks image");
				VerifyBinaryIsEmpty(parameters.TilesImageStream, "tiles image");
				VerifyAssembler(parameters, DataCreator.AssemblerTiles(parameters.Time, CommentType.None));
			});
		}

		[TestMethod]
		public void ShouldExport_TilesAssembler_BlocksImage()
		{
			Test(DataCreator.XmlDocumentTiles(), (model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Tiles;
				model.CommentType = CommentType.None;
				model.BinaryOutput = false;
				model.BinaryBlocksOutput = false;
				model.BlocksAsImage = true;
				model.TilesAsImage = false;

				// execute
				exporter.Export(parameters);

				// verify
				VerifyBinaryIsEmpty(parameters.PaletteStream, "pal");
				VerifyBinaryIsEmpty(parameters.BinaryStream, "bin");
				VerifyBinaryIsEmpty(parameters.TilesStream, "til");
				VerifyBinaryIsEmpty(parameters.MapStream, "map");
				VerifyBinary(parameters.BlocksImageStream, DataCreator.TilesImageBlocks(), "blocks image");
				VerifyBinaryIsEmpty(parameters.TilesImageStream, "tiles image");
				VerifyAssembler(parameters, DataCreator.AssemblerTiles(parameters.Time, CommentType.None, true));
			});
		}

		[TestMethod]
		public void ShouldExport_TilesAssembler_TilesImage()
		{
			Test(DataCreator.XmlDocumentTiles(), (model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Tiles;
				model.CommentType = CommentType.None;
				model.BinaryOutput = false;
				model.BinaryBlocksOutput = false;
				model.BlocksAsImage = false;
				model.TilesAsImage = true;

				// execute
				exporter.Export(parameters);

				// verify
				VerifyBinaryIsEmpty(parameters.PaletteStream, "pal");
				VerifyBinaryIsEmpty(parameters.BinaryStream, "bin");
				VerifyBinaryIsEmpty(parameters.TilesStream, "til");
				VerifyBinaryIsEmpty(parameters.MapStream, "map");
				VerifyBinaryIsEmpty(parameters.BlocksImageStream, "blocks image");
				VerifyBinary(parameters.TilesImageStream, DataCreator.TilesImageTiles(), "tiles image");
				VerifyAssembler(parameters, DataCreator.AssemblerTiles(parameters.Time, CommentType.None, true));
			});
		}

		[TestMethod]
		public void ShouldExport_TilesAssembler_BlocksTilesImage()
		{
			Test(DataCreator.XmlDocumentTiles(), (model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Tiles;
				model.CommentType = CommentType.None;
				model.BinaryOutput = false;
				model.BinaryBlocksOutput = false;
				model.BlocksAsImage = true;
				model.TilesAsImage = true;

				// execute
				exporter.Export(parameters);

				// verify
				VerifyBinaryIsEmpty(parameters.PaletteStream, "pal");
				VerifyBinaryIsEmpty(parameters.BinaryStream, "bin");
				VerifyBinaryIsEmpty(parameters.TilesStream, "til");
				VerifyBinaryIsEmpty(parameters.MapStream, "map");
				VerifyBinary(parameters.BlocksImageStream, DataCreator.TilesImageBlocks(), "blocks image");
				VerifyBinary(parameters.TilesImageStream, DataCreator.TilesImageTiles(), "tiles image");
				VerifyAssembler(parameters, DataCreator.AssemblerTiles(parameters.Time, CommentType.None, true));
			});
		}

		#endregion

		#region Assembler+Binary

		[TestMethod]
		public void ShouldExport_TilesAssembler_Binary_FullComments()
		{
			Test(DataCreator.XmlDocumentTiles(), (model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Tiles;
				model.CommentType = CommentType.Full;
				model.BinaryOutput = true;
				model.BinaryBlocksOutput = false;
				model.BlocksAsImage = false;
				model.TilesAsImage = false;

				// execute
				exporter.Export(parameters);

				// verify
				VerifyBinaryIsEmpty(parameters.TilesStream, "til");
				VerifyBinary(parameters.PaletteStream, DataCreator.TilesPal(), "pal");
				VerifyBinary(parameters.BinaryStream, DataCreator.TilesBin(), "bin");
				VerifyBinary(parameters.MapStream, DataCreator.TilesMap(), "map");
				VerifyBinaryIsEmpty(parameters.BlocksImageStream, "blocks image");
				VerifyBinaryIsEmpty(parameters.TilesImageStream, "tiles image");
				VerifyAssembler(parameters, DataCreator.AssemblerTilesAndBinary(parameters.Time, CommentType.Full));
			});
		}

		[TestMethod]
		public void ShouldExport_TilesAssembler_Binary_NoComments()
		{
			Test(DataCreator.XmlDocumentTiles(), (model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Tiles;
				model.CommentType = CommentType.None;
				model.BinaryOutput = true;
				model.BinaryBlocksOutput = false;
				model.BlocksAsImage = false;
				model.TilesAsImage = false;

				// execute
				exporter.Export(parameters);

				// verify
				VerifyBinaryIsEmpty(parameters.TilesStream, "til");
				VerifyBinary(parameters.PaletteStream, DataCreator.TilesPal(), "pal");
				VerifyBinary(parameters.BinaryStream, DataCreator.TilesBin(), "bin");
				VerifyBinary(parameters.MapStream, DataCreator.TilesMap(), "map");
				VerifyBinaryIsEmpty(parameters.BlocksImageStream, "blocks image");
				VerifyBinaryIsEmpty(parameters.TilesImageStream, "tiles image");
				VerifyAssembler(parameters, DataCreator.AssemblerTilesAndBinary(parameters.Time, CommentType.None));
			});
		}

		[TestMethod]
		public void ShouldExport_TilesAssembler_Binary_BlocksImage()
		{
			Test(DataCreator.XmlDocumentTiles(), (model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Tiles;
				model.CommentType = CommentType.Full;
				model.BinaryOutput = true;
				model.BinaryBlocksOutput = false;
				model.BlocksAsImage = true;
				model.TilesAsImage = false;

				// execute
				exporter.Export(parameters);

				// verify
				VerifyBinaryIsEmpty(parameters.TilesStream, "til");
				VerifyBinary(parameters.PaletteStream, DataCreator.TilesPal(), "pal");
				VerifyBinary(parameters.BinaryStream, DataCreator.TilesBin(), "bin");
				VerifyBinary(parameters.MapStream, DataCreator.TilesMap(), "map");
				VerifyBinary(parameters.BlocksImageStream, DataCreator.TilesImageBlocks(), "blocks image");
				VerifyBinaryIsEmpty(parameters.TilesImageStream, "tiles image");
				VerifyAssembler(parameters, DataCreator.AssemblerTilesAndBinary(parameters.Time, CommentType.None, true));
			});
		}

		[TestMethod]
		public void ShouldExport_TilesAssembler_Binary_TilesImage()
		{
			Test(DataCreator.XmlDocumentTiles(), (model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Tiles;
				model.CommentType = CommentType.Full;
				model.BinaryOutput = true;
				model.BinaryBlocksOutput = false;
				model.BlocksAsImage = false;
				model.TilesAsImage = true;

				// execute
				exporter.Export(parameters);

				// verify
				VerifyBinaryIsEmpty(parameters.TilesStream, "til");
				VerifyBinary(parameters.PaletteStream, DataCreator.TilesPal(), "pal");
				VerifyBinary(parameters.BinaryStream, DataCreator.TilesBin(), "bin");
				VerifyBinary(parameters.MapStream, DataCreator.TilesMap(), "map");
				VerifyBinaryIsEmpty(parameters.BlocksImageStream, "blocks image");
				VerifyBinary(parameters.TilesImageStream, DataCreator.TilesImageTiles(), "tiles image");
				VerifyAssembler(parameters, DataCreator.AssemblerTilesAndBinary(parameters.Time, CommentType.None, true));
			});
		}

		[TestMethod]
		public void ShouldExport_TilesAssembler_Binary_BlocksTilesImage()
		{
			Test(DataCreator.XmlDocumentTiles(), (model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Tiles;
				model.CommentType = CommentType.Full;
				model.BinaryOutput = true;
				model.BinaryBlocksOutput = false;
				model.BlocksAsImage = true;
				model.TilesAsImage = true;

				// execute
				exporter.Export(parameters);

				// verify
				VerifyBinaryIsEmpty(parameters.TilesStream, "til");
				VerifyBinary(parameters.PaletteStream, DataCreator.TilesPal(), "pal");
				VerifyBinary(parameters.BinaryStream, DataCreator.TilesBin(), "bin");
				VerifyBinary(parameters.MapStream, DataCreator.TilesMap(), "map");
				VerifyBinary(parameters.BlocksImageStream, DataCreator.TilesImageBlocks(), "blocks image");
				VerifyBinary(parameters.TilesImageStream, DataCreator.TilesImageTiles(), "tiles image");
				VerifyAssembler(parameters, DataCreator.AssemblerTilesAndBinary(parameters.Time, CommentType.None, true));
			});
		}
		#endregion

		#region Assembler+Binary+Blocks

		[TestMethod]
		public void ShouldExport_TilesAssembler_BinaryBlocks_FullComments()
		{
			Test(DataCreator.XmlDocumentTiles(), (model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Tiles;
				model.CommentType = CommentType.Full;
				model.BinaryOutput = true;
				model.BinaryBlocksOutput = true;
				model.BlocksAsImage = false;
				model.TilesAsImage = false;

				// execute
				exporter.Export(parameters);

				// verify
				VerifyBinary(parameters.TilesStream, DataCreator.TilesTil(), "til");
				VerifyBinary(parameters.PaletteStream, DataCreator.TilesPal(), "pal");
				VerifyBinary(parameters.BinaryStream, DataCreator.TilesBin(), "bin");
				VerifyBinary(parameters.MapStream, DataCreator.TilesMap(), "map");
				VerifyBinaryIsEmpty(parameters.BlocksImageStream, "blocks image");
				VerifyBinaryIsEmpty(parameters.TilesImageStream, "tiles image");
				VerifyAssembler(parameters, DataCreator.AssemblerTilesAndBinaryAndBlocks(parameters.Time, CommentType.Full));
			});
		}

		[TestMethod]
		public void ShouldExport_TilesAssembler_BinaryBlocks_NoComments()
		{
			Test(DataCreator.XmlDocumentTiles(), (model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Tiles;
				model.CommentType = CommentType.None;
				model.BinaryOutput = true;
				model.BinaryBlocksOutput = true;
				model.BlocksAsImage = false;
				model.TilesAsImage = false;

				// execute
				exporter.Export(parameters);

				// verify
				VerifyBinary(parameters.TilesStream, DataCreator.TilesTil(), "til");
				VerifyBinary(parameters.PaletteStream, DataCreator.TilesPal(), "pal");
				VerifyBinary(parameters.BinaryStream, DataCreator.TilesBin(), "bin");
				VerifyBinary(parameters.MapStream, DataCreator.TilesMap(), "map");
				VerifyBinaryIsEmpty(parameters.BlocksImageStream, "blocks image");
				VerifyBinaryIsEmpty(parameters.TilesImageStream, "tiles image");
				VerifyAssembler(parameters, DataCreator.AssemblerTilesAndBinaryAndBlocks(parameters.Time, CommentType.None));
			});
		}

		[TestMethod]
		public void ShouldExport_TilesAssembler_BinaryBlocks_BlocksImage()
		{
			Test(DataCreator.XmlDocumentTiles(), (model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Tiles;
				model.CommentType = CommentType.Full;
				model.BinaryOutput = true;
				model.BinaryBlocksOutput = true;
				model.BlocksAsImage = true;
				model.TilesAsImage = false;

				// execute
				exporter.Export(parameters);

				// verify
				VerifyBinary(parameters.TilesStream, DataCreator.TilesTil(), "til");
				VerifyBinary(parameters.PaletteStream, DataCreator.TilesPal(), "pal");
				VerifyBinary(parameters.BinaryStream, DataCreator.TilesBin(), "bin");
				VerifyBinary(parameters.MapStream, DataCreator.TilesMap(), "map");
				VerifyBinary(parameters.BlocksImageStream, DataCreator.TilesImageBlocks(), "blocks image");
				VerifyBinaryIsEmpty(parameters.TilesImageStream, "tiles image");
				VerifyAssembler(parameters, DataCreator.AssemblerTilesAndBinaryAndBlocks(parameters.Time, CommentType.None, true));
			});
		}

		[TestMethod]
		public void ShouldExport_TilesAssembler_BinaryBlocks_TilesImage()
		{
			Test(DataCreator.XmlDocumentTiles(), (model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Tiles;
				model.CommentType = CommentType.Full;
				model.BinaryOutput = true;
				model.BinaryBlocksOutput = true;
				model.BlocksAsImage = false;
				model.TilesAsImage = true;

				// execute
				exporter.Export(parameters);

				// verify
				VerifyBinary(parameters.TilesStream, DataCreator.TilesTil(), "til");
				VerifyBinary(parameters.PaletteStream, DataCreator.TilesPal(), "pal");
				VerifyBinary(parameters.BinaryStream, DataCreator.TilesBin(), "bin");
				VerifyBinary(parameters.MapStream, DataCreator.TilesMap(), "map");
				VerifyBinaryIsEmpty(parameters.BlocksImageStream, "blocks image");
				VerifyBinary(parameters.TilesImageStream, DataCreator.TilesImageTiles(), "tiles image");
				VerifyAssembler(parameters, DataCreator.AssemblerTilesAndBinaryAndBlocks(parameters.Time, CommentType.None, true));
			});
		}

		[TestMethod]
		public void ShouldExport_TilesAssembler_BinaryBlocks_BlocksTilesImage()
		{
			Test(DataCreator.XmlDocumentTiles(), (model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Tiles;
				model.CommentType = CommentType.Full;
				model.BinaryOutput = true;
				model.BinaryBlocksOutput = true;
				model.BlocksAsImage = true;
				model.TilesAsImage = true;

				// execute
				exporter.Export(parameters);

				// verify
				VerifyBinary(parameters.TilesStream, DataCreator.TilesTil(), "til");
				VerifyBinary(parameters.PaletteStream, DataCreator.TilesPal(), "pal");
				VerifyBinary(parameters.BinaryStream, DataCreator.TilesBin(), "bin");
				VerifyBinary(parameters.MapStream, DataCreator.TilesMap(), "map");
				VerifyBinary(parameters.BlocksImageStream, DataCreator.TilesImageBlocks(), "blocks image");
				VerifyBinary(parameters.TilesImageStream, DataCreator.TilesImageTiles(), "tiles image");
				VerifyAssembler(parameters, DataCreator.AssemblerTilesAndBinaryAndBlocks(parameters.Time, CommentType.None, true));
			});
		}

		#endregion

		#endregion

		#region Creating & Verifying

		private void Test(XmlDocument sourceDocument, Action<MainModel, ExportParameters, Exporter> tester)
		{
			// We use memory streams so that we can later on examine the results without writing out files - faster and more predictable. 
			using (
				MemoryStream sourceStream = new MemoryStream(), 
				binaryStream = new MemoryStream(), 
				tilesStream = new MemoryStream(),
				paletteStream = new MemoryStream(),
				mapStream = new MemoryStream(),
				blocksImageStream = new MemoryStream(),
				tilesImageStream = new MemoryStream())
			{
				// We want streams to be "constant", created only once per test and then reused whenever anyone calls out the closures.
				var parameters = new ExportParameters
				{
					Time = DateTime.Now,
					SourceStream = () => sourceStream,
					PaletteStream = () => paletteStream,
					BinaryStream = () => binaryStream,
					TilesStream = () => tilesStream,
					MapStream = () => mapStream,
					BlocksImageStream = () => blocksImageStream,
					TilesImageStream = () => tilesImageStream,
				};

				// We load default data and rely on each test to set it up as needed. This sets up model with default parameters, but we can later change them as needed in each specific test.
				var model = DataCreator.LoadModel(sourceDocument);
				model.AddImage(new SourceImage("level1", DataCreator.TilesImageLevel1()));

				// Prepare exporter with just created model and prepare remap data.
				var exporter = new Exporter(model);
				exporter.Remap();

				// Call out tester closure to actually perform the test.
				tester(model, parameters, exporter);
			}
		}

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

				for (var i=0; i<expectedLines.Count; i++)
				{
					var expectedLine = expectedLines[i];
					var actualLine = actualLines[i];
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

		private void VerifyBinaryIsEmpty(Func<Stream> stream, string explanation = "")
		{
			if (stream == null) return;

			using (var actual = new MemoryStream(((MemoryStream)stream()).ToArray()))
			{
				Assert.AreEqual(0, actual.Length, $"{explanation} size is not zero");
			}
		}

		#endregion
	}
}
