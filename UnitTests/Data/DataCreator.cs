using NextGraphics.Models;

using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace UnitTests.Data
{
	/// <summary>
	/// Creates commonly used data for unit tests.
	/// </summary>
	public class DataCreator
	{
		#region MainModel

		public static MainModel LoadModel(XmlDocument document = null)
		{
			var result = new MainModel();
			var source = document != null ? document : ModelDocument();
			result.Load(source);

			return result;
		}

		public static XmlDocument ModelDocument(string outputType = "blocks", string imageFormat = "0", string paletteMapping = "Custom")
		{
			var template = @"
<XML>
  <Project>
	<Name Projectname=""Level1"" />
	<File Path=""C:\tiles.bmp"" />
	<File Path=""/a/file/with/slashes"" />
	<File Path=""\the\file\with\backslashes"" />
	<Settings {0}=""true"" center=""4"" xSize=""9"" ySize=""10"" fourBit=""true"" binary=""false"" binaryBlocks=""true"" Repeats=""false"" MirrorX=""false"" MirrorY=""true"" Rotations=""false"" Transparent=""false"" Sort=""true"" blocksImage=""false"" tilesImage=""true"" transBlock=""true"" transTile=""false"" across=""5"" accurate=""983"" format=""{1}"" textFlips=""true"" reduce=""true"" />
	<Dialogs OutputIndex=""1"" ImageIndex=""1"" />
	<Palette Mapping=""{2}"" Transparent=""0"" Used=""6"" Start=""1"">
	  <Colour0 Red=""10"" Green=""11"" Blue=""12"" />
	  <Colour1 Red=""255"" Green=""255"" Blue=""255"" />
	  <Colour2 Red=""218"" Green=""218"" Blue=""218"" />
	  <Colour3 Red=""182"" Green=""182"" Blue=""182"" />
	  <Colour4 Red=""145"" Green=""145"" Blue=""145"" />
	  <Colour5 Red=""109"" Green=""109"" Blue=""109"" />
	  <Colour6 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour7 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour8 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour9 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour10 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour11 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour12 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour13 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour14 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour15 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour16 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour17 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour18 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour19 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour20 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour21 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour22 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour23 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour24 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour25 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour26 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour27 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour28 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour29 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour30 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour31 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour32 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour33 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour34 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour35 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour36 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour37 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour38 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour39 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour40 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour41 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour42 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour43 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour44 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour45 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour46 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour47 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour48 Red=""1"" Green=""2"" Blue=""3"" />
	  <Colour49 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour50 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour51 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour52 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour53 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour54 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour55 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour56 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour57 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour58 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour59 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour60 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour61 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour62 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour63 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour64 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour65 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour66 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour67 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour68 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour69 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour70 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour71 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour72 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour73 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour74 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour75 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour76 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour77 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour78 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour79 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour80 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour81 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour82 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour83 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour84 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour85 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour86 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour87 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour88 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour89 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour90 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour91 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour92 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour93 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour94 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour95 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour96 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour97 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour98 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour99 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour100 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour101 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour102 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour103 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour104 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour105 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour106 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour107 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour108 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour109 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour110 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour111 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour112 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour113 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour114 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour115 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour116 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour117 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour118 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour119 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour120 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour121 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour122 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour123 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour124 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour125 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour126 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour127 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour128 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour129 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour130 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour131 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour132 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour133 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour134 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour135 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour136 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour137 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour138 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour139 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour140 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour141 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour142 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour143 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour144 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour145 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour146 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour147 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour148 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour149 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour150 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour151 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour152 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour153 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour154 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour155 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour156 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour157 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour158 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour159 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour160 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour161 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour162 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour163 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour164 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour165 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour166 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour167 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour168 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour169 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour170 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour171 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour172 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour173 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour174 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour175 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour176 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour177 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour178 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour179 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour180 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour181 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour182 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour183 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour184 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour185 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour186 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour187 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour188 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour189 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour190 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour191 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour192 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour193 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour194 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour195 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour196 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour197 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour198 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour199 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour200 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour201 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour202 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour203 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour204 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour205 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour206 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour207 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour208 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour209 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour210 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour211 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour212 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour213 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour214 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour215 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour216 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour217 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour218 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour219 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour220 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour221 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour222 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour223 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour224 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour225 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour226 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour227 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour228 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour229 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour230 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour231 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour232 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour233 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour234 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour235 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour236 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour237 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour238 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour239 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour240 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour241 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour242 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour243 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour244 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour245 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour246 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour247 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour248 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour249 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour250 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour251 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour252 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour253 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour254 Red=""240"" Green=""240"" Blue=""240"" />
	  <Colour255 Red=""5"" Green=""6"" Blue=""7"" />
	</Palette>
  </Project>
</XML>
";

			var xml = String.Format(template, outputType, imageFormat, paletteMapping);

			var result = new XmlDocument();
			result.LoadXml(xml);
			return result;
		}

		#endregion

		#region Assembler

		public static string AssemblerOutputTiles(DateTime time, bool embedTiles = false, bool fullComments = false)
		{
			var template = @"
// Test.asm
// Created on {0} by the NextGraphics tool from
// patricia curtis at luckyredfish dot com

TEST_COLOURS:		equ	6

TestPalette:
%CommentedPaletteStart%			db	%01001001	//	72,72,72
			db	%11111111	//	255,255,255
			db	%11011011	//	218,218,218
			db	%10110110	//	182,182,182
			db	%10010010	//	145,145,145
			db	%01101101	//	109,109,109%CommentedPaletteEnd%
%UncommentedPaletteStart%			db	%01001001
			db	%11111111
			db	%11011011
			db	%10110110
			db	%10010010
			db	%01101101%UncommentedPaletteEnd%

TEST_TILE_SIZE:		equ	32

TEST_TILES:		equ	4

%TilesDefStart%TestTile0:
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
				.db	$00,$00,$00,$00%TilesDefEnd%

%FullCommentStart				// block data
				// number of tiles (characters) tall
				// number of tiles (characters) wide
					// Palette offset with the X mirror,Y mirror, Rotate bits if set
					// index of the character at this position that makes up the block
				//...... repeated wide x tall times

				//Note: Blocks/Tiles/characters output block 0 and tile 0 is blank.

TestTileWidth:	equ	1
TestTileHeight:	equ	1%FullCommentEnd%
// Collisions Left Width Top Height

TestBlock0:		.db	0,0
TestBlock1:		.db	0,1
TestBlock2:		.db	0,2
TestBlock3:		.db	0,3

%BinaryStart%TEST_FILE_SIZE	equ	128
TestFile:			dw	TEST_FILE_SIZE
			db	PATH,""game / level1 / test.bin"",0%BinaryEnd%
";

			var result = String.Format(template, time.ToString("F", CultureInfo.CurrentCulture));

			result = RemoveMarkers(result, "%TilesDefStart%", "%TilesDefEnd%", !embedTiles);
			result = RemoveMarkers(result, "%BinaryStart%", "%BinaryEnd%", embedTiles);
			result = RemoveMarkers(result, "%FullCommentStart%", "%FullCommentEnd%", !fullComments);
			result = RemoveMarkers(result, "%CommentedPaletteStart%", "%CommentedPaletteEnd%", !fullComments);
			result = RemoveMarkers(result, "%UncommentedPaletteStart%", "%UncommentedPaletteEnd%", fullComments);

			return result;
		}

		private static string RemoveMarkers(string source, string startMarker, string endMarker, bool removeWholeSection)
		{
			// If start marker is missing, return source string.
			var startIndex = source.IndexOf(startMarker);
			if (startIndex < 0) return source;

			// If end marker is missing, or its position is before start marker, return source string.
			var endIndex = source.IndexOf(endMarker);
			if (endIndex < 0) return source;
			if (endIndex <= startIndex) return source;

			// Is whole section between start/end marker is to be removed?
			if (removeWholeSection)
			{
				// Remove all text from start of start marker till end of end marker.
				var count = endIndex + endMarker.Length - startIndex;
				return source.Remove(startIndex, count);
			}

			// If not whole section, then just remove both markers.
			return source.Replace(startMarker, "").Replace(endMarker, "");
		}

		#endregion
	}

	public static class Extensions
	{
		public static List<string> ToLines(this string source)
		{
			var result = new List<string>();

			using (var reader = new StringReader(source))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					result.Add(line);
				}
			}

			return result;
		}

		public static List<string> ToLines(this Stream stream)
		{
			using (var reader = new StreamReader(stream))
			{
				return reader.ReadToEnd().ToLines();
			}
		}
	}
}
