// Test.asm
// Created on {0} by the NextGraphics tool from
// patricia curtis at luckyredfish dot com

TEST_COLOURS:		equ	6

TestPalette:
			db	%01001001
			db	%11111111
			db	%11011011
			db	%10110110
			db	%10010010
			db	%01101101

TEST_TILE_SIZE:		equ	32

TEST_TILES:		equ	4

// Collisions Left Width Top Height

TestBlock0:		.db	0,0
TestBlock1:		.db	0,1
TestBlock2:		.db	0,2
TestBlock3:		.db	0,3

TEST_FILE_SIZE	equ	128
TestFile:			dw	TEST_FILE_SIZE
			db	PATH,"game/level1/test.bin",0
