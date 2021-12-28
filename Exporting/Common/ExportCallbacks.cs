using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Exporting.Common
{
	public interface ExportCallbacks
	{
		void OnExportStarted();
		void OnExportCompleted();

		/// <summary>
		/// Provides file name of the tiles info file. Only called if options require this export.
		/// </summary>
		string OnExportTilesInfoFilename();

		/// <summary>
		/// Optional palette offset provider. Pre-selected palette index is passed in, and new index is returned. This is only called if needed. If no custom handling is needed, proposed value can simply be returned.
		/// </summary>
		byte OnExportPaletteOffsetMapper(byte proposed);

		/// <summary>
		/// 4-bit colour converter; proposed colour is passed as parameter, so implementor can simply return if no additional handling is needed.
		/// </summary>
		byte OnExportFourBitColourConverter(byte proposed);
	}
}
