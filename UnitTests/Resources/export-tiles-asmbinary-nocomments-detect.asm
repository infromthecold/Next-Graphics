// Test.asm
// Created on 14 March 2022 14:03:42 by the NextGraphics tool from
// patricia curtis at luckyredfish dot com

TEST_COLOURS:		equ	24


TEST_TILE_SIZE:		equ	32

TEST_TILES:		equ	5






TestBlock0:		.db	0,0
TestBlock1:		.db	0,1
TestBlock2:		.db	0,2
TestBlock3:		.db	0,3
TestBlock4:		.db	16,4


TEST_FILE_SIZE	equ	160
TestFile:			dw	TEST_FILE_SIZE
			db	PATH,"game/level1/test.bin",0
