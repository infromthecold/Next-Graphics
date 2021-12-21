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
	[TestClass]
	public class ExporterTesting
	{
		[TestMethod]
		public void ShouldExportBasicSourceFile()
		{
			Test((model, parameters, exporter) =>
			{
				// setup
				model.OutputType = OutputType.Tiles;
				model.CommentType = CommentType.None;
				model.BinaryOutput = false;
				model.BinaryBlocksOutput = false;

				// execute
				exporter.Export(parameters);

				// verify
				VerifySource(parameters, () => DataCreator.AssemblerOutputTiles(parameters.Time, embedTiles: false, fullComments: false));
			});
		}

		#region Creating & Verifying

		private void Test(Action<MainModel, ExportParameters, Exporter> tester)
		{
			// We use memory streams so that we can later on examine the results without writing out files - faster and more predictable. 
			using (MemoryStream sourceStream = new MemoryStream(), binaryStream = new MemoryStream(), blocksStream = new MemoryStream())
			{
				// We want streams to be "constant", created only once per test and then reused whenever anyone calls out the closures.
				var parameters = new ExportParameters
				{
					Time = DateTime.Now,
					SourceStreamProvider = () => sourceStream,
					BinaryStreamProvider = () => binaryStream,
					BlocksStreamProvider = () => blocksStream
				};

				// We load default data and rely on each test to set it up as needed. This sets up model with default parameters, but we can later change them as needed in each specific test.
				var model = DataCreator.LoadModel();

				// Prepare exporter with just created model and prepare remap data.
				var exporter = new Exporter(model);
				exporter.Remap();

				// Call out tester closure to actually perform the test.
				tester(model, parameters, exporter);
			}
		}

		private void VerifySource(ExportParameters parameters, Func<string> expectationBuilder) 
		{
			// Let "builder" closure prepare expected string.
			var expected = expectationBuilder();

			// This code trims all empty lines so we can compare just data.
			var expectedLines = expected.ToLines().Where(line => line.Trim().Length > 0).ToList();
			var actualLines = parameters.SourceStreamProvider().ToLines().Where(line => line.Trim().Length > 0).ToList();

			// Note: we could assert on both lists direclty, but then assertion errors are not very helpful...
			Assert.AreEqual(expectedLines.Count, actualLines.Count, $"Lines count is different");

			int i = 0;
			foreach (var expectedLine in expectedLines)
			{
				var actualLine = actualLines[i];
				Assert.AreEqual(expectedLine, actualLine, $"Line {i + 1} different");
			}
		}

		#endregion
	}
}
