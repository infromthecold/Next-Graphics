using NextGraphics.Exporting.Common;
using NextGraphics.Models;

namespace NextGraphics.Exporting.Exporters.Base
{
	public abstract class BaseExporter
	{
		protected ExportData ExportData { get; private set; }
		protected ExportParameters Parameters { get => ExportData.Parameters; }
		protected MainModel Model { get => ExportData.Model; }

		protected int IdReduction
		{
			get
			{
				if (ExportData.Chars[0].Transparent && Model.OutputType == OutputType.Sprites && !Model.IgnoreTransparentPixels)
				{
					return 1;
				}

				return 0;
			}
		}

		#region Exporting

		public void Export(ExportData data)
		{
			ExportData = data;

			OnExport();
		}

		protected abstract void OnExport();

		#endregion
	}
}
