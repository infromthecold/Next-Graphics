using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace NextGraphics
{

	//-------------------------------------------------------------------------------------------------------------------
	//
	// Byte array bitmap
	//
	//-------------------------------------------------------------------------------------------------------------------

	public class BitmapData : IDisposable
	{
		
		//-------------------------------------------------------------------------------------------------------------------
		//
		// variables
		//
		//-------------------------------------------------------------------------------------------------------------------

		public Bitmap Bitmap { get; private set; }
		public short[] Bits { get; private set; }
		public bool Disposed { get; private set; }
		public int Height { get; private set; }
		public int Width { get; private set; }
		protected GCHandle BitsHandle { get; private set; }

		//-------------------------------------------------------------------------------------------------------------------
		//
		// constructor
		//
		//-------------------------------------------------------------------------------------------------------------------

		public BitmapData(int width, int height)
		{
			Width		=	width;
			Height		=	height;
			Disposed	=	false;
			Bits		=	new short[width * height];	
			BitsHandle	=	GCHandle.Alloc(Bits, GCHandleType.Pinned);
			Bitmap		=	new Bitmap(width, height, width, PixelFormat.Format8bppIndexed, BitsHandle.AddrOfPinnedObject());
		}
		~BitmapData()
		{
			Dispose();
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// sets the byte array at the pixel x and y
		//
		//-------------------------------------------------------------------------------------------------------------------

		public void SetPixel(int x, int y, short colourIndex)
		{
			int index = x + (y * Width);
			Bits[index] = colourIndex;
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// get the byte array at the pixel x and y
		//
		//-------------------------------------------------------------------------------------------------------------------

		public Int16 GetPixel(int x, int y)
		{
			int index = x + (y * Width);
			return Bits[index];
		}

		public void Dispose()
		{
			if (Disposed)
			{
				return;
			}
			Disposed = true;
			Bitmap.Dispose();
			BitsHandle.Free();
		}
	}

	//-------------------------------------------------------------------------------------------------------------------
	//
	// A block definition 
	//
	//-------------------------------------------------------------------------------------------------------------------

	public class block
	{
		public	bool	repeated;
		public	bool	flippedX;
		public	bool	flippedY;
		public	bool	transparent;
		public	bool	rotated;
		public	bool	secondHalf;
		public	short	originalId;
		public	short	xPos;
		public	short	yPos;
		public	short	paletteOffset;
		public	block()
		{
			xPos			=	0;
			yPos			=	0;
			repeated		=	false;
			flippedX		=	false;
			flippedY		=	false;
			rotated			=	false;
			originalId		=	0;
			secondHalf		=	false;
			paletteOffset		=	0;
		}
		~block()
		{

		}
	}	

	//-------------------------------------------------------------------------------------------------------------------
	//
	// A sprite/object definition 
	//
	//-------------------------------------------------------------------------------------------------------------------

	public	class spriteInfo  : IDisposable
	{
		public	bool Disposed { get; private set; }
		public	block[] infos  { get; private set; }
		public	int Width { get; private set; }
		public	int Height { get; private set; }	
		public	int Size { get; private set; }	
		public	spriteInfo(int width, int height)
		{					
			Width				=	width;
			Height				=	height;
			Size				=	width*height;
			infos				=	new block[Size];
			for(int b=0;b<Size;b++)
			{
				infos[b]		=	new block();
			}
			Disposed	=	false;
		}
		~spriteInfo()
		{
			Dispose();
		}
		public void Dispose()
		{
			if (Disposed)
			{
				return;
			}
			Disposed = true;
		}
		public void SetData(int x, int y,bool repeated,bool flippedX,bool flippedY,bool rotated,bool transparent, short originalId, short paletteOffset)
		{
			int index		=	x + (y * Width);
			infos[index].repeated		=	repeated;
			infos[index].flippedX		=	flippedX;
			infos[index].flippedY		=	flippedY;
			infos[index].rotated		=	rotated;
			infos[index].transparent	=	transparent;			
			infos[index].paletteOffset	=	paletteOffset;
			if((originalId&1)==0)
			{
				infos[index].secondHalf =       false;
			}
			else
			{
				infos[index].secondHalf =       true;
			}
			infos[index].originalId		=	originalId;
			infos[index].xPos		=	(short) x;  
			infos[index].yPos		=	(short) y; 
		}
		public bool GetRepeated(int x, int y)
		{
			int index = x + (y * (Width));
			return infos[index].repeated;
		}
		public byte GetPaletteOffset(int x, int y)
		{
			int index = x + (y * (Width));
			return (byte)infos[index].paletteOffset;
		}			
		public bool GetFlippedX(int x, int y)
		{
			int index = x + (y * (Width));
			return infos[index].flippedX;
		}
		public bool GetFlippedY(int x, int y)
		{
			int index = x + (y * (Width));
			return infos[index].flippedY;
		}
		public bool GetRotated(int x, int y)
		{
			int index = x + (y * (Width));
			return infos[index].rotated;
		}
		public Int16 GetId(int x, int y)
		{
			int index = x + (y * (Width));
			return infos[index].originalId;
		}
		public Int16 GetXPos(int x, int y)
		{
			int index = x + (y * (Width));
			return infos[index].xPos;
		}
		public Int16 GetYpos(int x, int y)
		{
			int index = x + (y * (Width));
			return infos[index].yPos;
		}
		public bool GetWhichHalf(int x, int y)
		{
			int index = x + (y * (Width));
			return infos[index].secondHalf;
		}

	}

	//-------------------------------------------------------------------------------------------------------------------
	//
	// internal colour
	//
	//-------------------------------------------------------------------------------------------------------------------

	public	class Colour
	{
		public	byte	A	=	0;
		public	byte	R	=	0;
		public	byte	G	=	0;
		public	byte	B	=	0;
		public	Int32	getARGB()
		{
			return	A<<24 | R<<18 | G<<8 | B;
		}
	}
}
