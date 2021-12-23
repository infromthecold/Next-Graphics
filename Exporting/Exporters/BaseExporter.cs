using NextGraphics.Exporting.Common;
using NextGraphics.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Exporting.Exporters
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

		#region Helpers

		protected byte AsPalette8Bit(decimal red, decimal green, decimal blue)
		{
			byte r = (byte)Math.Round(red / (255 / 7));
			byte g = (byte)Math.Round(green / (255 / 7));
			byte b = (byte)Math.Round(blue / (255 / 3));
			return (byte)((r << 5) | (g << 2) | b);
		}

		protected byte AsPalette8Bit(Palette.Colour colour)
		{
			return AsPalette8Bit(colour.Red, colour.Green, colour.Blue);
		}

		#endregion
	}
}
