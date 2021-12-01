using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace NextGraphics
{
	public partial class Palette : Form
	{
		public	enum	PaletteMapping
		{ 
			mapped256,
			mapped512,
			mappedCustom,
		}
		//-------------------------------------------------------------------------------------------------------------------
		//
		// Spectrum Next palettes
		//
		// 512 colours
		// 
		//-------------------------------------------------------------------------------------------------------------------

		public	byte[,]		SpecNext512		={	{0x00,0x00,0x00},{0x00,0x00,0x24},{0x00,0x00,0x49},{0x00,0x00,0x6D},{0x00,0x00,0x92},{0x00,0x00,0xB6},{0x00,0x00,0xDB},{0x00,0x00,0xFF},{0x00,0x24,0x00},{0x00,0x24,0x24},{0x00,0x24,0x49},{0x00,0x24,0x6D},{0x00,0x24,0x92},{0x00,0x24,0xB6},{0x00,0x24,0xDB},{0x00,0x24,0xFF},
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
		
		//-------------------------------------------------------------------------------------------------------------------
		//
		// 256 colours
		//
		//-------------------------------------------------------------------------------------------------------------------

		public	byte[,]		SpecNext256		={	{0x00,0x00,0x00},{0x00,0x00,0x6D},{0x00,0x00,0xB6},{0x00,0x00,0xFF},{0x00,0x24,0x00},{0x00,0x24,0x6D},{0x00,0x24,0xB6},{0x00,0x24,0xFF},{0x00,0x49,0x00},{0x00,0x49,0x6D},{0x00,0x49,0xB6},{0x00,0x49,0xFF},{0x00,0x6D,0x00},{0x00,0x6D,0x6D},{0x00,0x6D,0xB6},{0x00,0x6D,0xFF},
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
		public	Main			parentForm;
		public	byte[,]			loadedPalette		=	new	byte[256,3];
		public	Button[]		colours			=	new	Button[256];
		private	Label[] 		numbers			=	new	Label[32];
		public	int			transIndex		=	0;	
		public	PaletteMapping		paletteSetting		=	PaletteMapping.mapped256;
		public int			loadedColourCount	=	255;
		public int			loadedColourStart	=	0;
		public	bool			fourBitOutput		=	false;
		public	ImageSelect		selectForm		=	new	ImageSelect();		
		//private	short			thisIndex		=	0;
		private	bool			littleEndian		=	false;
		private	Colour			thisColour		=	new Colour();
		private	Colour			thatColour		=	new Colour();
		private	Color			selectedColor		=	Color.FromArgb(64,64,64);
		private	bool			colourPicking		=	false;
		public	Button			colourClicked; 
		//private Color			CopiedColour;

		//-------------------------------------------------------------------------------------------------------------------
		//
		// Constructor
		//
		//-------------------------------------------------------------------------------------------------------------------

		public Palette()
		{
			InitializeComponent();
			createPalette();
			this.Width		=	520;
			this.Height		=	428;
			this.MinimumSize	=	new Size(this.Width, this.Height);
			this.MaximumSize	=	new Size(this.Width, this.Height);
			littleEndian		=	BitConverter.IsLittleEndian;
		}
		
		//-------------------------------------------------------------------------------------------------------------------
		//
		// Add all the buttons to make the palette clickable
		//
		//-------------------------------------------------------------------------------------------------------------------
		
		private	void	createPalette()
		{
			int	across	=	0;
			int	down	=	0;
			for(int c=0;c<256;c++)
			{
				colours[c]						=	new Button();
				this.Controls.Add(colours[c]);
				colours[c].Text						=	"";
				colours[c].Location					=	new Point(160+(across*20),10+(down*20));
				colours[c].Size						=	new Size(22, 22);
				colours[c].BackColor					=	Color.FromArgb(SpecNext256[c,0],SpecNext256[c,1],SpecNext256[c,2]);
				colours[c].Click					+=	openContextMenu;
				//colours[c].MouseHover					+=	colourMouseOver;
				//colours[c].Enabled					=	false;
				colours[c].Name						=	c.ToString();				
				colours[c].FlatStyle					=	FlatStyle.Flat;
				colours[c].FlatAppearance.BorderColor			=	SystemColors.ControlDark;
				colours[c].FlatAppearance.BorderSize			=	1;
				//colours[c].ForeColor					=	Color.Black;
				loadedPalette[c,0]					=	SystemColors.Control.R;
				loadedPalette[c,1]					=	SystemColors.Control.G;
				loadedPalette[c,2]					=	SystemColors.Control.B;
				across++;
				if(across>15)
				{
					across	=	0;
					down++;
				}
			}
			startIndex.Text				=	loadedColourStart.ToString();
			tColourIndex1.Text			=	transIndex.ToString();
			colours[transIndex].Text		=	"X";
			radioButton1.Checked			=	true;

		}

		public	void	resetPalette()
		{
		//	int	across	=	0;
		//	int	down	=	0;
			for(int c=0;c<256;c++)
			{
				colours[c].BackColor					=	Color.FromArgb(SpecNext256[c,0],SpecNext256[c,1],SpecNext256[c,2]);
				loadedPalette[c,0]					=	SystemColors.Control.R;
				loadedPalette[c,1]					=	SystemColors.Control.G;
				loadedPalette[c,2]					=	SystemColors.Control.B;
		
			}	
			selectForm.fullNames.Clear();
			selectForm.listBox1.Items.Clear();	
			paletteSetting				=	PaletteMapping.mapped256;
			transIndex				=	0;
			loadedColourCount			=	255;
			loadedColourStart			=	0;
			startIndex.Text				=	loadedColourStart.ToString();
			tColourIndex1.Text			=	transIndex.ToString();
			colours[transIndex].Text		=	"X";
			radioButton1.Checked			=	true;
			this.Invalidate(true);
			this.Refresh();

		}

		//-------------------------------------------------------------------------------------------------------------------
		// 
		//  highlight the colour under the mouse
		//
		//-------------------------------------------------------------------------------------------------------------------

		private void colourMouseOver(object sender, EventArgs e)
		{			
			Button	thisButton	=	(Button) sender;
			hexColour.Text		=	"#"+thisButton.BackColor.R.ToString("X2") + thisButton.BackColor.G.ToString("X2") + thisButton.BackColor.B.ToString("X2");
		}

		//-------------------------------------------------------------------------------------------------------------------
		// 
		//  setForm after loading project
		//
		//-------------------------------------------------------------------------------------------------------------------

		public	void	setLoadedProjectForms()
		{
			for(int c=0;c<loadedColourCount;c++)
			{
				colours[c].BackColor				=	Color.FromArgb(loadedPalette[c,0],loadedPalette[c,1],loadedPalette[c,2]);													
			}
			tColourIndex1.Text			=	transIndex.ToString();
			for(int c=0;c<256;c++)
			{
				colours[c].Text	=	"";
			}	
			colours[transIndex].Text		=	"X";
			numColours.Text			=	loadedColourCount.ToString();
		}

	
		//-------------------------------------------------------------------------------------------------------------------
		// 
		//  init the form
		//
		//-------------------------------------------------------------------------------------------------------------------

		public	void	setForm()
		{
			tColourIndex1.Text			=	transIndex.ToString();
			colours[transIndex].Text		=	"X";
			if(paletteSetting == PaletteMapping.mappedCustom)
			{
				for(int c=0;c<256;c++)
				{	
					colours[c].BackColor	=	Color.FromArgb(loadedPalette[c,0],loadedPalette[c,1],loadedPalette[c,2]);
					colours[c].FlatAppearance.BorderColor	=	SystemColors.ControlDark;
				}
				for(int c=loadedColourStart;c<loadedColourCount+loadedColourStart;c++)
				{	
					colours[c].BackColor	=	Color.FromArgb(loadedPalette[c,0],loadedPalette[c,1],loadedPalette[c,2]);										
					colours[c].FlatAppearance.BorderColor	=	selectedColor;
				}
			}


		}
		//-------------------------------------------------------------------------------------------------------------------
		// 
		// Click for the palette buttons
		//
		//-------------------------------------------------------------------------------------------------------------------

		private void openContextMenu(object sender, EventArgs e)
		{
			colourClicked		=	(Button)sender;
			if(colourPicking ==	true)
			{
				if(int.Parse(colourClicked.Name)>loadedColourCount+loadedColourStart)
				{
					MessageBox.Show("Transparent Colour out of range of used colours", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);						
					colourPicking			=	false;
				}
				else
				{ 	int	id	=	int.Parse(colourClicked.Name);
					if((id&15)!=0)
					{ 
						DialogResult moved = MessageBox.Show("Move Transparent Colour to the first palette colour", "Move", MessageBoxButtons.YesNo, MessageBoxIcon.Question);	
						if(moved==DialogResult.Yes)
						{
							int row	=	id/16;
							Color	temp				=	colours[row*16].BackColor;
							colours[row*16].BackColor		=	colours[(row*16)+(id&15)].BackColor;
							colours[(row*16)+(id&15)].BackColor	=	temp;
							colourClicked				=	colours[row*16];
							
							byte	tempB			=	loadedPalette[row*16,0];
							loadedPalette[row*16,0]		=	loadedPalette[(row*16)+(id&15),0];
							loadedPalette[(row*16)+(id&15),0]	=	tempB;
							tempB			=	loadedPalette[row*16,1];
							loadedPalette[row*16,1]		=	loadedPalette[(row*16)+(id&15),1];
							loadedPalette[(row*16)+(id&15),1]	=	tempB;
							tempB			=	loadedPalette[row*16,2];
							loadedPalette[row*16,2]		=	loadedPalette[(row*16)+(id&15),2];
							loadedPalette[(row*16)+(id&15),2]	=	tempB;
						}
					}
					setTColour(ref colourClicked);
					colourPicking			=	false;
				}
			}
			else
			{ 
				if(paletteSetting == PaletteMapping.mappedCustom)
				{ 				
					Point lowerLeft		=	new Point(0, colourClicked.Height);
					lowerLeft		=	colourClicked.PointToScreen(lowerLeft);           
					copyMenu.Show(lowerLeft);
				}
			}
		}

		//-------------------------------------------------------------------------------------------------------------------
		// 
		// open the menu
		//
		//-------------------------------------------------------------------------------------------------------------------

		private	void	openMixer(object sender, EventArgs e)
		{ 
			int	colourIndex	=	0;
	
			if(paletteSetting == PaletteMapping.mappedCustom)
			{ 
				colorDialog1.Color		= 	colourClicked.BackColor;
				if(colorDialog1.ShowDialog() == DialogResult.OK)  
				{  
					colourIndex			=	int.Parse(colourClicked.Name);
					colourClicked.BackColor		=	colorDialog1.Color;  
					loadedPalette[colourIndex,0]	=	colourClicked.BackColor.R;
					loadedPalette[colourIndex,1]	=	colourClicked.BackColor.G;
					loadedPalette[colourIndex,2]	=	colourClicked.BackColor.B;						
					setTColour(ref colours[transIndex]);
				} 
			}
			
		}
		
		//-------------------------------------------------------------------------------------------------------------------
		//
		// Set the transparent colour
		//
		//-------------------------------------------------------------------------------------------------------------------

		private	void	setTColour(ref	Button thisPanel)
		{
			int	colourIndex		=	int.Parse(tColourIndex1.Text);
			colours[colourIndex].Text	=	"";
			tColourIndex1.Text		=	thisPanel.Name;
			transIndex			=	int.Parse(tColourIndex1.Text);
			tColourIndex1.BackColor		=	thisPanel.BackColor;
			tColourIndex1.ForeColor		=	(384 - tColourIndex1.BackColor.R - tColourIndex1.BackColor.G - tColourIndex1.BackColor.B) > 0 ? Color.White : Color.Black;
			thisPanel.Text			=	"X";
			thisPanel.ForeColor		=	tColourIndex1.ForeColor;
		}

	
		
		//-------------------------------------------------------------------------------------------------------------------
		//
		// Load a palette file
		//
		//-------------------------------------------------------------------------------------------------------------------

		private void loadPalette(object sender, EventArgs e)
		{
			//selectForm.loadPalette(sender,e);

			//byte[]	bytesBuffer				=	new	byte[2];
			OpenFileDialog loadPaletteDialog		=	new OpenFileDialog();
			//loadPaletteDialog.InitialDirectory		=	Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			loadPaletteDialog.Multiselect			=	true;
			loadPaletteDialog.RestoreDirectory		=	true ;
			loadPaletteDialog.Filter			=	"Palette Files (*.act)|*.act|Mac Palette Files (*.8bct)|*.8bct|All Files (*.*)|*.*";
			loadPaletteDialog.FilterIndex			=	1;
			if (loadPaletteDialog.ShowDialog(this) == DialogResult.OK)
			{
				selectForm.fullNames.Clear();
				foreach(string name in loadPaletteDialog.FileNames)
				{ 
					selectForm.fullNames.Add(name);
				}
				selectForm.paletteFiles		=	true;
				selectForm.fillList();
				selectForm.StartPosition	=	FormStartPosition.CenterParent;
				selectForm.ShowDialog();	
				if (selectForm.DialogResult == DialogResult.OK && selectForm.fullNames.Count>0 && selectForm.listBox1.SelectedIndex>=0)
				{
					// open the palette import panel
					for(int c=0;c<selectForm.count;c++)
					{
						loadedPalette[selectForm.too+c,0]	=	selectForm.loadedPalette[selectForm.from+c,0];
						loadedPalette[selectForm.too+c,1]	=	selectForm.loadedPalette[selectForm.from+c,1];
						loadedPalette[selectForm.too+c,2]	=	selectForm.loadedPalette[selectForm.from+c,2];
						colours[selectForm.too+c].BackColor	=	Color.FromArgb(loadedPalette[selectForm.too+c,0],loadedPalette[selectForm.too+c,1],loadedPalette[selectForm.too+c,2]);
					}
					radioButton3.Checked		=	true;
					paletteSetting			=	PaletteMapping.mappedCustom;
					mappedLoadedCheckedChanged(sender,e);
				}
			}
		}		
		
		//-------------------------------------------------------------------------------------------------------------------
		//
		// Map loaded radio button change
		//
		//-------------------------------------------------------------------------------------------------------------------

		private void mappedLoadedCheckedChanged(object sender, EventArgs e)
		{	
			if(radioButton3.Checked == true)
			{ 
				paletteSetting		=	PaletteMapping.mappedCustom;
				label7.Visible		=	false;
				for(int c=0;c<256;c++)
				{				
					colours[c].BackColor	=	Color.FromArgb(loadedPalette[c,0],loadedPalette[c,1],loadedPalette[c,2]);
					//colours[c].Enabled	=	true;
					colours[c].FlatAppearance.BorderColor	=	SystemColors.ControlDark;
				}
				for(int c=0;c<loadedColourCount;c++)
				{											
					colours[c].FlatAppearance.BorderColor	=	selectedColor;
				}
				setTColour(ref colours[transIndex]);
			}
		}
				
		//-------------------------------------------------------------------------------------------------------------------
		//
		// Select a palette from file button click
		//
		//-------------------------------------------------------------------------------------------------------------------

		private void paletteFromFileButtonClick(object sender, EventArgs e)
		{
			// select	
			selectForm.fullNames.Clear();
			foreach(string name in parentForm.fullNames)
			{ 
				selectForm.fullNames.Add(name);
			}
			selectForm.StartPosition	=		FormStartPosition.CenterParent;
			selectForm.fillList();
			selectForm.ShowDialog();
			selectForm.paletteFiles		=	false;
			label7.Visible			=	false;
			
			if (selectForm.DialogResult == DialogResult.OK && selectForm.fullNames.Count>0 && selectForm.listBox1.SelectedIndex>=0)
			{	
				for(int c=0;c<selectForm.count;c++)
				{
					loadedPalette[selectForm.too+c,0]	=	selectForm.loadedPalette[selectForm.from+c,0];
					loadedPalette[selectForm.too+c,1]	=	selectForm.loadedPalette[selectForm.from+c,1];
					loadedPalette[selectForm.too+c,2]	=	selectForm.loadedPalette[selectForm.from+c,2];
					colours[selectForm.too+c].BackColor	=	Color.FromArgb(loadedPalette[selectForm.too+c,0],loadedPalette[selectForm.too+c,1],loadedPalette[selectForm.too+c,2]);
				}
				radioButton3.Checked	=	true;
				mappedFromImageCheckedChanged(sender,e);
			}			
		}
						
		//-------------------------------------------------------------------------------------------------------------------
		//
		// Mapped From Image button click
		//
		//-------------------------------------------------------------------------------------------------------------------

		private void mappedFromImageCheckedChanged(object sender, EventArgs e)
		{		
			if(radioButton3.Checked==true)
			{ 
				paletteSetting		=	PaletteMapping.mappedCustom;
				label7.Visible		=	false;
				for(int c=0;c<256;c++)
				{
					colours[c].BackColor			=	SystemColors.Control;
					//colours[c].Enabled			=	true;					
					colours[c].FlatAppearance.BorderColor	=	SystemColors.ControlDark;
				}
				for(int c=0;c<loadedColourCount;c++)
				{
					colours[c].BackColor			=	Color.FromArgb(loadedPalette[c,0],loadedPalette[c,1],loadedPalette[c,2]);						
					colours[c].FlatAppearance.BorderColor	=	selectedColor;
				}				
				setTColour(ref colours[transIndex]);
			}
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// Mapped to 512 palette button click
		//
		//-------------------------------------------------------------------------------------------------------------------
		
		private void mappedTo512PaletteCheckedChanged(object sender, EventArgs e)
		{
			if(radioButton2.Checked==true)
			{ 
				paletteSetting		=	PaletteMapping.mapped512;
				label7.Visible		=	true;
				
				for(int c=0;c<256;c++)
				{
					colours[c].BackColor			=	Color.FromArgb(SpecNext512[1+(c*2),0],SpecNext512[1+(c*2),1],SpecNext512[1+(c*2),2]);
				//	colours[c].Enabled			=	false;
					colours[c].FlatAppearance.BorderColor	=	SystemColors.ControlDark;
				}				
				setTColour(ref colours[transIndex]);
			}
		}
		
		//-------------------------------------------------------------------------------------------------------------------
		//
		// Mapped to 256 palette button click
		//
		//-------------------------------------------------------------------------------------------------------------------
				
		private void mappedTo256PaletteCheckedChanged(object sender, EventArgs e)
		{
			if(radioButton1.Checked==true)
			{ 
				paletteSetting		=	PaletteMapping.mapped256;
				label7.Visible		=	false;
				for(int c=0;c<256;c++)
				{
					colours[c].BackColor			=	Color.FromArgb(SpecNext256[(c),0],SpecNext256[(c),1],SpecNext256[(c),2]);
					//colours[c].Enabled			=	false;
					colours[c].FlatAppearance.BorderColor	=	SystemColors.ControlDark;
				}				
				setTColour(ref colours[transIndex]);
			}
		}

		//-------------------------------------------------------------------------------------------------------------------
		// 
		// set the mapping mode
		//
		//-------------------------------------------------------------------------------------------------------------------

		public	void	SetPaletteMapping(string mappingString)
		{
			if(mappingString=="Next256")
			{
				paletteSetting		=	PaletteMapping.mapped256;
				radioButton1.Checked	=	true;
				label7.Visible		=	false;
			}
			else if(mappingString=="Next512")
			{
				paletteSetting		=	PaletteMapping.mapped512;
				radioButton2.Checked	=	true;
				label7.Visible		=	true;
			}
			else if(mappingString=="Custom")
			{
				paletteSetting		=	PaletteMapping.mappedCustom;
				radioButton3.Checked	=	true;
				label7.Visible		=	false;
			}			
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// Map to the closest colour
		//
		//-------------------------------------------------------------------------------------------------------------------
					
		public	short	closestColor(Color thisColour, short reMap, int startColour)//, bool bitsFour) 
		{
			short colorReturn		=	-1;
			int biggestDifference	=	1000;
			if(reMap<0)
			{ 
				switch(paletteSetting)
				{
					case	PaletteMapping.mapped256:
						for (short i = 0; i < 256; i++) 
						{
							if (Math.Sqrt(Math.Pow(thisColour.R - SpecNext256[i,0],2) + Math.Pow(thisColour.G - SpecNext256[i,1],2) + Math.Pow(thisColour.B - SpecNext256[i,2],2)) < biggestDifference)
							{
								colorReturn = i;
								biggestDifference = (int) Math.Sqrt(Math.Pow(thisColour.R - SpecNext256[i,0],2) + Math.Pow(thisColour.G - SpecNext256[i,1],2) + Math.Pow(thisColour.B - SpecNext256[i,2],2));
							}
						}
					break;
					case	PaletteMapping.mapped512:
						for (short i = 0; i < 512; i++) 
						{
							if (Math.Sqrt(Math.Pow(thisColour.R - SpecNext512[i,0],2) + Math.Pow(thisColour.G - SpecNext512[i,1],2) + Math.Pow(thisColour.B - SpecNext512[i,2],2)) < biggestDifference)
							{
								colorReturn = i;
								biggestDifference = (int) Math.Sqrt(Math.Pow(thisColour.R - SpecNext512[i,0],2) + Math.Pow(thisColour.G - SpecNext512[i,1],2) + Math.Pow(thisColour.B - SpecNext512[i,2],2));
							}
						}
					break;	
					case	PaletteMapping.mappedCustom:
					
						for (short i = 0; i < loadedColourCount; i++) 
						{
							if (Math.Sqrt(Math.Pow(thisColour.R - loadedPalette[startColour+i,0],2) + Math.Pow(thisColour.G - loadedPalette[startColour+i,1],2) + Math.Pow(thisColour.B - loadedPalette[startColour+i,2],2)) < biggestDifference)
							{
								colorReturn = (short)(i + startColour);
								biggestDifference = (int) Math.Sqrt(Math.Pow(thisColour.R - loadedPalette[startColour+i,0],2) + Math.Pow(thisColour.G - loadedPalette[startColour+i,1],2) + Math.Pow(thisColour.B - loadedPalette[startColour+i,2],2));
							}
						}
						break;	
				}
			}
			else
			{ 
				for (short i = reMap; i < reMap+16; i++) 
				{
					if (Math.Sqrt(Math.Pow(thisColour.R - loadedPalette[startColour+i,0],2) + Math.Pow(thisColour.G - loadedPalette[startColour+i,1],2) + Math.Pow(thisColour.B- loadedPalette[startColour+i,2],2)) < biggestDifference)
					{
						colorReturn = (short)(i + startColour);
						biggestDifference = (int) Math.Sqrt(Math.Pow(thisColour.R - loadedPalette[startColour+i,0],2) + Math.Pow(thisColour.G - loadedPalette[startColour+i,1],2) + Math.Pow(thisColour.B - loadedPalette[startColour+i,2],2));
					}
				}
			}
			return colorReturn;
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// Set the maximum colours to use
		//
		//-------------------------------------------------------------------------------------------------------------------
			
		private void setMaxColoursToUseClick(object sender, EventArgs e)
		{
			if(int.TryParse(numColours.Text, out loadedColourCount))
			{
				if(loadedColourCount>256)
				{
					loadedColourCount	=	256;
					numColours.Text		=	"256";
				}
				if(loadedColourStart<0)
				{
					loadedColourStart	=	0;
					startIndex.Text		=	"0";
				}
				for(int c=0;c<256;c++)
				{											
					colours[c].FlatAppearance.BorderColor	=	SystemColors.ControlDark;
				}
				for(int c=0;c<loadedColourCount;c++)
				{											
					colours[loadedColourStart+c].FlatAppearance.BorderColor	=	selectedColor;
				}
				
			}
		}
		
		//-------------------------------------------------------------------------------------------------------------------
		//
		// Pick a transparent colour
		//
		//-------------------------------------------------------------------------------------------------------------------
			
		private void TransPickClick(object sender, EventArgs e)
		{
			colourPicking	=	true;
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// take the 24 bit colours and make them 8 bit Spectrum next style
		//
		//-------------------------------------------------------------------------------------------------------------------

		private byte EightbitPalette(decimal red, decimal green, decimal blue)
		{
			byte r = (byte)Math.Round(red / (255 / 7));
			byte g = (byte)Math.Round(green / (255 / 7));
			byte b = (byte)Math.Round(blue / (255 / 3));
			return (byte)((r << 5) | (g << 2) | b);

			//return	(red & 0x0E0) | ((green & 0x0E0)>>3) | (((blue & 0x0E0)>>6) | ((blue & 0x020)>>5));
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// turn a byte into a binary string
		//
		//-------------------------------------------------------------------------------------------------------------------		
		private string toBinary(byte num)
		{
			string outString = "";
			int bits = 0x080;
			for (int bit = 0; bit < 8; bit++)
			{
				if ((num & bits) == bits)
				{
					outString += "1";
				}
				else
				{
					outString += "0";
				}
				bits = bits >> 1;
			}
			return outString;
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// Save palette click
		//
		//-------------------------------------------------------------------------------------------------------------------

		private void savePaletteClick(object sender, EventArgs e)
		{
			int	lineNumber	=	1000;
			int lineStep	 =	10;
			SaveFileDialog savePaletteDialog		=	new SaveFileDialog();
			//savePaletteDialog.InitialDirectory		=	Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			savePaletteDialog.RestoreDirectory		=	true ;
			savePaletteDialog.Filter			= "Palette Files (*.act)|*.act|Mac Palette Files (*.8bct)|*.8bct|Spectrum Next (*.asm)|*.asm|Spectrum Next (*.bas)|*.asm|All Files (*.*)|*.*";
			savePaletteDialog.FilterIndex			=	1;
			if (savePaletteDialog.ShowDialog(this) == DialogResult.OK)
			{										
				if(savePaletteDialog.FilterIndex==3 || savePaletteDialog.FilterIndex == 4)
				{
					using (StreamWriter outputFile = new StreamWriter(savePaletteDialog.FileName))
					{
						if (savePaletteDialog.FilterIndex == 4)
						{
							outputFile.WriteLine(lineNumber.ToString() + "\tREM");
							lineNumber += lineStep;
							outputFile.WriteLine(lineNumber.ToString() + "\tREM\tExported Palette starts here");
							lineNumber += lineStep;
							outputFile.WriteLine(lineNumber.ToString() + "\tREM");
							lineNumber += lineStep;
							outputFile.Write(lineNumber.ToString() + "\tDATA\t");
							lineNumber += lineStep;
						}
						else
						{
							outputFile.WriteLine("ExportedPalette:");
						}
						for (int j = 0; j < loadedColourCount; j++)
						{
							if (savePaletteDialog.FilterIndex == 4)
							{
								outputFile.Write(EightbitPalette(loadedPalette[j+loadedColourStart, 0], loadedPalette[j+loadedColourStart, 1], loadedPalette[j+loadedColourStart, 2]).ToString());
								if (j < loadedColourCount - 1)
								{
									outputFile.Write(",");
								}
								else
								{
									outputFile.Write("\r\n");
								}
							}
							else
							{
								outputFile.WriteLine("\t\t\tdb\t%" + toBinary(EightbitPalette(loadedPalette[j+loadedColourStart, 0], loadedPalette[j+loadedColourStart, 1], loadedPalette[j+loadedColourStart, 2])) +
												"\t//\t" + loadedPalette[j+loadedColourStart, 0].ToString() + "," + loadedPalette[j+loadedColourStart, 1].ToString() + "," + loadedPalette[j+loadedColourStart, 2].ToString());
								//outputFile.WriteLine("\t\t\tdb\t%" + toBinary(EightbitPalette(loadedPalette[j, 0], loadedPalette[j, 1], loadedPalette[j, 2])));
							}
						}
					}
				}
				else
				{
					using (FileStream fsSource = new FileStream(savePaletteDialog.FileName, FileMode.Create, FileAccess.Write))
					{
						for (int i = 0; i < 256; i++)
						{
							fsSource.WriteByte(loadedPalette[i,0]);
							fsSource.WriteByte(loadedPalette[i,1]);
							fsSource.WriteByte(loadedPalette[i,2]);
						}	
						if (littleEndian == true)
						{
							fsSource.WriteByte((byte)(loadedColourCount&255));
							fsSource.WriteByte((byte)(loadedColourCount>>8));
						}
						else
						{
							fsSource.WriteByte((byte)(loadedColourCount>>8));
							fsSource.WriteByte((byte)(loadedColourCount&255));
						}
						if (littleEndian == true)
						{
							fsSource.WriteByte((byte)(transIndex&255));
							fsSource.WriteByte((byte)(transIndex>>8));
						}
						else
						{
							fsSource.WriteByte((byte)(transIndex>>8));
							fsSource.WriteByte((byte)(transIndex&255));
						}
					}
				}
			}
		}

		//-------------------------------------------------------------------------------------------------------------------
		// 
		// set colours of the buttons and clear the rest
		//
		//-------------------------------------------------------------------------------------------------------------------
		
		private void setButtonColours(object sender, EventArgs e)
		{			
			if(int.TryParse(numColours.Text, out loadedColourCount))
			{
				if(loadedColourCount>256)
				{
					loadedColourCount	=	256;
					numColours.Text		=	"256";
				}
				if(loadedColourStart>256)
				{
					loadedColourStart	=	0;
					startIndex.Text		=	"0";
				}
				for(int c=0;c<256;c++)
				{											
					colours[c].FlatAppearance.BorderColor	=	SystemColors.ControlDark;
				}
				for(int c=0;c<loadedColourCount;c++)
				{											
					colours[c+loadedColourStart].FlatAppearance.BorderColor	=	selectedColor;
				}
				
			}
		}

		//-------------------------------------------------------------------------------------------------------------------
		// 
		// check before closing
		//
		//-------------------------------------------------------------------------------------------------------------------
		
		private void checkNumColoursClosing(object sender, FormClosingEventArgs e)
		{
			if(numColours.Text=="255" && paletteSetting == PaletteMapping.mappedCustom)
			{ 
				var result = MessageBox.Show("Have you forgotten to set the number of colours to use?", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
				if (result == DialogResult.Cancel)
				{
					  e.Cancel = true;
				}
			}
		}

		private void clearColour(object sender, EventArgs e)
		{
			int	colourIndex			=	int.Parse(colourClicked.Name);
			colourClicked.BackColor				=	SystemColors.Control;  
			loadedPalette[colourIndex,0]		=	colourClicked.BackColor.R;
			loadedPalette[colourIndex,1]		=	colourClicked.BackColor.G;
			loadedPalette[colourIndex,2]		=	colourClicked.BackColor.B;						
			setTColour(ref colours[transIndex]);
		}

		private void pasteColour(object sender, EventArgs e)
		{				
			int	colourIndex						=	int.Parse(colourClicked.Name);
			colourClicked.BackColor				=	selectForm.CopiedColour;  
			loadedPalette[colourIndex,0]		=	colourClicked.BackColor.R;
			loadedPalette[colourIndex,1]		=	colourClicked.BackColor.G;
			loadedPalette[colourIndex,2]		=	colourClicked.BackColor.B;						
			setTColour(ref colours[transIndex]);
		}

		private void copyColour(object sender, EventArgs e)
		{
			selectForm.CopiedColour			=	colourClicked.BackColor;
		}

        private void clear(object sender, EventArgs e)
        {
			for (int c = 0; c < 256; c++)
			{
				if(c<loadedColourStart || c>=loadedColourStart+loadedColourCount)
				{ 
					colours[c].FlatAppearance.BorderColor	= SystemColors.ControlDark;
					colours[c].BackColor					= SystemColors.Control;
					loadedPalette[c, 0]						= colours[c].BackColor.R;
					loadedPalette[c, 1]						= colours[c].BackColor.G;
					loadedPalette[c, 2]						= colours[c].BackColor.B;
				}
			}
		} 
		public void setColourCount()
        {
			loadedColourCount	=	int.Parse(this.numColours.Text);
        }
		public void setStartIndex()
        {			
			loadedColourStart	=	int.Parse(this.startIndex.Text);
        }
		public void setColourCountText()
        {
			this.numColours.Text	=	loadedColourCount.ToString();
        }
		public void setStartIndexText()
        {			
			this.startIndex.Text	=	loadedColourStart.ToString();
        }
		private void setStart(object sender, KeyEventArgs e)
        {			
			loadedColourStart	=	int.Parse(this.startIndex.Text);
        }
       
        private void setButtonColours(object sender, KeyEventArgs e)
        {
			loadedColourCount	=	int.Parse(this.numColours.Text);
        }

        private void hexColour_TextChanged(object sender, EventArgs e)
        {
			setFromHex();
		}

        private void hexColour_TextChanged(object sender, KeyEventArgs e)
        {
			setFromHex();
		}

		private	void	setFromHex()
		{ 
			if (colourClicked != null)
			{
				if(hexColour.Text.Substring(0, 1)=="#")
				{ 
					if ( hexColour.Text.Length == 7)
					{ 
						colourClicked.BackColor = Color.FromArgb(Convert.ToInt32(hexColour.Text.Substring(1, 2), 16), Convert.ToInt32(hexColour.Text.Substring(3, 2), 16),Convert.ToInt32(hexColour.Text.Substring(5, 2), 16));
					}
				}
				else
                {
					if (hexColour.Text.Length == 6)
					{
						colourClicked.BackColor = Color.FromArgb(Convert.ToInt32(hexColour.Text.Substring(0, 2), 16), Convert.ToInt32(hexColour.Text.Substring(2, 2), 16), Convert.ToInt32(hexColour.Text.Substring(4, 2), 16));
					}
				}
			}
		}

        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void hexValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
			
			hexColour.Text		=	"#" + colourClicked.BackColor.R.ToString("X2") + colourClicked.BackColor.G.ToString("X2") + colourClicked.BackColor.B.ToString("X2");

		}

        private void moveRowUp(object sender, EventArgs e)
        {
			byte[,] tempPalette = new byte[16, 3];

			for(int c=0;c<16;c++)
            {
				tempPalette[c, 0]	=	loadedPalette[c, 0];
				tempPalette[c, 1]	=	loadedPalette[c, 1];
				tempPalette[c, 2]	=	loadedPalette[c, 2];
			}
			for (int c = 0; c < 256-16; c++)
			{
				loadedPalette[c, 0] = loadedPalette[c+16, 0];
				loadedPalette[c, 1] = loadedPalette[c+16, 1];
				loadedPalette[c, 2] = loadedPalette[c+16, 2];
			}
			for(int c=0;c<16;c++)
            {
				loadedPalette[c+(256-16), 0]	=	tempPalette[c, 0];
				loadedPalette[c+(256-16), 1]	=	tempPalette[c, 1];
				loadedPalette[c+(256-16), 2]	=	tempPalette[c, 2];
			}
			for(int c=0;c<256;c++)
            {
				colours[c].BackColor	=	Color.FromArgb(loadedPalette[c, 0], loadedPalette[c, 1],loadedPalette[c, 2]);
			}
		}

        private void moveRowDown(object sender, EventArgs e)
        {
			byte[,] tempPalette = new byte[16, 3];

			for(int c=0;c<16;c++)
            {
				tempPalette[c, 0]	=	loadedPalette[c+(256-16), 0];
				tempPalette[c, 1]	=	loadedPalette[c+(256-16), 1];
				tempPalette[c, 2]	=	loadedPalette[c+(256-16), 2];
			}
			for (int c = 255-16; c >=0 ; c--)
			{
				loadedPalette[c+16, 0] = loadedPalette[c, 0];
				loadedPalette[c+16, 1] = loadedPalette[c, 1];
				loadedPalette[c+16, 2] = loadedPalette[c, 2];
			}
			for(int c=0;c<16;c++)
            {
				loadedPalette[c, 0]	=	tempPalette[c, 0];
				loadedPalette[c, 1]	=	tempPalette[c, 1];
				loadedPalette[c, 2]	=	tempPalette[c, 2];
			}
			for(int c=0;c<256;c++)
            {
				colours[c].BackColor	=	Color.FromArgb(loadedPalette[c, 0], loadedPalette[c, 1],loadedPalette[c, 2]);
			}
        }

        private void outOk_Click(object sender, EventArgs e)
        {		
			loadedColourStart	=	int.Parse(this.startIndex.Text);
			loadedColourCount	=	int.Parse(this.numColours.Text);
        }
    }
}
