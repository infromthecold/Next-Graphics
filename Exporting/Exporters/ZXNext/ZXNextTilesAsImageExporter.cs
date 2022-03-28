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
	/// <summary>
	/// Exports all tiles as single image.
	/// </summary>
	public class ZXNextTilesAsImageExporter : BaseExporter
	{
		#region Overrides

		protected override void OnExport()
		{
			int startBlock = 0;
			if (!Model.TilesExportAsImageTransparent)
			{
				startBlock = 1;
			}

			int blocksCount = ExportData.BlocksCount - startBlock;
			int accross = blocksCount < Model.BlocksAcross ? blocksCount : Model.BlocksAcross;
			int down = (int)Math.Round((double)blocksCount / accross);

			Bitmap image = new Bitmap(Model.GridWidth * accross, Model.GridHeight * down, PixelFormat.Format24bppRgb);

			int yPos = 0;
			int xPos = 0;

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

			using (var stream = Parameters.TilesImageStream())
			{
				image.Save(stream, Model.ImageFormat.ToSystemImageFormat());
			}
		}

		#endregion
	}
}
