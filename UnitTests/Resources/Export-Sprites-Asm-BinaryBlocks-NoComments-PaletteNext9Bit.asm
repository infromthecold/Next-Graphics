// sprites.asm
// Created on {0} by the NextGraphics tool from
// patricia curtis at luckyredfish dot com

SPRITES_COLOURS:		equ	5

spritesPalette:
			db	%11100111,%00000001
			db	%00000000,%00000000
			db	%11111100,%00000000
			db	%01111000,%00000000
			db	%10010111,%00000001

SPRITES_SPRITE_SIZE:		equ	256

SPRITES_SPRITES:		equ	5


spritesFrames:		.dw	spritesFrame0,spritesFrame1,spritesFrame2,spritesFrame3,spritesFrame4,spritesFrame5,spritesFrame6,spritesFrame7,spritesFrame8,spritesFrame9,spritesFrame10,spritesFrame11,spritesFrame12,spritesFrame13
SPRITES_FILE_SIZE	equ	1280
spritesFile:			dw	SPRITES_FILE_SIZE
			db	PATH,"game/level1/sprites.bin",0
