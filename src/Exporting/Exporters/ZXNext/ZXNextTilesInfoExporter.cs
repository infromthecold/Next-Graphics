using NextGraphics.Exporting.Exporters.Base;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Exporting.Exporters.ZXNext
{
	public class ZXNextTilesInfoExporter : BaseExporter
	{
		#region Overrides

		protected override void OnExport()
		{
			using (var writer = new BinaryWriter(Parameters.TilesInfoStream()))
			{
				byte xChars = (byte)(Model.GridWidth / ExportData.ObjectSize);
				byte yChars = (byte)(Model.GridHeight / ExportData.ObjectSize);

				int value = 0;

				string filename = Parameters.ExportCallbacks != null ?
					Parameters.ExportCallbacks.OnExportTilesInfoFilename() :
					"(no-filename-provided)";

				writer.Write(filename);
				writer.Write(xChars);
				writer.Write(yChars);
				writer.Write((byte)ExportData.BlocksCount);

				for (int b = 0; b < ExportData.BlocksCount; b++)
				{
					for (int y = 0; y < yChars; y++)
					{
						for (int x = 0; x < xChars; x++)
						{
							value = ExportData.SortIndexes[ExportData.Sprites[b].GetId(x, y)];

							if (ExportData.Sprites[b].GetFlippedX(x, y) == true)
							{
								value = value | 1 << 15;
							}

							if (ExportData.Sprites[b].GetFlippedY(x, y) == true)
							{
								value = value | 1 << 14;
							}

							if (ExportData.Sprites[b].GetRotated(x, y) == true)
							{
								value = value | 1 << 13;
							}

							writer.Write((short)value);
						}
					}
				}
			}
		}

		#endregion
	}
}
