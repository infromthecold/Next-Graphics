// Test.asm
// Created on 15 March 2022 10:58:30 by the NextGraphics tool from
// patricia curtis at luckyredfish dot com

TEST_COLOURS:		equ	39

TestPalette:
			db	%11000111	//	235,51,255
			db	%00000000	//	0,0,0
			db	%11111101	//	255,238,51
			db	%01111000	//	109,235,0
			db	%10010111	//	163,192,255
			db	%11111111	//	255,255,255
			db	%11111010	//	255,204,204
			db	%11111111	//	237,237,237
			db	%11110010	//	255,163,163
			db	%00001011	//	15,91,255
			db	%00001010	//	0,63,199
			db	%00000101	//	0,39,122
			db	%11011011	//	219,219,219
			db	%11110010	//	255,128,128
			db	%10110110	//	200,200,200
			db	%11101101	//	255,92,92
			db	%00000001	//	0,16,51
			db	%10110110	//	182,182,182
			db	%11100101	//	255,51,51
			db	%10110110	//	164,164,164
			db	%11100000	//	255,15,15
			db	%10010010	//	146,146,146
			db	%11000000	//	235,0,0
			db	%10010010	//	128,128,128
			db	%10100000	//	199,0,0
			db	%10000000	//	158,0,0
			db	%01101101	//	109,109,109
			db	%01100000	//	122,0,0
			db	%00000110	//	0,50,158
			db	%01001001	//	91,91,91
			db	%01000000	//	87,0,0
			db	%01001001	//	73,73,73
			db	%00100000	//	51,0,0
			db	%01001001	//	55,55,55
			db	%11111010	//	255,219,204
			db	%00100100	//	36,36,36
			db	%11110110	//	255,191,163
			db	%00000101	//	0,27,87
			db	%11110110	//	255,166,128

TEST_SPRITE_SIZE:		equ	256

TEST_SPRITES:		equ	7

TestSprite0:
				.db	$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01
				.db	$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00
				.db	$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01
				.db	$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00
				.db	$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01
				.db	$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00
				.db	$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01
				.db	$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00
				.db	$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01
				.db	$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00
				.db	$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01
				.db	$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00
				.db	$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01
				.db	$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00
				.db	$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01
				.db	$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00,$01,$00
TestSprite1:
				.db	$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$02,$02,$02,$00,$00,$00,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$02,$02,$02,$02,$00,$00,$00,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$02,$02,$02,$02,$02,$00,$00,$00,$00,$00,$00
				.db	$00,$00,$00,$00,$02,$02,$00,$02,$02,$02,$00,$00,$00,$00,$00,$00
				.db	$00,$00,$00,$02,$02,$00,$00,$02,$02,$02,$00,$00,$00,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$02,$02,$02,$00,$00,$00,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$02,$02,$02,$00,$00,$00,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$02,$02,$02,$00,$00,$00,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$02,$02,$02,$00,$00,$00,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$02,$02,$02,$00,$00,$00,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$02,$02,$02,$00,$00,$00,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$02,$02,$02,$00,$00,$00,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$02,$02,$02,$00,$00,$00,$00,$00,$00
				.db	$00,$00,$00,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00
TestSprite2:
				.db	$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$03,$03,$03,$03,$03,$03,$03,$00,$00,$00,$00
				.db	$00,$00,$00,$00,$03,$03,$00,$00,$00,$00,$03,$03,$03,$00,$00,$00
				.db	$00,$00,$00,$03,$03,$00,$00,$00,$00,$00,$00,$03,$03,$03,$00,$00
				.db	$00,$00,$03,$03,$00,$00,$00,$00,$00,$00,$00,$03,$03,$03,$00,$00
				.db	$00,$00,$03,$03,$00,$00,$00,$00,$00,$00,$00,$03,$03,$03,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$03,$03,$03,$03,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$00,$00,$03,$03,$03,$03,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$00,$03,$03,$03,$03,$00,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$03,$03,$03,$03,$00,$00,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$03,$03,$03,$03,$00,$00,$00,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$03,$03,$03,$03,$00,$00,$00,$00,$00,$00,$00
				.db	$00,$00,$00,$00,$03,$03,$03,$03,$00,$00,$00,$00,$00,$00,$00,$00
				.db	$00,$00,$00,$03,$03,$03,$03,$00,$00,$00,$00,$00,$00,$00,$00,$00
				.db	$00,$00,$03,$03,$03,$03,$03,$03,$03,$03,$03,$03,$03,$03,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00
TestSprite3:
				.db	$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00
				.db	$00,$00,$00,$00,$04,$04,$04,$04,$04,$04,$04,$04,$00,$00,$00,$00
				.db	$00,$00,$00,$04,$04,$00,$00,$00,$00,$00,$04,$04,$04,$00,$00,$00
				.db	$00,$00,$04,$04,$00,$00,$00,$00,$00,$00,$00,$04,$04,$04,$00,$00
				.db	$00,$00,$04,$04,$00,$00,$00,$00,$00,$00,$00,$04,$04,$04,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$04,$04,$04,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$04,$04,$04,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$04,$04,$04,$04,$04,$04,$04,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$04,$04,$04,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$04,$04,$04,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$04,$04,$04,$00,$00
				.db	$00,$00,$04,$04,$00,$00,$00,$00,$00,$00,$00,$04,$04,$04,$00,$00
				.db	$00,$00,$04,$04,$00,$00,$00,$00,$00,$00,$04,$04,$04,$04,$00,$00
				.db	$00,$00,$00,$04,$04,$00,$00,$00,$00,$04,$04,$04,$04,$00,$00,$00
				.db	$00,$00,$00,$00,$04,$04,$04,$04,$04,$04,$04,$04,$00,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00
TestSprite4:
				.db	$05,$05,$05,$05,$05,$05,$05,$05,$05,$05,$05,$05,$05,$05,$05,$05
				.db	$05,$07,$07,$07,$07,$07,$07,$07,$07,$07,$07,$07,$07,$07,$07,$05
				.db	$05,$07,$0c,$0c,$0c,$0c,$0c,$0c,$0c,$0c,$0c,$0c,$0c,$0c,$07,$05
				.db	$05,$07,$0c,$0e,$0e,$0e,$0e,$0e,$0e,$0e,$0e,$0e,$0e,$0c,$07,$05
				.db	$05,$07,$0c,$0e,$11,$11,$11,$11,$11,$11,$11,$11,$0e,$0c,$07,$05
				.db	$05,$07,$0c,$0e,$11,$13,$13,$13,$13,$13,$13,$11,$0e,$0c,$07,$05
				.db	$05,$07,$0c,$0e,$11,$13,$15,$15,$15,$15,$13,$11,$0e,$0c,$07,$05
				.db	$05,$07,$0c,$0e,$11,$13,$15,$17,$17,$15,$13,$11,$0e,$0c,$07,$05
				.db	$05,$07,$0c,$0e,$11,$13,$15,$17,$17,$15,$13,$11,$0e,$0c,$07,$05
				.db	$05,$07,$0c,$0e,$11,$13,$15,$15,$15,$1a,$13,$11,$0e,$0c,$07,$05
				.db	$05,$07,$0c,$0e,$11,$13,$13,$13,$13,$13,$1d,$11,$0e,$0c,$07,$05
				.db	$05,$07,$0c,$0e,$11,$11,$11,$11,$11,$11,$11,$1f,$0e,$0c,$07,$05
				.db	$05,$07,$0c,$0e,$0e,$0e,$0e,$0e,$0e,$0e,$0e,$0e,$21,$0c,$07,$05
				.db	$05,$07,$0c,$0c,$0c,$0c,$0c,$0c,$0c,$0c,$0c,$0c,$0c,$23,$07,$05
				.db	$05,$07,$07,$07,$07,$07,$07,$07,$07,$07,$07,$07,$07,$07,$01,$05
				.db	$05,$05,$05,$05,$05,$05,$05,$05,$05,$05,$05,$05,$05,$05,$05,$05
TestSprite5:
				.db	$00,$06,$06,$06,$06,$06,$06,$06,$06,$06,$06,$06,$06,$06,$06,$00
				.db	$06,$08,$08,$08,$08,$08,$08,$08,$08,$08,$08,$08,$08,$08,$08,$06
				.db	$06,$08,$0d,$0d,$0d,$0d,$0d,$0d,$0d,$0d,$0d,$0d,$0d,$0d,$08,$06
				.db	$06,$08,$0d,$0f,$0f,$0f,$0f,$0f,$0f,$0f,$0f,$0f,$0f,$0d,$08,$06
				.db	$06,$08,$0d,$0f,$12,$12,$12,$12,$12,$12,$12,$12,$0f,$0d,$08,$06
				.db	$06,$08,$0d,$0f,$12,$14,$14,$14,$14,$14,$14,$12,$0f,$0d,$08,$06
				.db	$06,$08,$0d,$0f,$12,$14,$16,$16,$16,$16,$14,$12,$0f,$0d,$08,$06
				.db	$06,$08,$0d,$0f,$12,$14,$16,$18,$18,$16,$14,$12,$0f,$0d,$08,$06
				.db	$06,$08,$0d,$0f,$12,$14,$16,$18,$19,$16,$14,$12,$0f,$0d,$08,$06
				.db	$06,$08,$0d,$0f,$12,$14,$16,$16,$16,$1b,$14,$12,$0f,$0d,$08,$06
				.db	$06,$08,$0d,$0f,$12,$14,$14,$14,$14,$14,$1e,$12,$0f,$0d,$08,$06
				.db	$06,$08,$0d,$0f,$12,$12,$12,$12,$12,$12,$12,$20,$0f,$0d,$08,$06
				.db	$06,$08,$0d,$0f,$0f,$0f,$0f,$0f,$0f,$0f,$0f,$0f,$22,$0d,$08,$06
				.db	$06,$08,$0d,$0d,$0d,$0d,$0d,$0d,$0d,$0d,$0d,$0d,$0d,$24,$08,$06
				.db	$06,$08,$08,$08,$08,$08,$08,$08,$08,$08,$08,$08,$08,$08,$26,$06
				.db	$00,$06,$06,$06,$06,$06,$06,$06,$06,$06,$06,$06,$06,$06,$06,$00
TestSprite6:
				.db	$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$09,$0a,$0b,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$00,$00,$09,$0a,$0a,$0b,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$00,$09,$0a,$10,$0a,$0b,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$09,$0a,$10,$00,$0a,$0b,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$09,$0a,$10,$00,$00,$0a,$0b,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$09,$0a,$10,$00,$00,$00,$0a,$0b,$00,$00,$00
				.db	$00,$00,$00,$00,$09,$0a,$10,$00,$00,$00,$00,$0a,$0b,$00,$00,$00
				.db	$00,$00,$00,$09,$0a,$10,$00,$00,$00,$00,$00,$0a,$0b,$00,$00,$00
				.db	$00,$00,$1c,$1c,$1c,$1c,$1c,$1c,$1c,$1c,$1c,$1c,$1c,$1c,$00,$00
				.db	$00,$00,$10,$10,$10,$10,$10,$10,$10,$10,$10,$0a,$0b,$10,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$0a,$0b,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$0a,$0b,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$0a,$25,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$25,$25,$00,$00,$00
				.db	$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00

				// number of sprites

					// x offset from center of sprite
					// y offset from center of sprite
					// Palette offset with the X mirror,Y mirror, Rotate bits if set
					// 4 bit colour bit and pattern offset bit
					// index of the sprite at this position that makes up the frame

				//...... repeated wide x tall times


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


