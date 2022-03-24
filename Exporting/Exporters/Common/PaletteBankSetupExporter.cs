using NextGraphics.Exporting.Exporters.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Exporting.Exporters.Common
{
	/// <summary>
	/// Asks user for palette bank if needed, and if set, updates export data with the answer.
	/// </summary>
	public class PaletteBankSetupExporter : BaseExporter
	{
		protected override void OnExport()
		{
			// In some cases we must require user to provide the bank. In this case we'll use the same offset for all exported tiles. It will also change the offset value to something else but MaxValue which is used in the loop to determine which value is used. If export callbacks are not provided, then 0 will be used.
			if (Parameters.ExportCallbacks != null && Model.PaletteParsingMethod == Models.PaletteParsingMethod.ByPixels)
			{
				ExportData.DefaultPaletteBank = Parameters.ExportCallbacks.OnExportPaletteOffsetMapper(ExportData.DefaultPaletteBank);
			}
		}
	}
}
