using NextGraphics.Exporting.Exporters.Base;

using System.IO;

namespace NextGraphics.Exporting.Exporters.ZXNext
{
	public class ZXNextBinaryTileAttributesExporter : BaseExporter
	{
		#region Overrides

		protected override void OnExport()
		{
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

							// If auto banks handling is enabled, get the offset from previously parsed data, otherwise use palette offset from user (which we asked for previously).
							outInt = 0;
							if (Model.IsFourBitPaletteAutoBankingEnabled)
							{
								var block = ExportData.Blocks[b];
								if (block.IsAutoBankingSupported)
								{
									outInt = block.PaletteBank;
								}
							}
							else
							{
								outInt = ExportData.DefaultPaletteBank;
							}
							outInt <<= 4;	// palette bank offset is on bits 7-4

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
