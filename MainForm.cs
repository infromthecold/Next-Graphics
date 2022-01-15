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
	public partial class MainForm : Form, RemapCallbacks, ExportCallbacks
	{
		//-------------------------------------------------------------------------------------------------------------------
		//
		// variables
		//
		//-------------------------------------------------------------------------------------------------------------------

		private MainModel Model { get; set; }
		private Exporter Exporter { get; set; }
		private ExportPathProvider ExportPaths { get; set; }

		private RadioButton selectedRadio;
		private List<imageWindow> imageWindows = new List<imageWindow>();
#if PROPRIETARY
		public	parallaxTool		parallaxWindow		=	new	parallaxTool();
#endif
		private InfoForm infoForm = new InfoForm();
		private imageWindow blocksWindow = new imageWindow();
		private imageWindow charsWindow = new imageWindow();
		private PaletteForm paletteForm = new PaletteForm();
		private SettingsForm SettingsPanel = new SettingsForm();
		private imageWindow blocksView = new imageWindow();
		private palOffset offsetPanel = new palOffset();
		private rebuild rebuildDialog = new rebuild();

		private SaveFileDialog projectSaveDialog = new SaveFileDialog();
		private OpenFileDialog openProjectDialog = new OpenFileDialog();
		private OpenFileDialog batchProjectDialog = new OpenFileDialog();
		private SaveFileDialog outputFilesDialog = new SaveFileDialog();
		private OpenFileDialog addImagesDialog = new OpenFileDialog();

		private string parentDirectory = "f:/";
		private string projectPath = "";
		private bool isPaletteSet = false;

		private readonly NumberFormatInfo fmt = new NumberFormatInfo();
		private SolidBrush numberBrush = new SolidBrush(Color.White);
		private Font numberFont = new Font("Arial", 6, FontStyle.Bold);

#if DEBUG_WINDOW
		public	DEBUGFORM		DEBUG_WINDOW;
#endif

		#region Initialization & Disposal

		public MainForm()
		{
			var exportParameters = new ExportParameters();
			exportParameters.RemapCallbacks = this;
			exportParameters.ExportCallbacks = this;

			Model = new MainModel();
			Exporter = new Exporter(Model, exportParameters);

			InitializeComponent();

			ClearBitmap(Model.BlocksBitmap);
			blocksPictureBox.Image = Model.BlocksBitmap;
			blocksPictureBox.Height = Model.BlocksBitmap.Height;
			blocksPictureBox.Width = Model.BlocksBitmap.Width;
#if DEBUG_WINDOW
			DEBUG_WINDOW				=	new	DEBUGFORM();
			DEBUG_WINDOW.Show();
#endif

			ClearBitmap(Model.CharsBitmap);
			charsPictureBox.Image = Model.CharsBitmap;
			this.toolStripProgressBar1.Minimum = 0;
			this.toolStripProgressBar1.Maximum = 0;
			this.projectListBox.Items.Add(" " + Model.Name);

			blocksPictureBox.Invalidate();
			blocksPictureBox.Refresh();

			SettingsPanel.comments.SelectedIndex = 1;
			Model.UpdateBlocksAcross(blocksPictureBox.Width);

			SetForm();

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

		#endregion

		#region RemapCallbacks

		public bool OnRemapShowCharacterDebugData() => false;

		public void OnRemapStarted()
		{
			ClearBitmap(Model.BlocksBitmap);
			ClearBitmap(Model.CharsBitmap);
#if DEBUG_WINDOW
			ClearBitmap(DEBUG_WINDOW.DEBUG_IMAGE);
#endif

			Invoke(new Action(() =>
			{
				blocksPictureBox.Invalidate(true);
				blocksPictureBox.Update();

				charsPictureBox.Invalidate(true);
				charsPictureBox.Update();

				toolStripProgressBar1.Minimum = 0;
				toolStripProgressBar1.Maximum = 10000;
			}));
		}

		public void OnRemapUpdated()
		{
			Invoke(new Action(() =>
			{
				blocksPictureBox.Invalidate(true);
				blocksPictureBox.Update();
			}));
		}

		public void OnRemapCompleted(bool success)
		{
			Invoke(new Action(() =>
			{
				charsPictureBox.Invalidate(true);
				charsPictureBox.Update();

				blocksPictureBox.Invalidate(true);
				blocksPictureBox.Update();

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
				bitmap.CopyTo(Model.Palette, frame, Model.CharsBitmap);
#if DEBUG_WINDOW
				bitmap.CopyTo(Model.Palette, frame, DEBUG_WINDOW.DEBUG_IMAGE);
#endif
			}));
		}

		public void OnRemapDisplayBlock(Rectangle frame, IndexedBitmap bitmap)
		{
			Invoke(new Action(() =>
			{
				bitmap.CopyTo(Model.Palette, frame, Model.BlocksBitmap);
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

		public void OnExportStarted()
		{
		}

		public void OnExportCompleted()
		{
		}

		public string OnExportTilesInfoFilename()
		{
			// We should always have `ExportPaths` assigned when reaching this, but let's play safe...
			return ExportPaths.TilesInfoFilename ?? "tiles";
		}

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

		#region Events

		#region Main menu - File

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CreateNewProject(true);
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LoadProjectWithDialog();
		}

		private void addImagesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddImagesToProject(true);
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveProjectToFile(true);
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveProjectToFile(true, true);
		}

		private void exitToolsStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		#endregion

		#region Main menu - Tools

		private void rebuildFromTilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (blocksView.Visible == false)
			{
				blocksView = new imageWindow();
			}

			blocksView.MdiParent = this;
			blocksView.inputImage = new Bitmap(20 * Model.GridWidth, 12 * Model.GridHeight);
			blocksView.srcPicture.Image = blocksView.inputImage;

			blocksView.Show();

			Exporter.RebuildFromTiles((x, y, color) =>
			{
				blocksView.inputImage.SetPixel(x, y, color);
			});

			blocksView.Invalidate(true);
			blocksView.Update();
		}

		private void processMapToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// reduce the number of bytes in the map and remove unused blocks
#if PROPRIETARY
			parallaxWindow.processMap(sender,e);
#else
			MessageBox.Show("FUNCTIONALITY REMOVED", "Proprietary ", MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif
		}

		private void createParallaxToolStripMenuItem_Click(object sender, EventArgs e)
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
			batchProjectDialog.Multiselect = true;
			batchProjectDialog.RestoreDirectory = false;
			batchProjectDialog.InitialDirectory = parentDirectory + "\\Projects\\";
			batchProjectDialog.Filter = "Project Files (*.xml)|*.xml|All Files (*.*)|*.*";

			if (batchProjectDialog.ShowDialog(this) == DialogResult.OK)
			{
				RunLongOperation(() =>
				{
					foreach (String file in batchProjectDialog.FileNames)
					{
						SetParentFolder(Path.GetFullPath(file));
						CreateNewProject();
						LoadProjectFromFile(file);

						Exporter.Remap();

						ExportData($"{parentDirectory}\\Output\\{Model.Name.ToLower()}.asm");
					}
				});
			}
		}

		private void WAVToRAWToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog wavDialog = new OpenFileDialog();
			wavDialog.Multiselect = false;
			wavDialog.RestoreDirectory = false;
			wavDialog.InitialDirectory = parentDirectory + "\\Projects\\";
			wavDialog.Filter = "Wav Files (*.wav)|*.wav|All Files (*.*)|*.*";

			if (wavDialog.ShowDialog(this) == DialogResult.OK)
			{
				string wavFileName = wavDialog.FileName;
				string rawFileName = Path.ChangeExtension(wavDialog.FileName, "raw");
				FileInfo mapInfo = new FileInfo(wavFileName);
				byte[] wavArray;
				using (BinaryReader wavFile = new BinaryReader(File.Open(wavFileName, FileMode.Open)))
				{
					wavArray = new byte[mapInfo.Length];
					for (int r = 0; r < mapInfo.Length; r++)
					{
						wavArray[r] = wavFile.ReadByte();
					}
				}

				using (BinaryWriter rawFile = new BinaryWriter(File.Open(rawFileName, FileMode.OpenOrCreate)))
				{
					for (int r = 64; r < mapInfo.Length; r++)
					{
						byte snd = (byte)(wavArray[r]);

						rawFile.Write((byte)(snd));
					}
				}
			}
		}

		#endregion

		#region Main menu - Window

		private void cascadeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LayoutMdi(MdiLayout.Cascade);
		}

		private void tileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LayoutMdi(MdiLayout.TileVertical);
		}

		private void tileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LayoutMdi(MdiLayout.TileHorizontal);
		}

		private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Form childForm in MdiChildren)
			{
				childForm.Close();
			}
		}

		private void arrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LayoutMdi(MdiLayout.ArrangeIcons);
		}

		#endregion

		#region Main menu - Help

		private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://luckyredfish.com/next-graphics-version-2/");
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			About about = new About();
			about.ShowDialog();
		}

		#endregion

		#region Main tool strip

		private void newToolStripButton_Click(object sender, EventArgs e)
		{
			CreateNewProject(true);
		}

		private void openToolStripButton_Click(object sender, EventArgs e)
		{
			LoadProjectWithDialog();
		}

		private void saveToolStripButton_Click(object sender, EventArgs e)
		{
			SaveProjectToFile(true);
		}

		private void addImagesToolStripButton_Click(object sender, EventArgs e)
		{
			AddImagesToProject(true);
		}

		private void paletteToolStripButton_Click(object sender, EventArgs e)
		{
			paletteForm.StartPosition = FormStartPosition.CenterParent;
			paletteForm.Model = Model;
			paletteForm.ShowDialog();

			if (paletteForm.DialogResult == DialogResult.OK)
			{
				isPaletteSet = true;
			}
		}

		private void makeBlocksToolStripButton_Click(object sender, EventArgs e)
		{
			UpdateBlockWidth();
			UpdateBlockHeight();

			if (!isPaletteSet)
			{
				var result = MessageBox.Show("Do you want to set the palette mapping first?", "Palette mapping", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (result == DialogResult.Yes)
				{
					paletteToolStripButton.PerformClick();
					return;
				}
			}

			RunLongOperation(() => Exporter.Remap());
		}

		private void exportToolStripButton_Click(object sender, EventArgs e)
		{
			if (!Exporter.Data.IsRemapped)
			{
				MessageBox.Show("You need to remap the graphics before you can output!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			outputFilesDialog.FileName = Model.Name.ToLower();
			outputFilesDialog.Filter = "Machine Code (*.asm)|*.asm|All Files (*.*)|*.*";
			outputFilesDialog.FilterIndex = Model.OutputFilesFilterIndex;
			outputFilesDialog.RestoreDirectory = false;
			outputFilesDialog.InitialDirectory = $"{parentDirectory}\\Output\\";

			if (outputFilesDialog.ShowDialog() == DialogResult.OK)
			{
				Model.OutputFilesFilterIndex = outputFilesDialog.FilterIndex;
				ExportData(outputFilesDialog.FileName);
			}
		}

		private void infoToolStripButton_Click(object sender, EventArgs e)
		{
			if (infoForm == null) return;

			if (!infoForm.Visible)
			{
				infoForm = new InfoForm
				{
					MdiParent = this
				};
				infoForm.Show();
				infoForm.Top = 20;
				infoForm.Left = 20;
				infoForm.Refresh();
			}

			infoForm.Clear();

			RunLongOperation(() =>
			{
				Exporter.GenerateInfoString((color, text) =>
				{
					Invoke(new Action(() =>
					{
						infoForm.Append(color, text);
					}));
				});
			});
		}

		#endregion

		#region Output settings tool strip

		private void settingsButton_Click(object sender, EventArgs e)
		{
			SettingsPanel.StartPosition = FormStartPosition.CenterParent;
			SettingsPanel.Model = Model;
			SettingsPanel.ShowDialog();
		}

		private void exportAsBlocksRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			RadioButton rb = sender as RadioButton;
			if (rb.Checked)
			{
				// Keep track of the selected RadioButton by saving a reference to it.
				selectedRadio = rb;
			}
			Model.OutputType = OutputType.Tiles;
			charsPictureBox.Invalidate();
			charsPictureBox.Refresh();
		}

		private void exportAsSpritesRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			RadioButton rb = sender as RadioButton;
			if (rb.Checked)
			{
				// Keep track of the selected RadioButton by saving a reference to it.
				selectedRadio = rb;
			}
			Model.OutputType = OutputType.Sprites;
			charsPictureBox.Invalidate();
			charsPictureBox.Refresh();
		}

		private void blockWidthTextBox_Leave(object sender, EventArgs e)
		{
			UpdateBlockWidth();
		}

		private void blockHeightTextBox_Leave(object sender, EventArgs e)
		{
			UpdateBlockHeight();
		}

		#endregion

		#region Output picture boxes

		private void charsPictureBox_Paint(object sender, PaintEventArgs e)
		{
			// Paints the grid on the sprites/characters display.
			Graphics g = e.Graphics;
			Pen pen = new Pen(Color.Black);
			float[] dashValues = { 1, 1 };

			int divLines = 8;
			if (Model.OutputType == OutputType.Sprites)
			{
				divLines = 16;
			}

			pen.DashPattern = dashValues;

			// horizontal lines
			for (int y = 0; y < (blocksPictureBox.Image.Height / divLines) + 1; ++y)
			{
				g.DrawLine(pen, 0, y * divLines, blocksPictureBox.Image.Width, y * divLines);
			}

			// verticle lines
			for (int x = 0; x < (blocksPictureBox.Image.Width / divLines) + 1; ++x)
			{
				g.DrawLine(pen, x * divLines, 0, x * divLines, blocksPictureBox.Image.Height);
			}
		}

		private void charsPictureBox_Click(object sender, EventArgs e)
		{
			if (charsWindow != null)
			{
				if (charsWindow.Visible == false)
				{
					charsWindow = new imageWindow
					{
						MdiParent = this
					};
					if (Model.OutputType == OutputType.Sprites)
					{
						charsWindow.copyImage(Model.CharsBitmap, 16, 16, false);
					}
					else
					{
						charsWindow.copyImage(Model.CharsBitmap, 8, 8, false);
					}
					//	blocksWindow.loadImage(model.Filenames[index-1],model.GridXSize,model.GridYSize);
					charsWindow.Show();
					charsWindow.Height = this.Height - (toolStrip.Height + 200);
					charsWindow.Width = this.Width - (bottomPanel.Width + FilesView.Width + charsWindow.Left);
					charsWindow.Top = 20;
					charsWindow.Left = 20;
					charsWindow.Refresh();
				}
			}
		}

		private void blocksPictureBox_Paint(object sender, PaintEventArgs e)
		{
			// Paints the grid on the object/blocks display.
			Graphics g = e.Graphics;
			Pen pen = new Pen(Color.Black);
			float[] dashValues = { 4, 2 };

			pen.DashPattern = dashValues;

			// horizontal lines
			for (int y = 0; y < (blocksPictureBox.Image.Height / Model.GridHeight) + 1; ++y)
			{
				g.DrawLine(pen, 0, y * Model.GridHeight, blocksPictureBox.Image.Width, y * Model.GridHeight);
			}

			// verticle lines
			for (int x = 0; x < (blocksPictureBox.Image.Width / Model.GridWidth) + 1; ++x)
			{
				g.DrawLine(pen, x * Model.GridWidth, 0, x * Model.GridWidth, blocksPictureBox.Image.Height);
			}
		}

		private void blocksPictureBox_Click(object sender, EventArgs e)
		{
			if (blocksWindow != null)
			{
				if (blocksWindow.Visible == false)
				{
					blocksWindow = new imageWindow
					{
						MdiParent = this
					};
					blocksWindow.copyImage(Model.BlocksBitmap, Model.GridWidth, Model.GridHeight, true);
					//	blocksWindow.loadImage(model.Filenames[index-1],model.GridXSize,model.GridYSize);
					blocksWindow.Show();
					blocksWindow.Height = this.Height - (toolStrip.Height + 200);
					blocksWindow.Width = this.Width - (bottomPanel.Width + FilesView.Width + blocksWindow.Left);
					blocksWindow.Top = 20;
					blocksWindow.Left = 20;
					blocksWindow.Refresh();
				}
			}
		}

		#endregion

		#region Project tool strip

		private void moveDownImageToolStripButton_Click(object sender, EventArgs e)
		{
			// move down
			int thisIndex = this.projectListBox.SelectedIndex;
			if (thisIndex <= Model.Images.Count - 1 && thisIndex > 0)
			{

				var temp = Model.Images[thisIndex];
				Model.Images[thisIndex] = Model.Images[thisIndex - 1];
				Model.Images[thisIndex - 1] = temp;
				this.projectListBox.SelectedIndex = thisIndex + 1;
				UpdateProjectListBox();
			}
		}

		private void moveUpImageToolStripButton_Click(object sender, EventArgs e)
		{
			//Move up

			int thisIndex = this.projectListBox.SelectedIndex;
			if (thisIndex > 1 && thisIndex > 0)
			{
				thisIndex--;
				var temp = Model.Images[thisIndex - 1];
				Model.Images[thisIndex - 1] = Model.Images[thisIndex];
				Model.Images[thisIndex] = temp;
				this.projectListBox.SelectedIndex = thisIndex;
				UpdateProjectListBox();
			}
		}
		
		private void removeImageToolStripButton_Click(object sender, EventArgs e)
		{
			if (this.projectListBox.SelectedIndex == 0) return;

			foreach (Form childForm in MdiChildren)
			{
				childForm.Close();
			}

			var result = MessageBox.Show("Remove this image?", "Are you Sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (result == DialogResult.Yes)
			{
				Model.Images.RemoveAt(this.projectListBox.SelectedIndex - 1);
				UpdateProjectListBox();
			}
		}

		private void reloadImagesToolStripButton_Click(object sender, EventArgs e)
		{
			DisposeImageWindows();

			foreach (var image in Model.Images)
			{
				image.ReloadImage();
				imageWindows.Add(new imageWindow { MdiParent = this });
			}
		}

		private void projectListBox_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			int index = this.projectListBox.IndexFromPoint(e.Location);
			if (index != System.Windows.Forms.ListBox.NoMatches)
			{
				if (index == 0)
				{
					NewProjectForm newProjectForm = new NewProjectForm();
					newProjectForm.ShowDialog();
					if (newProjectForm.DialogResult == DialogResult.OK)
					{
						Model.Name = newProjectForm.textBox1.Text;
						this.projectListBox.Items[0] = " " + Model.Name;
					}
				}
				else
				{
					if (imageWindows[index - 1] != null)
					{
						if (imageWindows[index - 1].Visible == false)
						{
							imageWindows[index - 1] = new imageWindow
							{
								MdiParent = this
							};
							imageWindows[index - 1].loadImage(Model.Images[index - 1].Filename, Model.GridWidth, Model.GridHeight);
							imageWindows[index - 1].Show();
							imageWindows[index - 1].Height = this.Height - (toolStrip.Height + 100);
							imageWindows[index - 1].Width = this.Width - (FilesView.Width + imageWindows[index - 1].Left);
							imageWindows[index - 1].Top = 0;
							imageWindows[index - 1].Refresh();
						}
					}
				}
			}
		}

		#endregion

		#endregion

		#region Helpers

		/// <summary>
		/// Sets the parent folder for the project.
		/// </summary>
		public void SetParentFolder(string path)
		{
			DirectoryInfo parentDir = Directory.GetParent(path);
			parentDirectory = parentDir.Parent.FullName;
		}

		/// <summary>
		/// Clears all data and prepares UI for new project. Optionally it can also show the new project dialog.
		/// </summary>
		private void CreateNewProject(bool showDialog = false)
		{
			if (showDialog)
			{
				NewProjectForm newProjectForm = new NewProjectForm();

				newProjectForm.ShowDialog();

				if (newProjectForm.DialogResult == DialogResult.OK)
				{
					Model.Name = newProjectForm.textBox1.Text;
					CreateNewProject();
				}

				return;
			}

			Model.Clear();

			SetForm();

			ClearBitmap(Model.BlocksBitmap);
			blocksPictureBox.Image = Model.BlocksBitmap;
			blocksPictureBox.Height = Model.BlocksBitmap.Height;
			blocksPictureBox.Width = Model.BlocksBitmap.Width;
			blocksPictureBox.Invalidate(true);
			blocksPictureBox.Refresh();

			ClearBitmap(Model.CharsBitmap);
			charsPictureBox.Image = Model.CharsBitmap;
			charsPictureBox.Invalidate(true);
			charsPictureBox.Refresh();

			isPaletteSet = false;

			SetForm();
			DisposeImageWindows();
		}

		/// <summary>
		/// Saves current project to a file. Optionally it can also show save project dialog. Dialog can also be force shown which can be used for "save as" functionality.
		/// </summary>
		private void SaveProjectToFile(bool showDialog = false, bool forceDialog = false)
		{
			if (showDialog)
			{
				if (projectPath.Length == 0 || forceDialog)
				{
					projectSaveDialog.FileName = Model.Name + ".xml";
					projectSaveDialog.Filter = "Project Files (*.xml)|*.xml|All Files (*.*)|*.*";
					projectSaveDialog.FilterIndex = 1;
					projectSaveDialog.RestoreDirectory = false;
					projectSaveDialog.InitialDirectory = parentDirectory + "\\Projects\\";

					if (projectSaveDialog.ShowDialog() == DialogResult.OK)
					{
						projectPath = Path.ChangeExtension(projectSaveDialog.FileName, "xml");
						SetParentFolder(Path.GetFullPath(projectSaveDialog.FileName));
						SaveProjectToFile(false);
					}
				}
				else
				{
					SaveProjectToFile(false);
				}

				return;
			}

			// TODO: palette and settings values must be updated into model when closing those forms!
			int transIndex = Model.Palette.TransparentIndex;
			int loadedColourCount = Model.Palette.UsedCount;
			int loadedColourStart = Model.Palette.StartIndex;
			using (XmlTextWriter writer = new XmlTextWriter(projectPath, Encoding.UTF8))
			{
				writer.Formatting = Formatting.Indented;
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

		/// <summary>
		/// Loads project from file by showing the open project dialog.
		/// </summary>
		private void LoadProjectWithDialog()
		{
			openProjectDialog.Multiselect = false;
			openProjectDialog.RestoreDirectory = false;
			openProjectDialog.InitialDirectory = parentDirectory + "\\Projects\\";
			openProjectDialog.Filter = "Project Files (*.xml)|*.xml|All Files (*.*)|*.*";

			if (openProjectDialog.ShowDialog(this) == DialogResult.OK)
			{
				projectPath = openProjectDialog.FileName;
				SetParentFolder(Path.GetFullPath(openProjectDialog.FileName));
				LoadProjectFromFile(projectPath);
			}
		}

		/// <summary>
		/// Loads project from the given file.
		/// </summary>
		private void LoadProjectFromFile(string filename)
		{
			XmlDocument document = new XmlDocument();
			document.Load(filename);

			Model.Load(document);
			Model.UpdateBlocksAcross(blocksPictureBox.Width);

			isPaletteSet = false;

			SetForm();
			UpdateProjectListBox();

#if PROPRIETARY
			XmlNode parallax = xmlDoc.SelectSingleNode("//Project/parallax");
			if (parallax != null)
			{
				parallaxWindow.readParallax(parallax);
				this.parallaxWindow.loadProject();
			}
#endif
		}

		/// <summary>
		/// Adds image(s) to project. Optionally it can also show open image dialog.
		/// </summary>
		private void AddImagesToProject(bool showDialog = false)
		{
			if (showDialog)
			{
				addImagesDialog.Multiselect = true;
				addImagesDialog.RestoreDirectory = false;
				addImagesDialog.InitialDirectory = parentDirectory + "\\Renders\\";
				addImagesDialog.FilterIndex = Model.AddImagesFilterIndex;
				if (addImagesDialog.ShowDialog(this) == DialogResult.OK)
				{
					AddImagesToProject();
				}

				return;
			}

			// Note: as we keep dialog alive, we simply take the data from it. Not ideal way of doing things, but reduces the need to add additional state to the class or use separate function for handling.
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
				this.projectListBox.Items.Add("  " + Path.GetFileName(file));
			}

			if (rejected == true)
			{
				MessageBox.Show("Duplicate files are not allowed", "Duplicates Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		/// <summary>
		/// Exports data to the given path.
		/// </summary>
		private void ExportData(string sourceFilename)
		{
#if PROPRIETARY
			ExportParameters.FourBitColourConverter = (colour) => (byte)this.parallaxWindow.getColour(colourByte);
#endif

			RunLongOperation(() =>
			{
				try
				{
					ExportPaths = new ExportPathProvider(sourceFilename, Model.ImageFormat);

					ExportPaths.AssignExportStreams(Exporter.Data.Parameters);

					Exporter.Export();
				}
				catch (Exception e)
				{
					MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			});
		}

		/// <summary>
		/// Initalizes the settings from a loaded image.
		/// </summary>
		private void SetForm()
		{
			this.projectListBox.Items.Clear();
			this.projectListBox.Items.Add(" " + Model.Name);
			Model.Images.ForEach(filename => this.projectListBox.Items.Add(" " + filename));

			blockHeightTextBox.Text = Model.GridHeight.ToString();
			blockWidthTextBox.Text = Model.GridWidth.ToString();

			if (Model.OutputType == OutputType.Sprites)
			{
				exportAsSpritesRadioButton.Checked = true;
				exportAsBlocksRadioButton.Checked = false;
				selectedRadio = exportAsSpritesRadioButton;
				//charsPanel.Width	=	blocksAcross/outSize;
			}
			else //outputData.Blocks;
			{
				exportAsBlocksRadioButton.Checked = true;
				exportAsSpritesRadioButton.Checked = false;
				selectedRadio = exportAsBlocksRadioButton;
				//charsPanel.Width	=	blocksAcross/outSize;
			}

			this.Refresh();
		}

		/// <summary>
		/// Restores the bitmap data from the file list.
		/// </summary>
		private void UpdateProjectListBox()
		{
			// At this point SourceImage list is already setup in the model, so we need to check for bitmap validity, remove invalid images and establish image windows for each valid one.
			this.projectListBox.Items.Clear();
			this.projectListBox.Items.Add(" " + Model.Name);

			DisposeImageWindows();

			var removeNames = new List<string>();

			foreach (var image in Model.Images)
			{
				if (!image.IsImageValid)
				{
					removeNames.Add(image.Filename);
					continue;
				}

				projectListBox.Items.Add(" " + Path.GetFileName(image.Filename));
				imageWindows.Add(new imageWindow { MdiParent = this });
			}

			foreach (string name in removeNames)
			{
				Model.RemoveImage(name);
			}

			if (removeNames.Count > 0)
			{
				MessageBox.Show("Some images have been rejected as the image format is not supported", "Not Supported", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		/// <summary>
		/// Clears the given bitmap.
		/// </summary>
		private void ClearBitmap(Bitmap thisBitmap)
		{
			using (Graphics gfx = Graphics.FromImage(thisBitmap))
			using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 255, 0, 255)))
			{
				gfx.FillRectangle(brush, 0, 0, thisBitmap.Width, thisBitmap.Height);
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
				makeBlocksToolStripButton.Enabled = enable;
				exportToolStripButton.Enabled = enable;
				infoToolStripButton.Enabled = enable;
				settingsButton.Enabled = enable;
				exportAsSpritesRadioButton.Enabled = enable;
				exportAsBlocksRadioButton.Enabled = enable;
				blockWidthTextBox.Enabled = enable;
				blockHeightTextBox.Enabled = enable;

				fileMenu.Enabled = enable;
				toolsToolStripMenuItem.Enabled = enable;
			}

			EnableActions(false);

			await Task.Run(action);

			EnableActions(true);
		}

		/// <summary>
		/// Updates block width from UI to <see cref="Model"/>.
		/// </summary>
		private void UpdateBlockWidth()
		{
			int size;
			if (int.TryParse(blockWidthTextBox.Text, out size))
			{
				Model.GridWidth = size;

				if (Model.OutputType == OutputType.Sprites)
				{
					if (Model.GridWidth < 16)
					{
						Model.GridWidth = 16;
					}
					else if (Model.GridWidth > 320)
					{
						Model.GridWidth = 320;
					}
					else
					{
						Model.GridWidth = (Model.GridWidth + 15) & ~0xF;
					}
				}
				else
				{
					if (Model.GridWidth < 8)
					{
						Model.GridWidth = 8;
					}
					else if (Model.GridWidth > 128)
					{
						Model.GridWidth = 128;
					}
					else
					{
						Model.GridWidth = (Model.GridHeight + 7) & ~0x7;
					}
				}
				blockWidthTextBox.Text = Model.GridWidth.ToString();
			}
			else
			{
				Model.GridWidth = 32;
				blockWidthTextBox.Text = Model.GridWidth.ToString();
			}

			Model.UpdateBlocksAcross(blocksPictureBox.Width);

			blocksPictureBox.Invalidate();
			blocksPictureBox.Refresh();
		}

		/// <summary>
		/// Updates block height from UI to <see cref="Model"/>.
		/// </summary>
		private void UpdateBlockHeight()
		{
			int size;

			if (int.TryParse(blockHeightTextBox.Text, out size))
			{
				Model.GridHeight = size;

				if (Model.OutputType == OutputType.Sprites)
				{
					if (Model.GridHeight < 16)
					{
						Model.GridHeight = 16;
					}
					else if (Model.GridHeight > 320)
					{
						Model.GridHeight = 320;
					}
					else
					{
						Model.GridHeight = (Model.GridHeight + 15) & ~0xF;
					}
				}
				else
				{
					if (Model.GridHeight < 8)
					{
						Model.GridHeight = 8;
					}
					else if (Model.GridHeight > 128)
					{
						Model.GridHeight = 128;
					}
					else
					{
						Model.GridHeight = (Model.GridHeight + 7) & ~0x7;
					}
				}
				blockHeightTextBox.Text = Model.GridHeight.ToString();
			}
			else
			{
				Model.GridHeight = 32;
				blockHeightTextBox.Text = Model.GridHeight.ToString();
			}
			blocksPictureBox.Invalidate();
			blocksPictureBox.Refresh();
		}

		/// <summary>
		/// Disposes all image windows.
		/// </summary>
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

		#endregion
	}
}