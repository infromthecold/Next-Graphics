﻿using NextGraphics.Exporting.Exporters.Base;
using NextGraphics.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Exporting.Exporters.ZXNext
{
	public class ZXNextBinaryDataExporter : BaseExporter
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
							if (Model.SpritesFourBit || Model.OutputType == OutputType.Tiles)
							{
								var colourByte = (byte)(ExportData.Chars[s].GetPixel(x, y) & 0x0f);

								if (Parameters.ExportCallbacks != null)
								{
									colourByte = Parameters.ExportCallbacks.OnExportFourBitColourConverter(colourByte);
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
								var colourByte = (byte)ExportData.Chars[s].GetPixel(x, y);
								binFile.Write(colourByte);
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
