//#define PROPRIETARY         // do not use this as there is no code on git
//#define DEBUG_WINDOW
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Drawing.Drawing2D;
using NextGraphics.Models;

namespace NextGraphics
{	       
	public partial class Main : Form
	{
		//-------------------------------------------------------------------------------------------------------------------
		//
		// enumerations
		//
		//-------------------------------------------------------------------------------------------------------------------
			
		public enum blockType
		{	
			Original,
			Repeated,
			FlippedX,
			FlippedY,
			FlippedXY,
			Rotated, 			
			FlippedXRotated,		
			FlippedYRotated,		
			FlippedXYRotated,		
			Transparent,
		}

		public enum comments
		{
			noComments,
			fullComments
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// variables
		//
		//-------------------------------------------------------------------------------------------------------------------

		public MainModel Model { get; set; } = new MainModel();

		public	RadioButton		selectedRadio;
		public	int			outSize			=	16;		
		private	short			thisIndex		=	0;
		private	const int		MAX_BLOCKS		=	256;
		private	const int		MAX_CHAR_SIZE		=	256;
		private	const int		MAX_IMAGES		=	64;
		public	List<imageWindow>	imageWindows		=	new	List<imageWindow>();
#if PROPRIETARY
		public	parallaxTool		parallaxWindow		=	new	parallaxTool();
#endif
		public	imageWindow		blocksWindow		=	new	imageWindow();
		public	infoWindow		informationWindow	=	new	infoWindow();
		public	imageWindow		charsWindow		=	new	imageWindow();
		private	Bitmap	 		blocksPanel;
		private	Bitmap	 		charsPanel;
		private	bitsBitmap		tempBitmap;
		public	bitsBitmap[]		blockData		=	new	bitsBitmap[MAX_BLOCKS];	
		public	spriteInfo[]		blockInfo		=	new	spriteInfo[MAX_BLOCKS];
		public	bitsBitmap[]		charData		=	new	bitsBitmap[MAX_CHAR_SIZE+1];	
		private	bitsBitmap[]		tempData		=	new	bitsBitmap[MAX_CHAR_SIZE+1];	
		public	int[]			sortIndexs		=	new	int[MAX_CHAR_SIZE+1];	
		private	int			outXBlock		=	0;
		private	int			outYBlock		=	0;
		private	int			outBlock		=	0;
		private	int			outXChar		=	0;
		private	int			outYChar		=	0;
		private	int			outChar			=	0;
		private	Rectangle		src			=	new	Rectangle();
		private	Rectangle		dest			=	new	Rectangle();
		private	Rectangle		charDest		=	new	Rectangle();
		private	string			projectPath		=	"";
		private	string			outPath			=	"";
		private	string			binPath			=	"";
		private	string			tilesPath		=	"";
		private	bool			reverseByes		=	false;
		private	bool			PaletteSet		=	false;	
		public	PaletteForm			PaletteForm		=	new PaletteForm();
		private	SettingsForm	SettingsPanel			=	new SettingsForm();
		private	long			AveragingIndex		=	0;		
		public	imageWindow		blocksView		=	new	imageWindow();
		private	int			tranpearntChars		=	0;
		private	int			SortedIndex		=	0;
		private readonly NumberFormatInfo	fmt		=	new NumberFormatInfo();

		private palOffset		offsetPanel		=	new	palOffset();
		private	string			ParentDirectory		=	"f:/";
		private	Font			numberFont		=	new Font("Arial", 6,FontStyle.Bold);
		private	SolidBrush		numberBrush		=	new SolidBrush(Color.White);
		SaveFileDialog			projectSaveDialog	=	new SaveFileDialog();
		OpenFileDialog			openProjectDialog	=	new OpenFileDialog();
		OpenFileDialog			batchProjectDialog	=	new OpenFileDialog();
		SaveFileDialog			outputFilesDialog	=	new SaveFileDialog();
		OpenFileDialog			addImagesDialog		=	new OpenFileDialog();



		rebuild				rebuildDialog		=	new rebuild();
		
#if DEBUG_WINDOW
		public	DEBUGFORM		DEBUG_WINDOW;
#endif


		//-------------------------------------------------------------------------------------------------------------------
		//
		// Init
		//
		//-------------------------------------------------------------------------------------------------------------------
			
		public Main()
		{
			InitializeComponent();
			blocksPanel				=	new	Bitmap(128,1024,PixelFormat.Format24bppRgb);
			clearPanels(blocksPanel);
			blocksDisplay.Image			=	blocksPanel;
			blocksDisplay.Height			=	blocksPanel.Height;
			blocksDisplay.Width			=	blocksPanel.Width;
#if DEBUG_WINDOW
			DEBUG_WINDOW				=	new	DEBUGFORM();
			DEBUG_WINDOW.Show();
#endif
	
			charsPanel				=	new	Bitmap(128,256*16,PixelFormat.Format24bppRgb);
			clearPanels(charsPanel);
			charactersDisplay.Image			=	charsPanel;			
			this.toolStripProgressBar1.Minimum	=	0;
			this.toolStripProgressBar1.Maximum	=	0;
			this.listBox1.Items.Add(" "+Model.Name);
			reverseByes				=	BitConverter.IsLittleEndian;
			Model.BlocksAccross				=	(int)Math.Floor((float)blocksDisplay.Width/Model.GridXSize);
			blocksDisplay.Invalidate();
			blocksDisplay.Refresh();
			SettingsPanel.comments.SelectedIndex	=	1;
			setForm();
					
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
			string sep = string.Empty;
			foreach (var c in codecs)
			{
				string codecName		=	c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
				addImagesDialog.Filter		=	String.Format("{0}{1}{2} ({3})|{3}", addImagesDialog.Filter, sep, codecName, c.FilenameExtension);
				sep = "|";
			}
			addImagesDialog.Filter			=	String.Format("{0}{1}{2} ({3})|{3}", addImagesDialog.Filter, sep, "All Files", "*.*");
#if PROPRIETARY
			parallaxWindow.thePalette		=	thePalette;
			parallaxWindow.main				=	this;
			fmt.NegativeSign				=	"-";
#endif
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// Clear the bitmap
		//
		//-------------------------------------------------------------------------------------------------------------------

		private		void	clearPanels(Bitmap thisBitmap)
		{
			using (Graphics gfx = Graphics.FromImage(thisBitmap))
			using (SolidBrush brush = new SolidBrush(Color.FromArgb(255 , 255, 0, 255)))
			{
				gfx.FillRectangle(brush, 0, 0,thisBitmap.Width,thisBitmap.Height);
			}
		}


		//-------------------------------------------------------------------------------------------------------------------
		//
		// New Project name form
		//
		//-------------------------------------------------------------------------------------------------------------------

		private void showNewProjectForm(object sender, EventArgs e)
		{
			newProject newProjectForm		=	new newProject();
		
			newProjectForm.ShowDialog();

			if (newProjectForm.DialogResult == DialogResult.OK)
			{
				Model.Name	=	newProjectForm.textBox1.Text;	
				newProject();
			}
		}

		private	void	newProject()
		{
			Model.Clear();
			setForm();
			blocksPanel			=	new	Bitmap(128,512,PixelFormat.Format24bppRgb);
			clearPanels(blocksPanel);
			blocksDisplay.Image			=	blocksPanel;
			blocksDisplay.Height			=	blocksPanel.Height;
			blocksDisplay.Width			=	blocksPanel.Width;
			blocksDisplay.Invalidate(true);
			blocksDisplay.Refresh();
			charsPanel				=	new	Bitmap(128,256*16,PixelFormat.Format24bppRgb);
			clearPanels(charsPanel);
			charactersDisplay.Image			=	charsPanel;	
			charactersDisplay.Invalidate(true);
			charactersDisplay.Refresh();
			setForm();
			DisposeImageWindows();
		}

#if DEBUG_WINDOW

		//-------------------------------------------------------------------------------------------------------------------
		//
		// unused function for debugging 
		//
		//-------------------------------------------------------------------------------------------------------------------
		private	void	debugText(string line)
		{
			using (StreamWriter writer = new StreamWriter("f://debug.txt",true))
			{ 
				writer.WriteLine(line);
			}
		}
#endif

		//-------------------------------------------------------------------------------------------------------------------
		//
		// Save Project
		//
		//-------------------------------------------------------------------------------------------------------------------
		private void saveProject(object sender, EventArgs e)
		{
			if(projectPath.Length==0)
			{ 
				
				projectSaveDialog.FileName			=	Model.Name + ".xml";
				projectSaveDialog.Filter			=	"Project Files (*.xml)|*.xml|All Files (*.*)|*.*";
				projectSaveDialog.FilterIndex		=	1 ;
				projectSaveDialog.RestoreDirectory	=	false;			
				projectSaveDialog.InitialDirectory 	=	ParentDirectory + "\\Projects\\";			
				if(projectSaveDialog.ShowDialog() == DialogResult.OK)
				{
					setParentFolder(Path.GetFullPath(projectSaveDialog.FileName));
					projectPath				=	Path.ChangeExtension(projectSaveDialog.FileName,"xml");
					saveXMLFile(projectPath);	
				}

			}
			else
			{
				saveXMLFile(projectPath);
			}
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// Save Xml File 
		//
		//-------------------------------------------------------------------------------------------------------------------

		private	void	saveXMLFile(string prjPath)
		{
			// TODO: palette and settings values must be updated into model when closing those forms!
			int	transIndex		=	Model.Palette.TransparentIndex;
			int	loadedColourCount	=	Model.Palette.UsedCount;
			int	loadedColourStart	=	Model.Palette.StartIndex;
			using 	(XmlTextWriter writer = new XmlTextWriter(prjPath, Encoding.UTF8))
			{
				writer.Formatting				=	Formatting.Indented;
				XmlDocument document = Model.Save(projectNode =>
				{
#if PROPRIETARY
					projectNode.AppendChild(parallaxWindow.writeParallax(doc));
#endif
				});

				document.WriteContentTo(writer);
				writer.Flush();
				writer.Close();
				// mStream.Flush();
				//myStream.Write(doc.InnerXml);
			}
		}
		
		//-------------------------------------------------------------------------------------------------------------------
		// 
		// set the parent folder for the project
		//
		//-------------------------------------------------------------------------------------------------------------------
		public	void	setParentFolder(string path)
        {
			DirectoryInfo	parentDir =	Directory.GetParent(path);
			ParentDirectory	=	parentDir.Parent.FullName;
        }

		//-------------------------------------------------------------------------------------------------------------------
		//
		// Open Xml project File 
		//
		//-------------------------------------------------------------------------------------------------------------------

		private void openProject(object sender, EventArgs e)
		{
			openProjectDialog.Multiselect		=	false;
			openProjectDialog.RestoreDirectory	=	false;
			openProjectDialog.InitialDirectory 	=	ParentDirectory + "\\Projects\\";	
			openProjectDialog.Filter			=	"Project Files (*.xml)|*.xml|All Files (*.*)|*.*";
			
			if (openProjectDialog.ShowDialog(this) == DialogResult.OK)
			{
				setParentFolder(Path.GetFullPath(openProjectDialog.FileName));
				loadProject(openProjectDialog.FileName);	
			}
		}

		private void loadProject(string fileName)		
		{ 
			XmlDocument xmlDoc			=	new XmlDocument();
			xmlDoc.Load(fileName);

			Model.Load(xmlDoc);
			Model.BlocksAccross = (int)Math.Floor((float)blocksDisplay.Width / Model.GridXSize);

			setForm();
			UpdateImageList();

#if PROPRIETARY
			XmlNode parallax = xmlDoc.SelectSingleNode("//Project/parallax");
			if (parallax != null)
			{
				parallaxWindow.readParallax(parallax);
				this.parallaxWindow.loadProject();
			}
#endif
		}
		
		//-------------------------------------------------------------------------------------------------------------------
		//
		// Restore the bitmap data from the file list
		//
		//-------------------------------------------------------------------------------------------------------------------
		private void UpdateImageList()
		{
			// At this point SourceImage list is already setup in the model, so we need to check for bitmap validity, remove invalid images and establish image windows for each valid one.
			this.listBox1.Items.Clear();
			this.listBox1.Items.Add(" "+Model.Name);

			DisposeImageWindows();

			var removeNames = new List<string>();

			foreach (var image in Model.Images)
			{
				if (!image.IsImageValid)
				{
					removeNames.Add(image.Filename);
					continue;
				}

				listBox1.Items.Add(" "+Path.GetFileName(image.Filename));
				imageWindows.Add(new imageWindow { MdiParent = this });
			}

			foreach (string name in removeNames)
			{
				Model.RemoveImage(name);
			}

			if(removeNames.Count > 0)
			{				
				MessageBox.Show("Some images have been rejected as the image format is not supported", "Not Supported", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);	
			}
		}
				
		//-------------------------------------------------------------------------------------------------------------------
		//
		// Add a new image to the list
		//
		//-------------------------------------------------------------------------------------------------------------------

		private void AddImages(object sender, EventArgs e)
		{
			addImagesDialog.Multiselect		=	true;
			addImagesDialog.RestoreDirectory	=	false;
			addImagesDialog.InitialDirectory	=	ParentDirectory + "\\Renders\\";

			//addImagesDialog.DefaultExt		=	".bmp"; // Default file extension 
			addImagesDialog.FilterIndex = Model.AddImagesFilterIndex;
			if (addImagesDialog.ShowDialog(this) == DialogResult.OK)
			{

				//	setParentFolder(Path.GetFullPath(addImagesDialog.FileName));

				Model.AddImagesFilterIndex = addImagesDialog.FilterIndex;
				bool rejected = false;

				foreach (String file in addImagesDialog.FileNames)
				{
					bool found = false;
					for (int i = 0; i < Model.Images.Count; i++)
					{
						if (file == Model.Images[i].Filename)
						{
							found = true;
							rejected = true;
							break;
						}
					}

					if (found) continue;

					var image = new SourceImage(file);
					if (!image.IsImageValid) continue;

					Model.Images.Add(image);
					imageWindows.Add(new imageWindow { MdiParent = this });
					this.listBox1.Items.Add("  " + Path.GetFileName(file));
				}

				if (rejected == true)
				{
					MessageBox.Show("Duplicate files are not allowed", "Duplicates Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
			}
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// Save as, this should be 
		//
		//-------------------------------------------------------------------------------------------------------------------

		private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			projectPath		=	"";
			saveProject( sender,  e);
		}
		
		//-------------------------------------------------------------------------------------------------------------------
		//
		// Exit
		//
		//-------------------------------------------------------------------------------------------------------------------

		private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// make the images cascade
		//
		//-------------------------------------------------------------------------------------------------------------------
		
		private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LayoutMdi(MdiLayout.Cascade);
		}
		//-------------------------------------------------------------------------------------------------------------------
		//
		// make the images tiled verticle
		//
		//-------------------------------------------------------------------------------------------------------------------
		
		private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LayoutMdi(MdiLayout.TileVertical);
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// make the images tiled horizontal
		//
		//-------------------------------------------------------------------------------------------------------------------
		
		private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LayoutMdi(MdiLayout.TileHorizontal);
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// make the images Arranged ?
		//
		//-------------------------------------------------------------------------------------------------------------------

		private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LayoutMdi(MdiLayout.ArrangeIcons);
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// close all windows
		//
		//-------------------------------------------------------------------------------------------------------------------

		private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Form childForm in MdiChildren)
			{
				childForm.Close();
			}
		}

		
		//-------------------------------------------------------------------------------------------------------------------
		//
		// make the blocks, in other words do the cut and remap work init
		//
		//-------------------------------------------------------------------------------------------------------------------
	
		private void makeBlocksClick(object sender, EventArgs e)
		{
			checkBoundsOfYGridSize(sender, e);
			checkBoundsOfXGridSize(sender, e);

			if(PaletteSet==false)
			{
				var	result = MessageBox.Show("Do you want to set the palette mapping first?", "Palette mapping", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (result == DialogResult.Yes)  
				{
					openPaletteClick(sender,e);
					return;
				}
			}
			outXBlock	=	0;
			outYBlock	=	0;
			outBlock	=	0;			
			outXChar	=	0;
			outYChar	=	0;
			outChar		=	0;
			for(int b=0;b<MAX_BLOCKS;b++)
			{
				if(blockData[b]!=null)
				{ 
					blockData[b].Dispose();
					blockData[b]		=	null;
					blockData[b]		=	null;
				}
				if(blockInfo[b]!=null)
				{ 
					blockInfo[b].Dispose();
					blockInfo[b]		=	null;
					blockInfo[b]		=	null;
				}			
			}
			for(int c=0;c<MAX_CHAR_SIZE;c++)
			{
				if(tempData[c]!=null)
				{ 
					tempData[c].Dispose();
					tempData[c]=	null;
				}
			}
			CopyBlocksImage();
		}

		private void DisposeImageWindows()
		{
			foreach (imageWindow window in imageWindows)
			{
				if (window.inputImage != null)
				{
					window.inputImage.Dispose();
				}
				if (window != null)
				{
					window.Close();
					window.Dispose();
				}
			}

			imageWindows.Clear();
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// do the actual cut and remap work
		//
		//-------------------------------------------------------------------------------------------------------------------

		private void CopyBlocksImage()
		{	
			clearPanels(blocksPanel);
#if DEBUG_WINDOW
			clearPanels(DEBUG_WINDOW.DEBUG_IMAGE);
#endif
			clearPanels(charsPanel);
			blocksDisplay.Invalidate(true);
			blocksDisplay.Update();	
			charactersDisplay.Invalidate(true);
			charactersDisplay.Update();	
			this.toolStripProgressBar1.Minimum	=	0;
			this.toolStripProgressBar1.Maximum	=	10000;
			blockType thisCharType			=	new	blockType();	
			int	xChars				=	(Model.GridXSize/outSize);
			int	yChars				=	(Model.GridYSize/outSize);
			int	MaxLimit			=	MAX_CHAR_SIZE;
			int	PaletteOffset			=	0;
			bool	doingCharacters			=	true;
			tempBitmap			=	new	bitsBitmap(outSize,outSize);
			Model.GridXSize	=	int.Parse(textBox2.Text);
			Model.GridYSize =	int.Parse(textBox1.Text);

			if(Model.OutputType == OutputType.Sprites)
			{
				MaxLimit	=	128;
			}
			else
			{
				MaxLimit	=	MAX_CHAR_SIZE-1;
			}
			
			outXChar	=	0;
			outYChar	=	0;
			if(Model.OutputType == OutputType.Blocks)
			{ 
				// make the first block transparent
				if(SettingsPanel.transBlock.Checked==true)
				{ 

					if(blockData[0]==null)
					{ 
						blockData[0]	=	new	bitsBitmap(Model.GridXSize,Model.GridYSize);
						for(int y=0;y<Model.GridYSize;y++)
						{ 
							for(int x=0;x<Model.GridXSize;x++)
							{
								blockData[0].SetPixel(x,y, (short)Model.Palette.TransparentIndex);
							}
						}					
						blockToDisplay(ref blocksPanel,new Rectangle(0,0,Model.GridXSize,Model.GridYSize),ref blockData[0]);
					}					
					outXBlock	=	1;
					outBlock	=	1;
					if(blockInfo[0]==null)
					{ 
						blockInfo[0]		=	new	spriteInfo(Model.GridXSize/outSize,Model.GridYSize/outSize);
					}
				}
				if(SettingsPanel.transTile.Checked==true)
				{ 
					// and a blank character
					if(tempData[0]==null)
					{
						tempData[0]	=	new	bitsBitmap(outSize,outSize);
						for(int y=0;y<8;y++)
						{ 
							for(int x=0;x<8;x++)
							{
								tempData[0].SetPixel(x,y, (short)Model.Palette.TransparentIndex);
							}
						}					
					}
					
					outXChar	=	1;
					outChar		=	1;
				}
			}
			Model.BlocksAccross =	(int)Math.Floor((float)blocksDisplay.Width/Model.GridXSize);
			for(int s=0;s<Model.Images.Count;s++)
			{
				var image = Model.Images[s];

				if(Model.OutputType == OutputType.Blocks)
				{ 
					if((image.Image.Width%Model.GridXSize)>0)
					{
						// not width 
						MessageBox.Show("The image "+Path.GetFileName(image.Filename)+ " is not divisible by the width of your tiles, which will corrupt the output", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
					if((image.Image.Height%Model.GridYSize)>0)
					{
						// not height 
						MessageBox.Show("The image "+Path.GetFileName(image.Filename)+ " is not divisible by the height of your tiles, which will corrupt the output", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);					
					}
				}
				if(image.IsImageValid)
				{
					for(int yBlocks=0;yBlocks<((image.Image.Height+(Model.GridYSize-1))/Model.GridYSize);yBlocks++)
					{	
						for(int xBlocks=0;xBlocks<(image.Image.Width/Model.GridXSize);xBlocks++)
						{							
							src.X			=	xBlocks*Model.GridXSize;
							src.Y			=	yBlocks*Model.GridYSize;
							src.Width		=	Model.GridXSize;
							src.Height		=	Model.GridYSize;

							if(outBlock>=MAX_BLOCKS)
							{ 
								MessageBox.Show("Too many blocks/sprites", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
								blocksDisplay.Invalidate(true);
								blocksDisplay.Update();
								this.toolStripProgressBar1.Minimum	=	0;
								this.toolStripProgressBar1.Maximum	=	0;
								return;
							}							
							if(blockData[outBlock]==null)
							{ 
								blockData[outBlock]	=	new	bitsBitmap(Model.GridXSize,Model.GridYSize);
							}
							if(blockInfo[outBlock]==null)
							{ 
								blockInfo[outBlock]	=	new	spriteInfo(Model.GridXSize/outSize,Model.GridYSize/outSize);
							}
							if(SettingsPanel.reduce.Checked==true &&  Model.OutputType==OutputType.Sprites)
							{
								CopyRegionIntoBlock(image.Image, src,ref blockData[outBlock],ref blockInfo[outBlock],true);	
							}
							else
							{ 
								CopyRegionIntoBlock(image.Image, src,ref blockData[outBlock],ref blockInfo[outBlock],false);
							}

							if(SettingsPanel.FourBit.Checked==true || Model.OutputType==OutputType.Blocks)
							{ 
								remap4Bit(Model.GridXSize, Model.GridYSize);
							}
							for(int y=0;y<Model.GridYSize;y++)
							{ 
								for(int x=0;x<Model.GridXSize;x++)
								{
									if(blockData[outBlock].GetPixel(x,y)  != (short)Model.Palette.TransparentIndex)
									{ 
										goto	notBlank;
									}
								}
							}
							if(outBlock>0)
							{ 
								goto	dontDraw;
							}
notBlank:													
							for(int yChar=0;yChar<yChars;yChar++)
							{ 
								for(int xChar=0;xChar<xChars;xChar++)
								{	
									
									if(SettingsPanel.FourBit.Checked==true || Model.OutputType == OutputType.Blocks)
									{
										PaletteOffset	=		blockData[outBlock].GetPixel(xChar*outSize,yChar*outSize)&0x0f0;								
									}			
									for(int c=0;c<outChar;c++)
									{									
										thisCharType	=	checkRepeatedChar(c,xChar*outSize,yChar*outSize);
										if(thisCharType	!=	blockType.Original)
										{ 
											switch(thisCharType)
											{
												case	blockType.Repeated:			 //		 rep  flpX flpY  rot   trans
													blockInfo[outBlock].SetData(xChar,yChar,true,false,false,false,false,(short)c,(short)PaletteOffset, checkHasTrans(c));
												//	Console.Write("R   "+c.ToString()+",");
													goto	dontDrawCharacter;
												case	blockType.FlippedX:         //		 rep  flpX flpY  rot   trans
													blockInfo[outBlock].SetData(xChar,yChar,true,true,false,false,false,(short)c,(short)PaletteOffset, checkHasTrans(c));
												//	Console.Write("RX  "+c.ToString()+",");
													goto	dontDrawCharacter;
												case	blockType.FlippedY:         //		 rep  flpX flpY  rot   trans
													blockInfo[outBlock].SetData(xChar,yChar,true,false,true,false,false,(short)c,(short)PaletteOffset, checkHasTrans(c));
												//	Console.Write("RY  "+c.ToString()+",");
													goto	dontDrawCharacter;
												case	blockType.FlippedXY:         //		 rep  flpX flpY  rot   trans
													blockInfo[outBlock].SetData(xChar,yChar,true,true,true,false,false,(short)c,(short)PaletteOffset, checkHasTrans(c));
												//	Console.Write("RXY "+c.ToString()+",");
													goto	dontDrawCharacter;
												case	blockType.Rotated:         //		 rep  flpX flpY  rot   trans			
													blockInfo[outBlock].SetData(xChar,yChar,true,false,false,true,false,(short)c,(short)PaletteOffset, checkHasTrans(c));
												//	Console.Write("RR  "+c.ToString()+",");
													goto	dontDrawCharacter;
												case	blockType.FlippedXRotated:   //		 rep  flpX flpY  rot   trans		
													blockInfo[outBlock].SetData(xChar,yChar,true,true,false,true,false,(short)c,(short)PaletteOffset, checkHasTrans(c));
												//	Console.Write("RXR "+c.ToString()+",");
													goto	dontDrawCharacter;
												case	blockType.FlippedYRotated:  //		 rep  flpX flpY  rot   trans		
													blockInfo[outBlock].SetData(xChar,yChar,true,false,true,true,false,(short)c,(short)PaletteOffset, checkHasTrans(c));
												//	Console.Write("RYR "+c.ToString()+",");
													goto	dontDrawCharacter;
												case	blockType.FlippedXYRotated: //		 rep  flpX flpY  rot   trans
												//	Console.Write("RXYR"+c.ToString()+",");
													blockInfo[outBlock].SetData(xChar,yChar,true,true,true,true,false,(short)c,(short)PaletteOffset, checkHasTrans(c));
													goto	dontDrawCharacter;
												case	blockType.Transparent:      //		 rep  flpX  flpY  rot  trans
													blockInfo[outBlock].SetData(xChar,yChar,false,false,false,false,true,(short)c,(short)PaletteOffset, checkHasTrans(c));
												//	Console.Write("T   "+c.ToString()+",");
													goto	dontDrawCharacter;
											}
										}
									}			
									if(tempData[outChar]==null)
									{ 
										tempData[outChar]	=	new	bitsBitmap(outSize,outSize);
									}
									for(int y=0;y<outSize;y++)
									{ 
										for(int x=0;x<outSize;x++)
										{
											tempData[outChar].SetPixel(x,y,blockData[outBlock].GetPixel(x+(xChar*outSize),y+(yChar*outSize)));												
										}
									}
									bool	firstTrans	=	true;
									for(int y=0;y<outSize;y++)
									{ 
										for(int x=0;x<outSize;x++)
										{
											if(blockData[outBlock].GetPixel(x+(xChar*outSize),y+(yChar*outSize)) != Model.Palette.TransparentIndex)
											{
												firstTrans	=	false;
												goto		notTransFirst;
											}
										}
									}
									tempData[outChar].Trans		=	firstTrans;									
notTransFirst:								blockInfo[outBlock].SetData(xChar,yChar,false,false,false,false,firstTrans,(short)outChar,(short)PaletteOffset, checkHasTrans(outChar));
									//Console.Write("O   "+outChar.ToString()+" ");
									if(doingCharacters	== true)
									{ 
										outChar++;
									}
									if(outChar>MaxLimit)
									{
										doingCharacters	=	false;
									}
									//	MessageBox.Show("Too many characters in your tiles", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);										
									//	blocksDisplay.Invalidate(true);
									//	blocksDisplay.Update();
									//	this.toolStripProgressBar1.Minimum	=	0;
									//	this.toolStripProgressBar1.Maximum	=	0;
									//	return;
									
	
dontDrawCharacter:							;
								}
							}
							if(Model.OutputType == OutputType.Blocks)
							{ 
								BlocksLable.Text	=	"Blocks (" + outBlock.ToString() +")";
							}
							else
							{
								BlocksLable.Text	=	"Objects (" + outBlock.ToString() +")";
							}
							bool repeatedBlock = false;
							if(Model.OutputType == OutputType.Blocks)
							{
								for(int rb=0;rb<outBlock;rb++)
								{
									for(int yChar=0;yChar<yChars;yChar++)
									{ 
										for(int xChar=0;xChar<xChars;xChar++)
										{
											if(blockInfo[outBlock].GetId(xChar,yChar) != blockInfo[rb].GetId(xChar,yChar))
											{
												goto NextBlockCheck;
											}
										}
									}
									repeatedBlock = true;
									break;
NextBlockCheck:							;
								}
								
							}
							if(repeatedBlock==false)
							{ 
								outBlock++;
							}
							//Console.Write("- \r\n");
dontDraw:					;
						}
						//Console.Write("\r\n");					
						this.Invalidate(true);
						this.Update();		

					}
					//Console.Write("\r\n");
				}
			}


			tranpearntChars	=	0;
			SortedIndex	=	0;
			if(SettingsPanel.sortTransparent.Checked==true && Model.OutputType == OutputType.Blocks)
			{
				for(int passes=0;passes<2;passes++)
				{ 
					for(int s=0;s<outChar;s++)
					{ 					
						switch (passes)
						{
							case	0:
								if(checkHasTrans(s)==true)
								{
									if(charData[SortedIndex]!=null)
									{ 
										charData[SortedIndex].Dispose();
										charData[SortedIndex]	=	null;
									}						
									charData[SortedIndex]		=	new	bitsBitmap(outSize,outSize);
									for(int y=0;y<tempData[s].Height;y++)
									{									
										for(int x=0;x<tempData[s].Width;x++)
										{
											charData[SortedIndex].SetPixel(x,y,tempData[s].GetPixel(x,y));
										}
									}
									sortIndexs[s] = SortedIndex;
									tranpearntChars++;
									toDisplay(SortedIndex);
									SortedIndex++;
								}
							break;
							case	1:
								if(checkHasTrans(s)==false)
								{
									if(charData[SortedIndex]!=null)
									{ 
										charData[SortedIndex].Dispose();
										charData[SortedIndex]	=	null;
									}						
									charData[SortedIndex]		=	new	bitsBitmap(outSize,outSize);
									for(int y=0;y<tempData[s].Height;y++)
									{									
										for(int x=0;x<tempData[s].Width;x++)
										{
											charData[SortedIndex].SetPixel(x,y,tempData[s].GetPixel(x,y));
										}
									}
									sortIndexs[s] = SortedIndex;
									toDisplay(SortedIndex);
									SortedIndex++;
								}
							break;

						}
					}
				}
			}
			else
			{

				for(int s=0;s<outChar;s++)
				{ 
					if(charData[s]!=null)
					{ 
						charData[s].Dispose();
						charData[s]	=	null;
					}						
					charData[s]		=	new	bitsBitmap(outSize,outSize);
					charData[s].Trans	=	tempData[s].Trans;
					for(int y=0;y<tempData[s].Height;y++)
					{									
						for(int x=0;x<tempData[s].Width;x++)
						{
							charData[s].SetPixel(x,y,tempData[s].GetPixel(x,y));
						}
					}
					sortIndexs[s] = s;
					toDisplay(s);
				}
			}
			outXBlock	=	0; 
			outYBlock	=	0;
			for(int b=0;b<outBlock;b++)
			{
				dest.X			=	outXBlock*Model.GridXSize;
				dest.Y			=	outYBlock*Model.GridYSize;
				dest.Width		=	Model.GridXSize;
				dest.Height		=	Model.GridYSize;


				charsToBlocks(ref blocksPanel, dest, b);

				if(Model.OutputType == OutputType.Blocks)
				{
					BlocksLable.Text	=	"Blocks (" + b.ToString() +")";
				}
				else
				{
					setColissions(b);

					BlocksLable.Text	=	"Objects (" + b.ToString() +")";
				}
				outXBlock++;
				if(outXBlock>=Model.BlocksAccross)
				{
					outXBlock	=	0;
					outYBlock++;
				}

			}
			if(Model.OutputType == OutputType.Blocks)
			{ 
				SpritesLable.Text	=	"Characters (" + outChar.ToString() +"), Transparent (" + tranpearntChars.ToString() +")";
			}
			else
			{
				SpritesLable.Text	=	"Sprites (" + outChar.ToString() +")";
			}
#if DEBUG_WINDOW
			DEBUG_WINDOW.Invalidate(true);
			DEBUG_WINDOW.Update();		
#endif
			
			if(doingCharacters	==	false)
			{ 
				MessageBox.Show("Too many characters in your tiles", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);										
			}					
				
			charactersDisplay.Invalidate(true);
			charactersDisplay.Update();	
			blocksDisplay.Invalidate(true);
			blocksDisplay.Update();			
			this.toolStripProgressBar1.Minimum	=	0;
			this.toolStripProgressBar1.Maximum	=	0;
		}

		private		void	setColissions(int s)
		{
			for(int y=0;y<blockData[s].Height;y++)
			{									
				for(int x=0;x<blockData[s].Width;x++)
				{
					if(blockData[s].GetPixel(x,y) != (short)Model.Palette.TransparentIndex)
					{ 
						blockInfo[s].SetTop(y);
						goto	foundTop;
					}
				}
			}
foundTop:		for(int y=0;y<blockData[s].Height;y++)
			{									
				for(int x=0;x<blockData[s].Width;x++)
				{
					if(blockData[s].GetPixel(x,(blockData[s].Height-1)-y) != (short)Model.Palette.TransparentIndex)
					{ 
						blockInfo[s].SetBottom((blockData[s].Height-1)-y);
						goto	foundBottom;
					}
				}
			}	
foundBottom:		 for(int x=0;x<blockData[s].Width;x++)
			{
				for(int y=0;y<blockData[s].Height;y++)
				{		
					if(blockData[s].GetPixel(x,y) != (short)Model.Palette.TransparentIndex)
					{ 
						blockInfo[s].SetLeft(x);
						goto	foundLeft;
					}
				}
			}
foundLeft: 		for(int x=0;x<blockData[s].Width;x++)
			{		
				for(int y=0;y<blockData[s].Height;y++)
				{									
					if(blockData[s].GetPixel((blockData[s].Width-1)-x,y) != (short)Model.Palette.TransparentIndex)
					{ 
						blockInfo[s].SetRight((blockData[s].Width-1)-x);
						goto	foundRight;
					}
				}
			}	
foundRight:	;

		 }

		//-------------------------------------------------------------------------------------------------------------------
		//
		// Copy the bits bitmap (bitsBitmap) to a viewable bitmap
		//
		//-------------------------------------------------------------------------------------------------------------------

		private		void	charsToBlocks(ref Bitmap destBitmap, Rectangle destRegion,int currentBlock)
		{
			Color	pixelColor	=	new	Color();
			for(int chr=0;chr< (Model.GridXSize/ outSize) * (Model.GridYSize/ outSize); chr++)
			{ 
				bool	flipX		=	blockInfo[currentBlock].infos[chr].flippedX;
				bool	flipY		=	blockInfo[currentBlock].infos[chr].flippedY;
				bool	rotate		=	blockInfo[currentBlock].infos[chr].rotated;
				int	id		=	blockInfo[currentBlock].infos[chr].originalId;
				int	sortedId	=	sortIndexs[id];
				Bitmap	tempBitmap	=	new Bitmap(outSize,outSize);
				RotateFlipType	flips	=	RotateFlipType.RotateNoneFlipNone;
			//	String		flipString	=	"";
				for(int y=0;y< outSize; y++)
				{									
					for(int x=0;x< outSize; x++)
					{
						tempBitmap.SetPixel(x,y,SetFromPalette(charData[sortedId].GetPixel(x,y)));
					}
				}
				if(flipX== true && flipY == false && rotate == false)
				{
					flips = RotateFlipType.RotateNoneFlipX;
			//		flipString	=	"X";
				}
				else if(flipX == false && flipY == true && rotate == false)
				{
					flips = RotateFlipType.RotateNoneFlipY;
			//		flipString	=	"Y";
				}
				else if(flipX == true && flipY == true && rotate == false)
				{
					flips = RotateFlipType.RotateNoneFlipXY;
			//		flipString	=	"XY";
				}
				else if(flipX == false && flipY == false && rotate == true)
				{
					flips = RotateFlipType.Rotate90FlipNone;
			//		flipString	=	"R";
				}
				else if(flipX == true && flipY == false && rotate == true)
				{
					flips = RotateFlipType.Rotate90FlipX;
			//		flipString	=	"RX";
				}
				else if(flipX == false && flipY == true && rotate == true)
				{
					flips = RotateFlipType.Rotate90FlipY;
			//		flipString	=	"RY";
				}
				else if(flipX == true && flipY == true && rotate == true)
				{
					flips = RotateFlipType.Rotate90FlipXY;
			//		flipString	=	"RXY";
				}
				tempBitmap.RotateFlip(flips);
				if(blockInfo[currentBlock].infos[chr].transparent==false)
				{
					for(int y=0;y< outSize; y++)
					{									
						for(int x=0;x< outSize; x++)
						{
							pixelColor	=	tempBitmap.GetPixel(x,y);
							destBitmap.SetPixel(destRegion.X+(blockInfo[currentBlock].infos[chr].xPos* outSize) +x,destRegion.Y+(blockInfo[currentBlock].infos[chr].yPos* outSize) +y,pixelColor);
						}
					}
				//	g.DrawString(flipString, new Font("Areial",7.0f, FontStyle.Bold), new SolidBrush(Color.White),new Point(destRegion.X+(blockInfo[currentBlock].infos[chr].xPos* outSize)-2,destRegion.Y+(blockInfo[currentBlock].infos[chr].yPos* outSize) -2));
				}


			}
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// show the output
		//
		//-------------------------------------------------------------------------------------------------------------------
	
		private	 void	toDisplay(int c)
		{
			charDest.X	=	outXChar*outSize;
			charDest.Y	=	outYChar*outSize;
			charDest.Width	=	outSize;
			charDest.Height	=	outSize;
			blockToDisplay(ref charsPanel,charDest,ref charData[c]);	
#if DEBUG_WINDOW
			DEBUGToDisplay(charDest,ref charData[c]);
#endif
			outXChar++;
			if(outXChar>=charsPanel.Width/outSize)
			{
				outXChar	=	0;
				outYChar++;
			}
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// see if the character has a transparent pixel
		//
		//-------------------------------------------------------------------------------------------------------------------
	
		private	bool	checkHasTrans(int s)
		{
			for(int y=0;y<tempData[s].Height;y++)
			{									
				for(int x=0;x<tempData[s].Width;x++)
				{
					if(tempData[s].GetPixel(x,y) == (short)Model.Palette.TransparentIndex)
					{
						return	true;
					}
				}
			}
			return	false;
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// see if the character has a transparent pixel
		//
		//-------------------------------------------------------------------------------------------------------------------
	
		private	bool	blockHasTrans(int s)
		{
			for(int y=0;y<blockData[s].Height;y++)
			{									
				for(int x=0;x<blockData[s].Width;x++)
				{
					if(blockData[s].GetPixel(x,y) == (short)Model.Palette.TransparentIndex)
					{
						return	true;
					}
				}
			}
			return	false;
		}
		//-------------------------------------------------------------------------------------------------------------------
		//
		// re map the colours to 4 bits per pixel index mapping, limit them to 16 colours in the palette and get the offset
		//
		//-------------------------------------------------------------------------------------------------------------------
	
		private	void remap4Bit(int width, int height)
		{			
			AveragingIndex		=	0;
			for(int yCuts=0;yCuts<height/outSize;yCuts++)
			{ 
				for(int xCuts=0;xCuts<width/outSize;xCuts++)
				{ 
					for(int	y=0;y<outSize;y++)
					{		
						for(int	x=0;x<outSize;x++)
						{
							AveragingIndex	+=	(blockData[outBlock].GetPixel(x+(xCuts*outSize),y+(yCuts*outSize)));
						} 
					}
					AveragingIndex	=	(AveragingIndex/(outSize*outSize))&0x0f0;
				
					for(int	y=0;y<outSize;y++)
					{		
						for(int	x=0;x<outSize;x++)
						{
							
							thisIndex	=	Model.Palette.ClosestColor(SetFromPalette(blockData[outBlock].GetPixel(x+(xCuts*outSize),y+(yCuts*outSize))),(short) AveragingIndex,0); //thePalette.loadedColourStart);
							blockData[outBlock].SetPixel(x+(xCuts*outSize),y+(yCuts*outSize),thisIndex);
						} 
					}
				}
			}
		}
		
		//-------------------------------------------------------------------------------------------------------------------
		//
		// check for repeated / mirrored / rotated sprites and characters 
		//
		//-------------------------------------------------------------------------------------------------------------------
		private	blockType	checkRepeatedChar(int character, int xOffset,int yOffset)
		{
			int	samePixels	=	outSize*outSize;
			float	accurate	=	((float.Parse(SettingsPanel.Accuracy.Text)/100));
			float	pixelClose	=	samePixels*accurate;
			bool	hasTrans	=	false;
			bitsBitmap	rotateData	=	new	bitsBitmap(outSize,outSize);
			// does it have any trans pixels
			if(SettingsPanel.sortTransparent.Checked==true || Model.OutputType == OutputType.Sprites)
			{
				for(int y=0;y<outSize;y++)
				{ 
					for(int x=0;x<outSize;x++)
					{
						if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) == (short)Model.Palette.TransparentIndex)
						{							
							hasTrans	=	true;
							goto		transFound;
						}
					}
				}
			}	
transFound:		// do we ignore all repeats?
			if(SettingsPanel.Repeats.Checked==true)
			{ 
				return	blockType.Original;
			}
			// check to see if fully transparent
			if(SettingsPanel.Transparent.Checked==false && hasTrans==true)
			{
				for(int y=0;y<outSize;y++)
				{ 
					for(int x=0;x<outSize;x++)
					{
						if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) != (short)Model.Palette.TransparentIndex)
						{
							goto	RepeatedCheck;
						}
					}
				}
				return	blockType.Transparent;
			}	
RepeatedCheck:		// see how close to the original block it is	
			Int16	firstPixel	=	blockData[outBlock].GetPixel(0+xOffset,0+yOffset);
			for(int y=0;y<outSize;y++)
			{ 
				for(int x=0;x<outSize;x++)
				{
					if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) != firstPixel)
					{
						
						goto	notSolid;
					}
				}
			}
			pixelClose	=	outSize*outSize;	
notSolid:	;
			for(int y=0;y<outSize;y++)
			{ 
				for(int x=0;x<outSize;x++)
				{
					if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) != tempData[character].GetPixel(x,y))
					{
						samePixels--;
					}
				}
			}
			if(SettingsPanel.Transparent.Checked==true && hasTrans==true)
			{	
				if(samePixels==outSize*outSize)
				{
					return	blockType.Repeated;
				}
				return	blockType.Original;
			}	
			// if its close to original % and not containing transparent!
			if(samePixels>=pixelClose && (hasTrans==false  || Model.OutputType == OutputType.Sprites))
			{ 
				return	blockType.Repeated;
			}
			samePixels	=	outSize*outSize;

			if(SettingsPanel.mirrorX.Checked==false)
			{ 
				for(int y=0;y<outSize;y++)
				{ 
					for(int x=0;x<outSize;x++)
					{
						if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) != tempData[character].GetPixel((outSize-1)-x,y))	//x flip
						{
							samePixels--;
						}
					}
				}
				if(samePixels>=pixelClose)
				{ 
					return	blockType.FlippedX;
				}				
			}
			samePixels	=	outSize*outSize;
			if(SettingsPanel.mirrorY.Checked==false)
			{ 
				for(int y=0;y<outSize;y++)
				{ 
					for(int x=0;x<outSize;x++)
					{
						if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) != tempData[character].GetPixel(x,(outSize-1)-y))		// y flip
						{
							samePixels--;
						}
					}
				}
				if(samePixels>=pixelClose)
				{ 
					return	blockType.FlippedY;
				}
			}
			samePixels	=	outSize*outSize;
			if(SettingsPanel.mirrorY.Checked==false && SettingsPanel.mirrorX.Checked==false)
			{ 
				for(int y=0;y<outSize;y++)
				{ 
					for(int x=0;x<outSize;x++)
					{
						if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) != tempData[character].GetPixel((outSize-1)-x,(outSize-1)-y))	// xy flip
						{
							samePixels--;
						}
					}
				}
				if(samePixels>=pixelClose)
				{ 
					return	blockType.FlippedXY;
				}
			}
			for(int y=0;y<outSize;y++)
			{ 
				for(int x=0;x<outSize;x++)
				{
					rotateData.SetPixel((outSize-1)-y,x,tempData[character].GetPixel(x,y));
				}
			}
			

			samePixels	=	outSize*outSize;
			if(SettingsPanel.rotations.Checked==false)
			{ 
				for(int y=0;y<outSize;y++)
				{ 
					for(int x=0;x<outSize;x++)
					{
						if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) != rotateData.GetPixel(x,y))
						{
							samePixels--;
						}
					}
				}
				if(samePixels>=pixelClose)
				{ 
					return	blockType.Rotated;
				}
				samePixels	=	outSize*outSize;
				if(SettingsPanel.mirrorX.Checked==false)
				{
					for(int y=0;y<outSize;y++)
					{ 
						for(int x=0;x<outSize;x++)
						{
							if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) != rotateData.GetPixel((outSize-1)-x,y))	//x flip
							{
								samePixels--;
							}
						}
					}
					if(samePixels>=pixelClose)
					{ 
						return	blockType.FlippedXRotated;
					}
				}
				samePixels	=	outSize*outSize;
				if(SettingsPanel.mirrorY.Checked==false)
				{
					for(int y=0;y<outSize;y++)
					{ 
						for(int x=0;x<outSize;x++)
						{
							if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) !=  rotateData.GetPixel(x,(outSize-1)-y))		// y flip
							{
								samePixels--;
							}
						}
					}
					if(samePixels>=pixelClose)
					{ 
						return	blockType.FlippedYRotated;
					}
				}
				samePixels	=	outSize*outSize;
				if(SettingsPanel.mirrorY.Checked==false && SettingsPanel.mirrorX.Checked==false)
				{
					for(int y=0;y<outSize;y++)
					{ 
						for(int x=0;x<outSize;x++)
						{
							if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) != rotateData.GetPixel((outSize-1)-x,(outSize-1)-y))		// xy flip
							{
								samePixels--;
							}
						}
					}
					if(samePixels>=pixelClose)
					{ 
						return	blockType.FlippedXYRotated;
					}
				}
			}
			return	blockType.Original;
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// Copy an area from one bitmap to a bits bitmap (bitsBitmap)
		//
		//-------------------------------------------------------------------------------------------------------------------

		private void CopyRegionIntoBlock(Bitmap srcBitmap, Rectangle srcRegion,ref bitsBitmap outBlock,ref spriteInfo outInfo, bool reduce)
		{
			// clip because images may not be in blocks size 
			
			if(srcRegion.Y+srcRegion.Height>srcBitmap.Height)
			{
				srcRegion.Height	=	srcBitmap.Height-srcRegion.Y;
			}
			if(srcRegion.X+srcRegion.Width>srcBitmap.Width)
			{
				srcRegion.Width		=	srcBitmap.Width-srcRegion.X;
			}
			if(reduce==true)
			{
				// so we make the output block all transparent
				for(int	y=0;y<srcRegion.Height;y++)
				{	
					for(int	x=0;x<srcRegion.Width;x++)
					{
						outBlock.SetPixel(x,y,(short)Model.Palette.TransparentIndex);
					}
				}
				for(int	y=0;y<srcRegion.Height;y++)
				{	
					for(int	x=0;x<srcRegion.Width;x++)
					{
							
						if(Model.Palette.ClosestColor(srcBitmap.GetPixel(srcRegion.X+x,srcRegion.Y+y),-1,Model.Palette.StartIndex)!=(short)Model.Palette.TransparentIndex)
						{
							outInfo.setOffsetY((short)y);
							goto	checkLeft;
						}
					}
				}			
checkLeft:			for(int	x=0;x<srcRegion.Width;x++)
				{	
					for(int	y=0;y<srcRegion.Height;y++)
					{							
						if(Model.Palette.ClosestColor(srcBitmap.GetPixel(srcRegion.X+x,srcRegion.Y+y),-1,Model.Palette.StartIndex)!=(short)Model.Palette.TransparentIndex)
						{
							outInfo.setOffsetX((short)x);
							goto	xYDone;
						}
					}
				}
xYDone:				for(int	y=outInfo.GetOffsetY();y<srcRegion.Height;y++)
				{	
					for(int	x=outInfo.GetOffsetX();x<srcRegion.Width;x++)
					{
						outBlock.SetPixel(x-outInfo.GetOffsetX(),y-outInfo.GetOffsetY(),Model.Palette.ClosestColor(srcBitmap.GetPixel(srcRegion.X+x,srcRegion.Y+y),-1,Model.Palette.StartIndex));
					}
				}

			}
			else
			{
				outInfo.clearOffsets();			
				for(int	y=0;y<srcRegion.Height;y++)
				{	
					for(int	x=0;x<srcRegion.Width;x++)
					{
						outBlock.SetPixel(x,y,Model.Palette.ClosestColor(srcBitmap.GetPixel(srcRegion.X+x,srcRegion.Y+y),-1,Model.Palette.StartIndex));
					}
				}
			}								
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// Copy the bits bitmap (bitsBitmap) to a viewable bitmap
		//
		//-------------------------------------------------------------------------------------------------------------------

		private		void	blockToDisplay(ref Bitmap destBitmap, Rectangle destRegion,ref bitsBitmap inBlock)
		{
			for(int	y=0;y<destRegion.Height;y++)
			{		
				for(int	x=0;x<destRegion.Width;x++)
				{
					
					destBitmap.SetPixel(destRegion.X+x,destRegion.Y+y,SetFromPalette(inBlock.GetPixel(x,y)));				
				}
			}
		}
#if DEBUG_WINDOW
		private		void	DEBUGToDisplay(Rectangle destRegion,ref bitsBitmap inBlock)
		{
			for(int	y=0;y<destRegion.Height;y++)
			{		
				for(int	x=0;x<destRegion.Width;x++)
				{
					
					DEBUG_WINDOW.DEBUG_IMAGE.SetPixel(destRegion.X+x,destRegion.Y+y,SetFromPalette(inBlock.GetPixel(x,y)));				
				}
			}
		}
#endif
		//-------------------------------------------------------------------------------------------------------------------
		//
		// Set the colour from a palette in memory
		//
		//-------------------------------------------------------------------------------------------------------------------
		
		public 	Color	SetFromPalette(int theIndex)
		{
			switch(Model.Palette.Type)
			{ 
				case	PaletteType.Next256:
					return Color.FromArgb(255,Palette.SpecNext256[theIndex,0],Palette.SpecNext256[theIndex,1],Palette.SpecNext256[theIndex,2]);
				case	PaletteType.Next512:
					return Color.FromArgb(255,Palette.SpecNext512[theIndex,0],Palette.SpecNext512[theIndex,1],Palette.SpecNext512[theIndex,2]);
				case	PaletteType.Custom:
					return	Color.FromArgb(255,Model.Palette[theIndex,0],Model.Palette[theIndex,1],Model.Palette[theIndex,2]);
			}
			return	Color.FromArgb(255,255,255,255);
		}
		
		//-------------------------------------------------------------------------------------------------------------------
		//
		// take the 24 bit colours and make them 8 bit Spectrum next style
		//
		//-------------------------------------------------------------------------------------------------------------------
		
		private	byte	EightbitPalette (decimal  red,  decimal green, decimal blue)
		{
			byte	r	=	(byte)Math.Round(red/(255/7));
			byte	g	=	(byte)Math.Round(green/(255/7));
			byte	b	=	(byte)Math.Round(blue/(255/3));			
			return	(byte)((r << 5) | (g << 2) | b);

			//return	(red & 0x0E0) | ((green & 0x0E0)>>3) | (((blue & 0x0E0)>>6) | ((blue & 0x020)>>5));
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// turn a byte into a binary string
		//
		//-------------------------------------------------------------------------------------------------------------------		
		private	string	toBinary(byte num)
		{ 
			string	outString	=	"";
			int	bits		=	0x080;
			for(int bit=0;bit<8;bit++)
			{
				if((num&bits)==bits)
				{
					outString += "1";					
				}
				else
				{					
					outString += "0";
				}
				bits	=	bits >>1;
			}
			return	outString;
		}
		
		//-------------------------------------------------------------------------------------------------------------------
		//
		// paint the grid on the object/blocks display
		//
		//-------------------------------------------------------------------------------------------------------------------		

		private void blocksDisplay_Paint(object sender, PaintEventArgs e)
		{
			Graphics g	=	e.Graphics; 
			Pen pen = new Pen(Color.Black);
			float[] dashValues = { 4, 2};

			pen.DashPattern = dashValues;
			// horizontal lines
			for (int y = 0; y < (blocksDisplay.Image.Height / Model.GridYSize)+1; ++y)
			{
				g.DrawLine(pen, 0, y * Model.GridYSize, blocksDisplay.Image.Width, y * Model.GridYSize);
			}			
			// verticle lines
			for (int x = 0; x < (blocksDisplay.Image.Width/Model.GridXSize)+1; ++x)
			{
				g.DrawLine(pen, x * Model.GridXSize, 0, x * Model.GridXSize, blocksDisplay.Image.Height);
			}	
		}

		

		//-------------------------------------------------------------------------------------------------------------------
		//
		// paint the grid on the sprites/characters display
		//
		//-------------------------------------------------------------------------------------------------------------------		

		private void charsDisplay_Paint(object sender, PaintEventArgs e)
		{
			Graphics g	=	e.Graphics; 
			Pen pen = new Pen(Color.Black);
			float[] dashValues = { 1, 1};
			int	divLines	=	8;
			if(Model.OutputType == OutputType.Sprites)
			{
				divLines	=	16;
			}
			pen.DashPattern = dashValues;
			// horizontal lines
			for (int y = 0; y < (blocksDisplay.Image.Height / divLines)+1; ++y)
			{
				g.DrawLine(pen, 0, y * divLines, blocksDisplay.Image.Width, y * divLines);
			}			
			// verticle lines
			for (int x = 0; x < (blocksDisplay.Image.Width/divLines)+1; ++x)
			{
				g.DrawLine(pen, x * divLines, 0, x * divLines, blocksDisplay.Image.Height);
			}	
		}

	
		
		//-------------------------------------------------------------------------------------------------------------------
		//
		// paint the grid on the sprites/characters display
		//
		//-------------------------------------------------------------------------------------------------------------------		

		private void ouputFilesClick(object sender, EventArgs e)
		{ 
			if(blockInfo[0]==null)
			{				
				MessageBox.Show("You need to remap the graphics before you can output!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			outputFilesDialog.FileName		=	Model.Name.ToLower();
			outputFilesDialog.Filter		=	"Machine Code (*.asm)|*.asm|All Files (*.*)|*.*";
			outputFilesDialog.FilterIndex		=	Model.OutputFilesFilterIndex;
			outputFilesDialog.RestoreDirectory	=	false;				
			outputFilesDialog.InitialDirectory 	=	ParentDirectory + "\\Output\\";	
			
			if(outputFilesDialog.ShowDialog() == DialogResult.OK)
			{
					
				Model.OutputFilesFilterIndex	=		outputFilesDialog.FilterIndex;
				ouputFiles(outputFilesDialog.FileName);
			}
		}
		
		private void ouputFiles(string	outputFilesDialogFileName)
		{			
			//writeType	outputFileType		=	writeType.Assember;	
			int		xOffset			=	0;
			int		yOffset			=	0;
			byte		colourByte		=	0;
			byte		writeByte		=	0;
			int		lineNumber		=	10000;
			int		lineStep		=	5;
			string		lableString		=	"Sprites";
			string		tilesSave;
			int		binSize			=	0;
			int	numColours			=	Model.Palette.UsedCount;
			int	startColour			=	Model.Palette.StartIndex;
			switch (Model.CenterPosition)
			{
				case	0:						
					xOffset			=	-(Model.GridXSize/2);
					yOffset			=	-(Model.GridYSize/2);
				break;					
				case	1:						
					xOffset			=	0;
					yOffset			=	-(Model.GridYSize/2);
				break;									
				case	2:						
					xOffset			=	(Model.GridXSize/2);
					yOffset			=	-(Model.GridYSize/2);
				break;
				case	3:						
					xOffset			=	-(Model.GridXSize/2);
					yOffset			=	0;
				break;					
				case	4:						
					xOffset			=	0;
					yOffset			=	0;
				break;									
				case	5:						
					xOffset			=	(Model.GridXSize/2);
					yOffset			=	0;
				break;
				case	6:						
					xOffset			=	-(Model.GridXSize/2);
					yOffset			=	(Model.GridYSize/2);
				break;					
				case	7:						
					xOffset			=	0;
					yOffset			=	(Model.GridYSize/2);
				break;									
				case	8:						
					xOffset			=	(Model.GridXSize/2);
					yOffset			=	(Model.GridYSize/2);
				break;
			}
			 	
				// Define date to be displayed.
			DateTime todaysDate		=	DateTime.Now;
			string	lableNames		=	Regex.Replace(Model.Name,@"\s+", "");
			if(Path.HasExtension(outputFilesDialogFileName)==false)
			{
				outPath			=	outputFilesDialogFileName + ".asm";
				//outputFileType		=	writeType.Assember;
			}
			else
			{
				outPath			=	Path.ChangeExtension(outputFilesDialogFileName,"asm");
				//outputFileType		=	writeType.Assember;
			}
			if(SettingsPanel.binaryOut.Checked==true)
			{ 
				binPath			=	Path.ChangeExtension(outputFilesDialogFileName,"bin");
				tilesPath		=	Path.ChangeExtension(outputFilesDialogFileName,"til");
			}
			using (StreamWriter outputFile	=	new StreamWriter(outPath))
			{ 

				outputFile.WriteLine("// " + Model.Name + ".asm");
				outputFile.WriteLine("// Created on " + todaysDate.ToString("F", CultureInfo.CreateSpecificCulture("en-US")) + " by the NextGraphics tool from");
				outputFile.WriteLine("// patricia curtis at luckyredfish dot com\r\n");
				outputFile.WriteLine((lableNames + "_Colours").ToUpper() +":\t\tequ\t" + numColours.ToString() + "\r\n");

				if(Model.Palette.Type == PaletteType.Custom)
				{ 
					outputFile.WriteLine(lableNames + "Palette:");
					for(int j=0;j<numColours;j++)
					{
						if(SettingsPanel.comments.SelectedIndex==(int)comments.fullComments)
						{ 
							outputFile.WriteLine(	"\t\t\tdb\t%" + toBinary(EightbitPalette(Model.Palette[startColour+j,0],Model.Palette[startColour+j,1],Model.Palette[startColour+j,2])) +
										"\t//\t" + Model.Palette[startColour+j,0].ToString() +","+ Model.Palette[startColour+j,1].ToString() +","+ Model.Palette[startColour+j,2].ToString());
						}
						else
						{
							outputFile.WriteLine("\t\t\tdb\t%" + toBinary(EightbitPalette(Model.Palette[startColour+j,0],Model.Palette[startColour+j,1],Model.Palette[startColour+j,2])));
						}						
					}
				}
				else
				{

					if(Model.Palette.Type == PaletteType.Next256)
					{
						if(SettingsPanel.comments.SelectedIndex==(int)comments.fullComments)
						{ 
							outputFile.WriteLine("// Mapped to the spectrum next 256 palette");							
							lineNumber	+=	lineStep;

						}
					}
					else
					{
						if(SettingsPanel.comments.SelectedIndex==(int)comments.fullComments)
						{ 							
							outputFile.WriteLine("// Mapped to the spectrum next 512 palette");
						}
					}
				}
				if(Model.OutputType == OutputType.Sprites)
				{
					lableString	=	"Sprite";
				}
				else
				{
					lableString	=	"Tile";
				}
				outputFile.Write("\r\n");
				if(SettingsPanel.FourBit.Checked==true  && Model.OutputType == OutputType.Sprites)
				{ 
					outputFile.WriteLine((lableNames + "_" + lableString).ToUpper() + "_SIZE:\t\tequ\t128\r\n");
				}
				else if(Model.OutputType == OutputType.Sprites)
				{
					outputFile.WriteLine((lableNames + "_" + lableString).ToUpper() + "_SIZE:\t\tequ\t256\r\n");
				}
				else
				{
					outputFile.WriteLine((lableNames + "_" + lableString).ToUpper() + "_SIZE:\t\tequ\t32\r\n");
				}

				outputFile.WriteLine((lableNames + "_" + lableString).ToUpper() + "S:\t\tequ\t" + outChar.ToString() +"\r\n");

				// write the pixel data to the sprites
				if(SettingsPanel.binaryOut.Checked==true)
				{ 
					binSize	=	0;
					using(BinaryWriter binFile	=	new BinaryWriter(File.Open(binPath, FileMode.OpenOrCreate)))
					{ 
						for(int s=0;s<outChar;s++)
						{
							if(charData[s].Trans == true && Model.OutputType == OutputType.Sprites && SettingsPanel.Transparent.Checked == false)
							{
								continue;
							}
							for(int y=0;y<charData[s].Height;y++)
							{									
								for(int x=0;x<charData[s].Width;x++)
								{	
									if(SettingsPanel.FourBit.Checked==true || Model.OutputType == OutputType.Blocks)
									{ 
										colourByte		=	(byte)(charData[s].GetPixel(x,y)&0x0f);
										if((x&1)==0)
										{ 
#if PROPRIETARY
											writeByte	=	(byte) ((this.parallaxWindow.getColour(colourByte)&0x0f)<<4);
#else
											writeByte	=	(byte)((colourByte)<<4);											//	writeByte	=	(byte)((charData[s].GetPixel(x,y)&0x0f)<<4);
#endif
										}
										else
										{
#if PROPRIETARY
											writeByte	=	(byte) (writeByte | (this.parallaxWindow.getColour(colourByte)&0x0f));
#else
											writeByte	=	((byte) (writeByte | colourByte));								// writeByte	=	(byte) (writeByte | (charData[s].GetPixel(x,y)&0x0f));	
#endif
																							
											binFile.Write(writeByte);
											binSize++;
										}
									}
									else
									{											
										binFile.Write(charData[s].GetPixel(x,y));
										binSize++;
									}									
								}
							}
						}
					}
				}
				else
				{						
					for(int s=0;s<outChar;s++)
					{
						outputFile.WriteLine(lableNames + lableString + s.ToString() +":");
						for(int y=0;y<charData[s].Height;y++)
						{
							outputFile.Write("\t\t\t\t.db\t");

							for(int x=0;x<charData[s].Width;x++)
							{	
								if(SettingsPanel.FourBit.Checked==true || Model.OutputType == OutputType.Blocks)
								{ 
									if((x&1)==0)
									{ 
										writeByte	=	(byte)((charData[s].GetPixel(x,y)&0x0f)<<4);
									}
									else
									{
										writeByte	=	(byte) (writeByte | (charData[s].GetPixel(x,y)&0x0f));
										outputFile.Write("${0:x2}",writeByte);
										if(x<(charData[s].Width-1))
										{										
											outputFile.Write(",");
										}
										else
										{										
											outputFile.Write("\r\n");
										}											
									}
								}
								else
								{		
									outputFile.Write("${0:x2}",charData[s].GetPixel(x,y));
									if(x<(charData[s].Width-1))
									{										
										outputFile.Write(",");
									}
									else
									{										
										outputFile.Write("\r\n");
									}
								}									
							}
						}
					}
				}
				if(SettingsPanel.comments.SelectedIndex==(int)comments.fullComments && Model.OutputType == OutputType.Sprites)
				{ 
					outputFile.WriteLine("\r\n\t\t\t\t// number of sprites\r\n");
					outputFile.WriteLine("\t\t\t\t\t// x offset from center of sprite");
					outputFile.WriteLine("\t\t\t\t\t// y offset from center of sprite");
					outputFile.WriteLine("\t\t\t\t\t// Palette offset with the X mirror,Y mirror, Rotate bits if set");
					outputFile.WriteLine("\t\t\t\t\t// 4 bit colour bit and pattern offset bit");
					outputFile.WriteLine("\t\t\t\t\t// index of the sprite at this position that makes up the frame\r\n");							
					outputFile.WriteLine("\t\t\t\t//...... repeated wide x tall times\r\n");
							
				}
				else if(SettingsPanel.comments.SelectedIndex==(int)comments.fullComments)
				{
					outputFile.WriteLine("\r\n\t\t\t\t// block data");
					outputFile.WriteLine("\t\t\t\t// number of tiles (characters) tall");
					outputFile.WriteLine("\t\t\t\t// number of tiles (characters) wide");
					outputFile.WriteLine("\t\t\t\t\t// Palette offset with the X mirror,Y mirror, Rotate bits if set");
					outputFile.WriteLine("\t\t\t\t\t// index of the character at this position that makes up the block");							
					outputFile.WriteLine("\t\t\t\t//...... repeated wide x tall times\r\n");		
						
					outputFile.WriteLine("\t\t\t\t//Note: Blocks/Tiles/characters output block 0 and tile 0 is blank.\r\n");
					outputFile.WriteLine(lableNames + lableString + "Width:\tequ\t" + blockInfo[0].Width.ToString() );
					outputFile.WriteLine(lableNames + lableString + "Height:\tequ\t" + blockInfo[0].Height.ToString() );
				}		
	
				int		spriteCount		=	0;
				int	idReduction	=	0;
				if(charData[0].Trans == true && Model.OutputType == OutputType.Sprites && SettingsPanel.Transparent.Checked == false)
				{
					idReduction	=	1;
				}
				if(SettingsPanel.binaryOut.Checked==true && SettingsPanel.binaryBlocks.Checked==true)
				{ 
					using(BinaryWriter tileFile	=	new BinaryWriter(File.Open(tilesPath, FileMode.OpenOrCreate)))
					{ 						
						for(int s=0;s<outBlock;s++)
						{			
							// check to see if any square is transparent

							if(Model.OutputType == OutputType.Sprites)
							{ 
								spriteCount	=	blockInfo[s].Width*blockInfo[s].Height;

								for(int y=0;y<blockInfo[s].Height;y++)
								{
									for(int x=0;x<blockInfo[s].Width;x++)
									{
										if(blockInfo[s].GetTransparent(x,y)==true)
										{
											spriteCount--;
										}
									}
								}
								tileFile.Write(spriteCount);
							}
							for(int y=0;y<blockInfo[s].Height;y++)
							{
								for(int x=0;x<blockInfo[s].Width;x++)
								{											
									if(Model.OutputType == OutputType.Sprites)
									{
										if(blockInfo[s].GetTransparent(x,y)==true)
										{
											continue;
										}
										tileFile.Write(blockInfo[s].GetOffsetX()+(xOffset+(blockInfo[s].GetXPos(x,y)*outSize)));
										tileFile.Write(blockInfo[s].GetOffsetY()+(yOffset+(blockInfo[s].GetYpos(x,y)*outSize)));
											
										writeByte	=	0;
										if(blockInfo[s].GetPaletteOffset(x,y)!=0)
										{
											writeByte = (byte)blockInfo[s].GetPaletteOffset(x,y);
										}
										if(blockInfo[s].GetFlippedX(x,y)==true)
										{
											writeByte = (byte)(writeByte | 8);
										}
										if(blockInfo[s].GetFlippedY(x,y)==true)
										{
											writeByte = (byte)(writeByte | 4);
										}
										if(blockInfo[s].GetRotated(x,y)==true)
										{
											writeByte = (byte)(writeByte | 2);
										}					
										tileFile.Write(writeByte);											
										writeByte	=	0;								
										if(SettingsPanel.FourBit.Checked==true)
										{ 										
											writeByte = (byte)(writeByte | 128);
											if(((blockInfo[s].GetId(x,y)-idReduction)&1)==1)
											//if(((blockInfo[s].GetId(x,y))&1)==1)
											{				
												writeByte = (byte)(writeByte | 64);
											}
											tileFile.Write(writeByte);											
											//tileFile.Write(blockInfo[s].GetId(x,y)-idReduction);																		
											tileFile.Write((blockInfo[s].GetId(x,y)-idReduction)/2);
										}
										else
										{ 
											tileFile.Write(writeByte);											
											tileFile.Write(blockInfo[s].GetId(x,y));												
										}
									}
									else
									{
										writeByte	=	(byte)sortIndexs[blockInfo[s].GetId(x,y)];
										tileFile.Write( writeByte );	

										writeByte	=	0;									
										if(blockInfo[s].GetPaletteOffset(x,y)!=0)
										{
											writeByte = (byte)blockInfo[s].GetPaletteOffset(x,y);
										}
										if(blockInfo[s].GetFlippedX(x,y)==true)
										{
											writeByte = (byte)(writeByte | 8);
										}
										if(blockInfo[s].GetFlippedY(x,y)==true)
										{
											writeByte = (byte)(writeByte | 4);
										}
										if(blockInfo[s].GetRotated(x,y)==true)
										{
											writeByte = (byte)(writeByte | 2);
										}	
										tileFile.Write(writeByte);
									}								
								}					
							}
						}

					}
				}
				else
				{ 

					outputFile.Write("// Collisions Left Width Top Height\n");
					for(int s=0;s<outBlock;s++)
					{				
						if(Model.OutputType == OutputType.Sprites)
						{ 
							Rectangle			collision = new Rectangle(blockInfo[s].Left,blockInfo[s].Top,blockInfo[s].Right-blockInfo[s].Left,blockInfo[s].Bottom-blockInfo[s].Top);
							outputFile.Write(lableNames + "Collision" + s.ToString() +":");
							outputFile.Write("\t\t.db\t");
							outputFile.Write(collision.X.ToString() + ",");
							outputFile.Write(collision.Width.ToString() + ",");
							outputFile.Write(collision.Y.ToString() + ",");
							outputFile.Write(collision.Height.ToString() + "\n");
						}
					}
					outputFile.Write("\r\n");
					for(int s=0;s<outBlock;s++)
					{				
						if(Model.OutputType == OutputType.Sprites)
						{ 
							spriteCount	=	blockInfo[s].Width*blockInfo[s].Height;

							for(int y=0;y<blockInfo[s].Height;y++)
							{
								for(int x=0;x<blockInfo[s].Width;x++)
								{
									if(blockInfo[s].GetTransparent(x,y)==true)
									{
										spriteCount--;
									}
								}
							}

							outputFile.Write(lableNames + "Frame" + s.ToString() +":");
							outputFile.Write("\t\t.db\t");
							//outputFile.Write(blockInfo[s].Width.ToString() + ",");
							//outputFile.Write(blockInfo[s].Height.ToString() + ",\t");
							outputFile.Write(spriteCount.ToString() + ",\t");
						}
						else
						{		

							outputFile.Write(lableNames + "Block" + s.ToString() +":");							
							outputFile.Write("\t\t.db\t");

						}	
						// adjust x y pos based on the centerPanel setting
						for(int y=0;y<blockInfo[s].Height;y++)
						{

							int	writtenWidth		=	blockInfo[s].Width;
							for(int x=0;x<blockInfo[s].Width;x++)
							{				
								if(Model.OutputType == OutputType.Sprites)
								{ 
									if(blockInfo[s].GetTransparent(x,y)==true)
									{
										writtenWidth--;
										continue;
									}									
									outputFile.Write((blockInfo[s].GetOffsetX()+(xOffset+(blockInfo[s].GetXPos(x,y)*outSize))).ToString() + ",");
									outputFile.Write((blockInfo[s].GetOffsetY()+(yOffset+(blockInfo[s].GetYpos(x,y)*outSize))).ToString() + ",");
									string		textFlips	=	"";
									writeByte	=	0;
									if(blockInfo[s].GetPaletteOffset(x,y)!=0)
									{
										writeByte = (byte)blockInfo[s].GetPaletteOffset(x,y);
									}
									if(blockInfo[s].GetFlippedX(x,y)==true)
									{
										writeByte = (byte)(writeByte | 8);
										textFlips	=	"+XFLIP";

									}
									if(blockInfo[s].GetFlippedY(x,y)==true)
									{
										writeByte = (byte)(writeByte | 4);
										textFlips	+=	"+YFLIP";
									}
									if(blockInfo[s].GetRotated(x,y)==true)
									{
										writeByte = (byte)(writeByte | 2);
										textFlips	+=	"+ROT";
									}				
									if(SettingsPanel.textFlips.Checked==true)
									{
										outputFile.Write("\t"+lableNames.ToUpper()+"_OFFSET"+textFlips + ",\t");
									}
									else
									{ 
										outputFile.Write("\t"+lableNames.ToUpper()+"_OFFSET+"+writeByte.ToString() + ",\t");
									}
									writeByte	=	0;								
									if(SettingsPanel.FourBit.Checked==true)
									{ 										
										writeByte = (byte)(writeByte | 128);
										if(((blockInfo[s].GetId(x,y)-idReduction)&1)==1)
										{				
											writeByte = (byte)(writeByte | 64);
										}
									}
									outputFile.Write(writeByte.ToString() + ",");

									if(SettingsPanel.FourBit.Checked==true)
									{ 
										outputFile.Write(((blockInfo[s].GetId(x,y)-idReduction)/2).ToString());		
									}
									else
									{
										outputFile.Write(blockInfo[s].GetId(x,y).ToString());	
									}
									if(x<(writtenWidth)-1)
									{
										outputFile.Write(",\t");
									}
								}
								else
								{
									writeByte	=	0;
									
									if(blockInfo[s].GetPaletteOffset(x,y)!=0)
									{
										writeByte = (byte)blockInfo[s].GetPaletteOffset(x,y);
									}
									if(blockInfo[s].GetFlippedX(x,y)==true)
									{
										writeByte = (byte)(writeByte | 8);
									}
									if(blockInfo[s].GetFlippedY(x,y)==true)
									{
										writeByte = (byte)(writeByte | 4);
									}
									if(blockInfo[s].GetRotated(x,y)==true)
									{
										writeByte = (byte)(writeByte | 2);
									}	
									outputFile.Write(writeByte.ToString() + ",");
									writeByte	=	0;	
									outputFile.Write( sortIndexs[blockInfo[s].GetId(x,y)].ToString());		
									if(x<(blockInfo[s].Width)-1)
									{
										outputFile.Write(",\t");
									}
								}
								
							}									
							if(y<(blockInfo[s].Height)-1)
							{
								if(blockInfo[s].GetTransparent(blockInfo[s].Width-1,y)==false || Model.OutputType == OutputType.Blocks)
								{
									outputFile.Write(",\t");
								}
								if(spriteCount>10 && Model.OutputType == OutputType.Sprites)
								{
									outputFile.Write("\r\n\t\t\t\t.db\t\t");
								}
							}
							else
							{ 
								outputFile.Write("\r\n");
							}						
						}
					}
				}									
				outputFile.Write("\r\n");
				if(Model.OutputType == OutputType.Sprites)
				{ 
					outputFile.Write(lableNames + "Frames:\t\t.dw\t");
					for(int s=0;s<outBlock;s++)
					{
						outputFile.Write(lableNames + "Frame" + s.ToString());
						if(s<(outBlock)-1)
						{
							outputFile.Write(",");
						}
						else
						{ 
							outputFile.Write("\r\n");
						}
					}
				}
				if (SettingsPanel.binaryOut.Checked == true)
				{
					outputFile.Write(lableNames.ToUpper()+"_FILE_SIZE\tequ\t" + binSize.ToString());

					outputFile.Write("\r\n"+ lableNames	+	"File:\t\t\tdw\t" + Model.Name.ToUpper()+"_FILE_SIZE\r\n");
					outputFile.Write("\t\t\tdb\tPATH,\"game/level1/" + lableNames.ToLower() + ".bin\",0\r\n");

				}
			}
				
			int	yPos	=	0;
			int	xPos	=	0;
			// now output a blocks file BMP 
			if(Model.OutputType == OutputType.Blocks)
			{ 
				int	outBlocksAcross		=	int.Parse(SettingsPanel.tilesAcross.Text);
				int	outBlocksDown		=	(int)Math.Round((double)outBlock/outBlocksAcross)+1;

				string	blocksOutPath		=	Path.GetDirectoryName(outputFilesDialogFileName);						
				string	blocksOutFilename	=	Path.GetFileNameWithoutExtension(outputFilesDialogFileName);
				if(SettingsPanel.blocksOut.Checked==true)
				{ 
					Bitmap	outBlocks	=	new	Bitmap(Model.GridXSize*outBlocksAcross,Model.GridYSize*outBlocksDown,PixelFormat.Format24bppRgb);
					{
						yPos			=	0;
						xPos			=	0;
						int	startBlock	=	0;
						if(SettingsPanel.transBlock.Checked==false)
						{
							startBlock	=	1;
						}
						for(int b=startBlock;b<outBlock;b++)
						{
							for(int	y=0;y<Model.GridYSize;y++)
							{		
								for(int	x=0;x<Model.GridXSize;x++)
								{									
									outBlocks.SetPixel(x+(xPos*Model.GridXSize),yPos+y,SetFromPalette(blockData[b].GetPixel(x,y)));
								}
							}	
							xPos++;
							if(xPos>=outBlocksAcross)
							{ 
								xPos	=	0;
								yPos	+=	Model.GridYSize;		
							}			
						}
						switch(SettingsPanel.blocksFormat.SelectedIndex)
						{
							case	0: // bmp
								outBlocks.Save(blocksOutPath+"\\"+blocksOutFilename+"-blocks.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
								break;
							case	1: // png
							default:
								outBlocks.Save(blocksOutPath+"\\"+blocksOutFilename+"-blocks.png", System.Drawing.Imaging.ImageFormat.Png);
								break;
							case	2: // jpeg
								outBlocks.Save(blocksOutPath+"\\"+blocksOutFilename+"-blocks.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
								break;
						}
							
					}
				}
				if(SettingsPanel.tilesOut.Checked==true)
				{ 					
					int	Across	=	int.Parse(SettingsPanel.tilesAcross.Text);
					int	Down	=	(int)Math.Round((double)outChar/Across)+1;
					Bitmap	outChars	=	new	Bitmap(8*Across,8*Down,PixelFormat.Format24bppRgb);
					{
						yPos			=	0;
						xPos			=	0;
						int	startChar	=	0;
						if(SettingsPanel.transTile.Checked==false)
						{
							startChar	=	1;
						}
						for(int b=startChar;b<outChar;b++)
						{
							for(int	y=0;y<8;y++)
							{		
								for(int	x=0;x<8;x++)
								{
									outChars.SetPixel(x+(xPos*8),yPos+y,SetFromPalette(charData[b].GetPixel(x,y)));
								}
							}	
							xPos++;
							if(xPos>=Across)
							{ 
								xPos	=	0;
								yPos	+=	8;
								if(yPos>=outChars.Height)
								{
									break;
								}
							}
						}
						switch(SettingsPanel.blocksFormat.SelectedIndex)
						{
							case	0: // bmp
								tilesSave	=	blocksOutPath+"\\"+blocksOutFilename+"-tiles.bmp";
								outChars.Save(tilesSave, System.Drawing.Imaging.ImageFormat.Bmp);
								break;
							case	1: // png
							default:
								tilesSave	=	blocksOutPath+"\\"+blocksOutFilename+"-tiles.png";
								outChars.Save(tilesSave, System.Drawing.Imaging.ImageFormat.Png);
								break;
							case	2: // jpeg
								tilesSave	=	blocksOutPath+"\\"+blocksOutFilename+"-tiles.jpg";
								outChars.Save(tilesSave, System.Drawing.Imaging.ImageFormat.Jpeg);
								break;
						}
					}									
					using(BinaryWriter reverseFile	=	new BinaryWriter(File.Open(Path.ChangeExtension(outputFilesDialogFileName,"blk"), FileMode.OpenOrCreate)))
					{ 
							
						byte	xChars			=	(byte)(Model.GridXSize/outSize);
						byte	yChars			=	(byte)(Model.GridYSize/outSize);
						int	outInt			=	0;														
						reverseFile.Write(tilesSave);
						reverseFile.Write(xChars);
						reverseFile.Write(yChars);
						reverseFile.Write((byte)outBlock);
						for(int b=0;b<outBlock;b++)
						{								
							for(int	y=0;y<yChars;y++)
							{		
								for(int	x=0;x<xChars;x++)
								{	
									outInt	=	sortIndexs[blockInfo[b].GetId(x,y)];
									if(blockInfo[b].GetFlippedX(x,y)==true)
									{
										outInt	=	outInt | 1<<15;
									}
									if(blockInfo[b].GetFlippedY(x,y)==true)
									{
										outInt	=	outInt | 1<<14;
									}
									if(blockInfo[b].GetRotated(x,y)==true)
									{
										outInt	=	outInt | 1<<13;
									}										
									reverseFile.Write((short)outInt);									
								}
							}			
						}
					}
						

				}
					
				if(SettingsPanel.binaryOut.Checked==true)
				{
					byte	paletOffset	=	0;
					
					if(offsetPanel.ShowDialog() == DialogResult.OK)
					{
						paletOffset	=	(byte)offsetPanel.paletteOffset.Value;
					}

					using(BinaryWriter mapFile	=	new BinaryWriter(File.Open(Path.ChangeExtension(outputFilesDialogFileName,"map"), FileMode.OpenOrCreate)))
					{ 
							
						byte	xChars			=	(byte)(Model.GridXSize/outSize);
						byte	yChars			=	(byte)(Model.GridYSize/outSize);
						int	outInt			=	0;														
						for(int b=0;b<outBlock;b++)
						{								
							for(int	y=0;y<yChars;y++)
							{		
								for(int	x=0;x<xChars;x++)
								{	
									outInt	=	sortIndexs[blockInfo[b].GetId(x,y)];
									mapFile.Write((byte)outInt);	
									outInt = paletOffset;
									if(blockInfo[b].GetFlippedX(x,y)==true)
									{
										outInt	=	outInt | 1<<3;
									}
									if(blockInfo[b].GetFlippedY(x,y)==true)
									{
										outInt	=	outInt | 1<<2;
									}
									if(blockInfo[b].GetRotated(x,y)==true)
									{
										outInt	=	outInt | 1<<1;
									}											
									mapFile.Write((byte)outInt);									
								}
							}			
						}
					}
				}
			}
			else if(Model.OutputType == OutputType.Sprites)	// must be sprites out
			{
				if(SettingsPanel.tilesOut.Checked==true)
				{ 					
						
					string	blocksOutPath		=	Path.GetDirectoryName(outputFilesDialogFileName);						
					string	blocksOutFilename	=	Path.GetFileNameWithoutExtension(outputFilesDialogFileName);

					int	Across	=	int.Parse(SettingsPanel.tilesAcross.Text);
					int	Down	=	(int)Math.Round((double)outChar/Across);
					Bitmap	outChars	=	new	Bitmap(16*Across,16*Down,PixelFormat.Format24bppRgb);
					{
						yPos			=	0;
						xPos			=	0;
						int	startChar	=	0;

						for(int b=startChar;b<outChar;b++)
						{
							for(int	y=0;y<16;y++)
							{		
								for(int	x=0;x<16;x++)
								{
									outChars.SetPixel(x+(xPos*16),yPos+y,SetFromPalette(charData[b].GetPixel(x,y)));
								}
							}	
							xPos++;
							if(xPos>=Across)
							{ 
								xPos	=	0;
								yPos	+=	16;			
							}
						}
						switch(SettingsPanel.blocksFormat.SelectedIndex)
						{
							case	0: // bmp
								tilesSave	=	blocksOutPath+"\\"+blocksOutFilename+"-sprites.bmp";
								outChars.Save(tilesSave, System.Drawing.Imaging.ImageFormat.Bmp);
								break;
							case	1: // png
							default:
								tilesSave	=	blocksOutPath+"\\"+blocksOutFilename+"-sprites.png";
								outChars.Save(tilesSave, System.Drawing.Imaging.ImageFormat.Png);
								break;
							case	2: // jpeg
								tilesSave	=	blocksOutPath+"\\"+blocksOutFilename+"-sprites.jpg";
								outChars.Save(tilesSave, System.Drawing.Imaging.ImageFormat.Jpeg);
								break;
						}
					}
					for(int s=0;s<outBlock;s++)
					{		
						int	fx			=	0;
						int	fy			=	0;
						byte	xChars			=	(byte)(Model.GridXSize/outSize);
						byte	yChars			=	(byte)(Model.GridYSize/outSize);
						Bitmap	outSprite		=	new	Bitmap(Model.GridXSize,Model.GridYSize,PixelFormat.Format24bppRgb);
						{
							for(int	y=0;y<yChars;y++)
							{		
								for(int	x=0;x<xChars;x++)
								{		
										
									for(int	yp=0;yp<16;yp++)
									{		
										for(int	xp=0;xp<16;xp++)
										{
											
											if(blockInfo[s].GetTransparent(x,y)==true)
											{
												outSprite.SetPixel(xp+(x*16),yp+(y*16),SetFromPalette(Model.Palette.TransparentIndex));
											}
											else
											{
												fx	=	xp;
												fy	=	yp;
												if(blockInfo[s].GetFlippedX(x,y)==true)
												{
													fx	=	15-xp;
												}
												if(blockInfo[s].GetFlippedY(x,y)==true)
												{
													fy	=	15-yp;
												}
												outSprite.SetPixel(xp+(x*16),yp+(y*16),SetFromPalette(charData[blockInfo[s].GetId(x,y)].GetPixel(fx,fy)));
											}
										}
									}

								}
							}
							tilesSave	=	blocksOutPath+"\\"+blocksOutFilename+"-sprite"+ s.ToString() +".png";
							outSprite.Save(tilesSave, System.Drawing.Imaging.ImageFormat.Png);
						}
					}
				}
			}
			if(SettingsPanel.binaryOut.Checked==true)
			{
				using(BinaryWriter paletteFile	=	new BinaryWriter(File.Open(Path.ChangeExtension(outputFilesDialogFileName,"pal"), FileMode.OpenOrCreate)))
				{
					paletteFile.Write((byte) numColours);
					for(int j=0;j<numColours;j++)
					{
						paletteFile.Write((byte) EightbitPalette(Model.Palette[startColour+j,0],Model.Palette[startColour+j,1],Model.Palette[startColour+j,2]));
														
					}
				}
			}
			
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// open the palette
		//
		//-------------------------------------------------------------------------------------------------------------------	
		private void openPaletteClick(object sender, EventArgs e)
		{
			PaletteForm.StartPosition	=	FormStartPosition.CenterParent;
			PaletteForm.Model = Model;
			PaletteForm.ShowDialog();
			if (PaletteForm.DialogResult == DialogResult.OK)
			{
				PaletteSet	=	true;
			}			
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// adjust the position of the files in the list down
		//
		//-------------------------------------------------------------------------------------------------------------------	
		private void moveDownArrowClick(object sender, EventArgs e)
		{
			// move down
			int	thisIndex		=	this.listBox1.SelectedIndex;
			if(thisIndex<=Model.Images.Count-1 && thisIndex>0)
			{
				
				var	temp			=	Model.Images[thisIndex];
				Model.Images[thisIndex]		=	Model.Images[thisIndex-1];
				Model.Images[thisIndex-1]		=	temp;
				this.listBox1.SelectedIndex	=	thisIndex+1;
				UpdateImageList();
			}		
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// adjust the position of the files in the list up
		//
		//-------------------------------------------------------------------------------------------------------------------	

		private void moveUpArrowClick(object sender, EventArgs e)
		{
			//Move up
			
			int	thisIndex		=	this.listBox1.SelectedIndex;
			if(thisIndex>1 && thisIndex>0)
			{
				thisIndex--;
				var	temp			=	Model.Images[thisIndex-1];
				Model.Images[thisIndex-1]		=	Model.Images[thisIndex];
				Model.Images[thisIndex]		=	temp;
				this.listBox1.SelectedIndex	=	thisIndex;
				UpdateImageList();
			}
		}
		
		//-------------------------------------------------------------------------------------------------------------------
		//
		// delete a file from the list
		//
		//-------------------------------------------------------------------------------------------------------------------	
		private void removeFileFromListClick(object sender, EventArgs e)
		{
			if(this.listBox1.SelectedIndex>0)
			{
				foreach (Form childForm in MdiChildren)
				{
					childForm.Close();
				}
				var	result = MessageBox.Show("Remove this image?", "Are you Sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (result == DialogResult.Yes)  
				{
					Model.Images.RemoveAt(this.listBox1.SelectedIndex-1);
					UpdateImageList();
				}
			}
		}
		
		//-------------------------------------------------------------------------------------------------------------------
		//
		// set the output to blocks
		//
		//-------------------------------------------------------------------------------------------------------------------	
		
		private void blocksCheckedChanged(object sender, EventArgs e)
		{
			RadioButton rb = sender as RadioButton;
			if (rb.Checked)
			{
				// Keep track of the selected RadioButton by saving a reference
				// to it.
				selectedRadio = rb;
			}
			Model.OutputType			=	OutputType.Blocks;
			outSize			=	8;
			charactersDisplay.Invalidate();
			charactersDisplay.Refresh();		
		}
			
		//-------------------------------------------------------------------------------------------------------------------
		//
		// set the output to sprites
		//
		//-------------------------------------------------------------------------------------------------------------------	
	
		private void spritesCheckedChanged(object sender, EventArgs e)
		{
			RadioButton rb = sender as RadioButton;
			if (rb.Checked)
			{
				// Keep track of the selected RadioButton by saving a reference
				// to it.
				selectedRadio = rb;
			}			
			Model.OutputType			=	OutputType.Sprites;	
			outSize			=	16;		
			charactersDisplay.Invalidate();
			charactersDisplay.Refresh();			
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// init the settings for from a loaded image
		//
		//-------------------------------------------------------------------------------------------------------------------	
	
		private	void	setForm()
		{
			this.listBox1.Items.Clear();
			this.listBox1.Items.Add(" " + Model.Name);
			Model.Images.ForEach(filename => this.listBox1.Items.Add(" " + filename));

			textBox2.Text	=	Model.GridYSize.ToString();	
			textBox1.Text	=	Model.GridXSize.ToString();
			if(Model.OutputType == OutputType.Sprites)
			{
				spritesOut.Checked	=	true;
				blocksOut.Checked	=	false;	
				selectedRadio		=	spritesOut;
				outSize			=	16;
				//charsPanel.Width	=	blocksAcross/outSize;
			}
			else //outputData.Blocks;
			{ 
				blocksOut.Checked	=	true;
				spritesOut.Checked	=	false;	
				selectedRadio		=	blocksOut;
				outSize			=	8;
				//charsPanel.Width	=	blocksAcross/outSize;
			}
			this.Refresh();
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// check bouds of Y grid szie
		//
		//-------------------------------------------------------------------------------------------------------------------	
	
		private void checkBoundsOfYGridSize(object sender, EventArgs e)
		{
			int size;
			if(int.TryParse(textBox2.Text, out size))
			{
				Model.GridYSize = size;

				if (Model.OutputType == OutputType.Sprites)
				{
					if(Model.GridYSize<16)
					{
						Model.GridYSize	=	16;
					}
					else if (Model.GridYSize>320)
					{ 
						Model.GridYSize	=	320;
					}
					else
					{
						Model.GridYSize	=	(Model.GridYSize + 15) & ~0xF;						
					}
				}
				else
				{ 
					if(Model.GridYSize<8)
					{
						Model.GridYSize	=	8;
					}
					else if (Model.GridYSize>128)
					{ 
						Model.GridYSize	=	128;
					}
					else
					{
						Model.GridYSize	=	(Model.GridYSize + 7) & ~0x7;
					}
				}
				textBox2.Text	=	Model.GridYSize.ToString();
			}
			else
			{				
				Model.GridYSize	=	32;
				textBox2.Text	=	Model.GridYSize.ToString();
			}
			blocksDisplay.Invalidate();
			blocksDisplay.Refresh();			
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// check bouds of X grid szie
		//
		//-------------------------------------------------------------------------------------------------------------------	

		private void checkBoundsOfXGridSize(object sender, EventArgs e)
		{
			int size;
			if(int.TryParse(textBox1.Text, out size))
			{
				Model.GridXSize = size;

				if (Model.OutputType == OutputType.Sprites)
				{
					if(Model.GridXSize<16)
					{
						Model.GridXSize	=	16;
					}
					else if (Model.GridXSize>320)
					{ 
						Model.GridXSize	=	320;
					}
					else
					{
						Model.GridXSize	=	(Model.GridXSize + 15) & ~0xF;						
					}
				}
				else
				{ 
					if(Model.GridXSize<8)
					{
						Model.GridXSize	=	8;
					}
					else if (Model.GridXSize>128)
					{ 
						Model.GridXSize	=	128;
					}
					else
					{
						Model.GridXSize	=	(Model.GridYSize + 7) & ~0x7;
					}
				}							
				textBox1.Text	=	Model.GridXSize.ToString();
			}
			else
			{				
				Model.GridXSize	=	32;
				textBox1.Text	=	Model.GridXSize.ToString();
			}
			Model.BlocksAccross	=	(int)Math.Floor((float)blocksDisplay.Width/Model.GridXSize);
			blocksDisplay.Invalidate();
			blocksDisplay.Refresh();
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// open the ignore panel
		//
		//-------------------------------------------------------------------------------------------------------------------	
		
		private void openIgnorePanel(object sender, EventArgs e)
		{
			SettingsPanel.StartPosition	=	FormStartPosition.CenterParent;
			SettingsPanel.Model = Model;
			SettingsPanel.ShowDialog();
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// show about box
		//
		//-------------------------------------------------------------------------------------------------------------------	
	
		private void showAboutBox(object sender, EventArgs e)
		{
			About about	=	new	About();
			about.ShowDialog();
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// show help
		//
		//-------------------------------------------------------------------------------------------------------------------	
	
		private void openWebHelp(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://luckyredfish.com/next-graphics-version-2/");
			
		}

		private void rebuildFromTilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(blocksView.Visible==false)
			{
				blocksView	=	new	imageWindow();
			}
			blocksView.MdiParent		=	this;
			blocksView.inputImage		=	new	Bitmap(20*Model.GridXSize,12*Model.GridYSize);
			blocksView.srcPicture.Image	=	blocksView.inputImage;
			int	blocksX			=	0;
			int	blocksY			=	0;
			
			blocksView.Show();
			for(int s=0;s<outBlock;s++)		// do all the blocks
			{
				for(int y=0;y<blockInfo[s].Height;y++)
				{
					for(int x=0;x<blockInfo[s].Width;x++)
					{
						byte	tileId	=	(byte)sortIndexs[blockInfo[s].GetId(x,y)];
						for(int pixelY=0;pixelY<8;pixelY++)
						{
							for(int pixelX=0;pixelX<8;pixelX++)
							{ 
								int	readX		=	pixelX;
								int	readY		=	pixelY;
								if(blockInfo[s].GetFlippedX(x,y)==true)
								{
									readX		=	7-pixelX;
								}
								if(blockInfo[s].GetFlippedY(x,y)==true)
								{
									readY		=	7-pixelY;
								}
								if(blockInfo[s].GetRotated(x,y)==true)
								{
									int	temp	=	readX;
									readX		=	readY;
									readY		=	temp;									
									readX		=	7-readX;
								}
								Color	readColour	=	SetFromPalette(charData[tileId].GetPixel(readX,readY));								
								blocksView.inputImage.SetPixel((x*8)+pixelX+(blocksX*Model.GridXSize),(y*8)+pixelY+(blocksY*Model.GridYSize),readColour);									
							} 
						}
						
					}
				}
				blocksX++;
				if(blocksX>=20)
				{
					blocksX=0;
					blocksY++;					
				}
			}			
			blocksView.Invalidate(true);
			blocksView.Update();
		}

		private void reloadImages_Click(object sender, EventArgs e)
		{
			DisposeImageWindows();

			foreach (var image in Model.Images)
			{
				image.ReloadImage();
				imageWindows.Add(new imageWindow { MdiParent = this });
			}
		}

		// reduce the number of bytes in the map and remove unused blocks
		private void processMapToolStripMenuItem_Click(object sender, EventArgs e)
		{
#if PROPRIETARY
			parallaxWindow.processMap(sender,e);
#else
			MessageBox.Show("FUNCTIONALITY REMOVED", "Proprietary ", MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif
		}
	
		//-------------------------------------------------------------------------------------------------------------------
		//
		// Open the image and or rename the project
		//
		//-------------------------------------------------------------------------------------------------------------------

		private void listBox1MouseDoubleClick(object sender, MouseEventArgs e)
		{
			int index = this.listBox1.IndexFromPoint(e.Location);
			if (index != System.Windows.Forms.ListBox.NoMatches)
			{
				if(index==0)
				{
					newProject newProjectForm		=	new newProject();		
					newProjectForm.ShowDialog();
					if (newProjectForm.DialogResult == DialogResult.OK)
					{
						Model.Name	=	newProjectForm.textBox1.Text;				
						this.listBox1.Items[0] = " "+Model.Name;
					}
				}
				else
				{ 
					if(imageWindows[index-1]!=null)
					{ 
						if(imageWindows[index-1].Visible==false)
						{ 					
							imageWindows[index-1] = new imageWindow
							{
								MdiParent = this
							};
							imageWindows[index-1].loadImage(Model.Images[index-1].Filename,Model.GridXSize,Model.GridYSize);
							imageWindows[index-1].Show();
							imageWindows[index-1].Height	=	this.Height-(toolStrip.Height+100);
							imageWindows[index-1].Width	=	this.Width-(FilesView.Width+imageWindows[index-1].Left);
							imageWindows[index-1].Top	=	0;
							imageWindows[index-1].Refresh();
						}
					}
				}
			}
		}
		private void blocksDisplay_Click(object sender, EventArgs e)
		{
			if(blocksWindow!=null)
			{ 
				if(blocksWindow.Visible==false)
				{ 					
					blocksWindow = new imageWindow
					{
						MdiParent = this
					};
					blocksWindow.copyImage(blocksPanel,Model.GridXSize,Model.GridYSize,true);
				//	blocksWindow.loadImage(model.Filenames[index-1],model.GridXSize,model.GridYSize);
					blocksWindow.Show();
					blocksWindow.Height	=	this.Height-(toolStrip.Height+200);
					blocksWindow.Width	=	this.Width-(bottomPanel.Width+FilesView.Width+blocksWindow.Left);
					blocksWindow.Top	=	20;
					blocksWindow.Left	=	20;
					blocksWindow.Refresh();
				}
			}
		}
	
		private void charactersDisplay_Click(object sender, EventArgs e)
		{
			if(charsWindow!=null)
			{ 
				if(charsWindow.Visible==false)
				{ 					
					charsWindow = new imageWindow
					{
						MdiParent = this
					};
					if(Model.OutputType == OutputType.Sprites)
					{
						charsWindow.copyImage(charsPanel,16,16,false);
					}
					else
					{
						charsWindow.copyImage(charsPanel,8,8,false);
					}
				//	blocksWindow.loadImage(model.Filenames[index-1],model.GridXSize,model.GridYSize);
					charsWindow.Show();
					charsWindow.Height	=	this.Height-(toolStrip.Height+200);
					charsWindow.Width	=	this.Width-(bottomPanel.Width+FilesView.Width+charsWindow.Left);
					charsWindow.Top		=	20;
					charsWindow.Left	=	20;
					charsWindow.Refresh();
				}
			}
		}

		private void createParallax(object sender, EventArgs e)
		{
#if PROPRIETARY

			if (parallaxWindow!=null)
			{ 
				if(parallaxWindow.Visible==false)
				{
					MessageBox.Show("Chop your blocks first", "Dont forget", MessageBoxButtons.OK, MessageBoxIcon.Information);		
					parallaxWindow.StartPosition	=	FormStartPosition.CenterParent;
					parallaxWindow.ShowDialog();
					parallaxWindow.stop();
					parallaxWindow.Refresh();
				//	if (parallaxWindow.DialogResult == DialogResult.OK)
                //  {
						
				//	}
				}
			}
#else
			MessageBox.Show("FUNCTIONALITY REMOVED", "Proprietary ", MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif


		}

		private void batchProcessProjectsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			batchProjectDialog.Multiselect		=	true;
			batchProjectDialog.RestoreDirectory	=	false;
			batchProjectDialog.InitialDirectory 	=	ParentDirectory + "\\Projects\\";	
			batchProjectDialog.Filter		=	"Project Files (*.xml)|*.xml|All Files (*.*)|*.*";
			
			if (batchProjectDialog.ShowDialog(this) == DialogResult.OK)
			{
				foreach (String file in batchProjectDialog.FileNames) 
				{
					setParentFolder(Path.GetFullPath(file));
					newProject();
					loadProject(file);
					outXBlock	=	0;
					outYBlock	=	0;
					outBlock	=	0;			
					outXChar	=	0;
					outYChar	=	0;
					outChar		=	0;
					for(int b=0;b<MAX_BLOCKS;b++)
					{
						if(blockData[b]!=null)
						{ 
							blockData[b].Dispose();
							blockData[b]		=	null;
							blockData[b]		=	null;
						}
						if(blockInfo[b]!=null)
						{ 
							blockInfo[b].Dispose();
							blockInfo[b]		=	null;
							blockInfo[b]		=	null;
						}			
					}
					for(int c=0;c<MAX_CHAR_SIZE;c++)
					{
						if(tempData[c]!=null)
						{ 
							tempData[c].Dispose();
							tempData[c]=	null;
						}
					}
					SettingsPanel.comments.SelectedIndex	=	(int)comments.noComments;
					
					CopyBlocksImage();
					ouputFiles(ParentDirectory + "\\Output\\" + Model.Name.ToLower() + ".asm");

				}
			}
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// change the highlighted optoin in the list box, hook
		//
		//-------------------------------------------------------------------------------------------------------------------

		private void wAVToRAWToolStripMenuItem_Click(object sender, EventArgs e)
		{			
			OpenFileDialog			wavDialog		=	new OpenFileDialog();
			wavDialog.Multiselect		=	false;
			wavDialog.RestoreDirectory	=	false;
			wavDialog.InitialDirectory 	=	ParentDirectory + "\\Projects\\";	
			wavDialog.Filter		=	"Wav Files (*.wav)|*.wav|All Files (*.*)|*.*";

			
			if (wavDialog.ShowDialog(this) == DialogResult.OK)
			{
				string	wavFileName		=	wavDialog.FileName;				
				string	rawFileName		=	Path.ChangeExtension(wavDialog.FileName,"raw");
				FileInfo mapInfo = new FileInfo(wavFileName);
				byte[]	wavArray;
				using(BinaryReader wavFile	=	new BinaryReader(File.Open(wavFileName, FileMode.Open)))
				{ 
					wavArray	=	new	byte[mapInfo.Length];
					for(int r=0;r<mapInfo.Length;r++)
					{
						wavArray[r]	=	wavFile.ReadByte();
					}
				}	

				using(BinaryWriter rawFile	=	new BinaryWriter(File.Open(rawFileName, FileMode.OpenOrCreate)))
				{ 
					for(int r=64;r<mapInfo.Length;r++)
					{
						byte	snd	=	 (byte)(wavArray[r]);

						rawFile.Write((byte) (snd));
					}
				}
			}
		}

		private void toolStripButton7_Click(object sender, EventArgs e)
		{
			if(informationWindow!=null)
			{ 
				informationWindow.infoTextBox.Clear();
				if(informationWindow.Visible==false)
				{ 					
					informationWindow = new infoWindow
					{
						MdiParent = this
					};				
					informationWindow.Show();
					informationWindow.Top	=	20;
					informationWindow.Left	=	20;
					informationWindow.Refresh();
				}
				for(int b=0;b<outBlock;b++)
				{					
					informationWindow.infoTextBox.SelectionColor = Color.Black; 	
					informationWindow.infoTextBox.AppendText(b.ToString()+"\t");
					for(int chr=0;chr< (Model.GridXSize/ outSize) * (Model.GridYSize/ outSize); chr++)
					{ 
						informationWindow.infoTextBox.AppendText("\t");
						if(blockInfo[b].infos[chr].hasTranspearent==true)
						{ 
							informationWindow.infoTextBox.SelectionColor = Color.Red; 							
							informationWindow.infoTextBox.AppendText(sortIndexs[blockInfo[b].infos[chr].originalId].ToString() + ",");
						}
						else
						{
							informationWindow.infoTextBox.SelectionColor = Color.Black; 		
							informationWindow.infoTextBox.AppendText(sortIndexs[blockInfo[b].infos[chr].originalId].ToString() + ",");

						}
					}
					informationWindow.infoTextBox.AppendText("\r\n");
				}
				informationWindow.infoTextBox.AppendText("\r\nCOUNTS\r\n");
				int[]	counts	=	new	int[256];
				for(int c=0;c<256;c++)
				{
					counts[c] = 0;
				}
				for(int b=0;b<outBlock;b++)
				{					
					informationWindow.infoTextBox.SelectionColor = Color.Black; 	
					informationWindow.infoTextBox.AppendText(b.ToString()+"\t");
					for(int chr=0;chr< (Model.GridXSize/ outSize) * (Model.GridYSize/ outSize); chr++)
					{
						counts[sortIndexs[blockInfo[b].infos[chr].originalId]]++;
					}
				}
				for(int c=0;c<256;c++)
				{
					informationWindow.infoTextBox.AppendText(c.ToString());
					informationWindow.infoTextBox.AppendText("\t");
					informationWindow.infoTextBox.AppendText(counts[c].ToString()+"\r\n");
				}
				
			}
		}
	}
}