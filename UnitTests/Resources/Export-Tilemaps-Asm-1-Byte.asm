// Test.asm
// Created on {0} by the NextGraphics tool from
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
TEST_TILEMAPS:	equ	1

TEST_TILEMAP0_WIDTH:	equ	2
TEST_TILEMAP0_HEIGHT:	equ	2
TEST_TILEMAP0_SIZE:	equ	4
Test_Tilemap0:
			// Each tile is defined as single byte, representing the tile index.
			db	$01,$02
			db	$03,$04

