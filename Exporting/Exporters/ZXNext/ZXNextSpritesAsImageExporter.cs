using NextGraphics.Exporting.Common;
using NextGraphics.Exporting.Exporters.Base;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Exporting.Exporters.ZXNext
{
	public class ZXNextSpritesAsImageExporter : BaseExporter
	{
		#region Overrides

		protected override void OnExport()
		{
			if (Parameters.BlockImageStream == null) return;

			for (int s = 0; s < ExportData.BlocksCount; s++)
			{
				int fx = 0;
				int fy = 0;

				int spriteWidth = 16;
				int spriteHeight = 16;

				byte xChars = (byte)(Model.GridWidth / ExportData.ObjectSize);
				byte yChars = (byte)(Model.GridHeight / ExportData.ObjectSize);

				Bitmap bitmap = new Bitmap(Model.GridWidth, Model.GridHeight, PixelFormat.Format24bppRgb);

				for (int y = 0; y < yChars; y++)
				{
					for (int x = 0; x < xChars; x++)
					{
						for (int yp = 0; yp < spriteHeight; yp++)
						{
							for (int xp = 0; xp < spriteWidth; xp++)
							{
								if (ExportData.Sprites[s].GetTransparent(x, y))
								{
									bitmap.SetPixel(
										xp + (x * spriteWidth), 
										yp + (y * spriteHeight), 
										Model.Palette[Model.Palette.TransparentIndex].ToColor());
								}
								else
								{
									fx = xp;
									fy = yp;

									if (ExportData.Sprites[s].GetFlippedX(x, y))
									{
										fx = spriteWidth - 1 - xp;
									}

									if (ExportData.Sprites[s].GetFlippedY(x, y))
									{
										fy = spriteHeight - 1 - yp;
									}

									var spriteId = ExportData.Sprites[s].GetId(x, y);
									var colourIndex = ExportData.Chars[spriteId].GetPixel(fx, fy);
									var colour = Model.Palette[colourIndex];
									
									bitmap.SetPixel(
										xp + (x * spriteWidth), 
										yp + (y * spriteHeight),
										colour.ToColor());
								}
							}
						}

					}
				}

				// Each block is saved to its own file.
				// Note: original code always saved as PNG, but using proved unreliable for unit tests, so I changed it to respect global image format and use BMP for unit tests. Since the option is there, it makes sense to respect the chosen format anyway
				using (var stream = Parameters.BlockImageStream(s))
				{
					bitmap.Save(stream, Model.ImageFormat.ToSystemImageFormat());
				}
			}
		}

		#endregion
	}
}
