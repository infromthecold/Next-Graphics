using NextGraphics.Exporting.Common;
using NextGraphics.Models;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Exporting.Remapping
{
	/// <summary>
	/// Remaps blocks to make the data ready for export.
	/// </summary>
	public class Remapper
	{
		private ExportData Data { get; }
		private RemapCallbacks Callbacks { get; }
		private MainModel Model { get => Data.Model; }

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

		public Remapper(ExportData data, RemapCallbacks callbacks)
		{
			Data = data;
			Callbacks = callbacks;
		}

		#endregion

		#region Remapping

		public void Remap()
		{
			Callbacks?.OnRemapStarted();
			Callbacks?.OnRemapDebug($"Starting remap{Environment.NewLine}");

			Data.Clear();
			Data.BlockSize = CalculateBlockSize();
			Data.ImageOffset = CalculateImageOffset();

			objectSize = Data.Model.OutputType == OutputType.Sprites ? 16 : 8;
			maxObjectsCount = Data.Model.OutputType == OutputType.Sprites ? 128 : ExportData.MAX_OBJECTS - 1;
			objectsPerGridX = (Data.Model.GridWidth / objectSize);
			objectsPerGridY = (Data.Model.GridHeight / objectSize);

			if (Data.Model.OutputType == OutputType.Tiles)
			{
				Callbacks?.OnRemapDebug("Preparing data for tiles export");

				if (Data.Model.TransparentBlocks)
				{
					MakeFirstBlockTransparent();
				}

				if (Data.Model.TransparentTiles)
				{
					MakeFirstTileTransparent();
				}
			}

			Callbacks?.OnRemapDebug($"Reading images{Environment.NewLine}");

			for (int idx = 0; idx < Data.Model.Images.Count; idx++)
			{
				Callbacks?.OnRemapDebug($"Handling image {idx}{Environment.NewLine}");

				var image = Data.Model.Images[idx];
				var sourceRect = new Rectangle();

				if (Data.Model.OutputType == OutputType.Tiles)
				{
					CheckImageDimensions(image);
				}

				if (!image.IsImageValid)
				{
					Callbacks?.OnRemapDebug($"Image is invalid, ignoring{Environment.NewLine}");
					continue;
				}

				for (int yBlocks = 0; yBlocks < ((image.Image.Height + (Data.Model.GridHeight - 1)) / Data.Model.GridHeight); yBlocks++)
				{
					for (int xBlocks = 0; xBlocks < (image.Image.Width / Data.Model.GridWidth); xBlocks++)
					{
						sourceRect.X = xBlocks * Data.Model.GridWidth;
						sourceRect.Y = yBlocks * Data.Model.GridHeight;
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
							Data.Blocks[outBlock] = new IndexedBitmap(Data.Model.GridWidth, Data.Model.GridHeight);
						}

						if (Data.Sprites[outBlock] == null)
						{
							Data.Sprites[outBlock] = new SpriteInfo(objectsPerGridX, objectsPerGridY);
						}

						image.CopyRegionIntoBlock(
							Data.Model.Palette, 
							sourceRect, 
							Data.Model.Reduced && Data.Model.OutputType == OutputType.Sprites, 
							ref Data.Blocks[outBlock], 
							ref Data.Sprites[outBlock]);

						if (Data.Model.FourBit || Data.Model.OutputType == OutputType.Tiles)
						{
							Data.Blocks[outBlock].RemapTo4Bit(Data.Model.Palette, Data.Model.GridWidth, Data.Model.GridHeight, objectSize);
						}

						if (Data.Blocks[outBlock].IsTransparent(Data.Model.Palette.TransparentIndex))
						{
							// We only draw first transparent block.
							Callbacks?.OnRemapDebug($"Block is transparent{Environment.NewLine}");
							if (outBlock > 0)
							{
								continue;
							}
						}

						for (int yChar = 0; yChar < objectsPerGridY; yChar++)
						{
							for (int xChar = 0; xChar < objectsPerGridX; xChar++)
							{
								if (Data.Model.FourBit || Data.Model.OutputType == OutputType.Tiles)
								{
									paletteOffset = Data.Blocks[outBlock].GetPixel(xChar * objectSize, yChar * objectSize) & 0x0f0;
								}

								PrepareSpriteData(xChar, yChar);
							}
						}

						Callbacks?.OnRemapDisplayBlocksCount(outBlock);

						if (Model.OutputType == OutputType.Tiles)
						{
							if (!IsSpriteDuplicated(outBlock, objectsPerGridX, objectsPerGridY))
							{
								outBlock++;
							}
						}
						else
						{
							outBlock++;
						}

						Callbacks?.OnRemapDebug($"- {Environment.NewLine}");
					}

					Callbacks?.OnRemapDebug(Environment.NewLine);
					Callbacks?.OnRemapUpdated();
				}
			}

			int transparentCharactersCount = 0;

			if (Data.Model.TransparentFirst && Model.OutputType == OutputType.Tiles)
			{
				int sortedIndex = 0;

				// On first pass we only handle transparent blocks.
				PrepareCharacters(
					(index, transparent) => transparent ? sortedIndex : -1,
					() =>
					{
						transparentCharactersCount++;
						sortedIndex++;
					});

				// On second pass we only handle non-transparent blocks.
				PrepareCharacters(
					(index, transparent) => transparent ? -1 : sortedIndex,
					() =>
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
				if (outXBlock >= Model.BlocksAccross)
				{
					outXBlock = 0;
					outYBlock++;
				}
			}

			Callbacks?.OnRemapDebug("Remap completed");

			if (outChar > maxObjectsCount)
			{
				Callbacks?.OnRemapWarning("Too many characters in your tiles");
			}

			Callbacks?.OnRemapDisplayCharactersCount(outChar, transparentCharactersCount);
			Callbacks?.OnRemapCompleted(allImagesProcessed);
		}

		/// <summary>
		/// Prepares characters list by looping through all characters with callbacks called at specific points while handling each character.
		/// </summary>
		/// <param name="indexProvider">Called at the start of each iteration. Caller must return the index into which the character data should be created. Parameters are: loop index, true if temp data at loop index is transparent, false otherwise.</param>
		/// <param name="afterCharHandler">Called at the end of each iteration, caller can update its variables as neeeded.</param>
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

				var charBitmap = new IndexedBitmap(objectSize, objectSize);
				charBitmap.Transparent = tempData.Transparent;
				Data.Chars[indexForLoop] = charBitmap;

				for (int y = 0; y < tempData.Height; y++)
				{
					for (int x = 0; x < tempData.Width; x++)
					{
						Data.Chars[indexForLoop].SetPixel(x, y, tempData.GetPixel(x, y));
					}
				}

				Data.SortIndexes[i] = indexForLoop;
				RequestCharacterDisplay(indexForLoop);

				afterCharHandler();
			}
		}

		private void PrepareSpriteData(int x, int y)
		{
			// Returns true if sprite was created, false otherwise
			for (short c = 0; c < outChar; c++)
			{
				var repeatResult = RepeatedCharType(c, x * objectSize, y * objectSize);
				var isTransparent = Data.TempData[c].IsTransparent(Data.Model.Palette.TransparentIndex);

				switch (repeatResult)
				{
					case BlockType.Repeated:
						// rep  flpX flpY  rot   trans
						Callbacks?.OnRemapDebug($"R   {c},");
						Data.Sprites[outBlock].SetData(x, y, true, false, false, false, false, c, (short)paletteOffset, isTransparent);
						return;

					case BlockType.FlippedX:
						// rep  flpX flpY  rot   trans
						Callbacks?.OnRemapDebug($"RX  {c},");
						Data.Sprites[outBlock].SetData(x, y, true, true, false, false, false, c, (short)paletteOffset, isTransparent);
						return;

					case BlockType.FlippedY:
						// rep  flpX flpY  rot   trans
						Callbacks?.OnRemapDebug($"RY  {c},");
						Data.Sprites[outBlock].SetData(x, y, true, false, true, false, false, c, (short)paletteOffset, isTransparent);
						return;

					case BlockType.FlippedXY:
						// rep  flpX flpY  rot   trans
						Callbacks?.OnRemapDebug($"RXY {c},");
						Data.Sprites[outBlock].SetData(x, y, true, true, true, false, false, c, (short)paletteOffset, isTransparent);
						return;

					case BlockType.Rotated:
						// rep  flpX flpY  rot   trans
						Callbacks?.OnRemapDebug($"RR  {c},");
						Data.Sprites[outBlock].SetData(x, y, true, false, false, true, false, c, (short)paletteOffset, isTransparent);
						return;

					case BlockType.FlippedXRotated:
						// rep  flpX flpY  rot   trans		
						Callbacks?.OnRemapDebug($"RXR {c},");
						Data.Sprites[outBlock].SetData(x, y, true, true, false, true, false, c, (short)paletteOffset, isTransparent);
						return;

					case BlockType.FlippedYRotated:
						// rep  flpX flpY  rot   trans		
						Callbacks?.OnRemapDebug($"RYR {c},");
						Data.Sprites[outBlock].SetData(x, y, true, false, true, true, false, c, (short)paletteOffset, isTransparent);
						return;

					case BlockType.FlippedXYRotated:
						// rep  flpX flpY  rot   trans
						Callbacks?.OnRemapDebug($"RXYR{c},");
						Data.Sprites[outBlock].SetData(x, y, true, true, true, true, false, c, (short)paletteOffset, isTransparent);
						return;

					case BlockType.Transparent:
						// rep  flpX  flpY  rot  trans
						Callbacks?.OnRemapDebug($"T   {c},");
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

			Data.Sprites[outBlock].SetData(
				x, y,
				false, false, false, false,
				isBlockTransparent,
				(short)outChar, (short)paletteOffset,
				Data.TempData[outChar].IsTransparent(Data.Model.Palette.TransparentIndex));

			Callbacks?.OnRemapDebug($"O   {outChar} ");

			if (outChar <= maxObjectsCount)
			{
				outChar++;
			}
		}

		private void CheckImageDimensions(SourceImage image)
		{
			var isWidthDivisible = (image.Image.Width % Data.Model.GridWidth) == 0;
			var isHeightDivisible = (image.Image.Height % Data.Model.GridHeight) == 0;

			if (!isWidthDivisible && !isHeightDivisible)
			{
				Callbacks?.OnRemapWarning($"The image {Path.GetFileName(image.Filename)} ({image.Image.Width}x{image.Image.Height}) is not divisible by the width and height of your tiles ({Data.Model.GridWidth}x{Data.Model.GridHeight}), which will corrupt the output");
				allImagesProcessed = false;
			}
			else if (!isWidthDivisible)
			{
				Callbacks?.OnRemapWarning($"The image {Path.GetFileName(image.Filename)} ({image.Image.Width}x{image.Image.Height}) is not divisible by the width of your tiles ({Data.Model.GridWidth}), which will corrupt the output");
				allImagesProcessed = false;
			}
			else if (!isHeightDivisible)
			{
				Callbacks?.OnRemapWarning($"The image {Path.GetFileName(image.Filename)} ({image.Image.Width}x{image.Image.Height}) is not divisible by the height of your tiles ({Data.Model.GridHeight}), which will corrupt the output");
				allImagesProcessed = false;
			}
		}

		private void MakeFirstBlockTransparent()
		{
			if (Data.Blocks[0] == null)
			{
				Callbacks?.OnRemapDebug("Making first block transparent");

				Data.Blocks[0] = new IndexedBitmap(Data.Model.GridWidth, Data.Model.GridHeight);

				for (int y = 0; y < Data.Model.GridHeight; y++)
				{
					for (int x = 0; x < Data.Model.GridWidth; x++)
					{
						Data.Blocks[0].SetPixel(x, y, (short)Data.Model.Palette.TransparentIndex);
					}
				}

				Callbacks?.OnRemapDisplayBlock(new Rectangle(0, 0, Data.Model.GridWidth, Data.Model.GridHeight), Data.Blocks[0]);
			}

			outXBlock = 1;
			outBlock = 1;
			if (Data.Sprites[0] == null)
			{
				Callbacks?.OnRemapDebug("Making first sprite transparent");

				Data.Sprites[0] = new SpriteInfo(objectsPerGridX, objectsPerGridY);
			}
		}

		private void MakeFirstTileTransparent()
		{
			if (Data.TempData[0] == null)
			{
				Callbacks?.OnRemapDebug("Making first tile transparent");

				Data.TempData[0] = new IndexedBitmap(objectSize, objectSize);

				for (int y = 0; y < 8; y++)
				{
					for (int x = 0; x < 8; x++)
					{
						Data.TempData[0].SetPixel(x, y, (short)Data.Model.Palette.TransparentIndex);
					}
				}
			}

			objectGridX = 1;
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
				bool flipX = Data.Sprites[currentBlock].infos[chr].FlippedX;
				bool flipY = Data.Sprites[currentBlock].infos[chr].FlippedY;
				bool rotate = Data.Sprites[currentBlock].infos[chr].Rotated;
				int id = Data.Sprites[currentBlock].infos[chr].OriginalID;
				int sortedId = Data.SortIndexes[id];
				Bitmap tempBitmap = new Bitmap(objectSize, objectSize);
				RotateFlipType flips = RotateFlipType.RotateNoneFlipNone;

				String debugString = "";

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

				if (Data.Sprites[currentBlock].infos[chr].Transparent == false)
				{
					for (int y = 0; y < objectSize; y++)
					{
						for (int x = 0; x < objectSize; x++)
						{
							var pixelColor = tempBitmap.GetPixel(x, y);
							destBitmap.SetPixel(
								destRegion.X + (Data.Sprites[currentBlock].infos[chr].Position.X * objectSize) + x, 
								destRegion.Y + (Data.Sprites[currentBlock].infos[chr].Position.Y * objectSize) + y, 
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
								destRegion.X + (Data.Sprites[currentBlock].infos[chr].Position.X * objectSize) - 2,
								destRegion.Y + (Data.Sprites[currentBlock].infos[chr].Position.Y * objectSize) - 2));
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
			var frame = new Rectangle();
			frame.X = objectGridX * objectSize;
			frame.Y = objectGridY * objectSize;
			frame.Width = objectSize;
			frame.Height = objectSize;

			Callbacks?.OnRemapDisplayChar(frame, Data.Chars[index]);

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

		private int CountSamePixels(int xOffset, int yOffset, ref IndexedBitmap source, ref IndexedBitmap other, Func<IndexedBitmap, int, int, short> otherPixelProvider)
		{
			int result = objectSize * objectSize;
			var unreferencedOther = other;  // can't use ref inside closure

			source.PixelIterator(xOffset, yOffset, objectSize, (x, y, colour) =>
			{
				if (colour != otherPixelProvider(unreferencedOther, x, y))
				{
					result--;
				}

				return true;
			});

			return result;
		}

		private int CalculateBlockSize()
		{
			if (Data.Model.OutputType == OutputType.Sprites)
			{
				if (!Data.Model.FourBit)
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
			int samePixels = objectSize * objectSize;
			float accuracy = (float)Data.Model.Accuracy / 100f;
			float pixelClose = samePixels * accuracy;
			bool hasTransparentPixels = false;

			IndexedBitmap currentBlock = Data.Blocks[outBlock];
			IndexedBitmap rotateData = new IndexedBitmap(objectSize, objectSize);

			Func<IndexedBitmap, int, int, short> identicalPixelProvider = (bitmap, x, y) => bitmap.GetPixel(x, y);
			Func<IndexedBitmap, int, int, short> flippedXPixelProvider = (bitmap, x, y) => bitmap.GetPixel((objectSize - 1) - x, y);
			Func<IndexedBitmap, int, int, short> flippedYPixelProvider = (bitmap, x, y) => bitmap.GetPixel(x, (objectSize - 1) - y);
			Func<IndexedBitmap, int, int, short> flippedXYPixelProvider = (bitmap, x, y) => bitmap.GetPixel((objectSize - 1) - x, (objectSize - 1) - y);

			// does it have any transparent pixels?
			if (Data.Model.TransparentFirst || Data.Model.OutputType == OutputType.Sprites)
			{
				hasTransparentPixels = Data.Blocks[outBlock].HasTransparentPixels(Data.Model.Palette.TransparentIndex, xOffset, yOffset, objectSize);
			}

			// do we ignore all repeats?
			if (Data.Model.IgnoreCopies)
			{
				return BlockType.Original;
			}

			// check to see if fully transparent
			if (!Data.Model.IgnoreTransparentPixels && hasTransparentPixels)
			{
				if (Data.Blocks[outBlock].IsTransparent(Data.Model.Palette.TransparentIndex, xOffset, yOffset, objectSize))
				{
					return BlockType.Transparent;
				}
			}

			// see how close to the original block it is
			if (Data.Blocks[outBlock].IsColourBlock(xOffset, yOffset, objectSize))
			{
				pixelClose = objectSize * objectSize;
			}

			samePixels = CountSamePixels(xOffset, yOffset, ref Data.Blocks[outBlock], ref Data.TempData[character], identicalPixelProvider);
			if (Data.Model.IgnoreTransparentPixels && hasTransparentPixels)
			{
				if (samePixels == objectSize * objectSize)
				{
					return BlockType.Repeated;
				}
				return BlockType.Original;
			}

			// if its close to original % and not containing transparent!
			if (samePixels >= pixelClose && (hasTransparentPixels == false || Data.Model.OutputType == OutputType.Sprites))
			{
				return BlockType.Repeated;
			}

			samePixels = objectSize * objectSize;

			if (!Data.Model.IgnoreMirroredX)
			{
				samePixels = CountSamePixels(xOffset, yOffset, ref Data.Blocks[outBlock], ref Data.TempData[character], flippedXPixelProvider);
				if (samePixels >= pixelClose)
				{
					return BlockType.FlippedX;
				}
			}

			samePixels = objectSize * objectSize;
			if (!Data.Model.IgnoreMirroredY)
			{
				samePixels = CountSamePixels(xOffset, yOffset, ref Data.Blocks[outBlock], ref Data.TempData[character], flippedYPixelProvider);
				if (samePixels >= pixelClose)
				{
					return BlockType.FlippedY;
				}
			}

			samePixels = objectSize * objectSize;
			if (!Data.Model.IgnoreMirroredX && !Data.Model.IgnoreMirroredY)
			{
				samePixels = CountSamePixels(xOffset, yOffset, ref Data.Blocks[outBlock], ref Data.TempData[character], flippedXYPixelProvider);
				if (samePixels >= pixelClose)
				{
					return BlockType.FlippedXY;
				}
			}

			for (int y = 0; y < objectSize; y++)
			{
				for (int x = 0; x < objectSize; x++)
				{
					rotateData.SetPixel((objectSize - 1) - y, x, Data.TempData[character].GetPixel(x, y));
				}
			}

			samePixels = objectSize * objectSize;
			if (!Data.Model.IgnoreRotated)
			{
				samePixels = CountSamePixels(xOffset, yOffset, ref Data.Blocks[outBlock], ref rotateData, identicalPixelProvider);
				if (samePixels >= pixelClose)
				{
					return BlockType.Rotated;
				}

				samePixels = objectSize * objectSize;
				if (Data.Model.IgnoreMirroredX)
				{
					samePixels = CountSamePixels(xOffset, yOffset, ref Data.Blocks[outBlock], ref rotateData, flippedXPixelProvider);
					if (samePixels >= pixelClose)
					{
						return BlockType.FlippedXRotated;
					}
				}

				samePixels = objectSize * objectSize;
				if (!Data.Model.IgnoreMirroredY)
				{
					samePixels = CountSamePixels(xOffset, yOffset, ref Data.Blocks[outBlock], ref rotateData, flippedYPixelProvider);
					if (samePixels >= pixelClose)
					{
						return BlockType.FlippedYRotated;
					}
				}

				samePixels = objectSize * objectSize;
				if (!Data.Model.IgnoreMirroredX && !Data.Model.IgnoreMirroredY)
				{
					samePixels = CountSamePixels(xOffset, yOffset, ref Data.Blocks[outBlock], ref rotateData, flippedXYPixelProvider);
					if (samePixels >= pixelClose)
					{
						return BlockType.FlippedXYRotated;
					}
				}
			}
			return BlockType.Original;
		}

		#endregion
	}
}
