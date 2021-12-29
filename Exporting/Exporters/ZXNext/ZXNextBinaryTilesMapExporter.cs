using NextGraphics.Exporting.Exporters.Base;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Exporting.Exporters.ZXNext
{
	public class ZXNextBinaryTilesMapExporter : BaseExporter
	{
		#region Overrides

		protected override void OnExport()
		{
			byte paletOffset = 0;

			if (Parameters.ExportCallbacks != null)
			{
				paletOffset = Parameters.ExportCallbacks.OnExportPaletteOffsetMapper(paletOffset);
			}

			using (var mapFile = new BinaryWriter(Parameters.MapStream()))
			{
				byte xChars = (byte)(Model.GridWidth / ExportData.ObjectSize);
				byte yChars = (byte)(Model.GridHeight / ExportData.ObjectSize);
				int outInt = 0;

				for (int b = 0; b < ExportData.BlocksCount; b++)
				{
					for (int y = 0; y < yChars; y++)
					{
						for (int x = 0; x < xChars; x++)
						{
							outInt = ExportData.SortIndexes[ExportData.Sprites[b].GetId(x, y)];
							mapFile.Write((byte)outInt);
							outInt = paletOffset;

							if (ExportData.Sprites[b].GetFlippedX(x, y) == true)
							{
								outInt = outInt | 1 << 3;
							}

							if (ExportData.Sprites[b].GetFlippedY(x, y) == true)
							{
								outInt = outInt | 1 << 2;
							}

							if (ExportData.Sprites[b].GetRotated(x, y) == true)
							{
								outInt = outInt | 1 << 1;
							}

							mapFile.Write((byte)outInt);
						}
					}
				}
			}
		}

		#endregion
	}
}
