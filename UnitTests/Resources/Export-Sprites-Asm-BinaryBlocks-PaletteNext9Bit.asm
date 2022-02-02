// sprites.asm
// Created on {0} by the NextGraphics tool from
// patricia curtis at luckyredfish dot com

SPRITES_COLOURS:		equ	5

spritesPalette:
			db	%11100111,%00000001	//	255,36,255
			db	%00000000,%00000000	//	0,0,0
			db	%11111100,%00000000	//	255,255,0
			db	%01111000,%00000000	//	109,218,0
			db	%10010111,%00000001	//	145,182,255

SPRITES_SPRITE_SIZE:		equ	256

SPRITES_SPRITES:		equ	5


				// number of sprites

					// x offset from center of sprite
					// y offset from center of sprite
					// Palette offset with the X mirror,Y mirror, Rotate bits if set
					// 4 bit colour bit and pattern offset bit
					// index of the sprite at this position that makes up the frame

				//...... repeated wide x tall times


spritesFrames:		.dw	spritesFrame0,spritesFrame1,spritesFrame2,spritesFrame3,spritesFrame4,spritesFrame5,spritesFrame6,spritesFrame7,spritesFrame8,spritesFrame9,spritesFrame10,spritesFrame11,spritesFrame12,spritesFrame13
SPRITES_FILE_SIZE	equ	1280
spritesFile:			dw	SPRITES_FILE_SIZE
			db	PATH,"game/level1/sprites.bin",0
