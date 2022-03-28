// Test.asm
// Created on 15 March 2022 10:58:30 by the NextGraphics tool from
// patricia curtis at luckyredfish dot com

TEST_COLOURS:		equ	39


TEST_SPRITE_SIZE:		equ	256

TEST_SPRITES:		equ	7


				// number of sprites

					// x offset from center of sprite
					// y offset from center of sprite
					// Palette offset with the X mirror,Y mirror, Rotate bits if set
					// 4 bit colour bit and pattern offset bit
					// index of the sprite at this position that makes up the frame

				//...... repeated wide x tall times



TestFrames:		.dw	TestFrame0,TestFrame1,TestFrame2,TestFrame3,TestFrame4,TestFrame5,TestFrame6,TestFrame7,TestFrame8,TestFrame9,TestFrame10,TestFrame11


TEST_FILE_SIZE	equ	1792
TestFile:			dw	TEST_FILE_SIZE
			db	PATH,"game/level1/test.bin",0
