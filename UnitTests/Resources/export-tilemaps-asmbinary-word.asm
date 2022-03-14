// Test.asm
// Created on 15 March 2022 13:23:23 by the NextGraphics tool from
// patricia curtis at luckyredfish dot com

TEST_COLOURS:		equ	6


TEST_TILE_SIZE:		equ	32

TEST_TILES:		equ	0


				// block data
				// number of tiles (characters) tall
				// number of tiles (characters) wide
					// Palette offset with the X mirror,Y mirror, Rotate bits if set
					// index of the character at this position that makes up the block
				//...... repeated wide x tall times

				//Note: Blocks/Tiles/characters output block 0 and tile 0 is blank.

TestTileWidth:	equ	0
TestTileHeight:	equ	0




			// Each tilemap defines width and height (in tiles), total size in bytes and list of all tiles.
TEST_TILEMAPS:			equ	1

TEST_TILEMAP0_WIDTH:		equ	2
TEST_TILEMAP0_HEIGHT:		equ	2
TEST_TILEMAP0_SIZE:		equ	8

TEST_FILE_SIZE	equ	0
TestFile:			dw	TEST_FILE_SIZE
			db	PATH,"game/level1/test.bin",0
