// Test.asm
// Created on 14 March 2022 14:03:42 by the NextGraphics tool from
// patricia curtis at luckyredfish dot com

TEST_COLOURS:		equ	24

TestPalette:
			db	%11100011
			db	%10110110
			db	%10010010
			db	%01101101
			db	%01001001
			db	%11111111
			db	%11011011
			db	%00100100
			db	%00000000
			db	%11111011
			db	%11110110
			db	%11110010
			db	%11101001
			db	%11100100
			db	%11000000
			db	%10000000
			db	%11100011
			db	%11111110
			db	%11111101
			db	%11111100
			db	%11111100
			db	%11111000
			db	%11010100
			db	%10010000

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
				.db	$56,$12,$34,$78
				.db	$56,$12,$34,$78
				.db	$56,$12,$34,$78
				.db	$56,$12,$34,$78
				.db	$56,$12,$34,$78
				.db	$56,$12,$34,$78
				.db	$00,$12,$34,$00
TestTile2:
				.db	$00,$55,$55,$00
				.db	$66,$66,$66,$66
				.db	$11,$11,$11,$11
				.db	$22,$22,$22,$22
				.db	$33,$33,$33,$33
				.db	$44,$44,$44,$44
				.db	$77,$77,$77,$77
				.db	$00,$88,$88,$00
TestTile3:
				.db	$09,$99,$99,$90
				.db	$aa,$aa,$aa,$aa
				.db	$ab,$bb,$bb,$bb
				.db	$ab,$cc,$cc,$cc
				.db	$ab,$cd,$dd,$dd
				.db	$ab,$cd,$ee,$ee
				.db	$ab,$cd,$ef,$f0
				.db	$00,$cd,$ef,$00
TestTile4:
				.db	$00,$12,$34,$50
				.db	$11,$12,$34,$56
				.db	$22,$22,$34,$56
				.db	$33,$33,$34,$56
				.db	$44,$44,$44,$56
				.db	$55,$55,$55,$56
				.db	$66,$66,$66,$66
				.db	$00,$77,$77,$00





TestBlock0:		.db	0,0
TestBlock1:		.db	0,1
TestBlock2:		.db	0,2
TestBlock3:		.db	0,3
TestBlock4:		.db	16,4


