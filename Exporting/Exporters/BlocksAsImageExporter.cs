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
	public class BlocksAsImageExporter : BaseExporter
	{
		#region Overrides

		protected override void OnExport()
		{
			int accross = Model.BlocksAccross;
			int down = (int)Math.Round((double)ExportData.BlocksCount / accross) + 1;

			Bitmap image = new Bitmap(Model.GridWidth * accross, Model.GridHeight * down, PixelFormat.Format24bppRgb);

			int yPos = 0;
			int xPos = 0;

			int startBlock = 0;
			if (!Model.TransparentBlocks)
			{
				startBlock = 1;
			}

			for (int b = startBlock; b < ExportData.BlocksCount; b++)
			{
				for (int y = 0; y < Model.GridHeight; y++)
				{
					for (int x = 0; x < Model.GridWidth; x++)
					{
						image.SetPixel(
							x + (xPos * Model.GridWidth), 
							y + yPos,
							Model.Palette[ExportData.Blocks[b].GetPixel(x, y)].ToColor());
					}
				}

				xPos++;
				if (xPos >= accross)
				{
					xPos = 0;
					yPos += Model.GridHeight;
				}
			}

			switch (Model.ImageFormat)
			{
				case Models.ImageFormat.BMP:
					image.Save(Parameters.BlocksImageStream(), System.Drawing.Imaging.ImageFormat.Bmp);
					break;
				case Models.ImageFormat.PNG:
					image.Save(Parameters.BlocksImageStream(), System.Drawing.Imaging.ImageFormat.Png);
					break;
				case Models.ImageFormat.JPG:
					image.Save(Parameters.BlocksImageStream(), System.Drawing.Imaging.ImageFormat.Jpeg);
					break;
			}
		}

		#endregion
	}
}
