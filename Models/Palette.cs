﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextGraphics.Models
{
	public class Palette
	{
		private PaletteType _type = PaletteType.Next256;
		public PaletteType Type { 
			get => _type; 
			set {
				if (_type == value) return;

				_type = value;

				switch (value)
				{
					case PaletteType.Next256: Colours = FromDefaults(SpecNext256); break;
					case PaletteType.Next512: Colours = FromDefaults(SpecNext512); break;
					case PaletteType.Custom: Colours = customColours; break;
				}
			}
		}

		public int TransparentIndex { get; set; } = 0;
		public int StartIndex { get; set; } = 0;
		public int UsedCount { get; set; } = 255;
		public int LastValidIndex { get => StartIndex + UsedCount; }

		public List<Colour> Colours { get; set; } = FromDefaults(SpecNext256);

		/// <summary>
		/// Custom colours are maintained separately so we can easily switch between palettes.
		/// </summary>
		private List<Colour> customColours = CustomDefaults();

		/// <summary>
		/// Indexer for a shortcut, so we can use Palette as a list - Palette[i] instead of Palette.Colours[i]
		/// </summary>
		public Colour this[int index] => Colours[index];

		/// <summary>
		/// Indexes for a shortuct to individual colour component, allows reading palette as 2 dimensional array.
		/// </summary>
		public byte this[int index, int component] => Colours[index][component];

		#region Helpers

		public void Clear()
		{
			Type = PaletteType.Next256;
		}

		public void ReplaceColours(int index1, int index2)
		{
			for (var component = 0; component < 3; component++)
			{
				byte original = this[index1][component];
				this[index2][component] = this[index1][component];
				this[index1][component] = original;
			}
		}

		public short ClosestColor(Color color, short reMap, int startColour)
		{
			short result = -1;
			int biggestDifference = 1000;
			if (reMap < 0)
			{
				switch (Type)
				{
					case PaletteType.Next256:
						for (short i = 0; i < 256; i++)
						{
							if (Math.Sqrt(Math.Pow(color.R - SpecNext256[i, 0], 2) + Math.Pow(color.G - SpecNext256[i, 1], 2) + Math.Pow(color.B - SpecNext256[i, 2], 2)) < biggestDifference)
							{
								result = i;
								biggestDifference = (int)Math.Sqrt(Math.Pow(color.R - SpecNext256[i, 0], 2) + Math.Pow(color.G - SpecNext256[i, 1], 2) + Math.Pow(color.B - SpecNext256[i, 2], 2));
							}
						}
						break;
					case PaletteType.Next512:
						for (short i = 0; i < 512; i++)
						{
							if (Math.Sqrt(Math.Pow(color.R - SpecNext512[i, 0], 2) + Math.Pow(color.G - SpecNext512[i, 1], 2) + Math.Pow(color.B - SpecNext512[i, 2], 2)) < biggestDifference)
							{
								result = i;
								biggestDifference = (int)Math.Sqrt(Math.Pow(color.R - SpecNext512[i, 0], 2) + Math.Pow(color.G - SpecNext512[i, 1], 2) + Math.Pow(color.B - SpecNext512[i, 2], 2));
							}
						}
						break;
					case PaletteType.Custom:
						for (short i = 0; i < UsedCount; i++)
						{
							if (Math.Sqrt(Math.Pow(color.R - this[startColour + i, 0], 2) + Math.Pow(color.G - this[startColour + i, 1], 2) + Math.Pow(color.B - this[startColour + i, 2], 2)) < biggestDifference)
							{
								result = (short)(i + startColour);
								biggestDifference = (int)Math.Sqrt(Math.Pow(color.R - this[startColour + i, 0], 2) + Math.Pow(color.G - this[startColour + i, 1], 2) + Math.Pow(color.B - this[startColour + i, 2], 2));
							}
						}
						break;
				}
			}
			else
			{
				for (short i = reMap; i < reMap + 16; i++)
				{
					if (Math.Sqrt(Math.Pow(color.R - this[startColour + i, 0], 2) + Math.Pow(color.G - this[startColour + i, 1], 2) + Math.Pow(color.B - this[startColour + i, 2], 2)) < biggestDifference)
					{
						result = (short)(i + startColour);
						biggestDifference = (int)Math.Sqrt(Math.Pow(color.R - this[startColour + i, 0], 2) + Math.Pow(color.G - this[startColour + i, 1], 2) + Math.Pow(color.B - this[startColour + i, 2], 2));
					}
				}
			}
			return result;
		}

		private static List<Colour> FromDefaults(byte[,] colours)
		{
			var result = new List<Colour>();

			// We always maintain 256 colours, if default array is longer, we take every second, third etc. based on amound of colours present. Assumption here is that all palettes have 256 or its multiple.
			var count = colours.Length / 3;	// we have 3 component for each colour
			var delta = count / 256;		// 1 for 256, 2 for 512
			var i = delta - 1;				// 0,2,4... for 256, 1,3,5... for 512
			while (i < count)
			{
				result.Add(new Colour(colours[i, 0], colours[i, 1], colours[i, 2]));

				i += delta;
			}

			return result;
		}

		private static List<Colour> CustomDefaults()
		{
			var result = new List<Colour>();

			for (int i = 0; i < 256; i++)
			{
				result.Add(new Colour(230, 230, 230));
			}

			return result;
		}

		#endregion

		#region Default colours

		public static byte[,] SpecNext512 = {
			{0x00,0x00,0x00},{0x00,0x00,0x24},{0x00,0x00,0x49},{0x00,0x00,0x6D},{0x00,0x00,0x92},{0x00,0x00,0xB6},{0x00,0x00,0xDB},{0x00,0x00,0xFF},{0x00,0x24,0x00},{0x00,0x24,0x24},{0x00,0x24,0x49},{0x00,0x24,0x6D},{0x00,0x24,0x92},{0x00,0x24,0xB6},{0x00,0x24,0xDB},{0x00,0x24,0xFF},
			{0x00,0x49,0x00},{0x00,0x49,0x24},{0x00,0x49,0x49},{0x00,0x49,0x6D},{0x00,0x49,0x92},{0x00,0x49,0xB6},{0x00,0x49,0xDB},{0x00,0x49,0xFF},{0x00,0x6D,0x00},{0x00,0x6D,0x24},{0x00,0x6D,0x49},{0x00,0x6D,0x6D},{0x00,0x6D,0x92},{0x00,0x6D,0xB6},{0x00,0x6D,0xDB},{0x00,0x6D,0xFF},
			{0x00,0x92,0x00},{0x00,0x92,0x24},{0x00,0x92,0x49},{0x00,0x92,0x6D},{0x00,0x92,0x92},{0x00,0x92,0xB6},{0x00,0x92,0xDB},{0x00,0x92,0xFF},{0x00,0xB6,0x00},{0x00,0xB6,0x24},{0x00,0xB6,0x49},{0x00,0xB6,0x6D},{0x00,0xB6,0x92},{0x00,0xB6,0xB6},{0x00,0xB6,0xDB},{0x00,0xB6,0xFF},
			{0x00,0xDB,0x00},{0x00,0xDB,0x24},{0x00,0xDB,0x49},{0x00,0xDB,0x6D},{0x00,0xDB,0x92},{0x00,0xDB,0xB6},{0x00,0xDB,0xDB},{0x00,0xDB,0xFF},{0x00,0xFF,0x00},{0x00,0xFF,0x24},{0x00,0xFF,0x49},{0x00,0xFF,0x6D},{0x00,0xFF,0x92},{0x00,0xFF,0xB6},{0x00,0xFF,0xDB},{0x00,0xFF,0xFF},
			{0x24,0x00,0x00},{0x24,0x00,0x24},{0x24,0x00,0x49},{0x24,0x00,0x6D},{0x24,0x00,0x92},{0x24,0x00,0xB6},{0x24,0x00,0xDB},{0x24,0x00,0xFF},{0x24,0x24,0x00},{0x24,0x24,0x24},{0x24,0x24,0x49},{0x24,0x24,0x6D},{0x24,0x24,0x92},{0x24,0x24,0xB6},{0x24,0x24,0xDB},{0x24,0x24,0xFF},
			{0x24,0x49,0x00},{0x24,0x49,0x24},{0x24,0x49,0x49},{0x24,0x49,0x6D},{0x24,0x49,0x92},{0x24,0x49,0xB6},{0x24,0x49,0xDB},{0x24,0x49,0xFF},{0x24,0x6D,0x00},{0x24,0x6D,0x24},{0x24,0x6D,0x49},{0x24,0x6D,0x6D},{0x24,0x6D,0x92},{0x24,0x6D,0xB6},{0x24,0x6D,0xDB},{0x24,0x6D,0xFF},
			{0x24,0x92,0x00},{0x24,0x92,0x24},{0x24,0x92,0x49},{0x24,0x92,0x6D},{0x24,0x92,0x92},{0x24,0x92,0xB6},{0x24,0x92,0xDB},{0x24,0x92,0xFF},{0x24,0xB6,0x00},{0x24,0xB6,0x24},{0x24,0xB6,0x49},{0x24,0xB6,0x6D},{0x24,0xB6,0x92},{0x24,0xB6,0xB6},{0x24,0xB6,0xDB},{0x24,0xB6,0xFF},
			{0x24,0xDB,0x00},{0x24,0xDB,0x24},{0x24,0xDB,0x49},{0x24,0xDB,0x6D},{0x24,0xDB,0x92},{0x24,0xDB,0xB6},{0x24,0xDB,0xDB},{0x24,0xDB,0xFF},{0x24,0xFF,0x00},{0x24,0xFF,0x24},{0x24,0xFF,0x49},{0x24,0xFF,0x6D},{0x24,0xFF,0x92},{0x24,0xFF,0xB6},{0x24,0xFF,0xDB},{0x24,0xFF,0xFF},
			{0x49,0x00,0x00},{0x49,0x00,0x24},{0x49,0x00,0x49},{0x49,0x00,0x6D},{0x49,0x00,0x92},{0x49,0x00,0xB6},{0x49,0x00,0xDB},{0x49,0x00,0xFF},{0x49,0x24,0x00},{0x49,0x24,0x24},{0x49,0x24,0x49},{0x49,0x24,0x6D},{0x49,0x24,0x92},{0x49,0x24,0xB6},{0x49,0x24,0xDB},{0x49,0x24,0xFF},
			{0x49,0x49,0x00},{0x49,0x49,0x24},{0x49,0x49,0x49},{0x49,0x49,0x6D},{0x49,0x49,0x92},{0x49,0x49,0xB6},{0x49,0x49,0xDB},{0x49,0x49,0xFF},{0x49,0x6D,0x00},{0x49,0x6D,0x24},{0x49,0x6D,0x49},{0x49,0x6D,0x6D},{0x49,0x6D,0x92},{0x49,0x6D,0xB6},{0x49,0x6D,0xDB},{0x49,0x6D,0xFF},
			{0x49,0x92,0x00},{0x49,0x92,0x24},{0x49,0x92,0x49},{0x49,0x92,0x6D},{0x49,0x92,0x92},{0x49,0x92,0xB6},{0x49,0x92,0xDB},{0x49,0x92,0xFF},{0x49,0xB6,0x00},{0x49,0xB6,0x24},{0x49,0xB6,0x49},{0x49,0xB6,0x6D},{0x49,0xB6,0x92},{0x49,0xB6,0xB6},{0x49,0xB6,0xDB},{0x49,0xB6,0xFF},
			{0x49,0xDB,0x00},{0x49,0xDB,0x24},{0x49,0xDB,0x49},{0x49,0xDB,0x6D},{0x49,0xDB,0x92},{0x49,0xDB,0xB6},{0x49,0xDB,0xDB},{0x49,0xDB,0xFF},{0x49,0xFF,0x00},{0x49,0xFF,0x24},{0x49,0xFF,0x49},{0x49,0xFF,0x6D},{0x49,0xFF,0x92},{0x49,0xFF,0xB6},{0x49,0xFF,0xDB},{0x49,0xFF,0xFF},
			{0x6D,0x00,0x00},{0x6D,0x00,0x24},{0x6D,0x00,0x49},{0x6D,0x00,0x6D},{0x6D,0x00,0x92},{0x6D,0x00,0xB6},{0x6D,0x00,0xDB},{0x6D,0x00,0xFF},{0x6D,0x24,0x00},{0x6D,0x24,0x24},{0x6D,0x24,0x49},{0x6D,0x24,0x6D},{0x6D,0x24,0x92},{0x6D,0x24,0xB6},{0x6D,0x24,0xDB},{0x6D,0x24,0xFF},
			{0x6D,0x49,0x00},{0x6D,0x49,0x24},{0x6D,0x49,0x49},{0x6D,0x49,0x6D},{0x6D,0x49,0x92},{0x6D,0x49,0xB6},{0x6D,0x49,0xDB},{0x6D,0x49,0xFF},{0x6D,0x6D,0x00},{0x6D,0x6D,0x24},{0x6D,0x6D,0x49},{0x6D,0x6D,0x6D},{0x6D,0x6D,0x92},{0x6D,0x6D,0xB6},{0x6D,0x6D,0xDB},{0x6D,0x6D,0xFF},
			{0x6D,0x92,0x00},{0x6D,0x92,0x24},{0x6D,0x92,0x49},{0x6D,0x92,0x6D},{0x6D,0x92,0x92},{0x6D,0x92,0xB6},{0x6D,0x92,0xDB},{0x6D,0x92,0xFF},{0x6D,0xB6,0x00},{0x6D,0xB6,0x24},{0x6D,0xB6,0x49},{0x6D,0xB6,0x6D},{0x6D,0xB6,0x92},{0x6D,0xB6,0xB6},{0x6D,0xB6,0xDB},{0x6D,0xB6,0xFF},
			{0x6D,0xDB,0x00},{0x6D,0xDB,0x24},{0x6D,0xDB,0x49},{0x6D,0xDB,0x6D},{0x6D,0xDB,0x92},{0x6D,0xDB,0xB6},{0x6D,0xDB,0xDB},{0x6D,0xDB,0xFF},{0x6D,0xFF,0x00},{0x6D,0xFF,0x24},{0x6D,0xFF,0x49},{0x6D,0xFF,0x6D},{0x6D,0xFF,0x92},{0x6D,0xFF,0xB6},{0x6D,0xFF,0xDB},{0x6D,0xFF,0xFF},
			{0x92,0x00,0x00},{0x92,0x00,0x24},{0x92,0x00,0x49},{0x92,0x00,0x6D},{0x92,0x00,0x92},{0x92,0x00,0xB6},{0x92,0x00,0xDB},{0x92,0x00,0xFF},{0x92,0x24,0x00},{0x92,0x24,0x24},{0x92,0x24,0x49},{0x92,0x24,0x6D},{0x92,0x24,0x92},{0x92,0x24,0xB6},{0x92,0x24,0xDB},{0x92,0x24,0xFF},
			{0x92,0x49,0x00},{0x92,0x49,0x24},{0x92,0x49,0x49},{0x92,0x49,0x6D},{0x92,0x49,0x92},{0x92,0x49,0xB6},{0x92,0x49,0xDB},{0x92,0x49,0xFF},{0x92,0x6D,0x00},{0x92,0x6D,0x24},{0x92,0x6D,0x49},{0x92,0x6D,0x6D},{0x92,0x6D,0x92},{0x92,0x6D,0xB6},{0x92,0x6D,0xDB},{0x92,0x6D,0xFF},
			{0x92,0x92,0x00},{0x92,0x92,0x24},{0x92,0x92,0x49},{0x92,0x92,0x6D},{0x92,0x92,0x92},{0x92,0x92,0xB6},{0x92,0x92,0xDB},{0x92,0x92,0xFF},{0x92,0xB6,0x00},{0x92,0xB6,0x24},{0x92,0xB6,0x49},{0x92,0xB6,0x6D},{0x92,0xB6,0x92},{0x92,0xB6,0xB6},{0x92,0xB6,0xDB},{0x92,0xB6,0xFF},
			{0x92,0xDB,0x00},{0x92,0xDB,0x24},{0x92,0xDB,0x49},{0x92,0xDB,0x6D},{0x92,0xDB,0x92},{0x92,0xDB,0xB6},{0x92,0xDB,0xDB},{0x92,0xDB,0xFF},{0x92,0xFF,0x00},{0x92,0xFF,0x24},{0x92,0xFF,0x49},{0x92,0xFF,0x6D},{0x92,0xFF,0x92},{0x92,0xFF,0xB6},{0x92,0xFF,0xDB},{0x92,0xFF,0xFF},
			{0xB6,0x00,0x00},{0xB6,0x00,0x24},{0xB6,0x00,0x49},{0xB6,0x00,0x6D},{0xB6,0x00,0x92},{0xB6,0x00,0xB6},{0xB6,0x00,0xDB},{0xB6,0x00,0xFF},{0xB6,0x24,0x00},{0xB6,0x24,0x24},{0xB6,0x24,0x49},{0xB6,0x24,0x6D},{0xB6,0x24,0x92},{0xB6,0x24,0xB6},{0xB6,0x24,0xDB},{0xB6,0x24,0xFF},
			{0xB6,0x49,0x00},{0xB6,0x49,0x24},{0xB6,0x49,0x49},{0xB6,0x49,0x6D},{0xB6,0x49,0x92},{0xB6,0x49,0xB6},{0xB6,0x49,0xDB},{0xB6,0x49,0xFF},{0xB6,0x6D,0x00},{0xB6,0x6D,0x24},{0xB6,0x6D,0x49},{0xB6,0x6D,0x6D},{0xB6,0x6D,0x92},{0xB6,0x6D,0xB6},{0xB6,0x6D,0xDB},{0xB6,0x6D,0xFF},
			{0xB6,0x92,0x00},{0xB6,0x92,0x24},{0xB6,0x92,0x49},{0xB6,0x92,0x6D},{0xB6,0x92,0x92},{0xB6,0x92,0xB6},{0xB6,0x92,0xDB},{0xB6,0x92,0xFF},{0xB6,0xB6,0x00},{0xB6,0xB6,0x24},{0xB6,0xB6,0x49},{0xB6,0xB6,0x6D},{0xB6,0xB6,0x92},{0xB6,0xB6,0xB6},{0xB6,0xB6,0xDB},{0xB6,0xB6,0xFF},
			{0xB6,0xDB,0x00},{0xB6,0xDB,0x24},{0xB6,0xDB,0x49},{0xB6,0xDB,0x6D},{0xB6,0xDB,0x92},{0xB6,0xDB,0xB6},{0xB6,0xDB,0xDB},{0xB6,0xDB,0xFF},{0xB6,0xFF,0x00},{0xB6,0xFF,0x24},{0xB6,0xFF,0x49},{0xB6,0xFF,0x6D},{0xB6,0xFF,0x92},{0xB6,0xFF,0xB6},{0xB6,0xFF,0xDB},{0xB6,0xFF,0xFF},
			{0xDB,0x00,0x00},{0xDB,0x00,0x24},{0xDB,0x00,0x49},{0xDB,0x00,0x6D},{0xDB,0x00,0x92},{0xDB,0x00,0xB6},{0xDB,0x00,0xDB},{0xDB,0x00,0xFF},{0xDB,0x24,0x00},{0xDB,0x24,0x24},{0xDB,0x24,0x49},{0xDB,0x24,0x6D},{0xDB,0x24,0x92},{0xDB,0x24,0xB6},{0xDB,0x24,0xDB},{0xDB,0x24,0xFF},
			{0xDB,0x49,0x00},{0xDB,0x49,0x24},{0xDB,0x49,0x49},{0xDB,0x49,0x6D},{0xDB,0x49,0x92},{0xDB,0x49,0xB6},{0xDB,0x49,0xDB},{0xDB,0x49,0xFF},{0xDB,0x6D,0x00},{0xDB,0x6D,0x24},{0xDB,0x6D,0x49},{0xDB,0x6D,0x6D},{0xDB,0x6D,0x92},{0xDB,0x6D,0xB6},{0xDB,0x6D,0xDB},{0xDB,0x6D,0xFF},
			{0xDB,0x92,0x00},{0xDB,0x92,0x24},{0xDB,0x92,0x49},{0xDB,0x92,0x6D},{0xDB,0x92,0x92},{0xDB,0x92,0xB6},{0xDB,0x92,0xDB},{0xDB,0x92,0xFF},{0xDB,0xB6,0x00},{0xDB,0xB6,0x24},{0xDB,0xB6,0x49},{0xDB,0xB6,0x6D},{0xDB,0xB6,0x92},{0xDB,0xB6,0xB6},{0xDB,0xB6,0xDB},{0xDB,0xB6,0xFF},
			{0xDB,0xDB,0x00},{0xDB,0xDB,0x24},{0xDB,0xDB,0x49},{0xDB,0xDB,0x6D},{0xDB,0xDB,0x92},{0xDB,0xDB,0xB6},{0xDB,0xDB,0xDB},{0xDB,0xDB,0xFF},{0xDB,0xFF,0x00},{0xDB,0xFF,0x24},{0xDB,0xFF,0x49},{0xDB,0xFF,0x6D},{0xDB,0xFF,0x92},{0xDB,0xFF,0xB6},{0xDB,0xFF,0xDB},{0xDB,0xFF,0xFF},
			{0xFF,0x00,0x00},{0xFF,0x00,0x24},{0xFF,0x00,0x49},{0xFF,0x00,0x6D},{0xFF,0x00,0x92},{0xFF,0x00,0xB6},{0xFF,0x00,0xDB},{0xFF,0x00,0xFF},{0xFF,0x24,0x00},{0xFF,0x24,0x24},{0xFF,0x24,0x49},{0xFF,0x24,0x6D},{0xFF,0x24,0x92},{0xFF,0x24,0xB6},{0xFF,0x24,0xDB},{0xFF,0x24,0xFF},
			{0xFF,0x49,0x00},{0xFF,0x49,0x24},{0xFF,0x49,0x49},{0xFF,0x49,0x6D},{0xFF,0x49,0x92},{0xFF,0x49,0xB6},{0xFF,0x49,0xDB},{0xFF,0x49,0xFF},{0xFF,0x6D,0x00},{0xFF,0x6D,0x24},{0xFF,0x6D,0x49},{0xFF,0x6D,0x6D},{0xFF,0x6D,0x92},{0xFF,0x6D,0xB6},{0xFF,0x6D,0xDB},{0xFF,0x6D,0xFF},
			{0xFF,0x92,0x00},{0xFF,0x92,0x24},{0xFF,0x92,0x49},{0xFF,0x92,0x6D},{0xFF,0x92,0x92},{0xFF,0x92,0xB6},{0xFF,0x92,0xDB},{0xFF,0x92,0xFF},{0xFF,0xB6,0x00},{0xFF,0xB6,0x24},{0xFF,0xB6,0x49},{0xFF,0xB6,0x6D},{0xFF,0xB6,0x92},{0xFF,0xB6,0xB6},{0xFF,0xB6,0xDB},{0xFF,0xB6,0xFF},
			{0xFF,0xDB,0x00},{0xFF,0xDB,0x24},{0xFF,0xDB,0x49},{0xFF,0xDB,0x6D},{0xFF,0xDB,0x92},{0xFF,0xDB,0xB6},{0xFF,0xDB,0xDB},{0xFF,0xDB,0xFF},{0xFF,0xFF,0x00},{0xFF,0xFF,0x24},{0xFF,0xFF,0x49},{0xFF,0xFF,0x6D},{0xFF,0xFF,0x92},{0xFF,0xFF,0xB6},{0xFF,0xFF,0xDB},{0xFF,0xFF,0xFF}};

		public static byte[,] SpecNext256 = {
			{0x00,0x00,0x00},{0x00,0x00,0x6D},{0x00,0x00,0xB6},{0x00,0x00,0xFF},{0x00,0x24,0x00},{0x00,0x24,0x6D},{0x00,0x24,0xB6},{0x00,0x24,0xFF},{0x00,0x49,0x00},{0x00,0x49,0x6D},{0x00,0x49,0xB6},{0x00,0x49,0xFF},{0x00,0x6D,0x00},{0x00,0x6D,0x6D},{0x00,0x6D,0xB6},{0x00,0x6D,0xFF},
			{0x00,0x92,0x00},{0x00,0x92,0x6D},{0x00,0x92,0xB6},{0x00,0x92,0xFF},{0x00,0xB6,0x00},{0x00,0xB6,0x6D},{0x00,0xB6,0xB6},{0x00,0xB6,0xFF},{0x00,0xDB,0x00},{0x00,0xDB,0x6D},{0x00,0xDB,0xB6},{0x00,0xDB,0xFF},{0x00,0xFF,0x00},{0x00,0xFF,0x6D},{0x00,0xFF,0xB6},{0x00,0xFF,0xFF},
			{0x24,0x00,0x00},{0x24,0x00,0x6D},{0x24,0x00,0xB6},{0x24,0x00,0xFF},{0x24,0x24,0x00},{0x24,0x24,0x6D},{0x24,0x24,0xB6},{0x24,0x24,0xFF},{0x24,0x49,0x00},{0x24,0x49,0x6D},{0x24,0x49,0xB6},{0x24,0x49,0xFF},{0x24,0x6D,0x00},{0x24,0x6D,0x6D},{0x24,0x6D,0xB6},{0x24,0x6D,0xFF},
			{0x24,0x92,0x00},{0x24,0x92,0x6D},{0x24,0x92,0xB6},{0x24,0x92,0xFF},{0x24,0xB6,0x00},{0x24,0xB6,0x6D},{0x24,0xB6,0xB6},{0x24,0xB6,0xFF},{0x24,0xDB,0x00},{0x24,0xDB,0x6D},{0x24,0xDB,0xB6},{0x24,0xDB,0xFF},{0x24,0xFF,0x00},{0x24,0xFF,0x6D},{0x24,0xFF,0xB6},{0x24,0xFF,0xFF},
			{0x49,0x00,0x00},{0x49,0x00,0x6D},{0x49,0x00,0xB6},{0x49,0x00,0xFF},{0x49,0x24,0x00},{0x49,0x24,0x6D},{0x49,0x24,0xB6},{0x49,0x24,0xFF},{0x49,0x49,0x00},{0x49,0x49,0x6D},{0x49,0x49,0xB6},{0x49,0x49,0xFF},{0x49,0x6D,0x00},{0x49,0x6D,0x6D},{0x49,0x6D,0xB6},{0x49,0x6D,0xFF},
			{0x49,0x92,0x00},{0x49,0x92,0x6D},{0x49,0x92,0xB6},{0x49,0x92,0xFF},{0x49,0xB6,0x00},{0x49,0xB6,0x6D},{0x49,0xB6,0xB6},{0x49,0xB6,0xFF},{0x49,0xDB,0x00},{0x49,0xDB,0x6D},{0x49,0xDB,0xB6},{0x49,0xDB,0xFF},{0x49,0xFF,0x00},{0x49,0xFF,0x6D},{0x49,0xFF,0xB6},{0x49,0xFF,0xFF},
			{0x6D,0x00,0x00},{0x6D,0x00,0x6D},{0x6D,0x00,0xB6},{0x6D,0x00,0xFF},{0x6D,0x24,0x00},{0x6D,0x24,0x6D},{0x6D,0x24,0xB6},{0x6D,0x24,0xFF},{0x6D,0x49,0x00},{0x6D,0x49,0x6D},{0x6D,0x49,0xB6},{0x6D,0x49,0xFF},{0x6D,0x6D,0x00},{0x6D,0x6D,0x6D},{0x6D,0x6D,0xB6},{0x6D,0x6D,0xFF},
			{0x6D,0x92,0x00},{0x6D,0x92,0x6D},{0x6D,0x92,0xB6},{0x6D,0x92,0xFF},{0x6D,0xB6,0x00},{0x6D,0xB6,0x6D},{0x6D,0xB6,0xB6},{0x6D,0xB6,0xFF},{0x6D,0xDB,0x00},{0x6D,0xDB,0x6D},{0x6D,0xDB,0xB6},{0x6D,0xDB,0xFF},{0x6D,0xFF,0x00},{0x6D,0xFF,0x6D},{0x6D,0xFF,0xB6},{0x6D,0xFF,0xFF},
			{0x92,0x00,0x00},{0x92,0x00,0x6D},{0x92,0x00,0xB6},{0x92,0x00,0xFF},{0x92,0x24,0x00},{0x92,0x24,0x6D},{0x92,0x24,0xB6},{0x92,0x24,0xFF},{0x92,0x49,0x00},{0x92,0x49,0x6D},{0x92,0x49,0xB6},{0x92,0x49,0xFF},{0x92,0x6D,0x00},{0x92,0x6D,0x6D},{0x92,0x6D,0xB6},{0x92,0x6D,0xFF},
			{0x92,0x92,0x00},{0x92,0x92,0x6D},{0x92,0x92,0xB6},{0x92,0x92,0xFF},{0x92,0xB6,0x00},{0x92,0xB6,0x6D},{0x92,0xB6,0xB6},{0x92,0xB6,0xFF},{0x92,0xDB,0x00},{0x92,0xDB,0x6D},{0x92,0xDB,0xB6},{0x92,0xDB,0xFF},{0x92,0xFF,0x00},{0x92,0xFF,0x6D},{0x92,0xFF,0xB6},{0x92,0xFF,0xFF},
			{0xB6,0x00,0x00},{0xB6,0x00,0x6D},{0xB6,0x00,0xB6},{0xB6,0x00,0xFF},{0xB6,0x24,0x00},{0xB6,0x24,0x6D},{0xB6,0x24,0xB6},{0xB6,0x24,0xFF},{0xB6,0x49,0x00},{0xB6,0x49,0x6D},{0xB6,0x49,0xB6},{0xB6,0x49,0xFF},{0xB6,0x6D,0x00},{0xB6,0x6D,0x6D},{0xB6,0x6D,0xB6},{0xB6,0x6D,0xFF},
			{0xB6,0x92,0x00},{0xB6,0x92,0x6D},{0xB6,0x92,0xB6},{0xB6,0x92,0xFF},{0xB6,0xB6,0x00},{0xB6,0xB6,0x6D},{0xB6,0xB6,0xB6},{0xB6,0xB6,0xFF},{0xB6,0xDB,0x00},{0xB6,0xDB,0x6D},{0xB6,0xDB,0xB6},{0xB6,0xDB,0xFF},{0xB6,0xFF,0x00},{0xB6,0xFF,0x6D},{0xB6,0xFF,0xB6},{0xB6,0xFF,0xFF},
			{0xDB,0x00,0x00},{0xDB,0x00,0x6D},{0xDB,0x00,0xB6},{0xDB,0x00,0xFF},{0xDB,0x24,0x00},{0xDB,0x24,0x6D},{0xDB,0x24,0xB6},{0xDB,0x24,0xFF},{0xDB,0x49,0x00},{0xDB,0x49,0x6D},{0xDB,0x49,0xB6},{0xDB,0x49,0xFF},{0xDB,0x6D,0x00},{0xDB,0x6D,0x6D},{0xDB,0x6D,0xB6},{0xDB,0x6D,0xFF},
			{0xDB,0x92,0x00},{0xDB,0x92,0x6D},{0xDB,0x92,0xB6},{0xDB,0x92,0xFF},{0xDB,0xB6,0x00},{0xDB,0xB6,0x6D},{0xDB,0xB6,0xB6},{0xDB,0xB6,0xFF},{0xDB,0xDB,0x00},{0xDB,0xDB,0x6D},{0xDB,0xDB,0xB6},{0xDB,0xDB,0xFF},{0xDB,0xFF,0x00},{0xDB,0xFF,0x6D},{0xDB,0xFF,0xB6},{0xDB,0xFF,0xFF},
			{0xFF,0x00,0x00},{0xFF,0x00,0x6D},{0xFF,0x00,0xB6},{0xFF,0x00,0xFF},{0xFF,0x24,0x00},{0xFF,0x24,0x6D},{0xFF,0x24,0xB6},{0xFF,0x24,0xFF},{0xFF,0x49,0x00},{0xFF,0x49,0x6D},{0xFF,0x49,0xB6},{0xFF,0x49,0xFF},{0xFF,0x6D,0x00},{0xFF,0x6D,0x6D},{0xFF,0x6D,0xB6},{0xFF,0x6D,0xFF},
			{0xFF,0x92,0x00},{0xFF,0x92,0x6D},{0xFF,0x92,0xB6},{0xFF,0x92,0xFF},{0xFF,0xB6,0x00},{0xFF,0xB6,0x6D},{0xFF,0xB6,0xB6},{0xFF,0xB6,0xFF},{0xFF,0xDB,0x00},{0xFF,0xDB,0x6D},{0xFF,0xDB,0xB6},{0xFF,0xDB,0xFF},{0xFF,0xFF,0x00},{0xFF,0xFF,0x6D},{0xFF,0xFF,0xB6},{0xFF,0xFF,0xFF}};

		#endregion

		#region Declarations

		public class Colour
		{
			public byte Red { get; set; } = 0;
			public byte Green { get; set; } = 0;
			public byte Blue { get; set; } = 0;

			/// <summary>
			/// Indexer for array-like access to RGB components.
			/// </summary>
			public byte this[int index]
			{
				get
				{
					switch (index)
					{
						case 0: return Red;
						case 1: return Green;
						default: return Blue;
					}
				}
				set
				{
					switch (index)
					{
						case 0: Red = value; break;
						case 1: Green = value; break;
						default: Blue = value; break;
					}
				}
			}

			public Colour(byte red, byte green, byte blue)
			{
				Red = red;
				Green = green;
				Blue = blue;
			}

			/// <summary>
			/// Returns this <see cref="Palette.Colour"/> as 8-bit palette byte.
			/// </summary>
			public byte To8BitPaletteByte()
			{
				return AsPalette8Bit(Red, Green, Blue);
			}

			/// <summary>
			/// Note naming - color not colour - this is to indicate the result is System.Drawing.Color not Colour object
			/// </summary>
			public Color ToColor()
			{
				return Color.FromArgb(Red, Green, Blue);
			}

			/// <summary>
			/// Note naming - color not colour - this is to indicate the parameter is System.Drawing.Color not Colour object
			/// </summary>
			public void FromColor(Color color)
			{
				Red = color.R;
				Green = color.G;
				Blue = color.B;
			}

			private byte AsPalette8Bit(decimal red, decimal green, decimal blue)
			{
				byte r = (byte)Math.Round(red / (255 / 7));
				byte g = (byte)Math.Round(green / (255 / 7));
				byte b = (byte)Math.Round(blue / (255 / 3));
				return (byte)((r << 5) | (g << 2) | b);
			}
		}

		#endregion
	}
}
