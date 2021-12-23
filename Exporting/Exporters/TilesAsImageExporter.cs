using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Exporting.Exporters
{
	public class TilesAsImageExporter : BaseExporter
	{
		#region Overrides

		protected override void OnExport()
		{
			int across = Model.BlocksAccross;
			int down = (int)Math.Round((double)ExportData.BlocksCount / across) + 1;

			Bitmap image = new Bitmap(8 * across, 8 * down, PixelFormat.Format24bppRgb);

			int yPos = 0;
			int xPos = 0;

			int startChar = 0;
			if (!Model.TransparentTiles)
			{
				startChar = 1;
			}

			for (int b = startChar; b < ExportData.CharactersCount; b++)
			{
				for (int y = 0; y < 8; y++)
				{
					for (int x = 0; x < 8; x++)
					{
						image.SetPixel(
							x + (xPos * 8),
							y + yPos,
							Model.Palette[ExportData.Chars[b].GetPixel(x, y)].ToColor());
					}
				}

				xPos++;
				if (xPos >= across)
				{
					xPos = 0;
					yPos += 8;
					if (yPos >= image.Height)
					{
						break;
					}
				}
			}

			switch (Model.ImageFormat)
			{
				case Models.ImageFormat.BMP:
					image.Save(Parameters.TilesImageStream(), System.Drawing.Imaging.ImageFormat.Bmp);
					break;
				case Models.ImageFormat.PNG:
					image.Save(Parameters.TilesImageStream(), System.Drawing.Imaging.ImageFormat.Png);
					break;
				case Models.ImageFormat.JPG:
					image.Save(Parameters.TilesImageStream(), System.Drawing.Imaging.ImageFormat.Jpeg);
					break;
			}
		}
	}
		
	#endregion
}
