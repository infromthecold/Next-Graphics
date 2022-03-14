// Test.asm
// Created on 14 March 2022 14:55:31 by the NextGraphics tool from
// patricia curtis at luckyredfish dot com

TEST_COLOURS:		equ	23


TEST_TILE_SIZE:		equ	32

TEST_TILES:		equ	5








TEST_FILE_SIZE	equ	160
TestFile:			dw	TEST_FILE_SIZE
			db	PATH,"game/level1/test.bin",0
