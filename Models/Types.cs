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

	public enum PaletteParsingMethod
	{
		/// <summary>
		/// Scans source image for distinct colours pixel line by line. Resulting palette may feel random since object colours are not grouped together. Also doesn't support auto-colour banks very well. But it was the original method so leaving it in for manual banks handling.
		/// </summary>
		ByPixels,

		/// <summary>
		/// Scans source for distinct colours image object by object. Resulting palette groups colours for each object together so it should make more "sense" at quick glance. Supports auto-colour bank mapping. This is especially useful for 4-bit colours but can also be used for 8-bit colours.
		/// </summary>
		ByObjects
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
