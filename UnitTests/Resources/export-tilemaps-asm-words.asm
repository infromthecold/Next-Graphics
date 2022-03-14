// Test.asm
// Created on 15 March 2022 13:23:23 by the NextGraphics tool from
// patricia curtis at luckyredfish dot com

TEST_COLOURS:		equ	6

TestPalette:
			db	%01001001	//	72,72,72
			db	%11111111	//	255,255,255
			db	%11011011	//	218,218,218
			db	%10110110	//	182,182,182
			db	%10010010	//	145,145,145
			db	%01101101	//	109,109,109

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
Test_Tilemap0:
			// Each tile is defined as word with first (low) byte represents tile index and second (high) byte attributes.
			dw	$0108,$0204
			dw	$030c,$0400

