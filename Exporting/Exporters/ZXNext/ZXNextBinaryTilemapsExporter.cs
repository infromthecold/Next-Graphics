using NextGraphics.Exporting.Exporters.Base;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Exporting.Exporters.ZXNext
{
	/// <summary>
	/// Similarly named to <see cref="ZXNextBinaryTileAttributesExporter"/>, but the difference is important: this class exports each tilemap definition, while "tiles map" a list of all tiles with each containing attributes and index. The two can be complemented, for example, enabling index only export for tilemap definition (aka the result of this class ;) and then use "tiles map" to fetch attributes. Uses half the space for storing tilemaps definition, but comes on expense of not being able to set different attributes for different occurences of the same tile.
	/// </summary>
	public class ZXNextBinaryTilemapsExporter : BaseExporter
	{
		#region Overrides

		protected override void OnExport()
		{
			var tilemaps = Model.SourceTilemaps().ToList();

			for (int i = 0; i < tilemaps.Count; i++)
			{
				using (var file = new BinaryWriter(Parameters.TilemapsStream(i)))
				{
					var tilemap = tilemaps[i];

					for (int y = 0; y < tilemap.Data.Height; y++)
					{
						for (int x = 0; x < tilemap.Data.Width; x++)
						{
							var tile = tilemap.Data.Tiles[y, x];
							var index = (byte)tile.Index;
							var attributes = tile.ZXNextTileAttributes();

							switch (Model.TilemapExportType)
							{
								case Models.TilemapExportType.AttributesIndexAsWord:
								case Models.TilemapExportType.AttributesIndexAsTwoBytes:
									file.Write(index);
									file.Write(attributes);
									break;
								case Models.TilemapExportType.IndexOnly:
									file.Write(index);
									break;
							}
						}
					}
				}
			}
		}

		#endregion
	}
}
