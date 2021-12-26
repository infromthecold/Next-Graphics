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
using NextGraphics.Exporting;
using NextGraphics.Exporting.Common;
using System.Threading.Tasks;

namespace NextGraphics
{
	public partial class Main : Form, RemapCallbacks, ExportCallbacks
	{
		//-------------------------------------------------------------------------------------------------------------------
		//
		// enumerations
		//
		//-------------------------------------------------------------------------------------------------------------------

		public enum BlockType
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

		public MainModel Model { get; set; }
		public Exporter Exporter { get; set; }

		public RadioButton selectedRadio;
		public int objectSize = 16;
		private const int MAX_BLOCKS = 256;
		private const int MAX_CHAR_SIZE = 256;
		private const int MAX_IMAGES = 64;
		public List<imageWindow> imageWindows = new List<imageWindow>();
#if PROPRIETARY
		public	parallaxTool		parallaxWindow		=	new	parallaxTool();
#endif
		public imageWindow blocksWindow = new imageWindow();
		public infoWindow informationWindow = new infoWindow();
		public imageWindow charsWindow = new imageWindow();

		public IndexedBitmap[] Blocks = new IndexedBitmap[MAX_BLOCKS];
		public SpriteInfo[] Sprites = new SpriteInfo[MAX_BLOCKS];
		public IndexedBitmap[] Chars = new IndexedBitmap[MAX_CHAR_SIZE + 1];
		private IndexedBitmap[] TempData = new IndexedBitmap[MAX_CHAR_SIZE + 1];
		public int[] SortIndexes = new int[MAX_CHAR_SIZE + 1];
		private int outBlock = 0;
		private int outChar = 0;
		private string projectPath = "";
		private string outPath = "";
		private string binPath = "";
		private string tilesPath = "";
		private bool reverseByes = false;
		private bool PaletteSet = false;
		public PaletteForm PaletteForm = new PaletteForm();
		private SettingsForm SettingsPanel = new SettingsForm();
		public imageWindow blocksView = new imageWindow();
		private readonly NumberFormatInfo fmt = new NumberFormatInfo();

		private palOffset offsetPanel = new palOffset();
		private string ParentDirectory = "f:/";
		private Font numberFont = new Font("Arial", 6, FontStyle.Bold);
		private SolidBrush numberBrush = new SolidBrush(Color.White);
		SaveFileDialog projectSaveDialog = new SaveFileDialog();
		OpenFileDialog openProjectDialog = new OpenFileDialog();
		OpenFileDialog batchProjectDialog = new OpenFileDialog();
		SaveFileDialog outputFilesDialog = new SaveFileDialog();
		OpenFileDialog addImagesDialog = new OpenFileDialog();

		rebuild rebuildDialog = new rebuild();

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

			var exportParameters = new ExportParameters();
			exportParameters.RemapCallbacks = this;
			exportParameters.ExportCallbacks = this;

			Model = new MainModel();
			Exporter = new Exporter(Model, exportParameters);

			InitializeComponent();
			
			ClearBitmap(Model.BlocksBitmap);
			blocksDisplay.Image = Model.BlocksBitmap;
			blocksDisplay.Height = Model.BlocksBitmap.Height;
			blocksDisplay.Width = Model.BlocksBitmap.Width;
#if DEBUG_WINDOW
			DEBUG_WINDOW				=	new	DEBUGFORM();
			DEBUG_WINDOW.Show();
#endif

			ClearBitmap(Model.CharsBitmap);
			charactersDisplay.Image = Model.CharsBitmap;
			this.toolStripProgressBar1.Minimum = 0;
			this.toolStripProgressBar1.Maximum = 0;
			this.listBox1.Items.Add(" " + Model.Name);
			reverseByes = BitConverter.IsLittleEndian;
			Model.BlocksAccross = (int)Math.Floor((float)blocksDisplay.Width / Model.GridWidth);
			blocksDisplay.Invalidate();
			blocksDisplay.Refresh();
			SettingsPanel.comments.SelectedIndex = 1;
			setForm();

			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
			string sep = string.Empty;
			foreach (var c in codecs)
			{
				string codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
				addImagesDialog.Filter = String.Format("{0}{1}{2} ({3})|{3}", addImagesDialog.Filter, sep, codecName, c.FilenameExtension);
				sep = "|";
			}
			addImagesDialog.Filter = String.Format("{0}{1}{2} ({3})|{3}", addImagesDialog.Filter, sep, "All Files", "*.*");
#if PROPRIETARY
			parallaxWindow.thePalette		=	thePalette;
			parallaxWindow.main				=	this;
			fmt.NegativeSign				=	"-";
#endif
		}
		
		#region RemapCallbacks

		public bool OnRemapShowCharacterDebugData() => false;

		public void OnRemapStarted()
		{
			ClearBitmap(Model.BlocksBitmap);
			ClearBitmap(Model.CharsBitmap);
#if DEBUG_WINDOW
			clearPanels(DEBUG_WINDOW.DEBUG_IMAGE);
#endif

			Invoke(new Action(() =>
			{
				blocksDisplay.Invalidate(true);
				blocksDisplay.Update();

				charactersDisplay.Invalidate(true);
				charactersDisplay.Update();

				toolStripProgressBar1.Minimum = 0;
				toolStripProgressBar1.Maximum = 10000;
			}));
		}

		public void OnRemapUpdated()
		{
			Invoke(new Action(() =>
			{
				blocksDisplay.Invalidate(true);
				blocksDisplay.Update();
			}));
		}

		public void OnRemapCompleted(bool success)
		{
			Invoke(new Action(() =>
			{
				charactersDisplay.Invalidate(true);
				charactersDisplay.Update();

				blocksDisplay.Invalidate(true);
				blocksDisplay.Update();

				toolStripProgressBar1.Minimum = 0;
				toolStripProgressBar1.Maximum = 0;

#if DEBUG_WINDOW
				DEBUG_WINDOW.Invalidate(true);
				DEBUG_WINDOW.Update();
#endif
			}));
		}

		public void OnRemapDisplayChar(Rectangle frame, IndexedBitmap bitmap)
		{
			Invoke(new Action(() =>
			{
				blockToDisplay(Model.CharsBitmap, frame, bitmap);
#if DEBUG_WINDOW
				DEBUGToDisplay(frame, bitmap);
#endif
			}));
		}

		public void OnRemapDisplayBlock(Rectangle frame, IndexedBitmap bitmap)
		{
			Invoke(new Action(() =>
			{
				blockToDisplay(Model.BlocksBitmap, frame, bitmap);
			}));
		}

		public void OnRemapDisplayCharactersCount(int count, int transparentCount)
		{
			Invoke(new Action(() =>
			{
				if (Model.OutputType == OutputType.Tiles)
				{
					SpritesLable.Text = $"Characters ({count}), Transparent ({transparentCount})";
				}
				else
				{
					SpritesLable.Text = $"Sprites ({count})";
				}
			}));
		}

		public void OnRemapDisplayBlocksCount(int count)
		{
			Invoke(new Action(() =>
			{
				if (Model.OutputType == OutputType.Tiles)
				{
					BlocksLable.Text = $"Blocks ({count})";
				}
				else
				{
					BlocksLable.Text = $"Objects ({count})";
				}
			}));
		}

		public void OnRemapWarning(string message)
		{
			Invoke(new Action(() =>
			{
				MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}));
		}

		public void OnRemapDebug(string message)
		{
			//Console.Write(message);
		}

		#endregion

		#region ExportCallbacks

		public byte OnExportPaletteOffsetMapper(byte proposed)
		{
			offsetPanel.paletteOffset.Value = (int)proposed;

			if (offsetPanel.ShowDialog() == DialogResult.OK)
			{
				return (byte)offsetPanel.paletteOffset.Value;
			}

			return proposed;
		}

		public byte OnExportFourBitColourConverter(byte proposed)
		{
			return proposed;
		}

		#endregion

		//-------------------------------------------------------------------------------------------------------------------
		//
		// Clear the bitmap
		//
		//-------------------------------------------------------------------------------------------------------------------

		private void	ClearBitmap(Bitmap thisBitmap)
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

			ClearBitmap(Model.BlocksBitmap);
			blocksDisplay.Image = Model.BlocksBitmap;
			blocksDisplay.Height = Model.BlocksBitmap.Height;
			blocksDisplay.Width = Model.BlocksBitmap.Width;
			blocksDisplay.Invalidate(true);
			blocksDisplay.Refresh();
			
			ClearBitmap(Model.CharsBitmap);
			charactersDisplay.Image = Model.CharsBitmap;
			charactersDisplay.Invalidate(true);
			charactersDisplay.Refresh();

			PaletteSet = false;
			
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
			Model.BlocksAccross = (int)Math.Floor((float)blocksDisplay.Width / Model.GridWidth);

			PaletteSet = false;

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

			RunLongOperation(() => Exporter.Remap());
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
		// Copy the bits bitmap (bitsBitmap) to a viewable bitmap
		//
		//-------------------------------------------------------------------------------------------------------------------

		private		void	blockToDisplay(Bitmap destBitmap, Rectangle destRegion, IndexedBitmap inBlock)
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
			for (int y = 0; y < (blocksDisplay.Image.Height / Model.GridHeight)+1; ++y)
			{
				g.DrawLine(pen, 0, y * Model.GridHeight, blocksDisplay.Image.Width, y * Model.GridHeight);
			}			
			// verticle lines
			for (int x = 0; x < (blocksDisplay.Image.Width/Model.GridWidth)+1; ++x)
			{
				g.DrawLine(pen, x * Model.GridWidth, 0, x * Model.GridWidth, blocksDisplay.Image.Height);
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
			if(Sprites[0]==null)
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
			var parameters = new ExportParameters();
			parameters.RemapCallbacks = this;
			parameters.ExportCallbacks = this;

#if PROPRIETARY
			parameters.FourBitColourConverter = (colour) => (byte)this.parallaxWindow.getColour(colourByte);
#endif

			//writeType	outputFileType		=	writeType.Assember;	
			int imageXOffset			=	0;
			int		imageYOffset			=	0;
			byte		colourByte		=	0;
			byte		writeByte		=	0;
			int		lineNumber		=	10000;
			int		lineStep		=	5;
			string		BlockLabelName		=	"Sprites";
			string		tilesSave;
			int		binSize			=	0;
			int	numColours			=	Model.Palette.UsedCount;
			int	startColour			=	Model.Palette.StartIndex;
			switch (Model.CenterPosition)
			{
				case	0:						
					imageXOffset			=	-(Model.GridWidth/2);
					imageYOffset			=	-(Model.GridHeight/2);
				break;					
				case	1:						
					imageXOffset			=	0;
					imageYOffset			=	-(Model.GridHeight/2);
				break;									
				case	2:						
					imageXOffset			=	(Model.GridWidth/2);
					imageYOffset			=	-(Model.GridHeight/2);
				break;
				case	3:						
					imageXOffset			=	-(Model.GridWidth/2);
					imageYOffset			=	0;
				break;					
				case	4:						
					imageXOffset			=	0;
					imageYOffset			=	0;
				break;									
				case	5:						
					imageXOffset			=	(Model.GridWidth/2);
					imageYOffset			=	0;
				break;
				case	6:						
					imageXOffset			=	-(Model.GridWidth/2);
					imageYOffset			=	(Model.GridHeight/2);
				break;					
				case	7:						
					imageXOffset			=	0;
					imageYOffset			=	(Model.GridHeight/2);
				break;									
				case	8:						
					imageXOffset			=	(Model.GridWidth/2);
					imageYOffset			=	(Model.GridHeight/2);
				break;
			}
			 	
				// Define date to be displayed.
			DateTime todaysDate		=	DateTime.Now;
			string	LabelPrefix		=	Regex.Replace(Model.Name,@"\s+", "");
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
				outputFile.WriteLine((LabelPrefix + "_Colours").ToUpper() +":\t\tequ\t" + numColours.ToString() + "\r\n");

				if(Model.Palette.Type == PaletteType.Custom)
				{ 
					outputFile.WriteLine(LabelPrefix + "Palette:");
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
					BlockLabelName	=	"Sprite";
				}
				else
				{
					BlockLabelName	=	"Tile";
				}
				outputFile.Write("\r\n");
				if(SettingsPanel.FourBit.Checked==true  && Model.OutputType == OutputType.Sprites)
				{ 
					outputFile.WriteLine((LabelPrefix + "_" + BlockLabelName).ToUpper() + "_SIZE:\t\tequ\t128\r\n");
				}
				else if(Model.OutputType == OutputType.Sprites)
				{
					outputFile.WriteLine((LabelPrefix + "_" + BlockLabelName).ToUpper() + "_SIZE:\t\tequ\t256\r\n");
				}
				else
				{
					outputFile.WriteLine((LabelPrefix + "_" + BlockLabelName).ToUpper() + "_SIZE:\t\tequ\t32\r\n");
				}

				outputFile.WriteLine((LabelPrefix + "_" + BlockLabelName).ToUpper() + "S:\t\tequ\t" + outChar.ToString() +"\r\n");

				// write the pixel data to the sprites
				if(SettingsPanel.binaryOut.Checked==true)
				{ 
					binSize	=	0;
					using(BinaryWriter binFile	=	new BinaryWriter(File.Open(binPath, FileMode.OpenOrCreate)))
					{ 
						for(int s=0;s<outChar;s++)
						{
							if(Chars[s].Transparent == true && Model.OutputType == OutputType.Sprites && SettingsPanel.Transparent.Checked == false)
							{
								continue;
							}
							for(int y=0;y<Chars[s].Height;y++)
							{									
								for(int x=0;x<Chars[s].Width;x++)
								{	
									if(SettingsPanel.FourBit.Checked==true || Model.OutputType == OutputType.Tiles)
									{ 
										colourByte		=	(byte)(Chars[s].GetPixel(x,y)&0x0f);
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
										binFile.Write(Chars[s].GetPixel(x,y));
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
						outputFile.WriteLine(LabelPrefix + BlockLabelName + s.ToString() +":");
						for(int y=0;y<Chars[s].Height;y++)
						{
							outputFile.Write("\t\t\t\t.db\t");

							for(int x=0;x<Chars[s].Width;x++)
							{	
								if(SettingsPanel.FourBit.Checked==true || Model.OutputType == OutputType.Tiles)
								{ 
									if((x&1)==0)
									{ 
										writeByte	=	(byte)((Chars[s].GetPixel(x,y)&0x0f)<<4);
									}
									else
									{
										writeByte	=	(byte) (writeByte | (Chars[s].GetPixel(x,y)&0x0f));
										outputFile.Write("${0:x2}",writeByte);
										if(x<(Chars[s].Width-1))
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
									outputFile.Write("${0:x2}",Chars[s].GetPixel(x,y));
									if(x<(Chars[s].Width-1))
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
					outputFile.WriteLine(LabelPrefix + BlockLabelName + "Width:\tequ\t" + Sprites[0].Width.ToString() );
					outputFile.WriteLine(LabelPrefix + BlockLabelName + "Height:\tequ\t" + Sprites[0].Height.ToString() );
				}		
	
				int		spriteCount		=	0;
				int	idReduction	=	0;
				if(Chars[0].Transparent == true && Model.OutputType == OutputType.Sprites && SettingsPanel.Transparent.Checked == false)
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
								spriteCount	=	Sprites[s].Width*Sprites[s].Height;

								for(int y=0;y<Sprites[s].Height;y++)
								{
									for(int x=0;x<Sprites[s].Width;x++)
									{
										if(Sprites[s].GetTransparent(x,y)==true)
										{
											spriteCount--;
										}
									}
								}
								tileFile.Write(spriteCount);
							}
							for(int y=0;y<Sprites[s].Height;y++)
							{
								for(int x=0;x<Sprites[s].Width;x++)
								{											
									if(Model.OutputType == OutputType.Sprites)
									{
										if(Sprites[s].GetTransparent(x,y)==true)
										{
											continue;
										}
										tileFile.Write(Sprites[s].OffsetX+(imageXOffset+(Sprites[s].GetXPos(x,y)*objectSize)));
										tileFile.Write(Sprites[s].OffsetY+(imageYOffset+(Sprites[s].GetYpos(x,y)*objectSize)));
											
										writeByte	=	0;
										if(Sprites[s].GetPaletteOffset(x,y)!=0)
										{
											writeByte = (byte)Sprites[s].GetPaletteOffset(x,y);
										}
										if(Sprites[s].GetFlippedX(x,y)==true)
										{
											writeByte = (byte)(writeByte | 8);
										}
										if(Sprites[s].GetFlippedY(x,y)==true)
										{
											writeByte = (byte)(writeByte | 4);
										}
										if(Sprites[s].GetRotated(x,y)==true)
										{
											writeByte = (byte)(writeByte | 2);
										}					
										tileFile.Write(writeByte);											
										writeByte	=	0;								
										if(SettingsPanel.FourBit.Checked==true)
										{ 										
											writeByte = (byte)(writeByte | 128);
											if(((Sprites[s].GetId(x,y)-idReduction)&1)==1)
											//if(((blockInfo[s].GetId(x,y))&1)==1)
											{				
												writeByte = (byte)(writeByte | 64);
											}
											tileFile.Write(writeByte);											
											//tileFile.Write(blockInfo[s].GetId(x,y)-idReduction);																		
											tileFile.Write((Sprites[s].GetId(x,y)-idReduction)/2);
										}
										else
										{ 
											tileFile.Write(writeByte);											
											tileFile.Write(Sprites[s].GetId(x,y));												
										}
									}
									else
									{
										writeByte	=	(byte)SortIndexes[Sprites[s].GetId(x,y)];
										tileFile.Write( writeByte );	

										writeByte	=	0;									
										if(Sprites[s].GetPaletteOffset(x,y)!=0)
										{
											writeByte = (byte)Sprites[s].GetPaletteOffset(x,y);
										}
										if(Sprites[s].GetFlippedX(x,y)==true)
										{
											writeByte = (byte)(writeByte | 8);
										}
										if(Sprites[s].GetFlippedY(x,y)==true)
										{
											writeByte = (byte)(writeByte | 4);
										}
										if(Sprites[s].GetRotated(x,y)==true)
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
							Rectangle			collision = new Rectangle(Sprites[s].Left,Sprites[s].Top,Sprites[s].Right-Sprites[s].Left,Sprites[s].Bottom-Sprites[s].Top);
							outputFile.Write(LabelPrefix + "Collision" + s.ToString() +":");
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
							spriteCount	=	Sprites[s].Width*Sprites[s].Height;

							for(int y=0;y<Sprites[s].Height;y++)
							{
								for(int x=0;x<Sprites[s].Width;x++)
								{
									if(Sprites[s].GetTransparent(x,y)==true)
									{
										spriteCount--;
									}
								}
							}

							outputFile.Write(LabelPrefix + "Frame" + s.ToString() +":");
							outputFile.Write("\t\t.db\t");
							//outputFile.Write(blockInfo[s].Width.ToString() + ",");
							//outputFile.Write(blockInfo[s].Height.ToString() + ",\t");
							outputFile.Write(spriteCount.ToString() + ",\t");
						}
						else
						{		

							outputFile.Write(LabelPrefix + "Block" + s.ToString() +":");							
							outputFile.Write("\t\t.db\t");

						}	
						// adjust x y pos based on the centerPanel setting
						for(int y=0;y<Sprites[s].Height;y++)
						{

							int	writtenWidth		=	Sprites[s].Width;
							for(int x=0;x<Sprites[s].Width;x++)
							{				
								if(Model.OutputType == OutputType.Sprites)
								{ 
									if(Sprites[s].GetTransparent(x,y)==true)
									{
										writtenWidth--;
										continue;
									}									
									outputFile.Write((Sprites[s].OffsetX+(imageXOffset+(Sprites[s].GetXPos(x,y)*objectSize))).ToString() + ",");
									outputFile.Write((Sprites[s].OffsetY+(imageYOffset+(Sprites[s].GetYpos(x,y)*objectSize))).ToString() + ",");
									string		textFlips	=	"";
									writeByte	=	0;
									if(Sprites[s].GetPaletteOffset(x,y)!=0)
									{
										writeByte = (byte)Sprites[s].GetPaletteOffset(x,y);
									}
									if(Sprites[s].GetFlippedX(x,y)==true)
									{
										writeByte = (byte)(writeByte | 8);
										textFlips	=	"+XFLIP";

									}
									if(Sprites[s].GetFlippedY(x,y)==true)
									{
										writeByte = (byte)(writeByte | 4);
										textFlips	+=	"+YFLIP";
									}
									if(Sprites[s].GetRotated(x,y)==true)
									{
										writeByte = (byte)(writeByte | 2);
										textFlips	+=	"+ROT";
									}				
									if(SettingsPanel.textFlips.Checked==true)
									{
										outputFile.Write("\t"+LabelPrefix.ToUpper()+"_OFFSET"+textFlips + ",\t");
									}
									else
									{ 
										outputFile.Write("\t"+LabelPrefix.ToUpper()+"_OFFSET+"+writeByte.ToString() + ",\t");
									}
									writeByte	=	0;								
									if(SettingsPanel.FourBit.Checked==true)
									{ 										
										writeByte = (byte)(writeByte | 128);
										if(((Sprites[s].GetId(x,y)-idReduction)&1)==1)
										{				
											writeByte = (byte)(writeByte | 64);
										}
									}
									outputFile.Write(writeByte.ToString() + ",");

									if(SettingsPanel.FourBit.Checked==true)
									{ 
										outputFile.Write(((Sprites[s].GetId(x,y)-idReduction)/2).ToString());		
									}
									else
									{
										outputFile.Write(Sprites[s].GetId(x,y).ToString());	
									}
									if(x<(writtenWidth)-1)
									{
										outputFile.Write(",\t");
									}
								}
								else
								{
									writeByte	=	0;
									
									if(Sprites[s].GetPaletteOffset(x,y)!=0)
									{
										writeByte = (byte)Sprites[s].GetPaletteOffset(x,y);
									}
									if(Sprites[s].GetFlippedX(x,y)==true)
									{
										writeByte = (byte)(writeByte | 8);
									}
									if(Sprites[s].GetFlippedY(x,y)==true)
									{
										writeByte = (byte)(writeByte | 4);
									}
									if(Sprites[s].GetRotated(x,y)==true)
									{
										writeByte = (byte)(writeByte | 2);
									}	
									outputFile.Write(writeByte.ToString() + ",");
									writeByte	=	0;	
									outputFile.Write( SortIndexes[Sprites[s].GetId(x,y)].ToString());		
									if(x<(Sprites[s].Width)-1)
									{
										outputFile.Write(",\t");
									}
								}
								
							}									
							if(y<(Sprites[s].Height)-1)
							{
								if(Sprites[s].GetTransparent(Sprites[s].Width-1,y)==false || Model.OutputType == OutputType.Tiles)
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
					outputFile.Write(LabelPrefix + "Frames:\t\t.dw\t");
					for(int s=0;s<outBlock;s++)
					{
						outputFile.Write(LabelPrefix + "Frame" + s.ToString());
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
					outputFile.Write(LabelPrefix.ToUpper()+"_FILE_SIZE\tequ\t" + binSize.ToString());

					outputFile.Write("\r\n"+ LabelPrefix	+	"File:\t\t\tdw\t" + Model.Name.ToUpper()+"_FILE_SIZE\r\n");
					outputFile.Write("\t\t\tdb\tPATH,\"game/level1/" + LabelPrefix.ToLower() + ".bin\",0\r\n");

				}
			}
				
			int	yPos	=	0;
			int	xPos	=	0;
			// now output a blocks file BMP 
			if(Model.OutputType == OutputType.Tiles)
			{ 
				int	outBlocksAcross		=	int.Parse(SettingsPanel.tilesAcross.Text);
				int	outBlocksDown		=	(int)Math.Round((double)outBlock/outBlocksAcross)+1;

				string	blocksOutPath		=	Path.GetDirectoryName(outputFilesDialogFileName);						
				string	blocksOutFilename	=	Path.GetFileNameWithoutExtension(outputFilesDialogFileName);
				if(SettingsPanel.blocksOut.Checked==true)
				{ 
					Bitmap	outBlocks	=	new	Bitmap(Model.GridWidth*outBlocksAcross,Model.GridHeight*outBlocksDown,PixelFormat.Format24bppRgb);
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
							for(int	y=0;y<Model.GridHeight;y++)
							{		
								for(int	x=0;x<Model.GridWidth;x++)
								{									
									outBlocks.SetPixel(x+(xPos*Model.GridWidth),yPos+y,SetFromPalette(Blocks[b].GetPixel(x,y)));
								}
							}	
							xPos++;
							if(xPos>=outBlocksAcross)
							{ 
								xPos	=	0;
								yPos	+=	Model.GridHeight;		
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
									outChars.SetPixel(x+(xPos*8),yPos+y,SetFromPalette(Chars[b].GetPixel(x,y)));
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
							
						byte	xChars			=	(byte)(Model.GridWidth/objectSize);
						byte	yChars			=	(byte)(Model.GridHeight/objectSize);
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
									outInt	=	SortIndexes[Sprites[b].GetId(x,y)];
									if(Sprites[b].GetFlippedX(x,y)==true)
									{
										outInt	=	outInt | 1<<15;
									}
									if(Sprites[b].GetFlippedY(x,y)==true)
									{
										outInt	=	outInt | 1<<14;
									}
									if(Sprites[b].GetRotated(x,y)==true)
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
							
						byte	xChars			=	(byte)(Model.GridWidth/objectSize);
						byte	yChars			=	(byte)(Model.GridHeight/objectSize);
						int	outInt			=	0;														
						for(int b=0;b<outBlock;b++)
						{								
							for(int	y=0;y<yChars;y++)
							{		
								for(int	x=0;x<xChars;x++)
								{	
									outInt	=	SortIndexes[Sprites[b].GetId(x,y)];
									mapFile.Write((byte)outInt);	
									outInt = paletOffset;
									if(Sprites[b].GetFlippedX(x,y)==true)
									{
										outInt	=	outInt | 1<<3;
									}
									if(Sprites[b].GetFlippedY(x,y)==true)
									{
										outInt	=	outInt | 1<<2;
									}
									if(Sprites[b].GetRotated(x,y)==true)
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
									outChars.SetPixel(x+(xPos*16),yPos+y,SetFromPalette(Chars[b].GetPixel(x,y)));
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
						byte	xChars			=	(byte)(Model.GridWidth/objectSize);
						byte	yChars			=	(byte)(Model.GridHeight/objectSize);
						Bitmap	outSprite		=	new	Bitmap(Model.GridWidth,Model.GridHeight,PixelFormat.Format24bppRgb);
						{
							for(int	y=0;y<yChars;y++)
							{		
								for(int	x=0;x<xChars;x++)
								{		
										
									for(int	yp=0;yp<16;yp++)
									{		
										for(int	xp=0;xp<16;xp++)
										{
											
											if(Sprites[s].GetTransparent(x,y)==true)
											{
												outSprite.SetPixel(xp+(x*16),yp+(y*16),SetFromPalette(Model.Palette.TransparentIndex));
											}
											else
											{
												fx	=	xp;
												fy	=	yp;
												if(Sprites[s].GetFlippedX(x,y)==true)
												{
													fx	=	15-xp;
												}
												if(Sprites[s].GetFlippedY(x,y)==true)
												{
													fy	=	15-yp;
												}
												outSprite.SetPixel(xp+(x*16),yp+(y*16),SetFromPalette(Chars[Sprites[s].GetId(x,y)].GetPixel(fx,fy)));
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
			Model.OutputType			=	OutputType.Tiles;
			objectSize			=	8;
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
			objectSize			=	16;		
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

			blockHeightTextBox.Text	=	Model.GridHeight.ToString();	
			blockWidthTextBox.Text	=	Model.GridWidth.ToString();
			if(Model.OutputType == OutputType.Sprites)
			{
				spritesOut.Checked	=	true;
				blocksOut.Checked	=	false;	
				selectedRadio		=	spritesOut;
				objectSize			=	16;
				//charsPanel.Width	=	blocksAcross/outSize;
			}
			else //outputData.Blocks;
			{ 
				blocksOut.Checked	=	true;
				spritesOut.Checked	=	false;	
				selectedRadio		=	blocksOut;
				objectSize			=	8;
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
			if(int.TryParse(blockHeightTextBox.Text, out size))
			{
				Model.GridHeight = size;

				if (Model.OutputType == OutputType.Sprites)
				{
					if(Model.GridHeight<16)
					{
						Model.GridHeight	=	16;
					}
					else if (Model.GridHeight>320)
					{ 
						Model.GridHeight	=	320;
					}
					else
					{
						Model.GridHeight	=	(Model.GridHeight + 15) & ~0xF;						
					}
				}
				else
				{ 
					if(Model.GridHeight<8)
					{
						Model.GridHeight	=	8;
					}
					else if (Model.GridHeight>128)
					{ 
						Model.GridHeight	=	128;
					}
					else
					{
						Model.GridHeight	=	(Model.GridHeight + 7) & ~0x7;
					}
				}
				blockHeightTextBox.Text	=	Model.GridHeight.ToString();
			}
			else
			{				
				Model.GridHeight	=	32;
				blockHeightTextBox.Text	=	Model.GridHeight.ToString();
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
			if(int.TryParse(blockWidthTextBox.Text, out size))
			{
				Model.GridWidth = size;

				if (Model.OutputType == OutputType.Sprites)
				{
					if(Model.GridWidth<16)
					{
						Model.GridWidth	=	16;
					}
					else if (Model.GridWidth>320)
					{ 
						Model.GridWidth	=	320;
					}
					else
					{
						Model.GridWidth	=	(Model.GridWidth + 15) & ~0xF;						
					}
				}
				else
				{ 
					if(Model.GridWidth<8)
					{
						Model.GridWidth	=	8;
					}
					else if (Model.GridWidth>128)
					{ 
						Model.GridWidth	=	128;
					}
					else
					{
						Model.GridWidth	=	(Model.GridHeight + 7) & ~0x7;
					}
				}							
				blockWidthTextBox.Text	=	Model.GridWidth.ToString();
			}
			else
			{				
				Model.GridWidth	=	32;
				blockWidthTextBox.Text	=	Model.GridWidth.ToString();
			}
			Model.BlocksAccross	=	(int)Math.Floor((float)blocksDisplay.Width/Model.GridWidth);
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
			blocksView.inputImage		=	new	Bitmap(20*Model.GridWidth,12*Model.GridHeight);
			blocksView.srcPicture.Image	=	blocksView.inputImage;
			int	blocksX			=	0;
			int	blocksY			=	0;
			
			blocksView.Show();
			for(int s=0;s<outBlock;s++)		// do all the blocks
			{
				for(int y=0;y<Sprites[s].Height;y++)
				{
					for(int x=0;x<Sprites[s].Width;x++)
					{
						byte	tileId	=	(byte)SortIndexes[Sprites[s].GetId(x,y)];
						for(int pixelY=0;pixelY<8;pixelY++)
						{
							for(int pixelX=0;pixelX<8;pixelX++)
							{ 
								int	readX		=	pixelX;
								int	readY		=	pixelY;
								if(Sprites[s].GetFlippedX(x,y)==true)
								{
									readX		=	7-pixelX;
								}
								if(Sprites[s].GetFlippedY(x,y)==true)
								{
									readY		=	7-pixelY;
								}
								if(Sprites[s].GetRotated(x,y)==true)
								{
									int	temp	=	readX;
									readX		=	readY;
									readY		=	temp;									
									readX		=	7-readX;
								}
								Color	readColour	=	SetFromPalette(Chars[tileId].GetPixel(readX,readY));								
								blocksView.inputImage.SetPixel((x*8)+pixelX+(blocksX*Model.GridWidth),(y*8)+pixelY+(blocksY*Model.GridHeight),readColour);									
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
							imageWindows[index-1].loadImage(Model.Images[index-1].Filename,Model.GridWidth,Model.GridHeight);
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
					blocksWindow.copyImage(Model.BlocksBitmap, Model.GridWidth,Model.GridHeight,true);
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
						charsWindow.copyImage(Model.CharsBitmap,16,16,false);
					}
					else
					{
						charsWindow.copyImage(Model.CharsBitmap,8,8,false);
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
				RunLongOperation(() =>
				{
					foreach (String file in batchProjectDialog.FileNames)
					{
						setParentFolder(Path.GetFullPath(file));
						newProject();
						loadProject(file);

						Exporter.Remap();

						ouputFiles(ParentDirectory + "\\Output\\" + Model.Name.ToLower() + ".asm");
					}
				});
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
					for(int chr=0;chr< (Model.GridWidth/ objectSize) * (Model.GridHeight/ objectSize); chr++)
					{ 
						informationWindow.infoTextBox.AppendText("\t");
						if(Sprites[b].infos[chr].HasTransparent==true)
						{ 
							informationWindow.infoTextBox.SelectionColor = Color.Red; 							
							informationWindow.infoTextBox.AppendText(SortIndexes[Sprites[b].infos[chr].OriginalID].ToString() + ",");
						}
						else
						{
							informationWindow.infoTextBox.SelectionColor = Color.Black; 		
							informationWindow.infoTextBox.AppendText(SortIndexes[Sprites[b].infos[chr].OriginalID].ToString() + ",");

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
					for(int chr=0;chr< (Model.GridWidth/ objectSize) * (Model.GridHeight/ objectSize); chr++)
					{
						counts[SortIndexes[Sprites[b].infos[chr].OriginalID]]++;
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

		/// <summary>
		/// Disables all actions, runs the operation implemented as given action on background thread and enables all actions when complete.
		/// </summary>
		private async void RunLongOperation(Action action)
		{
			void EnableActions(bool enable)
			{
				newToolStripButton.Enabled = enable;
				openProjectButton.Enabled = enable;
				saveToolStripButton.Enabled = enable;
				addImagesToolStripButton.Enabled = enable;
				paletteToolStripButton.Enabled = enable;
				setOutputButton.Enabled = enable;
				outputFilesToolStripButton.Enabled = enable;
				settingsButton.Enabled = enable;
				spritesOut.Enabled = enable;
				blocksOut.Enabled = enable;
				blockWidthTextBox.Enabled = enable;
				blockHeightTextBox.Enabled = enable;

				fileMenu.Enabled = enable;
				toolsToolStripMenuItem.Enabled = enable;
			}

			EnableActions(false);

			await Task.Run(action);

			EnableActions(true);
		}
	}
}