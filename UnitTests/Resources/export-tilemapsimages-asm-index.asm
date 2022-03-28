// Test.asm
// Created on 25 March 2022 10:57:08 by the NextGraphics tool from
// patricia curtis at luckyredfish dot com

TEST_COLOURS:		equ	24

TestPalette:
			db	%11100011	//	255,0,255
			db	%10110110	//	182,182,182
			db	%10010010	//	145,145,145
			db	%01101101	//	109,109,109
			db	%01001001	//	72,72,72
			db	%11111111	//	255,255,255
			db	%11011011	//	218,218,218
			db	%00100100	//	36,36,36
			db	%00000000	//	0,0,0
			db	%11111011	//	255,218,218
			db	%11110110	//	255,182,182
			db	%11110010	//	255,145,145
			db	%11101001	//	255,72,72
			db	%11100100	//	255,36,36
			db	%11000000	//	218,0,0
			db	%10000000	//	145,0,0
			db	%11100011	//	255,0,255
			db	%11111110	//	255,255,182
			db	%11111101	//	255,255,72
			db	%11111100	//	255,255,36
			db	%11111100	//	255,255,0
			db	%11111000	//	255,218,0
			db	%11010100	//	218,182,0
			db	%10010000	//	145,145,0

TEST_TILE_SIZE:		equ	32

TEST_TILES:		equ	4

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
				.db	$61,$23,$77,$88
				.db	$00,$88,$88,$00
TestTile2:
				.db	$09,$99,$99,$90
				.db	$aa,$aa,$aa,$aa
				.db	$ab,$bb,$bb,$bb
				.db	$ab,$cc,$cc,$cc
				.db	$ab,$cd,$dd,$dd
				.db	$ab,$cd,$ee,$ee
				.db	$bc,$de,$ef,$f0
				.db	$00,$ff,$ff,$00
TestTile3:
				.db	$00,$12,$34,$50
				.db	$21,$12,$34,$56
				.db	$32,$22,$34,$56
				.db	$43,$33,$34,$56
				.db	$54,$44,$44,$56
				.db	$65,$55,$55,$56
				.db	$77,$66,$66,$66
				.db	$00,$77,$77,$00

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
TestBlock3:		.db	16,3

			// Each tilemap defines width and height (in tiles), total size in bytes and list of all tiles.
TEST_TILEMAPS:			equ	1

TEST_TILEMAP0_WIDTH:		equ	8
TEST_TILEMAP0_HEIGHT:		equ	4
TEST_TILEMAP0_SIZE:		equ	32
Test_Tilemap0:
			// Each tile is defined as single byte, representing the tile index.
			db	$01,$01,$01,$01,$01,$01,$01,$01
			db	$02,$02,$02,$02,$02,$02,$02,$02
			db	$03,$03,$03,$03,$03,$03,$03,$03
			db	$00,$00,$00,$00,$00,$00,$00,$00

