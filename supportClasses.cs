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

	public class bitsBitmap : IDisposable
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
		public bool Trans { get; set; }
		protected GCHandle BitsHandle { get; private set; }

		//-------------------------------------------------------------------------------------------------------------------
		//
		// constructor
		//
		//-------------------------------------------------------------------------------------------------------------------

		public bitsBitmap(int width, int height)
		{
			Width		=	width;
			Height		=	height;
			Disposed	=	false;
			Bits		=	new short[width * height];	
			BitsHandle	=	GCHandle.Alloc(Bits, GCHandleType.Pinned);
			Bitmap		=	new Bitmap(width, height, width, PixelFormat.Format8bppIndexed, BitsHandle.AddrOfPinnedObject());
		}
		~bitsBitmap()
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
			if(y>Height || x> Width || index>=Bits.Length)
			{
				return	0;
			}			
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
		public	bool	hasTranspearent;
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
			paletteOffset	=	0;
			hasTranspearent	=	false;
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
		public	bool	Disposed { get; private set; }
		public	block[] infos  { get; private set; }
		public	int	Width { get; private set; }
		public	int	Height { get; private set; }	
		public	int	Top { get; private set; }	
		public	int	Left { get; private set; }	
		public	int	Right { get; private set; }	
		public	int	Bottom { get; private set; }	
		
		public	short	offsetX;
		public	short	offsetY;

		public	int Size { get; private set; }	
		public	bool Used { get; private set; }	

		public	void	SetTop(int TopLine)
		{
			Top	=	TopLine;
		}
		public	void	SetLeft(int LeftLine)
		{
			Left	=	LeftLine;
		}
		public	void	SetRight(int RightLine)
		{
			Right	=	RightLine;
		}
		public	void	SetBottom(int BottomLine)
		{
			Bottom	=	BottomLine;
		}
		public	spriteInfo(int width, int height)
		{					
			
			offsetX				=	0;
			offsetY				=	0;
			Width				=	width;
			Height				=	height;
			Size				=	width*height;
			Used				=	true;
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
		public void SetData(int x, int y,bool repeated,bool flippedX,bool flippedY,bool rotated,bool transparent, short originalId, short paletteOffset, bool hasTran)
		{
			int index			=	x + (y * Width);
			infos[index].repeated		=	repeated;
			infos[index].flippedX		=	flippedX;
			infos[index].flippedY		=	flippedY;
			infos[index].rotated		=	rotated;
			infos[index].transparent	=	transparent;			
			infos[index].paletteOffset	=	paletteOffset;
			infos[index].hasTranspearent=	hasTran;
			if ((originalId&1)==0)
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
		public Int16 GetOffsetX()
		{
			return offsetX;
		}
		public Int16 GetOffsetY()
		{
			return offsetY;
		}
		public void setOffsetY(short newOffsetY)
		{
			offsetY  =	newOffsetY;
		}
		public void setOffsetX(short newOffsetX)
		{			
			offsetX  =	newOffsetX;
		}
		public void clearOffsets()
		{			
			offsetX  =	0;
			offsetY  =	0;
		}
		public bool GetTransparent(int x, int y)
		{
			int index = x + (y * (Width));
			return infos[index].transparent;
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
