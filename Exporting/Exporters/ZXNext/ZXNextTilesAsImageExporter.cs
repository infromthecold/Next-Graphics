using NextGraphics.Exporting.Exporters.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Exporting.Exporters.ZXNext
{
	public class ZXNextTilesAsImageExporter : BaseObjectAsImageExporter
	{
		#region Overrides

		protected override int OnTileWidth()
		{
			return 8;
		}

		protected override int OnTileHeight()
		{
			return OnTileWidth();
		}

		protected override int OnTilesPerHeight(int proposed)
		{
			return proposed + 1;
		}

		#endregion
	}
}
