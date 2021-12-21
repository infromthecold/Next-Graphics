using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Exporting.Common
{

	public class ExportParameters
	{
		/// <summary>
		/// Date and time of the export.
		/// </summary>
		public DateTime Time { get; set; } = DateTime.Now;

		/// <summary>
		/// Stream into which source file will be generated. Set property or result of the assigned function to not generate this file.
		/// </summary>
		public Func<Stream> SourceStreamProvider { get; set; }

		/// <summary>
		/// Stream into which binary data will be generated (if binary export is selected). Set property or result of the assigned function to not generate this file.
		/// </summary>
		public Func<Stream> BinaryStreamProvider { get; set; }

		/// <summary>
		/// Stream into which blocks data will be generated (if blocks export is selected). Set property or result of the assigned function to not generate this file.
		/// </summary>
		public Func<Stream> BlocksStreamProvider { get; set; }

		/// <summary>
		/// Constructor where each stream provider is manually assigned to corresponding properties.
		/// </summary>
		public ExportParameters()
		{
		}

		/// <summary>
		/// Constructor that automatically prepares stream providers based on the given source path.
		/// </summary>
		public ExportParameters(string sourcePath)
		{
			var pathWithExtension = Path.HasExtension(sourcePath) ? sourcePath : $"{sourcePath}.asm";

			SourceStreamProvider = () => File.OpenWrite(pathWithExtension);
			BinaryStreamProvider = () => File.OpenWrite(Path.ChangeExtension(pathWithExtension, "bin"));
			BlocksStreamProvider = () => File.OpenWrite(Path.ChangeExtension(pathWithExtension, "til"));
		}
	}

}
