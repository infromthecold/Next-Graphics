using NextGraphics.Exporting.Common;
using NextGraphics.Exporting.Exporters.Base;
using NextGraphics.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Exporting.Exporters.ZXNext
{
	public class ZXNextBinaryTilesExporter : BaseExporter
	{
		#region Overrides

		protected override void OnExport()
		{
			using (var writer = new BinaryWriter(Parameters.TilesStream()))
			{
				for (int s = 0; s < ExportData.BlocksCount; s++)
				{
					// check to see if any square is transparent
					if (Model.OutputType == OutputType.Sprites)
					{
						var spriteCount = ExportData.Sprites[s].Size;

						for (int y = 0; y < ExportData.Sprites[s].Height; y++)
						{
							for (int x = 0; x < ExportData.Sprites[s].Width; x++)
							{
								if (ExportData.Sprites[s].GetTransparent(x, y))
								{
									spriteCount--;
								}
							}
						}

						writer.Write(spriteCount);
					}

					for (int y = 0; y < ExportData.Sprites[s].Height; y++)
					{
						for (int x = 0; x < ExportData.Sprites[s].Width; x++)
						{
							if (Model.OutputType == OutputType.Sprites)
							{
								if (ExportData.Sprites[s].GetTransparent(x, y)) continue;

								writer.Write(ExportData.Sprites[s].OffsetX + (ExportData.ImageOffset.X + (ExportData.Sprites[s].GetXPos(x, y) * ExportData.ObjectSize)));
								writer.Write(ExportData.Sprites[s].OffsetY + (ExportData.ImageOffset.Y + (ExportData.Sprites[s].GetYpos(x, y) * ExportData.ObjectSize)));

								byte writeByte = 0;

								if (ExportData.Sprites[s].GetPaletteOffset(x, y) != 0)
								{
									writeByte = (byte)ExportData.Sprites[s].GetPaletteOffset(x, y);
								}

								if (ExportData.Sprites[s].GetFlippedX(x, y) == true)
								{
									writeByte = (byte)(writeByte | 8);
								}

								if (ExportData.Sprites[s].GetFlippedY(x, y) == true)
								{
									writeByte = (byte)(writeByte | 4);
								}

								if (ExportData.Sprites[s].GetRotated(x, y) == true)
								{
									writeByte = (byte)(writeByte | 2);
								}

								writer.Write(writeByte);

								writeByte = 0;
								if (Model.SpritesFourBit)
								{
									writeByte = (byte)(writeByte | 128);
									if (((ExportData.Sprites[s].GetId(x, y) - IdReduction) & 1) == 1)
									{
										writeByte = (byte)(writeByte | 64);
									}
									writer.Write(writeByte);
									writer.Write((ExportData.Sprites[s].GetId(x, y) - IdReduction) / 2);
								}
								else
								{
									writer.Write(writeByte);
									writer.Write(ExportData.Sprites[s].GetId(x, y));
								}
							}
							else
							{
								var writeByte = (byte)ExportData.SortIndexes[ExportData.Sprites[s].GetId(x, y)];
								writer.Write(writeByte);

								writeByte = 0;
								if (ExportData.Sprites[s].GetPaletteOffset(x, y) != 0)
								{
									writeByte = (byte)ExportData.Sprites[s].GetPaletteOffset(x, y);
								}

								if (ExportData.Sprites[s].GetFlippedX(x, y) == true)
								{
									writeByte = (byte)(writeByte | 8);
								}

								if (ExportData.Sprites[s].GetFlippedY(x, y) == true)
								{
									writeByte = (byte)(writeByte | 4);
								}

								if (ExportData.Sprites[s].GetRotated(x, y) == true)
								{
									writeByte = (byte)(writeByte | 2);
								}

								writer.Write(writeByte);
							}
						}
					}
				}
			}
		}

		#endregion
	}
}
