using NextGraphics.Exporting.Common;
using NextGraphics.Models;
using NextGraphics.Utils;

using System;
using System.Drawing;
using System.IO;

namespace NextGraphics.Exporting.Remapping
{
	/// <summary>
	/// Remaps blocks to make the data ready for export.
	/// </summary>
	/// <remarks>
	/// Note: this class is designed so that a new instance is created for each run. Calling <see cref="Remap"/> on previous instances will result in wrong results or even crashes. Such implementation is simpler and more DRY since we don't have to reset properties and fields to defaults - instead, default value is assigned at the place of definition which is again simpler and quicker than adding a property in one place and then searching for "reset" method where the default value is set.
	/// </remarks>
	public class Remapper
	{
		private ExportData Data { get; }
		private MainModel Model { get => Data.Model; }
		private RemapCallbacks Callbacks { get => Data.Parameters.RemapCallbacks; }

		private int objectSize = 0;
		private int maxObjectsCount = 0;
		private int objectsPerGridX = 0;
		private int objectsPerGridY = 0;
		private int paletteOffset = 0;

		private int objectGridX = 0;
		private int objectGridY = 0;

		private int outXBlock = 0;
		private int outYBlock = 0;
		private int outBlock = 0;
		private int outChar = 0;

		private bool allImagesProcessed = true;

		#region Initialization & disposal

		public Remapper(ExportData data)
		{
			Data = data;
		}

		#endregion

		#region Remapping

		public void Remap()
		{
			try
			{
				Callbacks?.OnRemapStarted();
				Callbacks?.OnRemapDebug($"Starting remap{Environment.NewLine}");

				Data.Clear();
				Data.BlockSize = CalculateBlockSize();
				Data.ImageOffset = CalculateImageOffset();

				// Ensure all sources are fresh in case user made some changes after we loaded them.
				Data.Model.ReloadSources();

				objectSize = Data.ObjectSize;
				maxObjectsCount = Data.Model.OutputType == OutputType.Sprites ? 128 : ExportData.MAX_OBJECTS - 1;
				objectsPerGridX = (Data.Model.GridWidth / objectSize);
				objectsPerGridY = (Data.Model.GridHeight / objectSize);

				if (Data.Model.OutputType == OutputType.Tiles)
				{
					Callbacks?.OnRemapDebug($"Preparing data for tiles export{Environment.NewLine}");

					if (Data.Model.TransparentFirst)
					{
						MakeFirstBlockTransparent();
						MakeFirstTileTransparent();
					}
				}

				Callbacks?.OnRemapDebug($"blocks={outBlock}, chars={outChar}{Environment.NewLine}");
				Callbacks?.OnRemapDebug($"Reading images{Environment.NewLine}");

				void LogSource(string type, int index, ISourceFile source)
				{
					Callbacks?.OnRemapDebug($"{Environment.NewLine}---------------------------{Environment.NewLine}Handling {type} {index} ({Path.GetFileName(source.Filename)}){Environment.NewLine}");
				}

				// Remapping is only needed for images and...
				Data.Model.ForEachSourceImage((image, idx) =>
				{
					LogSource("image", idx, image);

					if (!image.IsDataValid)
					{
						Callbacks?.OnRemapDebug($"Image is invalid, ignoring{Environment.NewLine}");
						return;
					}

					ParseImage(image.Filename, image.Data);
				});

				// ...tilemaps that are based off images.
				Data.Model.ForEachSourceTilemap((tilemap, idx) =>
				{
					if (!tilemap.IsSourceImage) return;

					LogSource("tilemap", idx, tilemap);

					if (!tilemap.IsDataValid)
					{
						Callbacks?.OnRemapDebug($"Tilemap is invalid, ignoring{Environment.NewLine}");
						return;
					}

					ParseImage(tilemap.Filename, tilemap.SourceBitmap);
				});

				int transparentCharactersCount = 0;

				if (Data.Model.TransparentFirst && Model.OutputType == OutputType.Tiles)
				{
					int sortedIndex = 0;

					// On first pass we only handle transparent blocks (only 1)
					PrepareCharacters(
						(index, transparent) => transparent && sortedIndex == 0 ? sortedIndex : -1, () => 
						{
							transparentCharactersCount++;
							sortedIndex++;
						});

					// On second pass we only handle non-transparent blocks.
					PrepareCharacters(
						(index, transparent) => transparent ? -1 : sortedIndex, () => 
						{
							sortedIndex++;
						});
				}
				else
				{
					// In this case we have straightforward loop.
					PrepareCharacters(
						(index, transparent) => index,
						() => { });
				}

				outXBlock = 0;
				outYBlock = 0;
				var frame = new Rectangle();

				for (int b = 0; b < outBlock; b++)
				{
					frame.X = outXBlock * Model.GridWidth;
					frame.Y = outYBlock * Model.GridHeight;
					frame.Width = Model.GridWidth;
					frame.Height = Model.GridHeight;

					CopyCharactersToBlocksBitmap(frame, b);

					Callbacks?.OnRemapDisplayBlocksCount(b);
					if (Model.OutputType == OutputType.Sprites)
					{
						SetSpriteCollisions(b);
					}

					outXBlock++;
					if (outXBlock >= Model.BlocksAcross)
					{
						outXBlock = 0;
						outYBlock++;
					}
				}

				Data.CharactersCount = outChar;
				Data.BlocksCount = outBlock;

				Callbacks?.OnRemapDebug("Remap completed");

				if (outChar > maxObjectsCount)
				{
					Callbacks?.OnRemapWarning("Too many characters in your tiles");
				}

				Data.IsRemapped = true;

				Callbacks?.OnRemapDisplayCharactersCount(outChar, transparentCharactersCount);
			}
			finally
			{
				// Before completed, we should reload all tilemaps so that they will use remapped blocks.
				Model.ForEachSourceTilemap((tilemap, idx) => tilemap.Reload());

				Callbacks?.OnRemapCompleted(allImagesProcessed);
			}
		}

		private void ParseImage(string filename, Bitmap image)
		{
			var sourceRect = new Rectangle();
			var yCount = image.Height / Data.Model.GridHeight;
			var xCount = image.Width / Data.Model.GridWidth;

			if (Data.Model.OutputType == OutputType.Tiles)
			{
				CheckImageDimensions(filename, image);
			}

			for (int by = 0; by < yCount; by++)
			{
				for (int bx = 0; bx < xCount; bx++)
				{
					Callbacks?.OnRemapDebug($"({bx},{by}) ");

					sourceRect.X = bx * Data.Model.GridWidth;
					sourceRect.Y = by * Data.Model.GridHeight;
					sourceRect.Width = Data.Model.GridWidth;
					sourceRect.Height = Data.Model.GridHeight;

					if (outBlock > ExportData.MAX_BLOCKS)
					{
						Callbacks?.OnRemapWarning($"Too many blocks/sprites{Environment.NewLine}");
						Callbacks?.OnRemapUpdated();
						Callbacks?.OnRemapCompleted(false);
						return;
					}

					if (Data.Blocks[outBlock] == null)
					{
						Data.Blocks[outBlock] = new IndexedBitmap(sourceRect.Width, sourceRect.Height);
					}

					if (Data.Sprites[outBlock] == null)
					{
						Data.Sprites[outBlock] = new SpriteInfo(objectsPerGridX, objectsPerGridY);
					}

					image.CopyRegionIntoBlock(
						Data.Model,
						sourceRect,
						Data.Blocks[outBlock],
						Data.Sprites[outBlock]);

					if (Data.Model.IsFourBitData)
					{
						Data.Blocks[outBlock].RemapTo4Bit(
							Data.Model.Palette,
							Data.Model.PaletteParsingMethod,
							Data.Model.GridWidth,
							Data.Model.GridHeight,
							objectSize);
					}

					if (Data.Blocks[outBlock].IsTransparent(Data.Model.Palette.TransparentIndex))
					{
						Callbacks?.OnRemapDebug($"T{Environment.NewLine}");

						// We only allow first block to be transparent. And we manually make it transparent at the start of remap if needed.
						if (outBlock > 0)
						{
							continue;
						}
					}

					for (int cy = 0; cy < objectsPerGridY; cy++)
					{
						for (int cx = 0; cx < objectsPerGridX; cx++)
						{
							if (Data.Model.IsFourBitData)
							{
								switch (Data.Model.PaletteParsingMethod)
								{
									case PaletteParsingMethod.ByPixels:
										paletteOffset = Data.Blocks[outBlock].GetPixel(cx * objectSize, cy * objectSize) & 0x0f0;
										break;

									case PaletteParsingMethod.ByObjects:
										paletteOffset = Data.Blocks[outBlock].PaletteBank;
										break;
								}
							}

							PrepareSpriteData(cx, cy);
						}
					}

					Callbacks?.OnRemapDisplayBlocksCount(outBlock);
					Callbacks?.OnRemapDebug($"/ blocks={outBlock}, chars={outChar}");

					switch (Model.OutputType)
					{
						case OutputType.Tiles:
							if (!IsSpriteDuplicated(outBlock, objectsPerGridX, objectsPerGridY))
							{
								outBlock++;
							}
							break;

						default:
							outBlock++;
							break;
					}

					Callbacks?.OnRemapDebug(Environment.NewLine);
				}

				Callbacks?.OnRemapDebug(Environment.NewLine);
				Callbacks?.OnRemapUpdated();
			}

			// After we establish all blocks, we should update tiles in previously parsed tilemaps so that palette banks will match. This is only needed when auto-banking is enabled.
			if (Data.Model.IsFourBitPaletteAutoBankingEnabled)
			{
				Model.ForEachSourceTilemap((tilemap, index) =>
				{
					tilemap.UpdateTiles(Data);
				});
			}
		}

		private void PrepareCharacters(Func<int, bool, int> indexProvider, Action afterCharHandler)
		{
			for (int i = 0; i < outChar; i++)
			{
				var tempData = Data.TempData[i];

				var isTransparent = tempData.IsTransparent(Data.Model.Palette.TransparentIndex);
				var indexForLoop = indexProvider(i, isTransparent);
				if (indexForLoop < 0) continue;

				if (Data.Chars[indexForLoop] != null)
				{
					Data.Chars[indexForLoop].Dispose();
					Data.Chars[indexForLoop] = null;
				}

				var charBitmap = new IndexedBitmap(objectSize, objectSize)
				{
					Transparent = tempData.Transparent
				};

				Data.Chars[indexForLoop] = charBitmap;
				Data.Chars[indexForLoop].CopyFrom(tempData, 0, 0);

				Data.SortIndexes[i] = indexForLoop;
				RequestCharacterDisplay(indexForLoop);

				afterCharHandler();
			}
		}

		private void PrepareSpriteData(int x, int y)
		{
			for (short c = 0; c < outChar; c++)
			{
				var repeatResult = RepeatedCharType(c, x * objectSize, y * objectSize);
				var isTransparent = Data.TempData[c].IsTransparent(Data.Model.Palette.TransparentIndex);

				switch (repeatResult)
				{
					case BlockType.Repeated:
						Callbacks?.OnRemapDebug($"={c} ");
						Data.Sprites[outBlock].SetData(x, y, true, false, false, false, false, c, (short)paletteOffset, isTransparent);
						return;

					case BlockType.FlippedX:
						Callbacks?.OnRemapDebug($"={c}X ");
						Data.Sprites[outBlock].SetData(x, y, true, true, false, false, false, c, (short)paletteOffset, isTransparent);
						return;

					case BlockType.FlippedY:
						Callbacks?.OnRemapDebug($"={c}Y ");
						Data.Sprites[outBlock].SetData(x, y, true, false, true, false, false, c, (short)paletteOffset, isTransparent);
						return;

					case BlockType.FlippedXY:
						Callbacks?.OnRemapDebug($"={c}XY ");
						Data.Sprites[outBlock].SetData(x, y, true, true, true, false, false, c, (short)paletteOffset, isTransparent);
						return;

					case BlockType.Rotated:
						Callbacks?.OnRemapDebug($"={c}R ");
						Data.Sprites[outBlock].SetData(x, y, true, false, false, true, false, c, (short)paletteOffset, isTransparent);
						return;

					case BlockType.FlippedXRotated:
						Callbacks?.OnRemapDebug($"={c}RX ");
						Data.Sprites[outBlock].SetData(x, y, true, true, false, true, false, c, (short)paletteOffset, isTransparent);
						return;

					case BlockType.FlippedYRotated:
						Callbacks?.OnRemapDebug($"={c}RY ");
						Data.Sprites[outBlock].SetData(x, y, true, false, true, true, false, c, (short)paletteOffset, isTransparent);
						return;

					case BlockType.FlippedXYRotated:
						Callbacks?.OnRemapDebug($"={c}RXY ");
						Data.Sprites[outBlock].SetData(x, y, true, true, true, true, false, c, (short)paletteOffset, isTransparent);
						return;

					case BlockType.Transparent:
						Callbacks?.OnRemapDebug($"T ");
						Data.Sprites[outBlock].SetData(x, y, false, false, false, false, true, c, (short)paletteOffset, isTransparent);
						return;
				}
			}

			if (Data.TempData[outChar] == null)
			{
				Data.TempData[outChar] = new IndexedBitmap(objectSize, objectSize);
			}

			bool isBlockTransparent = Data.Blocks[outBlock].IsTransparent(Data.Model.Palette.TransparentIndex);

			Data.TempData[outChar].CopyFrom(Data.Blocks[outBlock], x * objectSize, y * objectSize);
			Data.TempData[outChar].Transparent = isBlockTransparent;

			bool isSpriteTransparent = Data.TempData[outChar].IsTransparent(Data.Model.Palette.TransparentIndex);

			Callbacks?.OnRemapDebug($"{outBlock}*  ");
			Data.Sprites[outBlock].SetData(
				x, y,
				false, false, false, false,
				isBlockTransparent,
				(short)outChar,
				(short)paletteOffset,
				isSpriteTransparent);

			if (outChar <= maxObjectsCount)
			{
				outChar++;
			}
		}

		private void CheckImageDimensions(string filename, Bitmap image)
		{
			var isWidthDivisible = (image.Width % Data.Model.GridWidth) == 0;
			var isHeightDivisible = (image.Height % Data.Model.GridHeight) == 0;

			if (!isWidthDivisible && !isHeightDivisible)
			{
				Callbacks?.OnRemapWarning($"The image {Path.GetFileName(filename)} ({image.Width}x{image.Height}) is not divisible by the width and height of your tiles ({Data.Model.GridWidth}x{Data.Model.GridHeight}), which will corrupt the output");
				allImagesProcessed = false;
			}
			else if (!isWidthDivisible)
			{
				Callbacks?.OnRemapWarning($"The image {Path.GetFileName(filename)} ({image.Width}x{image.Height}) is not divisible by the width of your tiles ({Data.Model.GridWidth}), which will corrupt the output");
				allImagesProcessed = false;
			}
			else if (!isHeightDivisible)
			{
				Callbacks?.OnRemapWarning($"The image {Path.GetFileName(filename)} ({image.Width}x{image.Height}) is not divisible by the height of your tiles ({Data.Model.GridHeight}), which will corrupt the output");
				allImagesProcessed = false;
			}
		}

		private void MakeFirstBlockTransparent()
		{
			if (Data.Blocks[0] == null)
			{
				Callbacks?.OnRemapDebug($"Making first block transparent{Environment.NewLine}");

				Data.Blocks[0] = new IndexedBitmap(Data.Model.GridWidth, Data.Model.GridHeight);

				for (int y = 0; y < Data.Model.GridHeight; y++)
				{
					for (int x = 0; x < Data.Model.GridWidth; x++)
					{
						Data.Blocks[0].SetPixel(x, y, (short)Data.Model.Palette.TransparentIndex);
					}
				}

				Callbacks?.OnRemapDisplayBlock(new Point(0, 0), Data.Blocks[0]);
			}

			outXBlock = 1;
			outBlock = 1;
			if (Data.Sprites[0] == null)
			{
				Callbacks?.OnRemapDebug($"Making first sprite transparent{Environment.NewLine}");

				Data.Sprites[0] = new SpriteInfo(objectsPerGridX, objectsPerGridY);
			}
		}

		private void MakeFirstTileTransparent()
		{
			if (Data.TempData[0] == null)
			{
				Callbacks?.OnRemapDebug($"Making first tile transparent{Environment.NewLine}");

				Data.TempData[0] = new IndexedBitmap(objectSize, objectSize);

				for (int y = 0; y < 8; y++)
				{
					for (int x = 0; x < 8; x++)
					{
						Data.TempData[0].SetPixel(x, y, (short)Data.Model.Palette.TransparentIndex);
					}
				}
			}

			outChar = 1;
		}

		#endregion

		#region Helpers

		private void CopyCharactersToBlocksBitmap(Rectangle destRegion, int currentBlock)
		{
			var isDebugEnabled = Callbacks?.OnRemapShowCharacterDebugData() ?? false;
			var destBitmap = Data.Model.BlocksBitmap;

			for (int chr = 0; chr < (Model.GridWidth / objectSize) * (Model.GridHeight / objectSize); chr++)
			{
				bool flipX = Data.Sprites[currentBlock].Infos[chr].FlippedX;
				bool flipY = Data.Sprites[currentBlock].Infos[chr].FlippedY;
				bool rotate = Data.Sprites[currentBlock].Infos[chr].Rotated;
				int id = Data.Sprites[currentBlock].Infos[chr].OriginalID;
				int sortedId = Data.SortIndexes[id];
				Bitmap tempBitmap = new Bitmap(objectSize, objectSize);
				RotateFlipType flips = RotateFlipType.RotateNoneFlipNone;

				string debugString = "";

				for (int y = 0; y < objectSize; y++)
				{
					for (int x = 0; x < objectSize; x++)
					{
						var pixel = Data.Chars[sortedId].GetPixel(x, y);
						var pixelColor = Data.Model.Palette[pixel].ToColor();
						tempBitmap.SetPixel(x, y, pixelColor);
					}
				}

				if (flipX == true && flipY == false && rotate == false)
				{
					flips = RotateFlipType.RotateNoneFlipX;
					debugString = "X";
				}
				else if (flipX == false && flipY == true && rotate == false)
				{
					flips = RotateFlipType.RotateNoneFlipY;
					debugString = "Y";
				}
				else if (flipX == true && flipY == true && rotate == false)
				{
					flips = RotateFlipType.RotateNoneFlipXY;
					debugString = "XY";
				}
				else if (flipX == false && flipY == false && rotate == true)
				{
					flips = RotateFlipType.Rotate90FlipNone;
					debugString = "R";
				}
				else if (flipX == true && flipY == false && rotate == true)
				{
					flips = RotateFlipType.Rotate90FlipX;
					debugString = "RX";
				}
				else if (flipX == false && flipY == true && rotate == true)
				{
					flips = RotateFlipType.Rotate90FlipY;
					debugString = "RY";
				}
				else if (flipX == true && flipY == true && rotate == true)
				{
					flips = RotateFlipType.Rotate90FlipXY;
					debugString = "RXY";
				}

				tempBitmap.RotateFlip(flips);

				if (Data.Sprites[currentBlock].Infos[chr].Transparent == false)
				{
					for (int y = 0; y < objectSize; y++)
					{
						for (int x = 0; x < objectSize; x++)
						{
							var pixelColor = tempBitmap.GetPixel(x, y);

							destBitmap.SetPixel(
								destRegion.X + (Data.Sprites[currentBlock].Infos[chr].Position.X * objectSize) + x, 
								destRegion.Y + (Data.Sprites[currentBlock].Infos[chr].Position.Y * objectSize) + y, 
								pixelColor);
						}
					}

					if (isDebugEnabled)
					{
						Graphics.FromImage(destBitmap).DrawString(
							debugString,
							new Font("Areial", 7.0f, FontStyle.Bold),
							new SolidBrush(Color.White),
							new Point(
								destRegion.X + (Data.Sprites[currentBlock].Infos[chr].Position.X * objectSize) - 2,
								destRegion.Y + (Data.Sprites[currentBlock].Infos[chr].Position.Y * objectSize) - 2));
					}
				}
			}
		}

		private void SetSpriteCollisions(int s)
		{
			for (int y = 0; y < Data.Blocks[s].Height; y++)
			{
				for (int x = 0; x < Data.Blocks[s].Width; x++)
				{
					if (Data.Blocks[s].GetPixel(x, y) != (short)Model.Palette.TransparentIndex)
					{
						Data.Sprites[s].Top = y;
						goto foundTop;
					}
				}
			}

		foundTop:
			for (int y = 0; y < Data.Blocks[s].Height; y++)
			{
				for (int x = 0; x < Data.Blocks[s].Width; x++)
				{
					if (Data.Blocks[s].GetPixel(x, (Data.Blocks[s].Height - 1) - y) != (short)Model.Palette.TransparentIndex)
					{
						Data.Sprites[s].Bottom = (Data.Blocks[s].Height - 1) - y;
						goto foundBottom;
					}
				}
			}

		foundBottom:
			for (int x = 0; x < Data.Blocks[s].Width; x++)
			{
				for (int y = 0; y < Data.Blocks[s].Height; y++)
				{
					if (Data.Blocks[s].GetPixel(x, y) != (short)Model.Palette.TransparentIndex)
					{
						Data.Sprites[s].Left = x;
						goto foundLeft;
					}
				}
			}

		foundLeft:
			for (int x = 0; x < Data.Blocks[s].Width; x++)
			{
				for (int y = 0; y < Data.Blocks[s].Height; y++)
				{
					if (Data.Blocks[s].GetPixel((Data.Blocks[s].Width - 1) - x, y) != (short)Model.Palette.TransparentIndex)
					{
						Data.Sprites[s].Right = (Data.Blocks[s].Width - 1) - x;
						goto foundRight;
					}
				}
			}

		foundRight:;
		}

		private void RequestCharacterDisplay(int index)
		{
			var position = new Point
			{
				X = objectGridX * objectSize,
				Y = objectGridY * objectSize,
			};

			Callbacks?.OnRemapDisplayChar(position, Data.Chars[index]);

			int width = Model.CharsBitmap.Width;

			objectGridX++;
			if (objectGridX >= width / objectSize)
			{
				objectGridX = 0;
				objectGridY++;
			}
		}

		private bool IsSpriteDuplicated(int upToCount, int xCount, int yCount)
		{
			for (int i = 0; i < upToCount; i++)
			{
				for (int yChar = 0; yChar < yCount; yChar++)
				{
					for (int xChar = 0; xChar < xCount; xChar++)
					{
						if (Data.Sprites[upToCount].GetId(xChar, yChar) == Data.Sprites[i].GetId(xChar, yChar))
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		private int CalculateBlockSize()
		{
			if (Data.Model.OutputType == OutputType.Sprites)
			{
				if (!Data.Model.SpritesFourBit)
				{
					// 8-bit sprites use 1 byte per pixel and are 16x16 pixels.
					return 256;
				}
				else
				{
					// 4-bit sprites use 4-bits per pixel and are 16x16 pixels.
					return 128;
				}
			}
			else
			{
				// Tilemap tiles always use 4-bits per pixel and are 8x8 pixels.
				return 32;
			}
		}

		private Point CalculateImageOffset()
		{
			switch (Data.Model.CenterPosition)
			{
				case 0:
					return new Point(
						-(Data.Model.GridWidth / 2),
						-(Data.Model.GridHeight / 2)
					);
				case 1:
					return new Point(
						0,
						-(Data.Model.GridHeight / 2)
					);
				case 2:
					return new Point(
						(Data.Model.GridWidth / 2),
						-(Data.Model.GridHeight / 2)
					);
				case 3:
					return new Point(
						-(Data.Model.GridWidth / 2),
						0
					);
				case 4:
					return new Point(
						0,
						0
					);
				case 5:
					return new Point(
						(Data.Model.GridWidth / 2),
						0
					);
				case 6:
					return new Point(
						-(Data.Model.GridWidth / 2),
						(Data.Model.GridHeight / 2)
					);
				case 7:
					return new Point(
						0,
						(Data.Model.GridHeight / 2)
					);
				default:
					return new Point(
						(Data.Model.GridWidth / 2),
						(Data.Model.GridHeight / 2)
					);
			}
		}

		private BlockType RepeatedCharType(int character, int xOffset, int yOffset)
		{
			var compareBlock = Data.Blocks[outBlock];
			var comparedCharacter = Data.TempData[character];
			return compareBlock.RepeatedBlockType(Data.Model, comparedCharacter, xOffset, yOffset);
		}

		#endregion
	}
}
