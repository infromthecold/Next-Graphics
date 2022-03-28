// Test.asm
// Created on 15 March 2022 11:32:14 by the NextGraphics tool from
// patricia curtis at luckyredfish dot com

TEST_COLOURS:		equ	39


TEST_SPRITE_SIZE:		equ	256

TEST_SPRITES:		equ	7




// Collisions Left Width Top Height
TestCollision0:		.db	0,15,0,15
TestCollision1:		.db	3,10,1,13
TestCollision2:		.db	2,11,1,13
TestCollision3:		.db	2,11,1,13
TestCollision4:		.db	1,13,2,10
TestCollision5:		.db	2,10,1,13
TestCollision6:		.db	1,13,3,10
TestCollision7:		.db	2,10,1,13
TestCollision8:		.db	2,10,1,13
TestCollision9:		.db	0,15,0,15
TestCollision10:		.db	0,15,0,15
TestCollision11:		.db	2,11,1,13


TestFrame0:		.db	1,	0,0,	TEST_OFFSET+0,	0,0
TestFrame1:		.db	1,	0,0,	TEST_OFFSET+0,	0,1
TestFrame2:		.db	1,	0,0,	TEST_OFFSET+0,	0,2
TestFrame3:		.db	1,	0,0,	TEST_OFFSET+0,	0,3
TestFrame4:		.db	1,	0,0,	TEST_OFFSET+14,	0,1
TestFrame5:		.db	1,	0,0,	TEST_OFFSET+12,	0,1
TestFrame6:		.db	1,	0,0,	TEST_OFFSET+2,	0,1
TestFrame7:		.db	1,	0,0,	TEST_OFFSET+8,	0,1
TestFrame8:		.db	1,	0,0,	TEST_OFFSET+12,	0,1
TestFrame9:		.db	1,	0,0,	TEST_OFFSET+0,	0,4
TestFrame10:		.db	1,	0,0,	TEST_OFFSET+0,	0,5
TestFrame11:		.db	1,	0,0,	TEST_OFFSET+0,	0,6

TestFrames:		.dw	TestFrame0,TestFrame1,TestFrame2,TestFrame3,TestFrame4,TestFrame5,TestFrame6,TestFrame7,TestFrame8,TestFrame9,TestFrame10,TestFrame11


TEST_FILE_SIZE	equ	1792
TestFile:			dw	TEST_FILE_SIZE
			db	PATH,"game/level1/test.bin",0
