using NextGraphics.Exporting.Exporters.Base;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Exporting.Exporters.ZXNext
{
	public class ZXNextBinaryTileAttributesExporter : BaseExporter
	{
		#region Overrides

		protected override void OnExport()
		{
			byte paletteOffset =	byte.MaxValue;	// Any value above 16 is invalid, we use MaxValue to detect whether we should ask user.

			if (Parameters.ExportCallbacks != null && Model.PaletteParsingMethod == Models.PaletteParsingMethod.ByPixels)
			{
				paletteOffset = Parameters.ExportCallbacks.OnExportPaletteOffsetMapper(0);
			}

			using (var mapFile = new BinaryWriter(Parameters.TileAttributesStream()))
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

							// If manual banks handling is desired, use given palette offset, otherwise get the offset from previously parsed data.
							outInt = 0;
							if (paletteOffset != byte.MaxValue)
							{
								outInt = paletteOffset;
							}
							else
							{
								var block = ExportData.Blocks[b];
								if (block.IsAutoBankingSupported)
								{
									outInt = block.PaletteBank;
								}
							}
							outInt <<= 4;	// palette index is on bits 7-4

							if (ExportData.Sprites[b].GetFlippedX(x, y))
							{
								outInt |= 1 << 3;
							}

							if (ExportData.Sprites[b].GetFlippedY(x, y))
							{
								outInt |= 1 << 2;
							}

							if (ExportData.Sprites[b].GetRotated(x, y))
							{
								outInt |= 1 << 1;
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
