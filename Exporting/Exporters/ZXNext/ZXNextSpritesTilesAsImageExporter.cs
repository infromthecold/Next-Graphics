using NextGraphics.Exporting.Exporters.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Exporting.Exporters.ZXNext
{
	public class ZXNextSpritesTilesAsImageExporter : BaseObjectAsImageExporter
	{
		#region Overrides

		protected override int OnTileWidth()
		{
			return 16;
		}

		protected override int OnTileHeight()
		{
			return OnTileWidth();
		}

		protected override int OnStartingTile(int proposed)
		{
			// Sprites always start with first tile.
			return 0;
		}

		#endregion
	}
}
