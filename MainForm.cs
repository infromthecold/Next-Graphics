//#define PROPRIETARY         // do not use this as there is no code on git
//#define DEBUG_WINDOW
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Drawing.Imaging;
using System.Globalization;
using NextGraphics.Models;
using NextGraphics.Exporting;
using NextGraphics.Exporting.Common;
using NextGraphics.Main;
using NextGraphics.Utils;
using System.Threading.Tasks;
using System.Linq;

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

		private List<ImageForm> imageForms = new List<ImageForm>();
#if PROPRIETARY
		public	parallaxTool		parallaxWindow		=	new	parallaxTool();
#endif
		private InfoForm infoForm = new InfoForm();

		private ImageForm blocksForm = null;
		private ImageForm charsForm = null;
		private ImageForm rebuildTilesForm = null;

		private readonly PaletteForm paletteForm = new PaletteForm();
		private readonly SettingsForm settingsForm = new SettingsForm();
		private readonly PaletteOffsetForm offsetPanel = new PaletteOffsetForm();

		private readonly SaveFileDialog projectSaveDialog = new SaveFileDialog();
		private readonly OpenFileDialog projectOpenDialog = new OpenFileDialog();
		private readonly OpenFileDialog batchProjectDialog = new OpenFileDialog();
		private readonly SaveFileDialog outputFilesDialog = new SaveFileDialog();
		private readonly OpenFileDialog addImagesDialog = new OpenFileDialog();
		private readonly OpenFileDialog addTilemapsDialog = new OpenFileDialog();

		private string parentDirectory = "f:/";
		private string projectPath = string.Empty;
		private bool isPaletteSet = false;

		private readonly NumberFormatInfo numberFormatInfo = new NumberFormatInfo();

#if DEBUG_WINDOW
		public	DEBUGFORM		DEBUG_WINDOW;
#endif

		#region Initialization & Disposal

		public MainForm()
		{
			var exportParameters = new ExportParameters();
			exportParameters.RemapCallbacks = this;
			exportParameters.ExportCallbacks = this;

			Model = new MainModel
			{
				Name = Properties.Resources.NewProjectTitle
			};

			Model.BlocksAcrossWidthProvider = () => blocksPictureBox.Width;

			Model.OutputTypeChanged += Model_OutputTypeChanged;
			Model.GridWidthChanged += Model_GridWidthChanged;
			Model.GridHeightChanged += Model_GridHeightChanged;
			Model.RemapRequired += Model_RemapRequired;

			Exporter = new Exporter(Model, exportParameters);

			InitializeComponent();

#if DEBUG_WINDOW
			DEBUG_WINDOW = new DEBUGFORM();
			DEBUG_WINDOW.Show();
#endif
			toolStripProgressBar1.Minimum = 0;
			toolStripProgressBar1.Maximum = 0;

			ClearData();	// Clearing data will also take care of linking UI to fresh model data.
			SetForm();

			var imageFiltersBuilder = new DialogFilterBuilder();
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
			foreach (var c in codecs)
			{
				var codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
				imageFiltersBuilder.Add(codecName, c.FilenameExtension);
			}
			imageFiltersBuilder.Add("All Files", "*.*");
			addImagesDialog.Filter = imageFiltersBuilder.Filters;

			var tilemapFiltersBuilder = new DialogFilterBuilder();
			tilemapFiltersBuilder.Add("GBA Tile Map", "*.map");
			tilemapFiltersBuilder.Add("STM Tile Map", "*.stm");
			tilemapFiltersBuilder.Add("Text Tile Map", "*.txm;*.txt");
			tilemapFiltersBuilder.Add("All Files", "*.*");
			addTilemapsDialog.Filter = tilemapFiltersBuilder.Filters;

			var projectFiltersBuilder = new DialogFilterBuilder();
			projectFiltersBuilder.Add("Project Files", "*.xml");
			projectFiltersBuilder.Add("All Files", "*.*");
			projectOpenDialog.Filter = projectFiltersBuilder.Filters;
			projectSaveDialog.Filter = projectFiltersBuilder.Filters;

#if PROPRIETARY
			parallaxWindow.thePalette = thePalette;
			parallaxWindow.main = this;
			numberFormatInfo.NegativeSign = "-";
#endif

			var recentProject = Properties.Settings.Default.RecentProject;
			if (recentProject != null && File.Exists(recentProject))
			{
				LoadProjectFromFile(recentProject);
			}
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

		private void addTilemapsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddTilemapsToProject(true);
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
			Close();
		}

		#endregion

		#region Main menu - Tools

		private void rebuildFromTilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (rebuildTilesForm == null || !rebuildTilesForm.Visible)
			{
				rebuildTilesForm = new ImageForm
				{
					Text = "Rebuild From Tiles",
					MdiParent = this
				};
			}

			rebuildTilesForm.SetImage(new Bitmap(20 * Model.GridWidth, 12 * Model.GridHeight));
			rebuildTilesForm.Show();

			Exporter.RebuildFromTiles(rebuildTilesForm.SetPixel);

			rebuildTilesForm.Invalidate(true);
			rebuildTilesForm.Update();
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
					foreach (string file in batchProjectDialog.FileNames)
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
			AboutForm about = new AboutForm();
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

		private void addTilemapsToolStripButton_Click(object sender, EventArgs e)
		{
			AddTilemapsToProject(true);
		}

		private void paletteToolStripButton_Click(object sender, EventArgs e)
		{
			SelectPalette();
		}

		private void makeBlocksToolStripButton_Click(object sender, EventArgs e)
		{
			if (!isPaletteSet)
			{
				var result = MessageBox.Show("Do you want to set the palette mapping first?", "Palette mapping", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (result == DialogResult.Yes)
				{
					SelectPalette(() => {
						RemapData();
					});
					return;
				}
			}

			RemapData();
		}

		private void exportToolStripButton_Click(object sender, EventArgs e)
		{
			if (!Exporter.Data.IsRemapped)
			{
				SelectPalette(() =>
				{
					RemapData(() =>
					{
						ExportData();
					});
				});
				return;
			}

			ExportData();
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
			settingsForm.StartPosition = FormStartPosition.CenterParent;
			settingsForm.Model = Model;
			settingsForm.ShowDialog();
		}

		private void exportAsBlocksRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			RadioButton rb = sender as RadioButton;
			if (rb.Checked)
			{
				Model.OutputType = OutputType.Tiles;
			}
		}

		private void exportAsSpritesRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			RadioButton rb = sender as RadioButton;
			if (rb.Checked)
			{
				Model.OutputType = OutputType.Sprites;
			}
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
			if (charsForm == null || !charsForm.Visible)
			{
				charsForm = new ImageForm
				{
					Text = "Characters",
					MdiParent = this
				};
			}

			// Note: atm sprite/tile size is hard code and suits ZX Spectrum Next, it would be better to move this elsewhere - `MainModel` for example...
			charsForm.CopyImage(Model.CharsBitmap, Model.DefaultItemWidth(), Model.DefaultItemHeight(), 1f);

			MoveResizeAsMdiChild(charsForm, show: true);
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
			if (blocksForm == null || !blocksForm.Visible)
			{
				blocksForm = new ImageForm
				{
					Text = "Blocks",
					MdiParent = this
				};
			}

			blocksForm.CopyImage(Model.BlocksBitmap, Model.GridWidth, Model.GridHeight, 0.15f);

			MoveResizeAsMdiChild(blocksForm, show: true);
		}

		#endregion

		#region Project tool strip

		private void moveDownImageToolStripButton_Click(object sender, EventArgs e)
		{
			// move down
			int thisIndex = projectListBox.SelectedIndex;
			if (thisIndex <= Model.Sources.Count - 1 && thisIndex > 0)
			{
				var temp = Model.Sources[thisIndex];
				Model.Sources[thisIndex] = Model.Sources[thisIndex - 1];
				Model.Sources[thisIndex - 1] = temp;
				projectListBox.SelectedIndex = thisIndex + 1;
				UpdateProjectListBox();
				SaveProject();
			}
		}

		private void moveUpImageToolStripButton_Click(object sender, EventArgs e)
		{
			//Move up
			int thisIndex = projectListBox.SelectedIndex;
			if (thisIndex > 1 && thisIndex > 0)
			{
				thisIndex--;
				var temp = Model.Sources[thisIndex - 1];
				Model.Sources[thisIndex - 1] = Model.Sources[thisIndex];
				Model.Sources[thisIndex] = temp;
				projectListBox.SelectedIndex = thisIndex;
				UpdateProjectListBox();
				SaveProject();
			}
		}
		
		private void removeImageToolStripButton_Click(object sender, EventArgs e)
		{
			if (projectListBox.SelectedIndex == 0) return;

			foreach (Form childForm in MdiChildren)
			{
				childForm.Close();
			}

			var result = MessageBox.Show("Remove this image?", "Are you Sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (result == DialogResult.Yes)
			{
				Model.Sources.RemoveAt(projectListBox.SelectedIndex - 1);
				UpdateProjectListBox();
				SaveProject();
			}
		}

		private void reloadImagesToolStripButton_Click(object sender, EventArgs e)
		{
			DisposeImageWindows();

			Model.ForEachSourceImage((image, idx) =>
			{
				image.Reload();
				imageForms.Add(new ImageForm { MdiParent = this });
			});
		}

		private void projectListBox_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			int index = projectListBox.IndexFromPoint(e.Location);
			if (index == ListBox.NoMatches) return;

			if (index == 0)
			{
				if (ShowProjectNameDialog())
				{
					projectListBox.Items[0] = Model.Name.ToProjectItemTitle();
				}
			}
			else
			{
				if (Model.Sources[index - 1] is SourceImage image)
				{
					if (imageForms[index - 1] == null || !imageForms[index - 1].Visible)
					{
						imageForms[index - 1] = new ImageForm
						{
							Text = Path.GetFileName(image.Filename),
							MdiParent = this
						};
					}

					imageForms[index - 1].LoadImage(image.Filename, Model.GridWidth, Model.GridHeight);

					MoveResizeAsMdiChild(imageForms[index - 1], show: true);
				}
			}
		}

		#endregion

		#region Model events

		private void Model_OutputTypeChanged(object sender, MainModel.OutputTypeChangedEventArgs e)
		{
			UpdateOutputTypeControls(e.OutputType);
			charsPictureBox.Invalidate();
			charsPictureBox.Refresh();
		}

		private void Model_GridWidthChanged(object sender, MainModel.SizeChangedEventArgs e)
		{
			blockWidthTextBox.Text = e.Size.ToString();
		}

		private void Model_GridHeightChanged(object sender, MainModel.SizeChangedEventArgs e)
		{
			blockHeightTextBox.Text = e.Size.ToString();
		}

		private void Model_RemapRequired(object sender, EventArgs e)
		{
			Exporter.Data.Clear();
		}

		#endregion

		#endregion

		#region Project handling

		/// <summary>
		/// Sets the parent folder for the project.
		/// </summary>
		private void SetParentFolder(string path)
		{
			// Store parent folder for later use.
			DirectoryInfo parentDir = Directory.GetParent(path);
			parentDirectory = parentDir.Parent.FullName;

			// Save the path to the settings so we can reopen automatically on next launch.
			Properties.Settings.Default.RecentProject = path;
			Properties.Settings.Default.Save();
		}

		/// <summary>
		/// Shows the projet name dialog and returns true if user confirmed the name (in this case it also updates <see cref="Model.Name"/>), otherwise returns false.
		/// </summary>
		private bool ShowProjectNameDialog()
		{
			NewProjectForm newProjectForm = new NewProjectForm()
			{
				ProjectName = Model.Name,
			};

			newProjectForm.ShowDialog();

			if (newProjectForm.DialogResult == DialogResult.OK)
			{
				Model.Name = newProjectForm.ProjectName;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Clears all data and prepares UI for new project. Optionally it can also show the new project dialog (in this case, <see cref="Model.Name"/> will be updated if user confirms it).
		/// </summary>
		private void CreateNewProject(bool showDialog = false)
		{
			if (showDialog)
			{
				if (!ShowProjectNameDialog())
				{
					return;
				}
			}

			ClearData();
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

			SaveProject();
		}

		/// <summary>
		/// Loads project from file by showing the open project dialog.
		/// </summary>
		private void LoadProjectWithDialog()
		{
			projectOpenDialog.Multiselect = false;
			projectOpenDialog.RestoreDirectory = false;
			projectOpenDialog.InitialDirectory = parentDirectory + "\\Projects\\";

			if (projectOpenDialog.ShowDialog(this) == DialogResult.OK)
			{
				projectPath = projectOpenDialog.FileName;
				SetParentFolder(Path.GetFullPath(projectOpenDialog.FileName));
				LoadProjectFromFile(projectPath);
			}
		}

		/// <summary>
		/// Loads project from the given file.
		/// </summary>
		private void LoadProjectFromFile(string filename)
		{
			ClearData();

			Model.Load(filename);

			SetForm();
			UpdateProjectListBox();

#if PROPRIETARY
			XmlNode parallax = xmlDoc.SelectSingleNode("//Project/parallax");
			if (parallax != null)
			{
				parallaxWindow.readParallax(parallax);
				parallaxWindow.loadProject();
			}
#endif
		}

		private void SaveProject()
		{
			if (projectPath.Length == 0)
			{
				return;
			}

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
			}
		}

		private void ClearData()
		{
			isPaletteSet = false;
			projectPath = string.Empty;

			Model.Clear();
			Exporter.Data.Clear();

			// We must establish the link to new bitmaps since we recreate them in Model when calling Clear.
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
				addImagesDialog.InitialDirectory = parentDirectory;
				addImagesDialog.FilterIndex = Model.AddImagesFilterIndex;
				if (addImagesDialog.ShowDialog(this) == DialogResult.OK)
				{
					AddImagesToProject();
				}

				return;
			}

			// Note: as we keep dialog alive, we simply take the data from it. Not ideal way of doing things, but reduces the need to add additional state to the class or use separate function for handling.

			Model.AddImagesFilterIndex = addImagesDialog.FilterIndex;
			SaveProject();
			AddNewSources(addImagesDialog.FileNames, file => new SourceImage(file));
		}

		/// <summary>
		/// Adds tilemap(s) to project. Optionally it can also show open tilemap dialog.
		/// </summary>
		private void AddTilemapsToProject(bool showDialog = false)
		{
			if (showDialog)
			{
				addTilemapsDialog.Multiselect = true;
				addTilemapsDialog.RestoreDirectory = false;
				addTilemapsDialog.InitialDirectory = parentDirectory;
				addTilemapsDialog.FilterIndex = Model.AddTilemapsFilterIndex;
				if (addTilemapsDialog.ShowDialog(this) == DialogResult.OK)
				{
					AddTilemapsToProject();
				}

				return;
			}

			// Note: as we keep dialog alive, we simply take the data from it. Not ideal way of doing things, but reduces the need to add additional state to the class or use separate function for handling.

			Model.AddTilemapsFilterIndex = addTilemapsDialog.FilterIndex;
			SaveProject();

			AddNewSources(addTilemapsDialog.FileNames, file => SourceTilemap.Create(file));
		}

		private void AddNewSources(string[] filenames, Func<string, ISourceFile> handler)
		{
			var failedFiles = new List<string>();
			bool rejected = false;

			foreach (string file in filenames)
			{
				bool found = false;
				for (int i = 0; i < Model.Sources.Count; i++)
				{
					if (file == Model.Sources[i].Filename)
					{
						found = true;
						rejected = true;
						break;
					}
				}

				if (found) continue;

				var source = handler(file);
				if (source == null || !source.IsDataValid)
				{
					failedFiles.Add(Path.GetFileName(file));
					continue;
				}

				Model.Sources.Add(source);
			}

			UpdateProjectListBox();

			if (failedFiles.Count > 0)
			{
				var newline = Environment.NewLine;
				var names = string.Join(newline, failedFiles);
				MessageBox.Show($"Errors were detected in:{newline}{names}", "Errors Detected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			if (rejected)
			{
				MessageBox.Show("Duplicate files were detected and were ignored", "Duplicates Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		#endregion

		#region Exporting

		/// <summary>
		/// Requests user to select palette. If selected, given action is called (useful when chaining multiple actions). Note: if action is provided, it is assumed this call is part of multi-step process, so if palette is already set, no dialog is shown.
		/// </summary>
		private void SelectPalette(Action completed = null) 
		{
			if (completed != null && isPaletteSet)
			{
				completed();
				return;
			}

			paletteForm.StartPosition = FormStartPosition.CenterParent;
			paletteForm.Model = Model;
			paletteForm.ShowDialog();

			if (paletteForm.DialogResult == DialogResult.OK)
			{
				isPaletteSet = true;

				if (completed != null)
				{
					completed();
				}
			}
		}

		/// <summary>
		/// Remaps data. If successful, given action is called (useful when chaining multiple actions). All prerequisites must be met before calling.
		/// </summary>
		private void RemapData(Action completed = null)
		{
			UpdateBlockWidth();
			UpdateBlockHeight();

			RunLongOperation(() => {
				Exporter.Remap();

				if (completed != null)
				{
					Invoke(completed);
				}
			});
		}

		/// <summary>
		/// Exports data to the given path. If source filename is not given, dialog is shown to ask for it, otherwise the given file is used for exporting.
		/// </summary>
		private void ExportData(string sourceFilename = null)
		{
			if (sourceFilename == null)
			{
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

				return;
			}

#if PROPRIETARY
			ExportParameters.FourBitColourConverter = (colour) => (byte)parallaxWindow.getColour(colourByte);
#endif

			RunLongOperation(() =>
			{
				ExportPaths = new ExportPathProvider(sourceFilename, Model.ImageFormat);

				ExportPaths.AssignExportStreams(Exporter.Data.Parameters);

				Exporter.Export();
			});
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Initalizes the settings from a loaded image.
		/// </summary>
		private void SetForm()
		{
			projectListBox.Items.Clear();
			projectListBox.Items.Add(Model.Name.ToProjectItemTitle());
			Model.Sources.ForEach(source => projectListBox.Items.Add(source.ToProjectItemTitle()));

			blockHeightTextBox.Text = Model.GridHeight.ToString();
			blockWidthTextBox.Text = Model.GridWidth.ToString();

			UpdateOutputTypeControls(Model.OutputType);

			Refresh();
		}

		/// <summary>
		/// Updates project/output type controls to reflect the given <see cref="OutputType"/>.
		/// </summary>
		private void UpdateOutputTypeControls(OutputType type)
		{
			switch (type)
			{
				case OutputType.Sprites:
					exportAsSpritesRadioButton.Checked = true;
					break;
				default:
					exportAsBlocksRadioButton.Checked = true;
					break;
			}
		}

		/// <summary>
		/// Restores the bitmap data from the file list.
		/// </summary>
		private void UpdateProjectListBox()
		{
			// At this point SourceImage list is already setup in the model, so we need to check for bitmap validity, remove invalid images and establish image windows for each valid one.
			projectListBox.Items.Clear();
			projectListBox.Items.Add(Model.Name.ToProjectItemTitle());

			DisposeImageWindows();

			var removeNames = new List<string>();

			foreach (var source in Model.Sources)
			{
				if (!source.IsDataValid)
				{
					removeNames.Add(source.Filename);
					continue;
				}

				projectListBox.Items.Add(source.ToProjectItemTitle());

				if (source is SourceImage)
				{
					imageForms.Add(new ImageForm
					{
						Text = Path.GetFileName(source.Filename),
						MdiParent = this
					});
				}
			}

			foreach (string name in removeNames)
			{
				Model.RemoveSource(name);
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
		/// Updates block width from UI to <see cref="Model"/>.
		/// </summary>
		private void UpdateBlockWidth()
		{
			if (int.TryParse(blockWidthTextBox.Text, out int size))
			{
				// Make sure arbitrary value is constrained within allowed range.
				Model.GridWidth = Model.ConstrainItemWidth(size);

				// Make sure UI reflects the actual value, in case it was constrained.
				blockWidthTextBox.Text = Model.GridWidth.ToString();
			}
			else
			{
				// When unable to parse, leave original value.
				blockWidthTextBox.Text = Model.GridWidth.ToString();
			}

			blocksPictureBox.Invalidate();
			blocksPictureBox.Refresh();
		}

		/// <summary>
		/// Updates block height from UI to <see cref="Model"/>.
		/// </summary>
		private void UpdateBlockHeight()
		{
			if (int.TryParse(blockHeightTextBox.Text, out int size))
			{
				// Make sure arbitrary value is constrained within allowed range.
				Model.GridHeight = Model.ConstrainItemHeight(size);

				// Make sure UI reflects the actual value.
				blockHeightTextBox.Text = Model.GridHeight.ToString();
			}
			else
			{
				// When unable to parse, leave original value.
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
			foreach (ImageForm form in imageForms)
			{
				form.Dispose();
			}

			imageForms.Clear();
		}

		/// <summary>
		///  Moves and resizes given MDI child form, then refreshes it.
		/// </summary>
		private void MoveResizeAsMdiChild(Form form, int left = 20, int top = 20, bool show = false)
		{
			// We need to show form first, otherwise it won't refresh properly (well, `ImageForm` won't).
			if (show)
			{
				form.Show();
			}

			form.Left = left;
			form.Top = top;
			form.Width = Width - left - (resultsPanel.Width + filesPanel.Width + 20);
			form.Height = Height - top - (toolStrip.Height + 200);

			form.Refresh();
		}

		/// <summary>
		/// Disables all actions, runs the operation implemented as given action on background thread and enables all actions when complete.
		/// </summary>
		/// <remarks>
		/// Note that action is free to throw an exception. It will be caught and displayed to user.
		/// </remarks>
		private async void RunLongOperation(Action action)
		{
			void EnableActions(bool enable)
			{
				newToolStripButton.Enabled = enable;
				openProjectButton.Enabled = enable;
				saveToolStripButton.Enabled = enable;
				addTilemapsToolStripButton.Enabled = enable;
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

			Exception exception = null;

			await Task.Run(() =>
			{
				try
				{
					action();
				}
				catch (Exception e)
				{
					exception = e;
				}
			});

			if (exception != null)
			{
				MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}

			EnableActions(true);
		}

		#endregion
	}

	// namespace so we can use simply `Extensions` for class name without clashing with other extensions in other files...
	namespace Main
	{
		internal static class Extensions
		{
			public static string ToProjectItemTitle(this string name, string prefix = " ")
			{
				return $"{prefix} {name}";
			}

			public static string ToProjectItemTitle(this ISourceFile file)
			{
				string prefix;

				if (file is SourceImage)
				{
					prefix = "🏔️";
				}
				else if (file is SourceTilemap)
				{
					prefix = "🧱";
				}
				else
				{
					prefix = " ";
				}

				return Path.GetFileName(file.Filename).ToProjectItemTitle(prefix);
			}
		}
	}
}