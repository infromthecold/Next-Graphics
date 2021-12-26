using NextGraphics.Exporting.Common;
using NextGraphics.Exporting.Exporters.Base;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Exporting.Exporters.ZXNext
{
	public class ZXNextBlocksAsImageExporter : BaseExporter
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

			using (var stream = Parameters.BlocksImageStream())
			{
				image.Save(stream, Model.ImageFormat.ToSystemImageFormat());
			}
		}

		#endregion
	}
}
