using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Models
{
	public class Colour
	{
		public byte A = 0;
		public byte R = 0;
		public byte G = 0;
		public byte B = 0;

		public Int32 getARGB()
		{
			return A << 24 | R << 18 | G << 8 | B;
		}
	}
}
