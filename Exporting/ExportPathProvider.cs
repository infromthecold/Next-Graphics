using NextGraphics.Exporting.Common;
using NextGraphics.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Exporting
{
	/// <summary>
	/// Small helper class for generating export paths and filenames.
	/// </summary>
	/// <remarks>
	/// Note: at the moment this works for ZX Next export, but we may want to have a bit more generic approach should other types of export be added down the road (then again, the same goes for various export stream closures in <see cref="Exporting.Common.ExportParameters"/> with which this class represents roughly 1:1 mapping).
	/// </remarks>
	public class ExportPathProvider
	{
		private Dictionary<string, Stream> _streams = new Dictionary<string, Stream>();
		private ImageFormat _imageFormat;

		#region Initialization & Disposal

		public ExportPathProvider(string filename, ImageFormat imageFormat)
		{
			_imageFormat = imageFormat;
			SourceFilename = filename;
			DestinationPath = Path.GetDirectoryName(filename);
		}

		#endregion

		#region Paths

		public string DestinationPath { get; private set; }

		public string SourceFilename { get; private set; }

		public string PaletteFilename { get => Path.ChangeExtension(SourceFilename, "pal"); }

		public string BinaryFilename { get => Path.ChangeExtension(SourceFilename, "bin"); }

		public string MapFilename { get => Path.ChangeExtension(SourceFilename, "map"); }

		public string TilesFilename { get => Path.ChangeExtension(SourceFilename, "til"); }

		public string TilesInfoFilename { get => Path.ChangeExtension(SourceFilename, "blk"); }

		public string TilesImageFilename { get => FilenameWithAppendix("tiles", _imageFormat.Extension()); }

		public string BlocksImageFilename { get => FilenameWithAppendix("blocks", _imageFormat.Extension()); }

		public string BlockImageFilename(int index)
		{
			return FilenameWithAppendix($"sprite{index}", _imageFormat.Extension());
		}

		#endregion

		#region Streams

		public void AssignExportStreams(ExportParameters parameters)
		{
			parameters.SourceStream = () => PrepareStream(SourceFilename);
			parameters.PaletteStream = () => PrepareStream(PaletteFilename);
			parameters.BinaryStream = () => PrepareStream(BinaryFilename);
			parameters.MapStream = () => PrepareStream(MapFilename);
			parameters.TilesStream = () => PrepareStream(TilesFilename);
			parameters.TilesInfoStream = () => PrepareStream(TilesInfoFilename);
			parameters.TilesImageStream = () => PrepareStream(TilesImageFilename);
			parameters.BlocksImageStream = () => PrepareStream(BlocksImageFilename);
			parameters.BlockImageStream = (index) => PrepareStream(BlockImageFilename(index));
		}

		#endregion

		#region Helpers

		private Stream PrepareStream(string filename)
		{
			// If file already exists, delete it. In case of exceptions, we'll catch them in caller site and display dialog.
			if (File.Exists(filename))
			{
				File.Delete(filename);
			}

			// Streams dictionary holds on to each stream so multiple calls will result in reusing the same stream. By using filename as the key, it also makes it really simple to manage in generic way.
			if (!_streams.ContainsKey(filename))
			{
				_streams[filename] = File.OpenWrite(filename);
			}

			return _streams[filename];
		}

		private string FilenameWithAppendix(string appendix, string extension)
		{
			var filename = Path.GetFileNameWithoutExtension(SourceFilename);

			return Path.Combine(
				DestinationPath,
				$"{filename}-{appendix}.{extension}"
			);
		}

		#endregion
	}
}
