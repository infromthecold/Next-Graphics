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
		private List<Action<string>> handlers = new List<Action<string>>();

		public string Filters { get => builder.ToString(); }

		public void Add(string description, string extensions, Action<string> handler = null)
		{
			if (builder.Length > 0)
			{
				builder.Append("|");
			}

			builder.Append($"{description} ({extensions})|{extensions}");

			handlers.Add(handler);
		}

		public void Handle(int index, string filename)
		{
			if (handlers[index] != null)
			{
				handlers[index].Invoke(filename);
			}
		}
	}
}
