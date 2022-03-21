using NextGraphics.Utils;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NextGraphics.Models
{
	/// <summary>
	/// Support for text based tilemaps. Doesn't respect <see cref="MainModel"/> settings nor supports any tile attributes (flipping or rotation).
	/// 
	/// Requires tiles image from preferred bitmap editor exactly in the order the tiles will be exported from Next Graphics (ideally that image is attached to the project as source image).
	/// 
	/// The format requires each text row to contain comma delimited decimal numbers representing tile indices. Each row is required to have exact same number of numbers (aka columns). Therefore the first row is used to determine the number of columns. All whitespace is ignored however, including empty lines in betwen.
	/// 
	/// Example of 2x2 tilemap:
	/// 1,2
	/// 3,4
	/// </summary>
	public class SourceTilemapText : SourceTilemap
	{
		#region Initialization & Disposal

		public SourceTilemapText(string filename, MainModel model) : base(filename, model)
		{
		}

		public SourceTilemapText(string filename, MainModel model, TilemapData data) : base(filename, model, data)
		{
		}

		#endregion

		#region Overrides

		protected override TilemapData OnLoadDataFromFile(string filename, MainModel model)
		{
			try
			{
				var width = 0;

				var lines = new List<List<int>>();

				foreach (var line in File.ReadAllLines(filename))
				{
					// Skip empty lines.
					var trimmedLine = line.Trim();
					if (trimmedLine.Length == 0) continue;

					// Convert the line into array of numbers.
					var columns = LineToNumbers(line);

					// If this is the first line, assign width, otherwise ensure the width of subsequent lines matches.
					if (width == 0)
					{
						width = columns.Count;
					}
					else if (columns.Count != width)
					{
						throw new InvalidDataException($"Line {lines.Count + 1} has {columns.Count} columns, expected {width}");
					}

					// If all is well, add new line.
					lines.Add(columns);
				}

				// Prepare tilemap data structure.
				var result = new TilemapData(width, lines.Count);
				var y = 0;
				foreach (var row in lines)
				{
					var x = 0;
					foreach (var column in row)
					{
						result.SetTile(x, y, new TilemapData.Tile
						{
							Index = column
						});
						x++;
					}

					y++;
				}
				return result;
			}
			catch
			{
				return null;
			}
		}

		#endregion

		#region Helpers

		private List<int> LineToNumbers(string line)
		{
			return line
				.Split(',')
				.Select(x => int.Parse(x))
				.ToList();
		}

		#endregion
	}
}
