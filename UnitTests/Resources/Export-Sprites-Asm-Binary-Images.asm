// sprites.asm
// Created on {0} by the NextGraphics tool from
// patricia curtis at luckyredfish dot com

SPRITES_COLOURS:		equ	5

spritesPalette:
			db	%11100111	//	255,36,255
			db	%00000000	//	0,0,0
			db	%11111100	//	255,255,0
			db	%01111000	//	109,218,0
			db	%10010111	//	145,182,255

SPRITES_SPRITE_SIZE:		equ	256

SPRITES_SPRITES:		equ	5


				// number of sprites

					// x offset from center of sprite
					// y offset from center of sprite
					// Palette offset with the X mirror,Y mirror, Rotate bits if set
					// 4 bit colour bit and pattern offset bit
					// index of the sprite at this position that makes up the frame

				//...... repeated wide x tall times

// Collisions Left Width Top Height
spritesCollision0:		.db	0,15,0,15
spritesCollision1:		.db	3,10,1,13
spritesCollision2:		.db	2,11,1,13
spritesCollision3:		.db	2,11,1,13
spritesCollision4:		.db	1,13,2,10
spritesCollision5:		.db	2,10,1,13
spritesCollision6:		.db	1,13,3,10
spritesCollision7:		.db	2,10,1,13
spritesCollision8:		.db	2,10,1,13
spritesCollision9:		.db	0,15,0,1
spritesCollision10:		.db	0,15,0,1
spritesCollision11:		.db	0,15,0,1
spritesCollision12:		.db	0,15,0,1
spritesCollision13:		.db	0,15,0,1

spritesFrame0:		.db	1,	0,0,	SPRITES_OFFSET+0,	0,0
spritesFrame1:		.db	1,	0,0,	SPRITES_OFFSET+0,	0,1
spritesFrame2:		.db	1,	0,0,	SPRITES_OFFSET+0,	0,2
spritesFrame3:		.db	1,	0,0,	SPRITES_OFFSET+0,	0,3
spritesFrame4:		.db	1,	0,0,	SPRITES_OFFSET+14,	0,1
spritesFrame5:		.db	1,	0,0,	SPRITES_OFFSET+12,	0,1
spritesFrame6:		.db	1,	0,0,	SPRITES_OFFSET+2,	0,1
spritesFrame7:		.db	1,	0,0,	SPRITES_OFFSET+8,	0,1
spritesFrame8:		.db	1,	0,0,	SPRITES_OFFSET+12,	0,1
spritesFrame9:		.db	1,	0,0,	SPRITES_OFFSET+0,	0,4
spritesFrame10:		.db	1,	0,0,	SPRITES_OFFSET+0,	0,4
spritesFrame11:		.db	1,	0,0,	SPRITES_OFFSET+0,	0,4
spritesFrame12:		.db	1,	0,0,	SPRITES_OFFSET+0,	0,4
spritesFrame13:		.db	1,	0,0,	SPRITES_OFFSET+0,	0,4

spritesFrames:		.dw	spritesFrame0,spritesFrame1,spritesFrame2,spritesFrame3,spritesFrame4,spritesFrame5,spritesFrame6,spritesFrame7,spritesFrame8,spritesFrame9,spritesFrame10,spritesFrame11,spritesFrame12,spritesFrame13
SPRITES_FILE_SIZE	equ	1280
spritesFile:			dw	SPRITES_FILE_SIZE
			db	PATH,"game/level1/sprites.bin",0
