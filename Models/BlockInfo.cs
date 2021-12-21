using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Models
{
	/// <summary>
	/// Represents data for individual "block".
	/// </summary>
	public class BlockInfo
	{
		public short OriginalID { get; set; }
		public Point Position { get; set; }
		public short PaletteOffset { get; set; }
		public bool Repeated { get; set; }
		public bool FlippedX { get; set; }
		public bool FlippedY { get; set; }
		public bool Transparent { get; set; }
		public bool Rotated { get; set; }
		public bool SecondHalf { get; set; }
		public bool HasTransparent { get; set; }

		public BlockInfo()
		{
			Position = new Point();
			Repeated = false;
			FlippedX = false;
			FlippedY = false;
			Rotated = false;
			OriginalID = 0;
			SecondHalf = false;
			PaletteOffset = 0;
			HasTransparent = false;
		}
	}

}
