using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Exporting.Exporters
{
	public class BinaryPaletteExporter : BaseExporter
	{
		#region Overrides

		protected override void OnExport()
		{
			using (var writer = new BinaryWriter(Parameters.PaletteStream()))
			{
				writer.Write((byte)Model.Palette.UsedCount);

				for (int j = 0; j < Model.Palette.UsedCount; j++)
				{
					writer.Write((byte)AsPalette8Bit(Model.Palette[Model.Palette.StartIndex + j]));
				}
			}
		}

		#endregion
	}
}
