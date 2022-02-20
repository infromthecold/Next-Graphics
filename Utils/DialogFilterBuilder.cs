using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Utils
{
	public class DialogFilterBuilder
	{
		private StringBuilder builder = new StringBuilder();

		public string Filters { get => builder.ToString(); }

		public void Add(string description, string extensions)
		{
			if (builder.Length > 0)
			{
				builder.Append("|");
			}

			builder.Append($"{description} ({extensions})|{extensions}");
		}
	}
}
