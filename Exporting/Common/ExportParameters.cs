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
		#region Data

		/// <summary>
		/// Date and time of the export.
		/// </summary>
		public DateTime Time { get; set; } = DateTime.Now;

		#endregion

		#region Callbacks

		/// <summary>
		/// Optional palette offset provider. Pre-selected palette index is passed in, and new index is returned. This is only called if needed, and if no closure is assigned, pre-selected value is used.
		/// </summary>
		public Func<byte, byte> PaletteOffsetProvider { get; set; } = null;

		/// <summary>
		/// Optional 4-bit colour converter; if assigned, this will be used for converting colours, otherwise default one will be used.
		/// </summary>
		public Func<Byte, Byte> FourBitColourConverter { get; set; } = null;

		#endregion

		#region Output streams

		/// <summary>
		/// Stream into which source file will be generated. If property is null, this is not generated.
		/// </summary>
		/// <remarks>
		/// The reason for using closure is to allow setting up a stream without creating underlying file. Only if closure will be invoked (based on export options), the underlying file will actually be created.
		/// </remarks>
		public Func<Stream> SourceStream { get; set; }

		/// <summary>
		/// Stream into which palette data will be generated (only if configuration requires this). If property is null, this is not generated.
		/// </summary>
		/// <remarks>
		/// The reason for using closure is to allow setting up a stream without creating underlying file. Only if closure will be invoked (based on export options), the underlying file will actually be created.
		/// </remarks>
		public Func<Stream> PaletteStream { get; set; }

		/// <summary>
		/// Stream into which binary data will be generated (only if configuration requires this). If property null, this is not generated.
		/// </summary>
		/// <remarks>
		/// The reason for using closure is to allow setting up a stream without creating underlying file. Only if closure will be invoked (based on export options), the underlying file will actually be created.
		/// </remarks>
		public Func<Stream> BinaryStream { get; set; }

		/// <summary>
		/// Stream into which tiles data will be generated (only if configuration requires this). If property is null, this is not generated.
		/// </summary>
		/// <remarks>
		/// The reason for using closure is to allow setting up a stream without creating underlying file. Only if closure will be invoked (based on export options), the underlying file will actually be created.
		/// </remarks>
		public Func<Stream> TilesStream { get; set; }

		/// <summary>
		/// Stream into which map data will be generated (only if configuration requires this). If property is null, this is not generated.
		/// </summary>
		/// <remarks>
		/// The reason for using closure is to allow setting up a stream without creating underlying file. Only if closure will be invoked (based on export options), the underlying file will actually be created.
		/// </remarks>
		public Func<Stream> MapStream { get; set; }

		/// <summary>
		/// Stream into which blocks image will be generated (only if configuration requires this). If property is null, this is not generated.
		/// </summary>
		/// <remarks>
		/// The reason for using closure is to allow setting up a stream without creating underlying file. Only if closure will be invoked (based on export options), the underlying file will actually be created.
		/// </remarks>
		public Func<Stream> BlocksImageStream { get; set; }

		/// <summary>
		/// Stream into which tiles image will be generated (only if configuration requires this). If property is null, this is not generated.
		/// </summary>
		/// <remarks>
		/// The reason for using closure is to allow setting up a stream without creating underlying file. Only if closure will be invoked (based on export options), the underlying file will actually be created.
		/// </remarks>
		public Func<Stream> TilesImageStream { get; set; }

		#endregion

		#region Initialization & disposal

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

			SourceStream = () => File.OpenWrite(pathWithExtension);
			MapStream = () => File.OpenWrite(Path.ChangeExtension(pathWithExtension, "map"));
			BinaryStream = () => File.OpenWrite(Path.ChangeExtension(pathWithExtension, "bin"));
			TilesStream = () => File.OpenWrite(Path.ChangeExtension(pathWithExtension, "til"));
			PaletteStream = () => File.OpenWrite(Path.ChangeExtension(pathWithExtension, "pal"));
		}

		#endregion
	}
}
