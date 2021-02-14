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
		private	const int		MAX_BLOCKS		=	256;
		private	const int		MAX_CHAR_SIZE		=	255;
		private	const int		MAX_IMAGES		=	64;
		public	List<string> 		fullNames		=	new	List<string>();
		public	List<imageWindow>	imageWindows		=	new	List<imageWindow>();
		public	List<Bitmap>		sourceImages		=	new	List<Bitmap>();
#if PROPRIETARY
		public	parallaxTool		parallaxWindow		=	new	parallaxTool();
#endif
		public	imageWindow		blocksWindow		=	new	imageWindow();
		public	imageWindow		charsWindow		=	new	imageWindow();
		private	Bitmap	 		blocksPanel;
		private	Bitmap	 		charsPanel;
		public	bitsBitmap[]		blockData		=	new	bitsBitmap[MAX_BLOCKS];	
		public	spriteInfo[]		blockInfo		=	new	spriteInfo[MAX_BLOCKS];
		public	bitsBitmap[]		charData		=	new	bitsBitmap[MAX_CHAR_SIZE];	
		private	bitsBitmap[]		tempData		=	new	bitsBitmap[MAX_CHAR_SIZE];	
		public	int[]			sortIndexs		=	new	int[MAX_CHAR_SIZE];	
		private int			gridXSize		=	32;
		private int			gridYSize		=	32;
		private	int			outXBlock		=	0;
		private	int			outYBlock		=	0;
		private	int			outBlock		=	0;
		private	int			outXChar		=	0;
		private	int			outYChar		=	0;
		private	int			outChar			=	0;
		public	string			projectName		=	"Untitled project";
		private	Rectangle		src			=	new	Rectangle();
		private	Rectangle		dest			=	new	Rectangle();
		private	Rectangle		charDest		=	new	Rectangle();
		private	string			projectPath		=	"";
		private	string			outPath			=	"";
		private	string			binPath			=	"";
		private	string			tilesPath		=	"";
		private	int				blocksAcross		=	8;
		private	bool			reverseByes		=	false;
		private	int				outputFilterIndex	=	1;
		private	int				imageFilterIndex	=	1;
		private	bool			PaletteSet			=	false;	
		public	Palette			thePalette			=	new Palette();
		private	settingsPanel	SettingsPanel		=	new settingsPanel();
		private	long			AveragingIndex		=	0;		
		public	imageWindow		blocksView			=	new	imageWindow();
		private	int				tranpearntChars		= 0;
		private	int				SortedIndex			= 0;
		private readonly NumberFormatInfo	fmt			= new NumberFormatInfo();
	
		SaveFileDialog			projectSaveDialog	=	new SaveFileDialog();
		OpenFileDialog			openProjectDialog	=	new OpenFileDialog();
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
			blocksPanel				=	new	Bitmap(16*blocksAcross,32*16,PixelFormat.Format24bppRgb);
			clearPanels(blocksPanel);
			blocksDisplay.Image			=	blocksPanel;
			blocksDisplay.Height			=	blocksPanel.Height;
			blocksDisplay.Width			=	blocksPanel.Width;
#if DEBUG_WINDOW
			DEBUG_WINDOW				=	new	DEBUGFORM();
			DEBUG_WINDOW.Show();
#endif
	
			charsPanel				=	new	Bitmap(128,128,PixelFormat.Format24bppRgb);
			clearPanels(charsPanel);
			charactersDisplay.Image			=	charsPanel;			
			this.toolStripProgressBar1.Minimum	=	0;
			this.toolStripProgressBar1.Maximum	=	0;
			this.listBox1.Items.Add(" "+projectName);
			reverseByes				=	BitConverter.IsLittleEndian;
			thePalette.parentForm			=	this;
			thePalette.selectForm.parentForm	=	this;
			blocksAcross				=	(int)Math.Floor((float)blocksDisplay.Width/gridXSize);
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
			if(projectPath.Length==0)
			{ 
				
				projectSaveDialog.FileName		=	projectName + ".xml";
				projectSaveDialog.Filter		=	"Project Files (*.xml)|*.xml|All Files (*.*)|*.*";
				projectSaveDialog.FilterIndex		=	1 ;
				projectSaveDialog.RestoreDirectory	= false;			
				if(projectSaveDialog.ShowDialog() == DialogResult.OK)
				{
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
			int	transIndex		=	thePalette.transIndex;
			int	loadedColourCount	=	thePalette.loadedColourCount;
			int	centerPos		=	SettingsPanel.centerPosition;
			using 	(XmlTextWriter writer = new XmlTextWriter(prjPath, Encoding.UTF8))
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
				XmlNode		settingsNode		=			doc.CreateElement("Settings");
				XmlAttribute	settingsAttribute	=			null;	
				if(outType==outputData.Blocks)
				{ 
					settingsAttribute	=	doc.CreateAttribute("blocks");	
				}
				else if(outType==outputData.Sprites)
				{
					settingsAttribute	=	doc.CreateAttribute("sprites");	
				}
				else
				{					
					//MessageBox.Show("Should not get this as its not implemented", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);					
				}
				settingsAttribute.Value		= 	"true";
				settingsNode.Attributes.Append(settingsAttribute);
				settingsAttribute			=	doc.CreateAttribute("center");
				settingsAttribute.Value		= 	centerPos.ToString();
				settingsNode.Attributes.Append(settingsAttribute);
				settingsAttribute			=	doc.CreateAttribute("xSize");
				settingsAttribute.Value		= 	gridXSize.ToString();
				settingsNode.Attributes.Append(settingsAttribute);
				settingsAttribute			=	doc.CreateAttribute("ySize");
				settingsAttribute.Value		= 	gridYSize.ToString();	
				settingsNode.Attributes.Append(settingsAttribute);	
				settingsAttribute			=	doc.CreateAttribute("fourBit");
				if(SettingsPanel.FourBit.Checked==true)
				{					
					settingsAttribute.Value		= 	"true";
				}	
				else
				{
					settingsAttribute.Value		= 	"false";
				}
				settingsNode.Attributes.Append(settingsAttribute);	
				settingsAttribute			=	doc.CreateAttribute("binary");
				if(SettingsPanel.binaryOut.Checked==true)
				{					
					settingsAttribute.Value		= 	"true";
				}	
				else
				{
					settingsAttribute.Value		= 	"false";
				}
				settingsNode.Attributes.Append(settingsAttribute);
				settingsAttribute			=	doc.CreateAttribute("binaryBlocks");
				if(SettingsPanel.binaryBlocks.Checked==true)
				{					
					settingsAttribute.Value		= 	"true";
				}	
				else
				{
					settingsAttribute.Value		= 	"false";
				}
				settingsNode.Attributes.Append(settingsAttribute);

				settingsAttribute	=	doc.CreateAttribute("Repeats");		
				if(SettingsPanel.Repeats.Checked==true)
				{					
					settingsAttribute.Value		= 	"true";
				}	
				else
				{
					settingsAttribute.Value		= 	"false";
				}
				settingsNode.Attributes.Append(settingsAttribute);	
				settingsAttribute			=	doc.CreateAttribute("MirrorX");		
				if(SettingsPanel.mirrorX.Checked==true)
				{					
					settingsAttribute.Value		= 	"true";
				}	
				else
				{
					settingsAttribute.Value		= 	"false";
				}
				settingsNode.Attributes.Append(settingsAttribute);	
				settingsAttribute			=	doc.CreateAttribute("MirrorY");		
				if(SettingsPanel.mirrorY.Checked==true)
				{					
					settingsAttribute.Value		= 	"true";
				}	
				else
				{
					settingsAttribute.Value		= 	"false";
				}
				settingsNode.Attributes.Append(settingsAttribute);	
				settingsAttribute			=	doc.CreateAttribute("Rotations");		
				if(SettingsPanel.rotations.Checked==true)
				{					
					settingsAttribute.Value		= 	"true";
				}	
				else
				{
					settingsAttribute.Value		= 	"false";
				}
				settingsNode.Attributes.Append(settingsAttribute);	
				settingsAttribute			=	doc.CreateAttribute("Transparent");		
				if(SettingsPanel.Transparent.Checked==true)
				{					
					settingsAttribute.Value		= 	"true";
				}	
				else
				{
					settingsAttribute.Value		= 	"false";
				}
				settingsNode.Attributes.Append(settingsAttribute);		
				
				settingsAttribute				=	doc.CreateAttribute("Sort");		
				if(SettingsPanel.sortTransparent.Checked==true)
				{					
					settingsAttribute.Value		= 	"true";
				}	
				else
				{
					settingsAttribute.Value		= 	"false";
				}
				settingsNode.Attributes.Append(settingsAttribute);	
				
				settingsAttribute				=	doc.CreateAttribute("blocksImage");		
				if(SettingsPanel.blocksOut.Checked==true)
				{					
					settingsAttribute.Value		= 	"true";
				}	
				else
				{
					settingsAttribute.Value		= 	"false";
				}
				settingsNode.Attributes.Append(settingsAttribute);
				settingsAttribute				=	doc.CreateAttribute("tilesImage");		
				if(SettingsPanel.tilesOut.Checked==true)
				{					
					settingsAttribute.Value		= 	"true";
				}	
				else
				{
					settingsAttribute.Value		= 	"false";
				}
				settingsNode.Attributes.Append(settingsAttribute);
				settingsAttribute				=	doc.CreateAttribute("transBlock");		
				if(SettingsPanel.transBlock.Checked==true)
				{					
					settingsAttribute.Value		= 	"true";
				}	
				else
				{
					settingsAttribute.Value		= 	"false";
				}
				settingsNode.Attributes.Append(settingsAttribute);
				settingsAttribute				=	doc.CreateAttribute("transTile");		
				if(SettingsPanel.transTile.Checked==true)
				{					
					settingsAttribute.Value		= 	"true";
				}	
				else
				{
					settingsAttribute.Value		= 	"false";
				}
				settingsNode.Attributes.Append(settingsAttribute);
				settingsAttribute			=	doc.CreateAttribute("across");		
				settingsAttribute.Value			= 	SettingsPanel.tilesAcross.Text;
				settingsNode.Attributes.Append(settingsAttribute);

				settingsAttribute			=	doc.CreateAttribute("accurate");		
				settingsAttribute.Value			= 	SettingsPanel.Accuracy.Text;
				settingsNode.Attributes.Append(settingsAttribute);
			

				settingsAttribute			=	doc.CreateAttribute("format");		
				settingsAttribute.Value			= 	SettingsPanel.blocksFormat.SelectedIndex.ToString();
				settingsNode.Attributes.Append(settingsAttribute);

				projectNode.AppendChild(settingsNode);


#if PROPRIETARY
				projectNode.AppendChild(parallaxWindow.writeParallax(doc));
#endif
				XmlNode paletteNode		=	doc.CreateElement("Palette");
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
							colourAttribute.Value	=	thePalette.loadedPalette[c,2].ToString();									
							colourNode.Attributes.Append(colourAttribute);	
							paletteNode.AppendChild(colourNode);
				}							
				paletteNode.Attributes.Append(paletteAttribute);	
				// file dialogs				
				XmlNode		dialogsNode		=	doc.CreateElement("Dialogs");;
				XmlAttribute	dialogsAttribute	=	doc.CreateAttribute("OutputIndex");	
				dialogsAttribute.Value			=	outputFilesDialog.FilterIndex.ToString();
				dialogsNode.Attributes.Append(dialogsAttribute);
				dialogsAttribute			=	doc.CreateAttribute("ImageIndex");	
				dialogsAttribute.Value			=	addImagesDialog.FilterIndex.ToString();
				dialogsNode.Attributes.Append(dialogsAttribute);				
				projectNode.AppendChild(dialogsNode);

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
			
			openProjectDialog.Multiselect			=	false;
			openProjectDialog.RestoreDirectory		= false;
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
					this.listBox1.Items.Add(" "+projectName);

					XmlNode projectTypeNode			=	xmlDoc.SelectSingleNode("//Project/Settings");
					if(projectTypeNode!=null)
					{ 
						SettingsPanel.setCenter(int.Parse(projectTypeNode.Attributes["center"].Value));
						gridYSize				=	int.Parse(projectTypeNode.Attributes["ySize"].Value);
						gridXSize				=	int.Parse(projectTypeNode.Attributes["xSize"].Value);
						SettingsPanel.FourBit.Checked		=	bool.Parse(projectTypeNode.Attributes["fourBit"].Value);
						thePalette.fourBitOutput		=	SettingsPanel.FourBit.Checked;
						if(projectTypeNode.Attributes["sprites"]!=null)
						{
							outType	=	outputData.Sprites;
						}
						else
						{					
							outType	=	outputData.Blocks;
						}		
					
						SettingsPanel.binaryOut.Checked		=	bool.Parse(projectTypeNode.Attributes["binary"].Value);
						if(projectTypeNode.Attributes["binaryBlocks"]!=null)
						{
							SettingsPanel.binaryBlocks.Checked	=	bool.Parse(projectTypeNode.Attributes["binaryBlocks"].Value);						
						}
						SettingsPanel.binaryBlocks.Enabled	=	SettingsPanel.binaryOut.Checked;
					}				
					XmlNodeList fileNodes			=	xmlDoc.SelectNodes("//Project/File");
					if(fileNodes!=null)
					{ 
						fullNames.Clear();
						foreach(XmlNode fileNode in fileNodes)
						{				
							fullNames.Add(fileNode.Attributes["Path"].Value);
						}
						restoreFromList();
					}
					XmlNode	settingsNode			=	xmlDoc.SelectSingleNode("//Project/Settings");
					
					if(settingsNode!=null)
					{ 
						if(settingsNode.Attributes["Repeats"]!=null)
						{
							SettingsPanel.Repeats.Checked	= bool.Parse(settingsNode.Attributes["Repeats"].Value);
						}
						if(settingsNode.Attributes["MirrorX"]!=null)
						{
							SettingsPanel.mirrorX.Checked	= bool.Parse(settingsNode.Attributes["MirrorX"].Value);
						}
						if(settingsNode.Attributes["MirrorY"]!=null)
						{
							SettingsPanel.mirrorY.Checked	= bool.Parse(settingsNode.Attributes["MirrorY"].Value);						
						}
						if(settingsNode.Attributes["Rotations"]!=null)
						{
							SettingsPanel.rotations.Checked	= bool.Parse(settingsNode.Attributes["Rotations"].Value);
						}
						if(settingsNode.Attributes["Transparent"]!=null)
						{
							SettingsPanel.Transparent.Checked	= bool.Parse(settingsNode.Attributes["Transparent"].Value);							
						}
						if(settingsNode.Attributes["Sort"]!=null)
						{	
							SettingsPanel.sortTransparent.Checked	= bool.Parse(settingsNode.Attributes["Sort"].Value);							
						}
						if(settingsNode.Attributes["blocksImage"]!=null)
						{
							SettingsPanel.blocksOut.Checked		= bool.Parse(settingsNode.Attributes["blocksImage"].Value);							
						}
						if(settingsNode.Attributes["tilesImage"]!=null)
						{
							SettingsPanel.tilesOut.Checked		= bool.Parse(settingsNode.Attributes["tilesImage"].Value);							
						}
						if(settingsNode.Attributes["transBlock"]!=null)
						{
							SettingsPanel.transBlock.Checked	=	 bool.Parse(settingsNode.Attributes["transBlock"].Value);							
						}
						if(settingsNode.Attributes["transTile"]!=null)
						{
							SettingsPanel.transTile.Checked		= bool.Parse(settingsNode.Attributes["transTile"].Value);
						}
						if(settingsNode.Attributes["across"]!=null)
						{
							SettingsPanel.tilesAcross.Text	=	settingsNode.Attributes["across"].Value;							
						}
						if(settingsNode.Attributes["format"]!=null)
						{
							SettingsPanel.blocksFormat.SelectedIndex	=	int.Parse(settingsNode.Attributes["format"].Value);							
						}
						if(settingsNode.Attributes["accurate"]!=null)
						{
							SettingsPanel.Accuracy.Text			=	settingsNode.Attributes["accurate"].Value;							
						}
					}

					XmlNode parallax		= xmlDoc.SelectSingleNode("//Project/parallax");
					if(parallax != null)
					{
#if PROPRIETARY
							parallaxWindow.readParallax(parallax);
#endif
					}
					XmlNode	palette 			=	xmlDoc.SelectSingleNode("//Project/Palette");
					if(palette!=null)
					{ 
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
							if(colourNode.Attributes["Red"]!=null)
							{
								thePalette.loadedPalette[c,0]	=	byte.Parse(colourNode.Attributes["Red"].Value);
							}
							if(colourNode.Attributes["Green"]!=null)
							{
								thePalette.loadedPalette[c,1]	=	byte.Parse(colourNode.Attributes["Green"].Value);
							}
							if(colourNode.Attributes["Blue"]!=null)
							{
								thePalette.loadedPalette[c,2]	=	byte.Parse(colourNode.Attributes["Blue"].Value);
							}
						}
						thePalette.setLoadedProjectForms();				
						
					}
					XmlNode	Dialogs				=	xmlDoc.SelectSingleNode("//Project/Dialogs");
					if(Dialogs!=null)
					{ 
						thePalette.setForm();
						if(palette.Attributes["OutputIndex"]!=null)
						{
							outputFilterIndex	=	int.Parse(palette.Attributes["OutputIndex"].Value);
						}
						if(palette.Attributes["ImageIndex"]!=null)
						{
							imageFilterIndex	=	int.Parse(palette.Attributes["ImageIndex"].Value);
						}
					}									
					setForm();
					if (parallax != null)
					{
#if PROPRIETARY
						this.parallaxWindow.loadProject();
#endif
					}
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
			Bitmap	srcBitmap;
			foreach (string name in fullNames)
			{
				try 
				{ 
					using (var fs = new System.IO.FileStream(name, System.IO.FileMode.Open))
					{
					    var bmp	= new Bitmap(fs);
					    srcBitmap	= new Bitmap(bmp.Width,bmp.Height);
					    srcBitmap	= (Bitmap) bmp.Clone();
					}					
					if(IsSupported(new Bitmap(srcBitmap))==true)
					{
						sourceImages.Add(new Bitmap(srcBitmap));
						imageWindows.Add(new imageWindow { MdiParent = this});
						this.listBox1.Items.Add("  " + Path.GetFileName(name));	
					}
					else
					{
						removeNames.Add(name);
					}
				}
				catch// (System.ArgumentException ex)
				{	// do not remove the name as it could already be in the system			
					//removeNames.Add(name);
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
			
			addImagesDialog.Multiselect		=	true;
			addImagesDialog.RestoreDirectory	= false;
	
			//addImagesDialog.DefaultExt		=	".bmp"; // Default file extension 
			addImagesDialog.FilterIndex		=	imageFilterIndex;			
			Bitmap	srcBitmap;
			if (addImagesDialog.ShowDialog(this) == DialogResult.OK)
			{
				
				imageFilterIndex	=		addImagesDialog.FilterIndex;
				bool	rejected=false;
				foreach (imageWindow window in imageWindows)
				{
					if(window.inputImage!=null)
					{ 						
						window.inputImage.Dispose();
					}
					if(window!=null)
					{ 
						window.Close();
						window.Dispose();
					}
				}
				foreach (Bitmap bitMap in sourceImages)
				{
					if(bitMap!=null)
					{ 
						bitMap.Dispose();
					}
				}
				imageWindows.Clear();
				sourceImages.Clear();
				foreach (String file in addImagesDialog.FileNames) 
				{

					for(int i=0;i<fullNames.Count;i++)
					{
						if(file == fullNames[i])
						{
							rejected	=	true;
							goto rejectName;
						}
					}
					using (var fs = new System.IO.FileStream(file, System.IO.FileMode.Open))
					{
					    var bmp	= new Bitmap(fs);
					    srcBitmap	= new Bitmap(bmp.Width,bmp.Height);
					    srcBitmap	= (Bitmap) bmp.Clone();
					}					
					if(IsSupported(new Bitmap(srcBitmap))==true)
					{
						fullNames.Add(file);
						sourceImages.Add(new Bitmap(srcBitmap));
						imageWindows.Add(new imageWindow { MdiParent = this});
						this.listBox1.Items.Add("  " + Path.GetFileName(file));	
					}

rejectName:				;
				}
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
							imageWindows[index-1].Height	=	this.Height-(toolStrip.Height+100);
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
				if(tempData[c]!=null)
				{ 
					tempData[c].Dispose();
					tempData[c]=	null;
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
			
			outXChar	=	0;
			outYChar	=	0;
			if(outType == outputData.Blocks)
			{ 
				// make the first block transparent
				if(SettingsPanel.transBlock.Checked==true)
				{ 

					if(blockData[0]==null)
					{ 
						blockData[0]	=	new	bitsBitmap(gridXSize,gridYSize);
						for(int y=0;y<gridYSize;y++)
						{ 
							for(int x=0;x<gridXSize;x++)
							{
								blockData[0].SetPixel(x,y, (short)thePalette.transIndex);
							}
						}					
						blockToDisplay(ref blocksPanel,new Rectangle(0,0,gridXSize,gridYSize),ref blockData[0]);
					}					
					outXBlock	=	1;
					outBlock	=	1;
					if(blockInfo[0]==null)
					{ 
						blockInfo[0]		=	new	spriteInfo(gridXSize/outSize,gridYSize/outSize);
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
								tempData[0].SetPixel(x,y, (short)thePalette.transIndex);
							}
						}					
					}
					
					outXChar	=	1;
					outChar		=	1;
				}
			}
			for(int s=0;s<sourceImages.Count;s++)
			{ 			
				if(outType==outputData.Blocks)
				{ 
					if((sourceImages[s].Width%gridXSize)>0)
					{
						// not width 
						MessageBox.Show("The image "+Path.GetFileName(fullNames[s])+ " is not divisible by the width of your tiles, which will corrupt the output", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
					if((sourceImages[s].Height%gridYSize)>0)
					{
						// not height 
						MessageBox.Show("The image "+Path.GetFileName(fullNames[s])+ " is not divisible by the height of your tiles, which will corrupt the output", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);					
					}
				}
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
								blockData[outBlock]	=	new	bitsBitmap(gridXSize,gridYSize);
							}
							CopyRegionIntoBlock(sourceImages[s],src,ref blockData[outBlock]);
			
							if(SettingsPanel.FourBit.Checked==true || outType==outputData.Blocks)
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
									if(SettingsPanel.FourBit.Checked==true || outType==outputData.Blocks)
									{
										PaletteOffset	=		blockData[outBlock].GetPixel(xChar*outSize,yChar*outSize)&0x0f0;								
									}					
									for(int c=0;c<outChar;c++)
									{		
										PaletteOffset =	0;										
										thisCharType	=	checkRepeatedChar(c,xChar*outSize,yChar*outSize);
										
										if(thisCharType	!=	blockType.Original)
										{ 
											switch(thisCharType)
											{
												case	blockType.Repeated:			 //		 rep  flpX flpY  rot   trans
													blockInfo[outBlock].SetData(xChar,yChar,true,false,false,false,false,(short)c,(short)PaletteOffset, checkHasTrans(c));
													//charInfo[outChar].SetData(xChar,yChar,true,false,false,false,false,(short)c);
													goto	dontDrawCharacter;
												case	blockType.FlippedX:         //		 rep  flpX flpY  rot   trans
													blockInfo[outBlock].SetData(xChar,yChar,true,true,false,false,false,(short)c,(short)PaletteOffset, checkHasTrans(c));
													goto	dontDrawCharacter;
												case	blockType.FlippedY:         //		 rep  flpX flpY  rot   trans
													blockInfo[outBlock].SetData(xChar,yChar,true,false,true,false,false,(short)c,(short)PaletteOffset, checkHasTrans(c));
													goto	dontDrawCharacter;
												case	blockType.FlippedXY:         //		 rep  flpX flpY  rot   trans
													blockInfo[outBlock].SetData(xChar,yChar,true,true,true,false,false,(short)c,(short)PaletteOffset, checkHasTrans(c));
													goto	dontDrawCharacter;
												case	blockType.Rotated:         //		 rep  flpX flpY  rot   trans			
													blockInfo[outBlock].SetData(xChar,yChar,true,false,false,true,false,(short)c,(short)PaletteOffset, checkHasTrans(c));
													goto	dontDrawCharacter;
												case	blockType.FlippedXRotated:   //		 rep  flpX flpY  rot   trans		
													blockInfo[outBlock].SetData(xChar,yChar,true,true,false,true,false,(short)c,(short)PaletteOffset, checkHasTrans(c));
													goto	dontDrawCharacter;
												case	blockType.FlippedYRotated:  //		 rep  flpX flpY  rot   trans		
													blockInfo[outBlock].SetData(xChar,yChar,true,false,true,true,false,(short)c,(short)PaletteOffset, checkHasTrans(c));
													goto	dontDrawCharacter;
												case	blockType.FlippedXYRotated: //		 rep  flpX flpY  rot   trans
													blockInfo[outBlock].SetData(xChar,yChar,true,true,true,true,false,(short)c,(short)PaletteOffset, checkHasTrans(c));
													goto	dontDrawCharacter;
												case	blockType.Transparent:      //		 rep  flpX  flpY  rot  trans
													blockInfo[outBlock].SetData(xChar,yChar,false,false,false,false,true,(short)c,(short)PaletteOffset, checkHasTrans(c));
													goto	dontDrawCharacter;
											}
										}
									}	
									// copy character over as its not in any of the characters									
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
									blockInfo[outBlock].SetData(xChar,yChar,false,false,false,false,false,(short)outChar,(short)PaletteOffset, checkHasTrans(outChar));
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
									
	
dontDrawCharacter:							;
								}
							}
							if(outType == outputData.Blocks)
							{ 
								BlocksLable.Text	=	"Blocks (" + outBlock.ToString() +")";
							}
							else
							{
								BlocksLable.Text	=	"Objects (" + outBlock.ToString() +")";
							}
							outBlock++;
			
dontDraw:					;
						}					
					this.Invalidate(true);
					this.Update();		

					}
				}
			}	
			tranpearntChars	=	0;
			SortedIndex	=	0;
			if(SettingsPanel.sortTransparent.Checked==true)
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
				dest.X			=	outXBlock*gridXSize;
				dest.Y			=	outYBlock*gridYSize;
				dest.Width		=	gridXSize;
				dest.Height		=	gridYSize;

				charsToBlocks(ref blocksPanel, dest, b);

				if(outType == outputData.Blocks)
				{
					BlocksLable.Text	=	"Blocks (" + b.ToString() +")";
				}
				else
				{
					BlocksLable.Text	=	"Objects (" + b.ToString() +")";
				}
				outXBlock++;
				if(outXBlock>=blocksAcross)
				{
					outXBlock	=	0;
					outYBlock++;
				}
			}
			if(outType == outputData.Blocks)
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
			charactersDisplay.Invalidate(true);
			charactersDisplay.Update();	
			blocksDisplay.Invalidate(true);
			blocksDisplay.Update();			
			this.toolStripProgressBar1.Minimum	=	0;
			this.toolStripProgressBar1.Maximum	=	0;
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// Copy the bits bitmap (bitsBitmap) to a viewable bitmap
		//
		//-------------------------------------------------------------------------------------------------------------------

		private		void	charsToBlocks(ref Bitmap destBitmap, Rectangle destRegion,int currentBlock)
		{
			Color	pixelColor	=	new	Color();
			for(int chr=0;chr< (gridXSize/ outSize) * (gridYSize/ outSize); chr++)
			{ 
				for(int y=0;y< outSize; y++)
				{									
					for(int x=0;x< outSize; x++)
					{
						try
						{ 
							if(blockInfo[currentBlock].infos[chr].flippedX == false && blockInfo[currentBlock].infos[chr].flippedY == false && blockInfo[currentBlock].infos[chr].rotated == false)
							{
								pixelColor	=	SetFromPalette(charData[sortIndexs[blockInfo[currentBlock].infos[chr].originalId]].GetPixel(x,y));
							}
							else if(blockInfo[currentBlock].infos[chr].flippedX == true && blockInfo[currentBlock].infos[chr].flippedY == false && blockInfo[currentBlock].infos[chr].rotated == false)
							{
								pixelColor	=	SetFromPalette(charData[sortIndexs[blockInfo[currentBlock].infos[chr].originalId]].GetPixel(7-x,y));
							}
							else if(blockInfo[currentBlock].infos[chr].flippedX == false && blockInfo[currentBlock].infos[chr].flippedY == true && blockInfo[currentBlock].infos[chr].rotated == false)
							{
								pixelColor	=	SetFromPalette(charData[sortIndexs[blockInfo[currentBlock].infos[chr].originalId]].GetPixel(x,7-y));
							}
							else if(blockInfo[currentBlock].infos[chr].flippedX == true && blockInfo[currentBlock].infos[chr].flippedY == true && blockInfo[currentBlock].infos[chr].rotated == false)
							{
								pixelColor	=	SetFromPalette(charData[sortIndexs[blockInfo[currentBlock].infos[chr].originalId]].GetPixel(7-x,7-y));
							}
							else if(blockInfo[currentBlock].infos[chr].flippedX == false && blockInfo[currentBlock].infos[chr].flippedY == false && blockInfo[currentBlock].infos[chr].rotated == true)
							{
								pixelColor	=	SetFromPalette(charData[sortIndexs[blockInfo[currentBlock].infos[chr].originalId]].GetPixel(7-y,x));
							}
							else if(blockInfo[currentBlock].infos[chr].flippedX == true && blockInfo[currentBlock].infos[chr].flippedY == false && blockInfo[currentBlock].infos[chr].rotated == true)
							{
								pixelColor	=	SetFromPalette(charData[sortIndexs[blockInfo[currentBlock].infos[chr].originalId]].GetPixel(y,x));
							}
							else if(blockInfo[currentBlock].infos[chr].flippedX == false && blockInfo[currentBlock].infos[chr].flippedY == true && blockInfo[currentBlock].infos[chr].rotated == true)
							{
								pixelColor	=	SetFromPalette(charData[sortIndexs[blockInfo[currentBlock].infos[chr].originalId]].GetPixel(7-y,7-x));
							}
							else if(blockInfo[currentBlock].infos[chr].flippedX == true && blockInfo[currentBlock].infos[chr].flippedY == true && blockInfo[currentBlock].infos[chr].rotated == true)
							{
								pixelColor	=	SetFromPalette(charData[sortIndexs[blockInfo[currentBlock].infos[chr].originalId]].GetPixel(y,7-x));
							}
							destBitmap.SetPixel(destRegion.X+(blockInfo[currentBlock].infos[chr].xPos*8)+x,destRegion.Y+(blockInfo[currentBlock].infos[chr].yPos*8)+y,pixelColor);
						}
						finally
                        {

                        }
					}
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
					if(tempData[s].GetPixel(x,y) == (short)thePalette.transIndex)
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
					if(blockData[s].GetPixel(x,y) == (short)thePalette.transIndex)
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
							
							thisIndex	=	thePalette.closestColor(SetFromPalette(blockData[outBlock].GetPixel(x+(xCuts*outSize),y+(yCuts*outSize))),(short) AveragingIndex);
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
			// does it have any trans pixels
			if(SettingsPanel.sortTransparent.Checked==true)
			{
				for(int y=0;y<outSize;y++)
				{ 
					for(int x=0;x<outSize;x++)
					{
						if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) == (short)thePalette.transIndex)
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
						if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) != (short)thePalette.transIndex)
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
						//goto	flippedXCheck;
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
			if(samePixels>=pixelClose && hasTrans==false)
			{ 
				return	blockType.Repeated;
			}
//flippedXCheck:		
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
							//goto	flippedYCheck;
						}
					}
				}
				if(samePixels>=pixelClose)
				{ 
					return	blockType.FlippedX;
				}				
			}
//flippedYCheck:	
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
							//goto	flippedXYCheck;
						}
					}
				}
				if(samePixels>=pixelClose)
				{ 
					return	blockType.FlippedY;
				}
				//return	blockType.FlippedY;
			}
//flippedXYCheck:
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
							//goto	rotatedCheck;
						}
					}
				}
				if(samePixels>=pixelClose)
				{ 
					return	blockType.FlippedXY;
				}
				//return	blockType.FlippedXY;
			}
//rotatedCheck:		
			samePixels	=	outSize*outSize;
			if(SettingsPanel.rotations.Checked==false)
			{ 
				for(int y=0;y<outSize;y++)
				{ 
					for(int x=0;x<outSize;x++)
					{
						if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) != tempData[character].GetPixel((outSize-1)-y,x))
						{
							samePixels--;
							//goto	flippedRotatedXCheck;
						}
					}
				}
				if(samePixels>=pixelClose)
				{ 
					return	blockType.Rotated;
				}
				//return	blockType.Rotated;
//flippedRotatedXCheck:		
				samePixels	=	outSize*outSize;
				if(SettingsPanel.mirrorX.Checked==false)
				{
					for(int y=0;y<outSize;y++)
					{ 
						for(int x=0;x<outSize;x++)
						{
							if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) != tempData[character].GetPixel(y,x))	//x flip
							{
								samePixels--;
							//goto	flippedRotatedYCheck;
							}
						}
					}
					if(samePixels>=pixelClose)
					{ 
						return	blockType.FlippedXRotated;
					}
				}
//flippedRotatedYCheck:	
				samePixels	=	outSize*outSize;
				if(SettingsPanel.mirrorY.Checked==false)
				{
					for(int y=0;y<outSize;y++)
					{ 
						for(int x=0;x<outSize;x++)
						{
							if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) != tempData[character].GetPixel((outSize-1)-y,(outSize-1)-x))		// y flip
							{
								samePixels--;
								//goto	flippedRotatedXYCheck;
							}
						}
					}
					if(samePixels>=pixelClose)
					{ 
						return	blockType.FlippedYRotated;
					}
				}
//flippedRotatedXYCheck:		
				samePixels	=	outSize*outSize;
				if(SettingsPanel.mirrorY.Checked==false && SettingsPanel.mirrorX.Checked==false)
				{
					for(int y=0;y<outSize;y++)
					{ 
						for(int x=0;x<outSize;x++)
						{
							if(blockData[outBlock].GetPixel(x+xOffset,y+yOffset) != tempData[character].GetPixel(y,(outSize-1)-x))	// xy flip
							{
								samePixels--;
								//goto	blockIsOriginal;
							}
						}
					}
					if(samePixels>=pixelClose)
					{ 
						return	blockType.FlippedXYRotated;
					}
				}
			}
//blockIsOriginal:
			return	blockType.Original;
		}

		//-------------------------------------------------------------------------------------------------------------------
		//
		// Copy an area from one bitmap to a bits bitmap (bitsBitmap)
		//
		//-------------------------------------------------------------------------------------------------------------------

		private void CopyRegionIntoBlock(Bitmap srcBitmap, Rectangle srcRegion,ref bitsBitmap outBlock)
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

			for(int	y=0;y<srcRegion.Height;y++)
			{		
				
				for(int	x=0;x<srcRegion.Width;x++)
				{
					outBlock.SetPixel(x,y,thePalette.closestColor(srcBitmap.GetPixel(srcRegion.X+x,srcRegion.Y+y),-1));
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
			switch(thePalette.paletteSetting)
			{ 
				case	Palette.PaletteMapping.mapped256:
					return Color.FromArgb(255,thePalette.SpecNext256[theIndex,0],thePalette.SpecNext256[theIndex,1],thePalette.SpecNext256[theIndex,2]);
				case	Palette.PaletteMapping.mapped512:
					return Color.FromArgb(255,thePalette.SpecNext512[theIndex,0],thePalette.SpecNext512[theIndex,1],thePalette.SpecNext512[theIndex,2]);
				case	Palette.PaletteMapping.mappedCustom:
					return	Color.FromArgb(255,thePalette.loadedPalette[theIndex,0],thePalette.loadedPalette[theIndex,1],thePalette.loadedPalette[theIndex,2]);
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
			if(blockInfo[0]==null)
			{				
				MessageBox.Show("You need to remap the graphics before you can output!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			int	numColours			=	thePalette.loadedColourCount;
			
			outputFilesDialog.FileName		=	projectName;// + ".asm";
			outputFilesDialog.Filter		=	"Machine Code (*.asm)|*.asm|Basic (*.bas)|*.bas|All Files (*.*)|*.*";
			outputFilesDialog.FilterIndex		=	outputFilterIndex;
			outputFilesDialog.RestoreDirectory	= false;	
			writeType	outputFileType		=	writeType.Assember;	
			int			xOffset			=	0;
			int			yOffset			=	0;
			byte		writeByte		=	0;
			int			lineNumber		=	10000;
			int			lineStep		=	5;
			string		lableString		=	"Sprites";
			string		tilesSave;
			switch (SettingsPanel.centerPosition)
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
			if(outputFilesDialog.ShowDialog() == DialogResult.OK)
			{
				
				outputFilterIndex	=		outputFilesDialog.FilterIndex;
				 // Define date to be displayed.
				DateTime todaysDate		=	DateTime.Now;
				string	lableNames		=	Regex.Replace(projectName,@"\s+", "");
				if(Path.HasExtension(outputFilesDialog.FileName)==false)
				{
					outPath			=	outputFilesDialog.FileName + ".asm";
					outputFileType		=	writeType.Assember;
				}
				else
				{
					if(Path.GetExtension(outputFilesDialog.FileName)==".bas")
					{
						outPath			=	outputFilesDialog.FileName;
						outputFileType		=	writeType.Basic;
					}					
					else
					{					
						outPath			=	Path.ChangeExtension(outputFilesDialog.FileName,"asm");
						outputFileType		=	writeType.Assember;
					}
				}
				if(SettingsPanel.binaryOut.Checked==true)
				{ 
					binPath			=	Path.ChangeExtension(outputFilesDialog.FileName,"bin");
					tilesPath		=	Path.ChangeExtension(outputFilesDialog.FileName,"til");
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
								if(SettingsPanel.comments.SelectedIndex==(int)comments.fullComments)
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
							if(SettingsPanel.comments.SelectedIndex==(int)comments.fullComments)
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
							if(SettingsPanel.comments.SelectedIndex==(int)comments.fullComments)
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
					if(SettingsPanel.FourBit.Checked==true  && outType==outputData.Sprites)
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
						// write the pixel data to the sprites
					if(SettingsPanel.binaryOut.Checked==true)
					{ 
						using(BinaryWriter binFile	=	new BinaryWriter(File.Open(binPath, FileMode.OpenOrCreate)))
						{ 
							for(int s=0;s<outChar;s++)
							{
								for(int y=0;y<charData[s].Height;y++)
								{									
									for(int x=0;x<charData[s].Width;x++)
									{	
										if(SettingsPanel.FourBit.Checked==true || outType==outputData.Blocks)
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
					}
					else
					{						
						for(int s=0;s<outChar;s++)
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
									if(SettingsPanel.FourBit.Checked==true || outType==outputData.Blocks)
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
						if(SettingsPanel.comments.SelectedIndex==(int)comments.fullComments && outType==outputData.Sprites)
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
						
					}
					else if(outType==outputData.Blocks==true)
					{
						outputFile.WriteLine(lineNumber.ToString()  + "\tLET\t"+  lableNames + lableString + "Width = " +  blockInfo[0].Width.ToString());								
						lineNumber	+=	lineStep;
						outputFile.WriteLine(lineNumber.ToString()  + "\tLET\t"+  lableNames + lableString + "Height = " +  blockInfo[0].Width.ToString());								
						lineNumber	+=	lineStep;
					}	
					int FrameCounter		=	0;
					if(SettingsPanel.binaryOut.Checked==true && SettingsPanel.binaryBlocks.Checked==true)
					{ 
						using(BinaryWriter tileFile	=	new BinaryWriter(File.Open(tilesPath, FileMode.OpenOrCreate)))
						{ 
							for(int s=0;s<outBlock;s++)
							{				
								if(outType==outputData.Sprites)
								{ 
									tileFile.Write(blockInfo[s].Width);
									tileFile.Write(blockInfo[s].Height.ToString() + ",\t");
								}
								for(int y=0;y<blockInfo[s].Height;y++)
								{
									for(int x=0;x<blockInfo[s].Width;x++)
									{				
										if(outType==outputData.Sprites)
										{ 
											tileFile.Write((xOffset+(blockInfo[s].GetXPos(x,y)*outSize)));
											tileFile.Write((yOffset+(blockInfo[s].GetYpos(x,y)*outSize)));
											
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
												if((blockInfo[s].GetId(x,y)&1)==1)
												{				
													writeByte = (byte)(writeByte | 64);
												}
											}
											tileFile.Write(writeByte);											
											tileFile.Write(blockInfo[s].GetId(x,y));												
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
											//writeByte	=	(byte)sortIndexs[blockInfo[s].GetId(x,y)];
											//tileFile.Write( writeByte );	
										}								
									}					
								}
							}

						}
					}
					else
					{ 
						
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
										if(SettingsPanel.FourBit.Checked==true)
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
										if(outputFileType==writeType.Basic)						
										{
											outputFile.WriteLine(lineNumber.ToString()  + "\tLET\t"+  lableNames + "Tile" + s.ToString() +"Index"+FrameCounter.ToString()+" = " +  sortIndexs[blockInfo[s].GetId(x,y)].ToString());								
											lineNumber	+=	lineStep;	
											FrameCounter++;
											outputFile.WriteLine(lineNumber.ToString()  + "\tREM");								
											lineNumber	+=	lineStep;
										}
										else
										{ 
											outputFile.Write( sortIndexs[blockInfo[s].GetId(x,y)].ToString());		
											if(x<(blockInfo[s].Width)-1)
											{
												outputFile.Write(",\t");
											}
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
							}
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
				
				int	yPos	=	0;
				int	xPos	=	0;
				// now output a blocks file BMP 
				if(outType==outputData.Blocks)
				{ 
					int	blocksAcross		=	int.Parse(SettingsPanel.tilesAcross.Text);
					int	blocksDown		=	(int)Math.Round((double)outBlock/blocksAcross)+1;

					string	blocksOutPath		=	Path.GetDirectoryName(outputFilesDialog.FileName);						
					string	blocksOutFilename	=	Path.GetFileNameWithoutExtension(outputFilesDialog.FileName);
					if(SettingsPanel.blocksOut.Checked==true)
					{ 
						Bitmap	outBlocks	=	new	Bitmap(gridXSize*blocksAcross,gridYSize*blocksDown,PixelFormat.Format24bppRgb);
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
								for(int	y=0;y<gridYSize;y++)
								{		
									for(int	x=0;x<gridXSize;x++)
									{									
										outBlocks.SetPixel(x+(xPos*gridXSize),yPos+y,SetFromPalette(blockData[b].GetPixel(x,y)));
									}
								}	
								xPos++;
								if(xPos>=blocksAcross)
								{ 
									xPos	=	0;
									yPos	+=	gridYSize;		
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
						int	Down	=	(int)Math.Round((double)outChar/Across);
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
						using(BinaryWriter reverseFile	=	new BinaryWriter(File.Open(Path.ChangeExtension(outputFilesDialog.FileName,"blk"), FileMode.OpenOrCreate)))
						{ 
							
							byte	xChars			=	(byte)(gridXSize/outSize);
							byte	yChars			=	(byte)(gridYSize/outSize);
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
			SettingsPanel.StartPosition	=	FormStartPosition.CenterParent;
			if(SettingsPanel.ShowDialog()	==	DialogResult.OK)
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

		private void rebuildFromTilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(blocksView.Visible==false)
			{
				blocksView	=	new	imageWindow();
			}
			blocksView.MdiParent		=	this;
			blocksView.inputImage		=	new	Bitmap(20*gridXSize,12*gridYSize);
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
								blocksView.inputImage.SetPixel((x*8)+pixelX+(blocksX*gridXSize),(y*8)+pixelY+(blocksY*gridYSize),readColour);									
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
			Bitmap	srcBitmap;
			foreach (imageWindow window in imageWindows)
			{
				if(window.inputImage!=null)
				{ 						
					window.inputImage.Dispose();
				}
				if(window!=null)
				{ 
					window.Close();
					window.Dispose();
				}
			}
			foreach (Bitmap bitMap in sourceImages)
			{
				if(bitMap!=null)
				{ 
					bitMap.Dispose();
				}
			}
			imageWindows.Clear();
			sourceImages.Clear();
			foreach (String file in fullNames) 
			{				
				using (var fs = new System.IO.FileStream(file, System.IO.FileMode.Open))
				{
					var bmp	= new Bitmap(fs);
					srcBitmap	= new Bitmap(bmp.Width,bmp.Height);
					srcBitmap	= (Bitmap) bmp.Clone();
				}					
				if(IsSupported(new Bitmap(srcBitmap))==true)
				{
					sourceImages.Add(new Bitmap(srcBitmap));
					imageWindows.Add(new imageWindow { MdiParent = this});
				}
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
					blocksWindow.copyImage(blocksPanel,gridXSize,gridYSize);
				//	blocksWindow.loadImage(fullNames[index-1],gridXSize,gridYSize);
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
					charsWindow.copyImage(charsPanel,8,8);
				//	blocksWindow.loadImage(fullNames[index-1],gridXSize,gridYSize);
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
	}
}