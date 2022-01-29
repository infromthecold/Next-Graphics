// Test.asm
// Created on {0} by the NextGraphics tool from
// patricia curtis at luckyredfish dot com

TEST_COLOURS:		equ	6

TestPalette:
			db	%01001001,%00000000	//	72,72,72
			db	%11111111,%00000001	//	255,255,255
			db	%11011011,%00000000	//	218,218,218
			db	%10110110,%00000001	//	182,182,182
			db	%10010010,%00000000	//	145,145,145
			db	%01101101,%00000001	//	109,109,109

TEST_TILE_SIZE:		equ	32

TEST_TILES:		equ	4

TestTile0:
				.db	$00,$11,$11,$00
				.db	$01,$22,$22,$10
				.db	$12,$22,$22,$22
				.db	$12,$14,$22,$23
				.db	$32,$22,$22,$40
				.db	$04,$33,$54,$45
				.db	$05,$44,$44,$55
				.db	$00,$55,$55,$50
TestTile1:
				.db	$01,$11,$11,$00
				.db	$12,$22,$22,$10
				.db	$12,$22,$12,$23
				.db	$22,$42,$22,$24
				.db	$32,$22,$32,$35
				.db	$44,$33,$43,$45
				.db	$05,$44,$54,$50
				.db	$00,$55,$55,$00
TestTile2:
				.db	$00,$11,$11,$00
				.db	$01,$32,$22,$10
				.db	$12,$22,$22,$23
				.db	$32,$21,$32,$35
				.db	$04,$35,$43,$45
				.db	$05,$44,$44,$50
				.db	$00,$55,$55,$00
				.db	$00,$00,$00,$00
TestTile3:
				.db	$00,$00,$00,$00
				.db	$00,$11,$10,$00
				.db	$01,$32,$21,$10
				.db	$32,$42,$52,$33
				.db	$54,$33,$33,$45
				.db	$05,$54,$44,$50
				.db	$00,$05,$55,$00
				.db	$00,$00,$00,$00

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

