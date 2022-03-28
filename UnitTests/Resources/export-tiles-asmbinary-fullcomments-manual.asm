// Test.asm
// Created on 14 March 2022 14:03:42 by the NextGraphics tool from
// patricia curtis at luckyredfish dot com

TEST_COLOURS:		equ	23


TEST_TILE_SIZE:		equ	32

TEST_TILES:		equ	5


				// block data
				// number of tiles (characters) tall
				// number of tiles (characters) wide
					// Palette offset with the X mirror,Y mirror, Rotate bits if set
					// index of the character at this position that makes up the block
				//...... repeated wide x tall times

				//Note: Blocks/Tiles/characters output block 0 and tile 0 is blank.

TestTileWidth:	equ	1
TestTileHeight:	equ	1



TestBlock0:		.db	0,0
TestBlock1:		.db	0,1
TestBlock2:		.db	0,2
TestBlock3:		.db	0,3
TestBlock4:		.db	0,4


TEST_FILE_SIZE	equ	160
TestFile:			dw	TEST_FILE_SIZE
			db	PATH,"game/level1/test.bin",0
