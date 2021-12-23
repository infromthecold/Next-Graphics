using NextGraphics.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Exporting.Exporters
{
	public class BinaryTilesDataExporter : BaseExporter
	{
		#region Overrides

		protected override void OnExport()
		{
			int binSize = 0;
			byte writeByte = 0;

			using (var binFile = new BinaryWriter(Parameters.BinaryStream()))
			{
				for (int s = 0; s < ExportData.CharactersCount; s++)
				{
					if (ExportData.Chars[s].Transparent == true && Model.OutputType == OutputType.Sprites && !Model.IgnoreTransparentPixels)
					{
						continue;
					}

					for (int y = 0; y < ExportData.Chars[s].Height; y++)
					{
						for (int x = 0; x < ExportData.Chars[s].Width; x++)
						{
							if (Model.FourBit || Model.OutputType == OutputType.Tiles)
							{
								var colourByte = (byte)(ExportData.Chars[s].GetPixel(x, y) & 0x0f);

								if (Parameters.FourBitColourConverter != null)
								{
									colourByte = Parameters.FourBitColourConverter(colourByte);
								}

								if ((x & 1) == 0)
								{
									writeByte = (byte)((colourByte & 0x0f) << 4);
								}
								else
								{
									writeByte = ((byte)(writeByte | (colourByte & 0x0f)));

									binFile.Write(writeByte);
									binSize++;
								}
							}
							else
							{
								binFile.Write(ExportData.Chars[s].GetPixel(x, y));
								binSize++;
							}
						}
					}
				}
			}

			ExportData.BinarySize = binSize;
		}

		#endregion
	}
}
