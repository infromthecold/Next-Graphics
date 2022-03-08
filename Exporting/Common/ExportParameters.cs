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
		/// Optional remapper callbacks implementation.
		/// </summary>
		public RemapCallbacks RemapCallbacks { get; set; } = null;

		/// <summary>
		/// Optional export callbacks implementation.
		/// </summary>
		public ExportCallbacks ExportCallbacks { get; set; } = null;

		#endregion

		#region Output streams
		
		// Note: The reason for using closures is to allow setting up a stream without creating underlying file. Only if closure will be invoked (based on options), the underlying file will actually be created.

		/// <summary>
		/// Stream into which source file will be generated. If property is null, this is not generated.
		/// </summary>
		public Func<Stream> SourceStream { get; set; }

		/// <summary>
		/// Stream into which palette data will be generated (only if configuration requires this). If property is null, this is not generated.
		/// </summary>
		public Func<Stream> PaletteStream { get; set; }

		/// <summary>
		/// Stream into which binary data will be generated (only if configuration requires this). If property null, this is not generated.
		/// </summary>
		public Func<Stream> BinaryStream { get; set; }

		/// <summary>
		/// Stream into which each tile attributes and index will be generated (only if configuration requires this). If property is null, this is not generated.
		/// </summary>
		public Func<Stream> TileAttributesStream { get; set; }

		/// <summary>
		/// Stream into which tiles data will be generated (only if configuration requires this). If property is null, this is not generated.
		/// </summary>
		public Func<Stream> SpriteAttributesStream { get; set; }

		/// <summary>
		/// Stream into which tiles info data will be generated (only if configuration requires this). If property is null, this is not generated.
		/// </summary>
		public Func<Stream> TilesInfoStream { get; set; }

		/// <summary>
		/// Stream into which tiles image will be generated (only if configuration requires this). If property is null, this is not generated.
		/// </summary>
		public Func<Stream> SpritesImageStream { get; set; }

		/// <summary>
		/// Stream into which each individual tilemap will be generated (only if configuration requires this). Called for each individual tilemap with tilemap index passed in as parameter. If property is null, this is not generated.
		/// </summary>
		public Func<int, Stream> TilemapsStream { get; set; }

		/// <summary>
		/// Stream into which blocks image will be generated (only if configuration requires this). If property is null, this is not generated.
		/// </summary>
		public Func<Stream> TilesImageStream { get; set; }

		/// <summary>
		/// Stream into which individual block images will be generated (only if configuration requires this). Called for each individual block, which index is passed in as parameter. If property is null, this is not generated.
		/// </summary>
		public Func<int, Stream> SpriteImageStream { get; set; }

		#endregion

		#region Initialization & disposal

		/// <summary>
		/// Constructor where each stream provider is manually assigned to corresponding properties.
		/// </summary>
		public ExportParameters()
		{
		}

		#endregion
	}
}
