// Test.asm
// Created on 14 March 2022 14:55:31 by the NextGraphics tool from
// patricia curtis at luckyredfish dot com

TEST_COLOURS:		equ	23

TestPalette:
			db	%11100011,%00000001	//	255,0,255
			db	%10110110,%00000001	//	182,182,182
			db	%10010010,%00000000	//	145,145,145
			db	%01101101,%00000001	//	109,109,109
			db	%01001001,%00000000	//	72,72,72
			db	%11111111,%00000001	//	255,255,255
			db	%11111011,%00000000	//	255,218,218
			db	%11111110,%00000001	//	255,255,182
			db	%11111101,%00000000	//	255,255,72
			db	%11111100,%00000001	//	255,255,36
			db	%11111100,%00000000	//	255,255,0
			db	%11111000,%00000000	//	255,218,0
			db	%11011011,%00000000	//	218,218,218
			db	%00100100,%00000001	//	36,36,36
			db	%00000000,%00000000	//	0,0,0
			db	%11110110,%00000001	//	255,182,182
			db	%11010100,%00000000	//	218,182,0
			db	%11110010,%00000000	//	255,145,145
			db	%11101001,%00000000	//	255,72,72
			db	%11100100,%00000001	//	255,36,36
			db	%11000000,%00000000	//	218,0,0
			db	%10000000,%00000000	//	145,0,0
			db	%10010000,%00000000	//	145,145,0

TEST_TILE_SIZE:		equ	32

TEST_TILES:		equ	5

TestTile0:
				.db	$00,$00,$00,$00
				.db	$00,$00,$00,$00
				.db	$00,$00,$00,$00
				.db	$00,$00,$00,$00
				.db	$00,$00,$00,$00
				.db	$00,$00,$00,$00
				.db	$00,$00,$00,$00
				.db	$00,$00,$00,$00
TestTile1:
				.db	$00,$12,$34,$00
				.db	$5c,$12,$34,$de
				.db	$5c,$12,$34,$de
				.db	$5c,$12,$34,$de
				.db	$5c,$12,$34,$de
				.db	$5c,$12,$34,$de
				.db	$5c,$12,$34,$de
				.db	$00,$12,$34,$00
TestTile2:
				.db	$00,$55,$55,$00
				.db	$cc,$cc,$cc,$cc
				.db	$11,$11,$11,$11
				.db	$22,$22,$22,$22
				.db	$33,$33,$33,$33
				.db	$44,$44,$44,$44
				.db	$dd,$dd,$dd,$dd
				.db	$00,$ee,$ee,$00
TestTile3:
				.db	$06,$66,$66,$60
				.db	$ff,$ff,$ff,$ff
				.db	$ff,$ff,$ff,$ff
				.db	$ff,$22,$22,$22
				.db	$ff,$23,$33,$33
				.db	$ff,$23,$44,$44
				.db	$ff,$23,$4d,$d0
				.db	$00,$23,$4d,$00
TestTile4:
				.db	$00,$78,$9a,$b0
				.db	$77,$78,$9a,$bb
				.db	$88,$88,$9a,$bb
				.db	$99,$99,$9a,$bb
				.db	$aa,$aa,$aa,$bb
				.db	$bb,$bb,$bb,$bb
				.db	$bb,$bb,$bb,$bb
				.db	$00,$33,$33,$00

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
TestBlock4:		.db	0,4


