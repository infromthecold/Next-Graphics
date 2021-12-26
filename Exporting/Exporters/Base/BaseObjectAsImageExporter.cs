using NextGraphics.Exporting.Common;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Exporting.Exporters.Base
{
	public abstract class BaseObjectAsImageExporter : BaseExporter
	{
		#region Overrides

		protected override void OnExport()
		{
			int tileWidth = OnTileWidth();
			int tileHeight = OnTileHeight();

			int across = Model.BlocksAccross;
			int down = OnTilesPerHeight((int)Math.Round((double)ExportData.CharactersCount / across));

			Bitmap image = new Bitmap(tileWidth * across, tileHeight * down, PixelFormat.Format24bppRgb);

			int yPos = 0;
			int xPos = 0;

			int startChar = 0;
			if (!Model.TransparentTiles)
			{
				startChar = 1;
			}
			startChar = OnStartingTile(startChar);

			for (int b = startChar; b < ExportData.CharactersCount; b++)
			{ 
				for (int y = 0; y < tileHeight; y++)
				{
					for (int x = 0; x < tileWidth; x++)
					{
						image.SetPixel(
							x + (xPos * tileWidth),
							y + yPos,
							Model.Palette[ExportData.Chars[b].GetPixel(x, y)].ToColor());
					}
				}

				xPos++;
				if (xPos >= across)
				{
					xPos = 0;
					yPos += tileHeight;
					if (yPos >= image.Height)
					{
						break;
					}
				}
			}

			using (var stream = Parameters.TilesImageStream())
			{
				image.Save(stream, Model.ImageFormat.ToSystemImageFormat());
			}
		}

		#endregion

		#region Subclass

		protected abstract int OnTileWidth();
		protected abstract int OnTileHeight();

		protected virtual int OnTilesPerHeight(int proposed)
		{
			return proposed;
		}

		protected virtual int OnStartingTile(int proposed)
		{
			return proposed;
		}

		#endregion
	}
}
