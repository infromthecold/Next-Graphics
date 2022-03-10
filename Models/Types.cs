using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Models
{
	public enum OutputType
	{
		Sprites,
		Tiles,
	}

	public enum PaletteType
	{
		Next256,
		Next512,
		Custom
	}

	public enum PaletteFormat
	{
		Next8Bit,
		Next9Bit
	}

	public enum FourBitParsingMethod
	{
		Manual,
		DetectPaletteBanks
	}

	public enum ImageFormat
	{
		BMP,
		PNG,
		JPG
	}

	public enum CommentType
	{
		None,
		Full
	}

	public enum TilemapExportType
	{
		AttributesIndexAsWord,
		AttributesIndexAsTwoBytes,
		IndexOnly
	}

	public enum BlockType
	{
		Original,
		Repeated,
		FlippedX,
		FlippedY,
		FlippedXY,
		Rotated,
		FlippedXRotated,
		FlippedYRotated,
		FlippedXYRotated,
		Transparent,
	}
}
