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
using System.Globalization;
using System.Text.RegularExpressions;

namespace NextGraphics
{
	public partial class Main : Form
	{
		//-------------------------------------------------------------------------------------------------------------------
		//
		// enumerations
		//
		//-------------------------------------------------------------------------------------------------------------------
			
		public	enum	outputData
		{ 
			Sprites,
			Blocks,
		}
		public	enum	writeType
		{ 
			Assember,
			Basic
		}
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
	
		public	RadioButton		selectedRadio;
		public	outputData		outType			=	outputData.Sprites;
		public	int			outSize			=	16;		
		private	short			thisIndex		=	0;
		private	Colour			thisColour		=	new Colour();
		private	Colour			thatColour		=	new Colour();
		private	const int		MAX_BLOCKS		=	256;
		private	const int		MAX_CHAR_SIZE		=	512;
		private	const int		MAX_IMAGES		=	64;
		public	List<string> 		fullNames		=	new	List<string>();
		private	List<imageWindow>	imageWindows		=	new	List<imageWindow>();
		private	List<Bitmap>		sourceImages		=	new	List<Bitmap>();
		private	Bitmap	 		blocksPanel;
		private	Bitmap	 		charsPanel;
		private	BitmapData[]		blockData		=	new	BitmapData[MAX_BLOCKS];	
		private	spriteInfo[]		blockInfo		=	new	spriteInfo[MAX_BLOCKS];
		private	BitmapData[]		charData		=	new	BitmapData[MAX_CHAR_SIZE];	
		private int			gridXSize		=	32;
		private int			gridYSize		=	32;
		private	int			outXBlock		=	0;
		private	int			outYBlock		=	0;
		private	int			outBlock		=	0;
		private	int			outXChar		=	0;
		private	int			outYChar		=	0;
		private	int			outChar			=	0;
		private	string			projectName		=	"Untitled project";
		private	Rectangle		src			=	new	Rectangle();
		private	Rectangle		dest			=	new	Rectangle();
		private	Rectangle		charDest		=	new	Rectangle();
		private	string			outPath			=	"";
		private	string			binPath			=	"";
		private	int			blocksAcross		=	44;
		private	bool			reverseByes		=	false;
		private	bool			PaletteSet		=	false;	
		private	Palette			thePalette		=	new Palette();
		private	IgnorePanel		ignorePanel		=	new IgnorePanel();
		private	centerPanel		centerPanel		=	new centerPanel();
		private	long			AveragingIndex		=	0;

		//-------------------------------------------------------------------------------------------------------------------
		//
		// Init
		//
		//-------------------------------------------------------------------------------------------------------------------
			
		public Main()
		{
			InitializeComponent();
			blocksPanel				=	new	Bitmap(16*blocksAcross,32*32,PixelFormat.Format24bppRgb);
			clearPanels(blocksPanel);
			blocksDisplay.Image			=	blocksPanel;
			blocksDisplay.Height			=	blocksPanel.Height;
			blocksDisplay.Width			=	blocksPanel.Width;				
			charsPanel				=	new	Bitmap(256,128,PixelFormat.Format24bppRgb);
			clearPanels(charsPanel);
			charactersDisplay.Image			=	charsPanel;			
			this.toolStripProgressBar1.Minimum	=	0;
			this.toolStripProgressBar1.Maximum	=	0;
			this.listBox1.Items.Add(" "+projectName);
			reverseByes				=	BitConverter.IsLittleEndian;
			thePalette.parentForm			=	this;
			blocksAcross				=	(int)Math.Floor((float)blocksDisplay.Width/gridXSize);
			blocksDisplay.Invalidate();
			blocksDisplay.Refresh();
			ignorePanel.comments.SelectedIndex	=	1;
			setForm();
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
				projectName	=	newProjectForm.textBox1.Text;				
				this.listBox1.Items[0] = " "+projectName;
			}
		}

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

		//-------------------------------------------------------------------------------------------------------------------
		//
		// Save Project
		//
		//-------------------------------------------------------------------------------------------------------------------

		private void saveProject(object sender, EventArgs e)
		{
			if(outPath.Length==0)
			{ 
				SaveFileDialog saveFileDialog1		=	new SaveFileDialog();
				saveFileDialog1.FileName		=	projectName + ".xml";
				saveFileDialog1.Filter			=	"Project Files (*.xml)|*.xml|All Files (*.*)|*.*";
				saveFileDialog1.FilterIndex		=	1 ;
				saveFileDialog1.RestoreDirectory	=	true ;			
				if(saveFileDialog1.ShowDialog() == DialogResult.OK)
				{
					outPath				=	Path.ChangeExtension(saveFileDialog1.FileName,"xml");
					saveXMLFile(outPath);	
				}

			}
			else
			{
				saveXMLFile(outPath);
			}
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// Save Xml File 
		//
		//-------------------------------------------------------------------------------------------------------------------

		private	void	saveXMLFile(string outPath)
		{			
			int	transIndex		=	thePalette.transIndex;
			int	loadedColourCount	=	thePalette.loadedColourCount;
			int	centerPos		=	centerPanel.centerPosition;
			using 	(XmlTextWriter writer = new XmlTextWriter(outPath, Encoding.UTF8))
			{
				writer.Formatting				=	Formatting.Indented;
				XmlDocument	doc				=	new XmlDocument();

				XmlNode		rootNode			=	doc.CreateElement("", "XML", "");
						doc.AppendChild(rootNode);
				XmlNode 	projectNode			=	doc.CreateElement("Project");	
						rootNode.AppendChild(projectNode);

				XmlNode		nameNode	=			doc.CreateElement("Name");
				XmlAttribute	attribute			=	doc.CreateAttribute("Projectname");	
						attribute.Value			= 	projectName;
						nameNode.Attributes.Append(attribute);	
						projectNode.AppendChild(nameNode);
				foreach (string name in fullNames)
				{
					XmlNode		fileNode	=	doc.CreateElement("File");
					attribute	=	doc.CreateAttribute("Path");
					attribute.Value =	name;
					fileNode.Attributes.Append(attribute);
					projectNode.AppendChild(fileNode);				
				}	
				XmlNode		typeNode	=			doc.CreateElement("Type");
				XmlAttribute	typeAttribute	=			null;	
				if(outType==outputData.Blocks)
				{ 
					typeAttribute	=	doc.CreateAttribute("blocks");	
				}
				else if(outType==outputData.Sprites)
				{
					typeAttribute	=	doc.CreateAttribute("sprites");	
				}
				else
				{					
					//MessageBox.Show("Should not get this as its not implemented", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);					
				}
				typeAttribute.Value		= 	"true";
				typeNode.Attributes.Append(typeAttribute);
				typeAttribute			=	doc.CreateAttribute("center");
				typeAttribute.Value		= 	centerPos.ToString();
				typeNode.Attributes.Append(typeAttribute);
				typeAttribute			=	doc.CreateAttribute("xSize");
				typeAttribute.Value		= 	gridXSize.ToString();
				typeNode.Attributes.Append(typeAttribute);
				typeAttribute			=	doc.CreateAttribute("ySize");
				typeAttribute.Value		= 	gridYSize.ToString();	
				typeNode.Attributes.Append(typeAttribute);	
				typeAttribute			=	doc.CreateAttribute("fourBit");
				if(FourBit.Checked==true)
				{					
					typeAttribute.Value		= 	"true";
				}	
				else
				{
					typeAttribute.Value		= 	"false";
				}
				typeNode.Attributes.Append(typeAttribute);	
				projectNode.AppendChild(typeNode);
				XmlNode		IgnoreNode	=	doc.CreateElement("Ignore");
				XmlAttribute	IgnoreAttribute	=	doc.CreateAttribute("Repeats");		
				if(ignorePanel.Repeats.Checked==true)
				{					
					IgnoreAttribute.Value		= 	"true";
				}	
				else
				{
					IgnoreAttribute.Value		= 	"false";
				}
				IgnoreNode.Attributes.Append(IgnoreAttribute);	
				IgnoreAttribute			=	doc.CreateAttribute("MirrorX");		
				if(ignorePanel.mirrorX.Checked==true)
				{					
					IgnoreAttribute.Value		= 	"true";
				}	
				else
				{
					IgnoreAttribute.Value		= 	"false";
				}
				IgnoreNode.Attributes.Append(IgnoreAttribute);	
				IgnoreAttribute			=	doc.CreateAttribute("MirrorY");		
				if(ignorePanel.mirrorY.Checked==true)
				{					
					IgnoreAttribute.Value		= 	"true";
				}	
				else
				{
					IgnoreAttribute.Value		= 	"false";
				}
				IgnoreNode.Attributes.Append(IgnoreAttribute);	
				IgnoreAttribute			=	doc.CreateAttribute("Rotations");		
				if(ignorePanel.rotations.Checked==true)
				{					
					IgnoreAttribute.Value		= 	"true";
				}	
				else
				{
					IgnoreAttribute.Value		= 	"false";
				}
				IgnoreNode.Attributes.Append(IgnoreAttribute);	
				IgnoreAttribute			=	doc.CreateAttribute("Transparent");		
				if(ignorePanel.Transparent.Checked==true)
				{					
					IgnoreAttribute.Value		= 	"true";
				}	
				else
				{
					IgnoreAttribute.Value		= 	"false";
				}
				IgnoreNode.Attributes.Append(IgnoreAttribute);					
				projectNode.AppendChild(IgnoreNode);
				XmlNode		paletteNode		=	doc.CreateElement("Palette");
				XmlAttribute	paletteAttribute	=	doc.CreateAttribute("Mapping");	
				switch((int)thePalette.paletteSetting)
				{ 
					case	0: //mapped256
						paletteAttribute.Value		=	"Next256";
					break;
					case	1: //mapped512
						paletteAttribute.Value		=	"Next512";
					break;
					case	2: //mappedCustom
						paletteAttribute.Value		=	"Custom";
					break;
				}					
				paletteNode.Attributes.Append(paletteAttribute);	
				paletteAttribute		=	doc.CreateAttribute("Transparent");
				paletteAttribute.Value		=	transIndex.ToString();				
				paletteNode.Attributes.Append(paletteAttribute);	
				paletteAttribute		=	doc.CreateAttribute("Used");
				paletteAttribute.Value		=	loadedColourCount.ToString();				
				paletteNode.Attributes.Append(paletteAttribute);	
				for(int c=0;c<256;c++)
				{ 
					XmlNode		colourNode		=	doc.CreateElement("Colour" + c.ToString());
					XmlAttribute	colourAttribute		=	doc.CreateAttribute("Red");							
							colourAttribute.Value	=	thePalette.loadedPalette[c,0].ToString();									
							colourNode.Attributes.Append(colourAttribute);
							colourAttribute		=	doc.CreateAttribute("Green");							
							colourAttribute.Value	=	thePalette.loadedPalette[c,1].ToString();									
							colourNode.Attributes.Append(colourAttribute);
							colourAttribute		=	doc.CreateAttribute("Blue");							
							colourAttribute.Value	=	thePalette.loadedPalette[c,1].ToString();									
							colourNode.Attributes.Append(colourAttribute);	
							paletteNode.AppendChild(colourNode);
				}							
				paletteNode.Attributes.Append(paletteAttribute);	
				projectNode.AppendChild(paletteNode);
				doc.WriteContentTo(writer);
				writer.Flush();
				writer.Close();
				// mStream.Flush();
				//myStream.Write(doc.InnerXml);
			}
		}
		
		//-------------------------------------------------------------------------------------------------------------------
		//
		// Open Xml project File 
		//
		//-------------------------------------------------------------------------------------------------------------------

		private void openProject(object sender, EventArgs e)
		{		
			OpenFileDialog openProjectDialog		=	new OpenFileDialog();
			openProjectDialog.InitialDirectory		=	Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			openProjectDialog.Multiselect			=	false;
			openProjectDialog.RestoreDirectory		=	true ;
			openProjectDialog.Filter			=	"Project Files (*.xml)|*.xml|All Files (*.*)|*.*";
			if (openProjectDialog.ShowDialog(this) == DialogResult.OK)
			{
				XmlDocument xmlDoc			= new XmlDocument();

				xmlDoc.Load(openProjectDialog.FileName);

				this.listBox1.Items.Clear();	
				
				XmlNode projectNameNode			=	xmlDoc.SelectSingleNode("//Project/Name");
				if(projectNameNode.Attributes["Projectname"]!=null)
				{ 
					projectName				=	projectNameNode.Attributes["Projectname"].Value;

					XmlNode projectTypeNode			=	xmlDoc.SelectSingleNode("//Project/Type");					
					centerPanel.setCenter(int.Parse(projectTypeNode.Attributes["center"].Value));
					gridYSize				=	int.Parse(projectTypeNode.Attributes["ySize"].Value);
					gridXSize				=	int.Parse(projectTypeNode.Attributes["xSize"].Value);
					FourBit.Checked				=	bool.Parse(projectTypeNode.Attributes["fourBit"].Value);
					thePalette.fourBitOutput		=	FourBit.Checked;
					if(projectTypeNode.Attributes["sprites"]!=null)
					{
						outType	=	outputData.Sprites;
					}
					else
					{					
						outType	=	outputData.Blocks;
					}

					this.listBox1.Items.Add(" "+projectName);
				
					XmlNodeList fileNodes			=	xmlDoc.SelectNodes("//Project/File");
					fullNames.Clear();
					foreach(XmlNode fileNode in fileNodes)
					{				
						fullNames.Add(fileNode.Attributes["Path"].Value);
					}
					restoreFromList();
					XmlNode	ignore				=	xmlDoc.SelectSingleNode("//Project/Ignore");
					if(ignore.Attributes["Repeats"]!=null)
					{
						if(ignore.Attributes["Repeats"].Value=="true")
						{
							ignorePanel.Repeats.Checked	=	true;
						}
						else
						{							
							ignorePanel.Repeats.Checked	=	false;
						}
					}
					if(ignore.Attributes["MirrorX"]!=null)
					{
						if(ignore.Attributes["MirrorX"].Value=="true")
						{
							ignorePanel.mirrorX.Checked	=	true;
						}
						else
						{							
							ignorePanel.mirrorX.Checked	=	false;
						}
					}
					if(ignore.Attributes["MirrorY"]!=null)
					{
						if(ignore.Attributes["MirrorY"].Value=="true")
						{
							ignorePanel.mirrorY.Checked	=	true;
						}
						else
						{							
							ignorePanel.mirrorY.Checked	=	false;
						}
					}
					if(ignore.Attributes["Rotations"]!=null)
					{
						if(ignore.Attributes["Rotations"].Value=="true")
						{
							ignorePanel.rotations.Checked	=	true;
						}
						else
						{							
							ignorePanel.rotations.Checked	=	false;
						}
					}
					if(ignore.Attributes["Transparent"]!=null)
					{
						if(ignore.Attributes["Transparent"].Value=="true")
						{
							ignorePanel.Transparent.Checked	=	true;
						}
						else
						{							
							ignorePanel.Transparent.Checked	=	false;
						}
					}					
					XmlNode	palette 			=	xmlDoc.SelectSingleNode("//Project/Palette");
					if(palette.Attributes["Mapping"]!=null)
					{
						thePalette.SetPaletteMapping(palette.Attributes["Mapping"].Value);
					}
					if(palette.Attributes["Transparent"]!=null)
					{
						thePalette.transIndex				=	int.Parse(palette.Attributes["Transparent"].Value); 
					}
					if(palette.Attributes["Used"]!=null)
					{
						thePalette.loadedColourCount	=	int.Parse(palette.Attributes["Used"].Value); 
					}
					for(int c=0;c<256;c++)
					{
						XmlNode	colourNode	=	xmlDoc.SelectSingleNode("//Project/Palette/Colour"+c.ToString());
						if(palette.Attributes["Red"]!=null)
						{
							thePalette.loadedPalette[c,0]	=	byte.Parse(palette.Attributes["Red"].Value);
						}
						if(palette.Attributes["Green"]!=null)
						{
							thePalette.loadedPalette[c,1]	=	byte.Parse(palette.Attributes["Green"].Value);
						}
						if(palette.Attributes["Blue"]!=null)
						{
							thePalette.loadedPalette[c,2]	=	byte.Parse(palette.Attributes["Blue"].Value);
						}
					}
					thePalette.setForm();
				}
			}
		}
		
		//-------------------------------------------------------------------------------------------------------------------
		//
		// Restore the bitmap data from the file list
		//
		//-------------------------------------------------------------------------------------------------------------------
		private	 void	restoreFromList()
		{			
			this.listBox1.Items.Clear();
			this.listBox1.Items.Add(" "+ProductName);
			sourceImages.Clear();
			imageWindows.Clear();
			bool			removed			=	false;
			List<string> 		removeNames		=	new	List<string>();
			removeNames.Clear();
			foreach (string name in fullNames)
			{
				try 
				{ 
					if(IsSupported(new Bitmap(name))==true)
					{
						sourceImages.Add(new Bitmap(name));
						imageWindows.Add(new imageWindow { MdiParent = this});
						this.listBox1.Items.Add("  " + Path.GetFileName(name));	
					}
					else
					{
						removeNames.Add(name);
					}
				}
				catch //(System.ArgumentException ex)
				{
					removeNames.Add(name);
				}
			}
			foreach (string name in removeNames)
			{
				removed		= true;
				fullNames.Remove(name);
			}
			if(removed==true)
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
			OpenFileDialog openFileDialog		=	new OpenFileDialog();
			openFileDialog.InitialDirectory		=	Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			openFileDialog.Multiselect		=	true;
			openFileDialog.RestoreDirectory		=	true ;
			
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
			string sep = string.Empty;
			foreach (var c in codecs)
			{
				string codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
				openFileDialog.Filter = String.Format("{0}{1}{2} ({3})|{3}", openFileDialog.Filter, sep, codecName, c.FilenameExtension);
				sep = "|";
			}
			openFileDialog.Filter		= String.Format("{0}{1}{2} ({3})|{3}", openFileDialog.Filter, sep, "All Files", "*.*"); 
			openFileDialog.DefaultExt	= ".bmp"; // Default file extension 
	

			if (openFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				bool	rejected=false;
				foreach (String file in openFileDialog.FileNames) 
				{

					for(int i=0;i<fullNames.Count;i++)
					{
						if(file == fullNames[i])
						{
							rejected	=	true;
							goto rejectName;
						}
					}
					fullNames.Add(file);
rejectName:				;
				}
				restoreFromList();
				if(rejected==true)
				{
					MessageBox.Show("Duplicate files are not allowed", "Duplicates Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);				
				}
			}
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
						projectName	=	newProjectForm.textBox1.Text;				
						this.listBox1.Items[0] = " "+projectName;
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
							imageWindows[index-1].loadImage(fullNames[index-1],gridXSize,gridYSize);
							imageWindows[index-1].Show();
							imageWindows[index-1].Height	=	this.Height-(bottomPanel.Height+toolStrip.Height+100);
							imageWindows[index-1].Width	=	this.Width-(FilesView.Width+imageWindows[index-1].Left);
							imageWindows[index-1].Top	=	0;
							imageWindows[index-1].Refresh();
						}
					}
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
			outPath		=	"";
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
		// change the highlighted optoin in the list box, hook
		//
		//-------------------------------------------------------------------------------------------------------------------
		
		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// make the blocks, in other words do the cut and remap work init
		//
		//-------------------------------------------------------------------------------------------------------------------
	
		private void makeBlocksClick(object sender, EventArgs e)
		{
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
				if(charData[c]!=null)
				{ 
					charData[c].Dispose();
					charData[c]	=	null;
				}
			}
			CopyBlocksImage();
		}
		
		//-------------------------------------------------------------------------------------------------------------------
		//
		// do the actual cut and remap work
		//
		//-------------------------------------------------------------------------------------------------------------------
	
		private void CopyBlocksImage()
		{		
			clearPanels(blocksPanel);
			clearPanels(charsPanel);
			blocksDisplay.Invalidate(true);
			blocksDisplay.Update();	
			charactersDisplay.Invalidate(true);
			charactersDisplay.Update();	
			this.toolStripProgressBar1.Minimum	=	0;
			this.toolStripProgressBar1.Maximum	=	10000;
			blockType thisCharType			=	new	blockType();	
			int	xChars				=	(gridXSize/outSize);
			int	yChars				=	(gridYSize/outSize);
			int	MaxLimit			=	MAX_CHAR_SIZE;
			int	PaletteOffset			=	0;
			if(outType == outputData.Sprites)
			{
				MaxLimit	=	128;
			}
			else
			{
				MaxLimit	=	MAX_CHAR_SIZE;
			}

			if(outType == outputData.Blocks)
			{ 
				// make the first block transparent
				if(blockData[0]==null)
				{ 
					blockData[0]	=	new	BitmapData(gridXSize,gridYSize);
					for(int y=0;y<gridYSize;y++)
					{ 
						for(int x=0;x<gridXSize;x++)
						{
							blockData[0].SetPixel(x,y, (short)thePalette.transIndex);
						}
					}
					blockToDisplay(ref blocksPanel,new Rectangle(0,0,gridXSize,gridYSize),ref blockData[0]);
				}
				// and a blank character
				if(charData[0]==null)
				{
					charData[0]	=	new	BitmapData(outSize,outSize);
					for(int y=0;y<8;y++)
					{ 
						for(int x=0;x<8;x++)
						{
							charData[0].SetPixel(x,y, (short)thePalette.transIndex);
						}
					}
					blockToDisplay(ref charsPanel,new Rectangle(0,0,outSize,outSize),ref charData[0]);
				}

				if(blockInfo[0]==null)
				{ 
					blockInfo[0]		=	new	spriteInfo(gridXSize/outSize,gridYSize/outSize);
				}
				outXChar	=	1;
				outXBlock	=	1;
				outBlock	=	1;
				outChar		=	1;
			
			}
			for(int s=0;s<sourceImages.Count;s++)
			{ 						
				if(sourceImages[s]!=null)
				{					
					for(int yBlocks=0;yBlocks<((sourceImages[s].Height+(gridYSize-1))/gridYSize);yBlocks++)
					{	
						for(int xBlocks=0;xBlocks<(sourceImages[s].Width/gridXSize);xBlocks++)
						{							
							src.X			=	xBlocks*gridXSize;
							src.Y			=	yBlocks*gridYSize;
							src.Width		=	gridXSize;
							src.Height		=	gridYSize;

							dest.X			=	outXBlock*gridXSize;
							dest.Y			=	outYBlock*gridYSize;
							dest.Width		=	gridXSize;
							dest.Height		=	gridYSize;
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
								blockData[outBlock]	=	new	BitmapData(gridXSize,gridYSize);
							}
							CopyRegionIntoBlock(sourceImages[s],src,ref blockData[outBlock]);
			
							if(FourBit.Checked==true || outType==outputData.Blocks)
							{ 
								remap4Bit(gridXSize, gridYSize);
							}
							for(int y=0;y<gridYSize;y++)
							{ 
								for(int x=0;x<gridXSize;x++)
								{
									if(blockData[outBlock].GetPixel(x,y)  != (short)thePalette.transIndex)
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
							if(blockInfo[outBlock]==null)
							{ 
								blockInfo[outBlock]	=	new	spriteInfo(gridXSize/outSize,gridYSize/outSize);
							}
							
							for(int yChar=0;yChar<yChars;yChar++)
							{ 
								for(int xChar=0;xChar<xChars;xChar++)
								{	
									if(FourBit.Checked==true || outType==outputData.Blocks)
									{
										PaletteOffset	=		blockData[outBlock].GetPixel(xChar*outSize,yChar*outSize)&0x0f0;								
									}					
									for(int c=0;c<outChar;c++)
									{		
										PaletteOffset =	0;										
										thisCharType	=	checkRepeatedChar(outBlock,c,xChar*outSize,yChar*outSize);
										
										if(thisCharType	!=	blockType.Original)
										{ 
											switch(thisCharType)
											{
												case	blockType.Repeated:
													blockInfo[outBlock].SetData(xChar,yChar,true,false,false,false,false,(short)c,(short)PaletteOffset);
													//charInfo[outChar].SetData(xChar,yChar,true,false,false,false,false,(short)c);
													goto	dontDrawCharacter;
												case	blockType.FlippedX:
													blockInfo[outBlock].SetData(xChar,yChar,true,true,false,false,false,(short)c,(short)PaletteOffset);
													goto	dontDrawCharacter;
												case	blockType.FlippedY:
													blockInfo[outBlock].SetData(xChar,yChar,true,false,true,false,false,(short)c,(short)PaletteOffset);
													goto	dontDrawCharacter;
												case	blockType.FlippedXY:
													blockInfo[outBlock].SetData(xChar,yChar,true,true,true,false,false,(short)c,(short)PaletteOffset);
													goto	dontDrawCharacter;
												case	blockType.Rotated:			
													blockInfo[outBlock].SetData(xChar,yChar,true,false,false,true,false,(short)c,(short)PaletteOffset);
													goto	dontDrawCharacter;
												case	blockType.FlippedXRotated:		
													blockInfo[outBlock].SetData(xChar,yChar,true,true,false,true,false,(short)c,(short)PaletteOffset);
													goto	dontDrawCharacter;
												case	blockType.FlippedYRotated:		
													blockInfo[outBlock].SetData(xChar,yChar,true,false,true,true,false,(short)c,(short)PaletteOffset);
													goto	dontDrawCharacter;
												case	blockType.FlippedXYRotated:
													blockInfo[outBlock].SetData(xChar,yChar,true,true,true,true,false,(short)c,(short)PaletteOffset);
													goto	dontDrawCharacter;
												case	blockType.Transparent:
													blockInfo[outBlock].SetData(xChar,yChar,false,false,false,false,true,(short)c,(short)PaletteOffset);
													goto	dontDrawCharacter;
											}
										}
									}	
									blockInfo[outBlock].SetData(xChar,yChar,false,false,false,false,false,(short)outChar,(short)PaletteOffset);
									// copy character over as its not in any of the characters									
									if(charData[outChar]==null)
									{ 
										charData[outChar]	=	new	BitmapData(outSize,outSize);
									}
									for(int y=0;y<outSize;y++)
									{ 
										for(int x=0;x<outSize;x++)
										{
											charData[outChar].SetPixel(x,y,blockData[outBlock].GetPixel(x+(xChar*outSize),y+(yChar*outSize)));												
										}
									}
									charDest.X	=	outXChar*outSize;
									charDest.Y	=	outYChar*outSize;
									charDest.Width	=	outSize;
									charDest.Height	=	outSize;
									blockToDisplay(ref charsPanel,charDest,ref charData[outChar]);	
									outXChar++;
									outChar++;
									if(outChar>=MaxLimit)
									{
										MessageBox.Show("Too many characters in your tiles", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);										
										blocksDisplay.Invalidate(true);
										blocksDisplay.Update();
										this.toolStripProgressBar1.Minimum	=	0;
										this.toolStripProgressBar1.Maximum	=	0;
										return;
									}
									if(outType == outputData.Blocks)
									{ 
										label2.Text	=	"Characters (" + outChar.ToString() +")";
									}
									else
									{
										label2.Text	=	"Sprites (" + outChar.ToString() +")";
									}
									if(outXChar>=charsPanel.Width/outSize)
									{
										outXChar	=	0;
										outYChar++;
									}
dontDrawCharacter:							;
								}
							}
							blockToDisplay(ref blocksPanel,dest,ref blockData[outBlock]);	
							if(outType == outputData.Blocks)
							{ 
								label1.Text	=	"Blocks (" + outBlock.ToString() +")";
							}
							else
							{
								label1.Text	=	"Objects (" + outBlock.ToString() +")";
							}
							outXBlock++;
							outBlock++;
							if(outXBlock>=blocksAcross)
							{
								outXBlock	=	0;
								outYBlock++;
							}
dontDraw:					;
						}					
					this.Invalidate(true);
					this.Update();		
					}
				}
			}		
			blocksDisplay.Invalidate(true);
			blocksDisplay.Update();			
			this.toolStripProgressBar1.Minimum	=	0;
			this.toolStripProgressBar1.Maximum	=	0;
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
							SetFromPalette(blockData[outBlock].GetPixel(x+(xCuts*outSize),y+(yCuts*outSize)));
							thisIndex	=	thePalette.closestColor(thisColour.R,thisColour.G, thisColour.B,(short) AveragingIndex);
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
	
		private	blockType	checkRepeatedChar(int block,int character, int xOffset,int yOffset)
		{
			if(ignorePanel.Repeats.Checked==true)
			{ 
				return	blockType.Original;
			}
			if(ignorePanel.Transparent.Checked==false)
			{
				for(int y=0;y<outSize;y++)
				{ 
					for(int x=0;x<outSize;x++)
					{
						if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) != (short)thePalette.transIndex)
						{
							goto	RepeatedCheck;
						}
					}
				}
			}			
			return	blockType.Transparent;
RepeatedCheck:		for(int y=0;y<outSize;y++)
			{ 
				for(int x=0;x<outSize;x++)
				{
					if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) != charData[character].GetPixel(x,y))
					{
						goto	flippedXCheck;
					}
				}
			}
			return	blockType.Repeated;
flippedXCheck:		if(ignorePanel.mirrorX.Checked==false)
			{ 
				for(int y=0;y<outSize;y++)
				{ 
					for(int x=0;x<outSize;x++)
					{
						if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) != charData[character].GetPixel((outSize-1)-x,y))	//x flip
						{
							goto	flippedYCheck;
						}
					}
				}
				return	blockType.FlippedX;
			}
flippedYCheck:		if(ignorePanel.mirrorY.Checked==false)
			{ 
				for(int y=0;y<outSize;y++)
				{ 
					for(int x=0;x<outSize;x++)
					{
						if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) != charData[character].GetPixel(x,(outSize-1)-y))		// y flip
						{
							goto	flippedXYCheck;
						}
					}
				}
				return	blockType.FlippedY;
			}
flippedXYCheck:		if(ignorePanel.mirrorY.Checked==false && ignorePanel.mirrorX.Checked==false)
			{ 
				for(int y=0;y<outSize;y++)
				{ 
					for(int x=0;x<outSize;x++)
					{
						if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) != charData[character].GetPixel((outSize-1)-x,(outSize-1)-y))	// xy flip
						{
							goto	rotatedCheck;
						}
					}
				}
				return	blockType.FlippedXY;
			}
rotatedCheck:		if(ignorePanel.rotations.Checked==false)
			{ 
				for(int y=0;y<outSize;y++)
				{ 
					for(int x=0;x<outSize;x++)
					{
						if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) != charData[character].GetPixel(y,x))
						{
							goto	flippedRotatedXCheck;
						}
					}
				}
				return	blockType.Rotated;
flippedRotatedXCheck:		if(ignorePanel.mirrorX.Checked==false)
				{
					for(int y=0;y<outSize;y++)
					{ 
						for(int x=0;x<outSize;x++)
						{
							if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) != charData[character].GetPixel(y,(outSize-1)-x))	//x flip
							{
								goto	flippedRotatedYCheck;
							}
						}
					}
					return	blockType.FlippedXRotated;
				}
flippedRotatedYCheck:		if(ignorePanel.mirrorY.Checked==false)
				{
					for(int y=0;y<outSize;y++)
					{ 
						for(int x=0;x<outSize;x++)
						{
							if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) != charData[character].GetPixel((outSize-1)-y,x))		// y flip
							{
								goto	flippedRotatedXYCheck;
							}
						}
					}
					return	blockType.FlippedYRotated;
				}
flippedRotatedXYCheck:		if(ignorePanel.mirrorY.Checked==false && ignorePanel.mirrorX.Checked==false)
				{
					for(int y=0;y<outSize;y++)
					{ 
						for(int x=0;x<outSize;x++)
						{
							if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) != charData[character].GetPixel((outSize-1)-y,(outSize-1)-x))	// xy flip
							{
								goto	blockIsOriginal;
							}
						}
					}
					return	blockType.FlippedXYRotated;
				}
			}
blockIsOriginal:	return	blockType.Original;
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// Copy an area from one bitmap to a bits bitmap (BitmapData)
		//
		//-------------------------------------------------------------------------------------------------------------------

		private void CopyRegionIntoBlock(Bitmap srcBitmap, Rectangle srcRegion,ref BitmapData outBlock)
		{
			for(int	y=0;y<srcRegion.Height;y++)
			{		
				for(int	x=0;x<srcRegion.Width;x++)
				{
					Color	pixelColour	=	srcBitmap.GetPixel(srcRegion.X+x,srcRegion.Y+y);							
					thatColour.R		=	pixelColour.R;
					thatColour.G		=	pixelColour.G;
					thatColour.B		=	pixelColour.B;
					thisIndex		=	thePalette.closestColor(thatColour.R,thatColour.G, thatColour.B,-1);					
					outBlock.SetPixel(x,y,thisIndex);

				}
			}								
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// Copy the bits bitmap (BitmapData) to a viewable bitmap
		//
		//-------------------------------------------------------------------------------------------------------------------

		private		void	blockToDisplay(ref Bitmap destBitmap, Rectangle destRegion,ref BitmapData inBlock)
		{
			for(int	y=0;y<destRegion.Height;y++)
			{		
				for(int	x=0;x<destRegion.Width;x++)
				{
					SetFromPalette(inBlock.GetPixel(x,y));
					destBitmap.SetPixel(destRegion.X+x,destRegion.Y+y,Color.FromArgb(thisColour.getARGB()));				
				}
			}
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// Set the colour from a palette in memory
		//
		//-------------------------------------------------------------------------------------------------------------------
		
		private	void	SetFromPalette(int theIndex)
		{
			switch(thePalette.paletteSetting)
			{ 
				case	Palette.PaletteMapping.mapped256:
					thisColour.R		=	thePalette.SpecNext256[theIndex,0];
					thisColour.G		=	thePalette.SpecNext256[theIndex,1];							
					thisColour.B		=	thePalette.SpecNext256[theIndex,2];
				return;
				case	Palette.PaletteMapping.mapped512:
					thisColour.R		=	thePalette.SpecNext512[theIndex,0];
					thisColour.G		=	thePalette.SpecNext512[theIndex,1];							
					thisColour.B		=	thePalette.SpecNext512[theIndex,2];
				return;
				case	Palette.PaletteMapping.mappedCustom:
					thisColour.R		=	thePalette.loadedPalette[theIndex,0];
					thisColour.G		=	thePalette.loadedPalette[theIndex,1];							
					thisColour.B		=	thePalette.loadedPalette[theIndex,2];
				return;
			}
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
			for (int y = 0; y < (blocksDisplay.Image.Height / gridYSize)+1; ++y)
			{
				g.DrawLine(pen, 0, y * gridYSize, blocksDisplay.Image.Width, y * gridYSize);
			}			
			// verticle lines
			for (int x = 0; x < (blocksDisplay.Image.Width/gridXSize)+1; ++x)
			{
				g.DrawLine(pen, x * gridXSize, 0, x * gridXSize, blocksDisplay.Image.Height);
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
			if(outType == outputData.Sprites)
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
			int	numColours			=	thePalette.loadedColourCount;
			SaveFileDialog saveFileDialog1		=	new SaveFileDialog();
			saveFileDialog1.FileName		=	projectName;// + ".asm";
			saveFileDialog1.Filter			=	"Machine Code (*.asm)|*.asm|Basic (*.bas)|*.bas|All Files (*.*)|*.*";
			saveFileDialog1.FilterIndex		=	1 ;
			saveFileDialog1.RestoreDirectory	=	true ;	
			writeType	outputFileType		=	writeType.Assember;	
			int		xOffset			=	0;
			int		yOffset			=	0;
			byte		writeByte		=	0;
			int		lineNumber		=	10000;
			int		lineStep		=	5;
			string		lableString		=	"Sprites";
			switch(centerPanel.centerPosition)
			{
				case	0:						
					xOffset			=	-(gridXSize/2);
					yOffset			=	-(gridYSize/2);
				break;					
				case	1:						
					xOffset			=	0;
					yOffset			=	-(gridYSize/2);
				break;									
				case	2:						
					xOffset			=	(gridXSize/2);
					yOffset			=	-(gridYSize/2);
				break;
				case	3:						
					xOffset			=	-(gridXSize/2);
					yOffset			=	0;
				break;					
				case	4:						
					xOffset			=	0;
					yOffset			=	0;
				break;									
				case	5:						
					xOffset			=	(gridXSize/2);
					yOffset			=	0;
				break;
				case	6:						
					xOffset			=	-(gridXSize/2);
					yOffset			=	(gridYSize/2);
				break;					
				case	7:						
					xOffset			=	0;
					yOffset			=	(gridYSize/2);
				break;									
				case	8:						
					xOffset			=	(gridXSize/2);
					yOffset			=	(gridYSize/2);
				break;
			}
			if(saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				 // Define date to be displayed.
				DateTime todaysDate		=	DateTime.Now;
				string	lableNames		=	Regex.Replace(projectName,@"\s+", "");
				if(Path.HasExtension(saveFileDialog1.FileName)==false)
				{
					outPath			=	saveFileDialog1.FileName + ".asm";
					outputFileType		=	writeType.Assember;
				}
				else
				{
					if(Path.GetExtension(saveFileDialog1.FileName)==".bas")
					{
						outPath			=	saveFileDialog1.FileName;
						outputFileType		=	writeType.Basic;
					}					
					else
					{					
						outPath			=	Path.ChangeExtension(saveFileDialog1.FileName,"asm");
						outputFileType		=	writeType.Assember;
					}
				}
				if(binaryOut.Checked==true)
				{ 
					binPath			=	Path.ChangeExtension(saveFileDialog1.FileName,"bin");
				}
				using (StreamWriter outputFile	=	new StreamWriter(outPath))
				{ 
					if(outputFileType==writeType.Basic)
					{
						outputFile.WriteLine(lineNumber.ToString() + "\tREM\t" + projectName + ".bas");
						lineNumber+=lineStep;
						outputFile.WriteLine(lineNumber.ToString() + "\tREM\tCreated on " + todaysDate.ToString("F", CultureInfo.CreateSpecificCulture("en-US")) + " by the NextGraphics tool from");							
						lineNumber+=lineStep;
						outputFile.WriteLine(lineNumber.ToString() + "\tREM\tpatricia curtis at luckyredfish dot com");
						lineNumber+=lineStep;
						outputFile.WriteLine(lineNumber.ToString() + "\tLET\t"+ lableNames + "Colours = " + numColours.ToString());
						lineNumber+=lineStep;
						outputFile.WriteLine(lineNumber.ToString()  + "\tREM");								
						lineNumber	+=	lineStep;
					}
					else
					{ 
						outputFile.WriteLine("// " + projectName + ".asm");
						outputFile.WriteLine("// Created on " + todaysDate.ToString("F", CultureInfo.CreateSpecificCulture("en-US")) + " by the NextGraphics tool from");
						outputFile.WriteLine("// patricia curtis at luckyredfish dot com\r\n");
						outputFile.WriteLine(lableNames + "Colours:\t\tequ\t" + numColours.ToString());
					}

					if(thePalette.paletteSetting==Palette.PaletteMapping.mappedCustom)
					{ 
						if(outputFileType==writeType.Basic)
						{						
							outputFile.WriteLine(lineNumber.ToString()  + "\tREM");								
							lineNumber	+=	lineStep;
							outputFile.WriteLine(lineNumber.ToString() + "\tREM\t"+ lableNames + " Palette starts here" );
							lineNumber+=lineStep;							
							outputFile.WriteLine(lineNumber.ToString()  + "\tREM");								
							lineNumber	+=	lineStep;
							outputFile.Write(lineNumber.ToString() + "\tDATA\t");
							lineNumber+=lineStep;
						}
						else
						{ 
							outputFile.WriteLine(lableNames + "Palette:");
						}
						for(int j=0;j<numColours;j++)
						{
							if(outputFileType==writeType.Basic)
							{
								outputFile.Write(EightbitPalette(thePalette.loadedPalette[j,0],thePalette.loadedPalette[j,1],thePalette.loadedPalette[j,2]).ToString());
								if(j<numColours-1)
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
								if(ignorePanel.comments.SelectedIndex==(int)comments.fullComments)
								{ 
									outputFile.WriteLine(	"\t\t\tdb\t%" + toBinary(EightbitPalette(thePalette.loadedPalette[j,0],thePalette.loadedPalette[j,1],thePalette.loadedPalette[j,2])) +
												"\t//\t" + thePalette.loadedPalette[j,0].ToString() +","+ thePalette.loadedPalette[j,1].ToString() +","+ thePalette.loadedPalette[j,2].ToString());
								}
								else
								{
									outputFile.WriteLine("\t\t\tdb\t%" + toBinary(EightbitPalette(thePalette.loadedPalette[j,0],thePalette.loadedPalette[j,1],thePalette.loadedPalette[j,2])));
								}						
							}
						}
					}
					else
					{

						if(thePalette.paletteSetting==Palette.PaletteMapping.mapped256)
						{
							if(ignorePanel.comments.SelectedIndex==(int)comments.fullComments)
							{ 
								if(outputFileType==writeType.Basic)
								{
									
									outputFile.WriteLine(lineNumber.ToString()  + "\tREM");								
									lineNumber	+=	lineStep;
									outputFile.WriteLine(lineNumber.ToString() + "\tREM\tMapped to the spectrum next 256 palette");									
									lineNumber	+=	lineStep;
								}
								else
								{									
									outputFile.WriteLine("// Mapped to the spectrum next 256 palette");							
									lineNumber	+=	lineStep;
								}
							}
						}
						else//	if(thePalette.paletteSetting==Palette.PaletteMapping.mapped512)
						{
							if(ignorePanel.comments.SelectedIndex==(int)comments.fullComments)
							{ 							
								if(outputFileType==writeType.Basic)
								{
									outputFile.WriteLine(lineNumber.ToString()  + "\tREM");								
									lineNumber	+=	lineStep;
									outputFile.WriteLine(lineNumber.ToString() + "\tREM\tMapped to the spectrum next 512 palette");								
									lineNumber	+=	lineStep;
								}
								else
								{
									outputFile.WriteLine("// Mapped to the spectrum next 512 palette");
								}
							}
						}
					}
					if(outType==outputData.Sprites)
					{
						lableString	=	"Sprite";
					}
					else
					{
						lableString	=	"Tile";
					}
					if(FourBit.Checked==true  && outType==outputData.Sprites)
					{ 
						if(outputFileType==writeType.Basic)
						{
							outputFile.WriteLine(lineNumber.ToString()  + "\tLET\t"+  lableNames + lableString + "sSize = 128");								
							lineNumber	+=	lineStep;
						}
						else
						{									
							outputFile.WriteLine(lableNames + lableString + "sSize:\t\tequ\t128");
						}
					}
					else if(outType==outputData.Sprites)
					{
						if(outputFileType==writeType.Basic)
						{
							outputFile.WriteLine(lineNumber.ToString()  + "\tLET\t"+  lableNames + lableString + "sSize = 256");								
							lineNumber	+=	lineStep;
						}
						else
						{																
							outputFile.WriteLine(lableNames + lableString + "sSize:\t\tequ\t256");
						}
					}
					else
					{
						if(outputFileType==writeType.Basic)
						{
							outputFile.WriteLine(lineNumber.ToString()  + "\tLET\t"+  lableNames + lableString + "sSize = 32");								
							lineNumber	+=	lineStep;
						}
						else
						{	
							outputFile.WriteLine(lableNames + lableString + "sSize:\t\tequ\t32");
						}
					}
					if(outputFileType==writeType.Basic)
					{
						outputFile.WriteLine(lineNumber.ToString()  + "\tLET\t"+  lableNames + lableString + "s = " + outChar.ToString());								
						lineNumber	+=	lineStep;
					}
					else
					{							
						outputFile.WriteLine(lableNames + lableString + "s:\t\tequ\t" + outChar.ToString());
					}
					for(int s=0;s<outChar;s++)
					{						
						// write the pixel data to the sprites
						if(binaryOut.Checked==true)
						{ 
							using(StreamWriter binFile	=	new StreamWriter(binPath))
							{ 
								for(int y=0;y<charData[s].Height;y++)
								{									
									for(int x=0;x<charData[s].Width;x++)
									{	
										if(FourBit.Checked==true || outType==outputData.Blocks)
										{ 
											if((x&1)==0)
											{ 
												writeByte	=	(byte)((charData[s].GetPixel(x,y)&0x0f)<<4);
											}
											else
											{
												writeByte	=	(byte) (writeByte | (charData[s].GetPixel(x,y)&0x0f));												
												binFile.Write(writeByte);									
											}
										}
										else
										{											
											binFile.Write(charData[s].GetPixel(x,y));
										}									
									}
								}
							}
						}
						else
						{
							if(outputFileType==writeType.Basic)
							{
							
								outputFile.WriteLine(lineNumber.ToString()  + "\tREM");								
								lineNumber	+=	lineStep;
								outputFile.WriteLine(lineNumber.ToString()  + "\tREM\t"+  lableNames + lableString + s.ToString() + " Starts here");								
								lineNumber	+=	lineStep;
								outputFile.WriteLine(lineNumber.ToString()  + "\tREM");								
								lineNumber	+=	lineStep;
							}
							else
							{	
								outputFile.WriteLine(lableNames + lableString + s.ToString() +":");
							}
							for(int y=0;y<charData[s].Height;y++)
							{
								if(outputFileType==writeType.Basic)
								{
									outputFile.Write(lineNumber.ToString()  + "\tDATA\t");								
									lineNumber	+=	lineStep;
								}
								else
								{							
									outputFile.Write("\t\t\t\t.db\t");
								}
								for(int x=0;x<charData[s].Width;x++)
								{	
									if(FourBit.Checked==true || outType==outputData.Blocks)
									{ 
										if((x&1)==0)
										{ 
											writeByte	=	(byte)((charData[s].GetPixel(x,y)&0x0f)<<4);
										}
										else
										{
											writeByte	=	(byte) (writeByte | (charData[s].GetPixel(x,y)&0x0f));
											switch(outputFileType)
											{ 
												case	writeType.Basic:											
													outputFile.Write(writeByte.ToString());	
													if(x<(charData[s].Width-1))
													{										
														outputFile.Write(",");
													}
													else
													{										
														outputFile.Write("\r\n");
													}											
												break;
												case	writeType.Assember:
													outputFile.Write("${0:x2}",writeByte);
													if(x<(charData[s].Width-1))
													{										
														outputFile.Write(",");
													}
													else
													{										
														outputFile.Write("\r\n");
													}						
												break;
											}										
										}
									}
									else
									{
										switch(outputFileType)
										{ 
											case	writeType.Basic:			
												outputFile.Write(charData[s].GetPixel(x,y).ToString());
												if(x<(charData[s].Width-1))
												{										
													outputFile.Write(",");
												}
												else
												{										
													outputFile.Write("\r\n");
												}											
											break;
											case	writeType.Assember:			
												outputFile.Write("${0:x2}",charData[s].GetPixel(x,y));
												if(x<(charData[s].Width-1))
												{										
													outputFile.Write(",");
												}
												else
												{										
													outputFile.Write("\r\n");
												}							
											break;
										}	
									}									
								}
							}
						}
					}	
					if(outputFileType!=writeType.Basic)
					{
						if(ignorePanel.comments.SelectedIndex==(int)comments.fullComments && outType==outputData.Sprites)
						{ 
							outputFile.WriteLine("\r\n\t\t\t\t// number of sprites wide");
							outputFile.WriteLine("\t\t\t\t// number of sprites tall\r\n");
							outputFile.WriteLine("\t\t\t\t\t// x offset from center of sprite");
							outputFile.WriteLine("\t\t\t\t\t// y offset from center of sprite");
							outputFile.WriteLine("\t\t\t\t\t// Palette offset with the X mirror,Y mirror, Rotate bits if set");
							outputFile.WriteLine("\t\t\t\t\t// 4 bit colour bit and pattern offset bit");
							outputFile.WriteLine("\t\t\t\t\t// index of the sprite at this position that makes up the frame\r\n");							
							outputFile.WriteLine("\t\t\t\t//...... repeated wide x tall times\r\n");
							
						}
						else if(ignorePanel.comments.SelectedIndex==(int)comments.fullComments)
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
						
					}
					else if(outType==outputData.Blocks==true)
					{
						outputFile.WriteLine(lineNumber.ToString()  + "\tLET\t"+  lableNames + lableString + "Width = " +  blockInfo[0].Width.ToString());								
						lineNumber	+=	lineStep;
						outputFile.WriteLine(lineNumber.ToString()  + "\tLET\t"+  lableNames + lableString + "Height = " +  blockInfo[0].Width.ToString());								
						lineNumber	+=	lineStep;
					}		
					int FrameCounter		=	0;
					for(int s=0;s<outBlock;s++)
					{				
						if(outType==outputData.Sprites)
						{ 
							if(outputFileType==writeType.Basic)
							{
								outputFile.WriteLine(lineNumber.ToString()  + "\tREM");															
								lineNumber	+=	lineStep;
								outputFile.WriteLine(lineNumber.ToString()  + "\tLET\t"+  lableNames + "Frame" + s.ToString() +"Width = " + blockInfo[s].Width.ToString());								
								lineNumber	+=	lineStep;
								outputFile.WriteLine(lineNumber.ToString()  + "\tLET\t"+  lableNames + "Frame" + s.ToString() +"Height = " + blockInfo[s].Width.ToString());								
								lineNumber	+=	lineStep;
							}
							else
							{ 
								outputFile.Write(lableNames + "Frame" + s.ToString() +":");
								outputFile.Write("\t\t.db\t");
								outputFile.Write(blockInfo[s].Width.ToString() + ",");
								outputFile.Write(blockInfo[s].Height.ToString() + ",\t");
							}
						}
						else
						{		
							if(outputFileType!=writeType.Basic)						
							{ 
								outputFile.Write(lableNames + "Block" + s.ToString() +":");							
								outputFile.Write("\t\t.db\t");
							}
						}
						// adjust x y pos based on the centerPanel setting
						FrameCounter		=	0;
						for(int y=0;y<blockInfo[s].Height;y++)
						{
							for(int x=0;x<blockInfo[s].Width;x++)
							{				
								if(outType==outputData.Sprites)
								{ 
									if(outputFileType==writeType.Basic)						
									{	
										outputFile.WriteLine(lineNumber.ToString()  + "\tLET\t"+  lableNames + "Sprite" + s.ToString() +"XOffset"+FrameCounter.ToString()+" = " + (xOffset+(blockInfo[s].GetXPos(x,y)*outSize)).ToString());								
										lineNumber	+=	lineStep;
										outputFile.WriteLine(lineNumber.ToString()  + "\tLET\t"+  lableNames + "Sprite" + s.ToString() +"YOffset"+FrameCounter.ToString()+" = " + (yOffset+(blockInfo[s].GetYpos(x,y)*outSize)).ToString());								
										lineNumber	+=	lineStep;
							
									}
									else
									{ 
										outputFile.Write((xOffset+(blockInfo[s].GetXPos(x,y)*outSize)).ToString() + ",");
										outputFile.Write((yOffset+(blockInfo[s].GetYpos(x,y)*outSize)).ToString() + ",");
									}
								}
								writeByte	=	0;
								if(outType==outputData.Sprites)
								{ 								
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
									if(outputFileType==writeType.Basic)						
									{	
										outputFile.WriteLine(lineNumber.ToString()  + "\tLET\t"+  lableNames + "Sprite" + s.ToString() +"Attribute2Bits"+FrameCounter.ToString()+" = " + writeByte.ToString() );								
										lineNumber	+=	lineStep;							
									}
									else
									{ 										
										outputFile.Write(writeByte.ToString() + ",");
									}
									writeByte	=	0;
								
									if(FourBit.Checked==true)
									{ 										
										writeByte = (byte)(writeByte | 128);
										if((blockInfo[s].GetId(x,y)&1)==1)
										{				
											writeByte = (byte)(writeByte | 64);
										}
									}
									if(outputFileType==writeType.Basic)						
									{	
										outputFile.WriteLine(lineNumber.ToString()  + "\tLET\t"+  lableNames + "Sprite" + s.ToString() +"FourBitBits"+FrameCounter.ToString()+" = " + writeByte.ToString() );								
										lineNumber	+=	lineStep;							
									}
									else
									{	
										outputFile.Write(writeByte.ToString() + ",");
									}
								}
								else
								{
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
									if(outputFileType==writeType.Basic)						
									{
										outputFile.WriteLine(lineNumber.ToString()  + "\tLET\t"+  lableNames + "Tile" + s.ToString() +"MirrorRotateBits"+FrameCounter.ToString()+" = " + writeByte.ToString() );								
										lineNumber	+=	lineStep;	
									}
									else
									{
										outputFile.Write(writeByte.ToString() + ",");
									}
									writeByte	=	0;									
								}
								if(outputFileType==writeType.Basic)						
								{
									outputFile.WriteLine(lineNumber.ToString()  + "\tLET\t"+  lableNames + "Tile" + s.ToString() +"Index"+FrameCounter.ToString()+" = " +  blockInfo[s].GetId(x,y).ToString());								
									lineNumber	+=	lineStep;	
									FrameCounter++;
									outputFile.WriteLine(lineNumber.ToString()  + "\tREM");								
									lineNumber	+=	lineStep;
								}
								else
								{ 
									outputFile.Write(blockInfo[s].GetId(x,y).ToString());		
									if(x<(blockInfo[s].Width)-1)
									{
										outputFile.Write(",\t");
									}
								}
							}
							if(outputFileType!=writeType.Basic)						
							{
									
								if(y<(blockInfo[s].Height)-1)
								{
									outputFile.Write(",\t");
								}
								else
								{ 
									outputFile.Write("\r\n");
								}
							}
							//else
							//{
							//	outputFile.WriteLine(lineNumber.ToString()  + "\tREM");								
							//	lineNumber	+=	lineStep;
							//}
						}
					}
					if(outputFileType!=writeType.Basic)
					{ 									
						outputFile.Write("\r\n");
						if(outType==outputData.Sprites)
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
					}
				}
				// now output a blocks file BMP 
				if(outType==outputData.Blocks)
				{ 
					string	blocksOutPath		=	Path.GetDirectoryName(saveFileDialog1.FileName);						
					string	blocksOutFilename	=	Path.GetFileNameWithoutExtension(saveFileDialog1.FileName);
					Bitmap	outBlocks	=	new	Bitmap(gridXSize,gridYSize*outBlock);
					{
						int	yPos	=	0;
						for(int b=0;b<outBlock;b++)
						{
							for(int	y=0;y<gridYSize;y++)
							{		
								for(int	x=0;x<gridXSize;x++)
								{
									SetFromPalette(blockData[b].GetPixel(x,y));
									outBlocks.SetPixel(x,yPos+y,Color.FromArgb(thisColour.getARGB()));
								}
							}	
							yPos+=gridYSize;			
						}
						outBlocks.Save(blocksOutPath+"\\"+blocksOutFilename+"-blocks.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
					}
					Bitmap	outChars	=	new	Bitmap(8,8*outChar);
					{
						int	yPos	=	0;
						for(int b=0;b<outChar;b++)
						{
							for(int	y=0;y<8;y++)
							{		
								for(int	x=0;x<8;x++)
								{
									SetFromPalette(charData[b].GetPixel(x,y));
									outChars.SetPixel(x,yPos+y,Color.FromArgb(thisColour.getARGB()));
								}
							}	
							yPos+=8;			
						}
						outChars.Save(blocksOutPath+"\\"+blocksOutFilename+"-tiles.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
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
		
		
			thePalette.StartPosition	=	FormStartPosition.CenterParent;
			thePalette.ShowDialog();
			if (thePalette.DialogResult == DialogResult.OK)
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
			if(thisIndex<=fullNames.Count-1 && thisIndex>0)
			{
				
				string	temp			=	fullNames[thisIndex];
				fullNames[thisIndex]		=	fullNames[thisIndex-1];
				fullNames[thisIndex-1]		=	temp;
				restoreFromList();
				this.listBox1.SelectedIndex	=	thisIndex+1;
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
				string	temp			=	fullNames[thisIndex-1];
				fullNames[thisIndex-1]		=	fullNames[thisIndex];
				fullNames[thisIndex]		=	temp;
				restoreFromList();
				this.listBox1.SelectedIndex	=	thisIndex;
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
					
					fullNames.RemoveAt(this.listBox1.SelectedIndex-1);
					restoreFromList();
				}  							
			}
		}
		//-------------------------------------------------------------------------------------------------------------------
		//
		// check pixel format to see if its supported
		//
		//-------------------------------------------------------------------------------------------------------------------	

		private		bool	IsSupported(Bitmap srcBitmap)
		{
			switch (srcBitmap.PixelFormat)
			{
				case PixelFormat.Format16bppRgb555:
				case PixelFormat.Format16bppArgb1555:
				case PixelFormat.Format16bppRgb565:						
				case PixelFormat.Format24bppRgb:
				case PixelFormat.Format32bppArgb:
				//case PixelFormat.Format32bppPArgb:						
				case PixelFormat.Format32bppRgb:
				case PixelFormat.Format8bppIndexed:
					return	true;
			}
			return	false;
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
			outType			=	outputData.Blocks;
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
			outType			=	outputData.Sprites;	
			outSize			=	16;		
			charactersDisplay.Invalidate();
			charactersDisplay.Refresh();			
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// init the settings for from a loaded image
		//
		//-------------------------------------------------------------------------------------------------------------------	
	
		public	void	setForm()
		{
			textBox2.Text	=	gridYSize.ToString();	
			textBox1.Text	=	gridXSize.ToString();
			if(outType==outputData.Sprites)
			{
				spritesOut.Checked	=	true;
				blocksOut.Checked	=	false;	
				selectedRadio		=	spritesOut;
				outSize			=	16;
			}
			else //outputData.Blocks;
			{ 
				blocksOut.Checked	=	true;
				spritesOut.Checked	=	false;	
				selectedRadio		=	blocksOut;
				outSize			=	8;
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
			if(int.TryParse(textBox2.Text, out gridYSize))
			{ 
				if(gridYSize<8)
				{
					gridYSize	=	8;
				}
				else if (gridYSize>128)
				{ 
					gridYSize	=	128;
				}
				else
				{
					if(outType==outputData.Sprites)
					{
						gridYSize	=	(gridYSize + 15) & ~0xF;	
					}
					else
					{
						gridYSize	=	(gridYSize + 7) & ~0x7;
					}
				}				
				textBox2.Text	=	gridYSize.ToString();
			}
			else
			{				
				gridYSize	=	32;
				textBox2.Text	=	gridYSize.ToString();
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
			if(int.TryParse(textBox1.Text, out gridXSize))
			{ 
				if(gridXSize<8)
				{
					gridXSize	=	8;
				}
				else if (gridXSize>128)
				{ 
					gridXSize	=	128;
				}
				else
				{ 
					if(outType==outputData.Sprites)
					{
						gridXSize	=	(gridXSize + 15) & ~0xF;	
					}
					else
					{
						gridXSize	=	(gridXSize + 7) & ~0x7;
					}
				}				
				textBox1.Text	=	gridXSize.ToString();
			}
			else
			{				
				gridXSize	=	32;
				textBox1.Text	=	gridXSize.ToString();
			}
			blocksAcross	=	(int)Math.Floor((float)blocksDisplay.Width/gridXSize);
			blocksDisplay.Invalidate();
			blocksDisplay.Refresh();
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// 4 bit colour check box changed
		//
		//-------------------------------------------------------------------------------------------------------------------	
		
		private void fourBitColourChanged(object sender, EventArgs e)
		{
			CheckBox		box	=	(CheckBox)sender;
			thePalette.fourBitOutput	=	box.Checked;	
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// open the ignore panel
		//
		//-------------------------------------------------------------------------------------------------------------------	
		
		private void openIgnorePanel(object sender, EventArgs e)
		{
			ignorePanel.StartPosition	=	FormStartPosition.CenterParent;
			if(ignorePanel.ShowDialog()	==	DialogResult.OK)
			{
			}
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// open the center panel
		//
		//-------------------------------------------------------------------------------------------------------------------	
		
		private void openCenterPanel(object sender, EventArgs e)
		{
			centerPanel.StartPosition	=	FormStartPosition.CenterParent;
			if(centerPanel.ShowDialog()	==	DialogResult.OK)
			{
			}
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
	}
}