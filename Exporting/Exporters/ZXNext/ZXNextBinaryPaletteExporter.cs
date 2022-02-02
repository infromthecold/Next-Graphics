using NextGraphics.Exporting.Exporters.Base;

using System.IO;
using System.Linq;

namespace NextGraphics.Exporting.Exporters.ZXNext
{
	public class ZXNextBinaryPaletteExporter : BaseExporter
	{
		#region Overrides

		protected override void OnExport()
		{
			using (var writer = new BinaryWriter(Parameters.PaletteStream()))
			{
				writer.Write((byte)Model.Palette.UsedCount);

				for (int j = 0; j < Model.Palette.UsedCount; j++)
				{
					Model.Palette[Model.Palette.StartIndex + j].ToRawBytes(Model.PaletteFormat).ForEach(x =>
					{
						writer.Write(x);
					});
				}
			}
		}

		#endregion
	}
}
