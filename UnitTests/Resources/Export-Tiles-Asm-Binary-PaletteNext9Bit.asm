// Test.asm
// Created on {0} by the NextGraphics tool from
// patricia curtis at luckyredfish dot com

TEST_COLOURS:		equ	6

TestPalette:
			db	%01001001,%00000000	//	72,72,72
			db	%11111111,%00000001	//	255,255,255
			db	%11011011,%00000000	//	218,218,218
			db	%10110110,%00000001	//	182,182,182
			db	%10010010,%00000000	//	145,145,145
			db	%01101101,%00000001	//	109,109,109

TEST_TILE_SIZE:		equ	32

TEST_TILES:		equ	4


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

TEST_FILE_SIZE	equ	128
TestFile:			dw	TEST_FILE_SIZE
			db	PATH,"game/level1/test.bin",0
