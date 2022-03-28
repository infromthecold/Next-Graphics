using NextGraphics.Models;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Exporting.Common
{
	public interface RemapCallbacks
	{
		bool OnRemapShowCharacterDebugData();

		void OnRemapStarted();
		void OnRemapUpdated();
		void OnRemapCompleted(bool success);

		void OnRemapDisplayChar(Point position, IndexedBitmap bitmap);
		void OnRemapDisplayBlock(Point position, IndexedBitmap bitmap);
		void OnRemapDisplayCharactersCount(int count, int transparentCount);
		void OnRemapDisplayBlocksCount(int count);

		void OnRemapWarning(string message);
		void OnRemapDebug(string message);  // Whatever you use for logging, use Write not WriteLine, newline will be appended as needed
	}
}
