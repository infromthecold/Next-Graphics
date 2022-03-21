using NextGraphics.Utils;

using System;
using System.Collections.Generic;
using System.Drawing;

namespace NextGraphics.Models
{
	/// <summary>
	/// Support for tilemaps from image files. This tilemap source fully respects all <see cref="MainModel"/> settings.
	/// </summary>
	public class SourceTilemapImage : SourceTilemap
	{
		#region Initialization & Disposal

		public SourceTilemapImage(string filename, MainModel model) : base(filename, model)
		{
		}

		public SourceTilemapImage(string filename, MainModel model, TilemapData data) : base(filename, model, data)
		{
		}

		#endregion

		#region Overrides

		protected override TilemapData OnLoadDataFromFile(string filename, MainModel model)
		{
			try
			{
				Bitmap image = new Bitmap(filename);

				var tiles = new List<IndexedBitmap>();

				var result = new TilemapData(
					image.Width / model.GridWidth, 
					image.Height / model.GridHeight);

				(int Index, BlockType Type) MatchingTile(IndexedBitmap tile)
				{
					for (int i = 0; i < tiles.Count; i++)
					{
						var existingTile = tiles[i];
						var blockType = tile.RepeatedBlockType(model, existingTile);

						// If we find a match, we can stop looking.
						if (blockType != BlockType.Original)
						{
							return (i, blockType);
						}
					}

					// If we find no match, take block either as transparent or original based on comparisong with itself (this will either return "transparent" or "repeated", in later case we need to use "original").
					var selfType = tile.RepeatedBlockType(model, tile);
					var type = selfType == BlockType.Repeated ? BlockType.Original : selfType;
					return (-1, type);
				}

				(bool FlippedX, bool FlippedY, bool Rotated) TileAttributes(BlockType type)
				{
					var flippedX = false;
					var flippedY = false;
					var rotated = false;

					switch (type)
					{
						case BlockType.FlippedX:
							flippedX = true;
							break;

						case BlockType.FlippedY:
							flippedY = true;
							break;

						case BlockType.FlippedXY:
							flippedX = true;
							flippedY = true;
							break;

						case BlockType.Rotated:
							rotated = true;
							break;

						case BlockType.FlippedXRotated:
							flippedX = true;
							rotated = true;
							break;

						case BlockType.FlippedYRotated:
							flippedY = true;
							rotated = true;
							break;

						case BlockType.FlippedXYRotated:
							flippedX = true;
							flippedY = true;
							rotated = true;
							break;
					}

					return (flippedX, flippedY, rotated);
				}

				// If user previously mapped data, we can be far more accurate by reusing the tiles.
				if (model.ExportData != null)
				{
					for (int i = 0; i < model.ExportData.BlocksCount; i++)
					{
						tiles.Add(model.ExportData.Blocks[i]);
					}
				}

				// Map tiles from source image.
				var y = 0;
				var tileY = 0;
				while (y < image.Height)
				{
					var x = 0;
					var tileX = 0;
					while (x < image.Width)
					{
						// Prepare the tile and copy data from source image.
						var tile = new IndexedBitmap(model.GridWidth, model.GridHeight);
						var rect = new Rectangle(x, y, tile.Width, tile.Height);
						image.CopyRegionIntoBlock(model, rect, tile);

						// Check if we already have the same tile represented (possibly in another variant, flipped or rotated).
						var (tileIndex, tileType) = MatchingTile(tile);

						// If this is a new tile, add it to the list and adjust the index to "point" to newly added tile.
						if (tileIndex < 0)
						{
							tiles.Add(tile);
							tileIndex = tiles.Count - 1;
						}

						// Prepare tile attributes.
						var matchedTile = tiles[tileIndex];
						var (isFlippedX, isFlippedY, isRotated) = TileAttributes(tileType);

						// Set the tile in tilemap structure.
						var tilemapTile = new TilemapData.Tile
						{
							Index = tileIndex,
							PaletteBank = matchedTile.IsAutoBankingSupported ? Math.Max(0, matchedTile.PaletteBank) : 0,
							FlippedX = isFlippedX,
							FlippedY = isFlippedY,
							RotatedClockwise = isRotated
						};
						result.SetTile(tileX, tileY, tilemapTile);

						x += model.GridWidth;
						tileX++;
					}

					y += model.GridHeight;
					tileY++;
				}

				AssignSourceBitmap(image);

				return result;
			}
			catch
			{
				return null;
			}
		}

		#endregion
	}
}
