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
using NextGraphics.Models;
using NextGraphics.Exporting;
using NextGraphics.Exporting.Common;
using NextGraphics.Main;
using NextGraphics.Utils;
using System.Threading.Tasks;

namespace NextGraphics
{
	public partial class MainForm : Form, RemapCallbacks, ExportCallbacks
	{
		private MainModel Model { get; set; }
		private Exporter Exporter { get; set; }
		private ExportPathProvider ExportPaths { get; set; }

		private List<ImageForm> imageForms = new List<ImageForm>();
		private List<ImageForm> tilemapForms = new List<ImageForm>();
		private InfoForm infoForm = new InfoForm();

		private ImageForm rebuildTilesForm = null;

		private readonly PaletteForm paletteForm = new PaletteForm();
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
		private bool isDataReloaded = false;

#if DEBUG_WINDOW
		public DebugForm debugForm;
#endif

#if PROPRIETARY
		public	parallaxTool		parallaxWindow		=	new	parallaxTool();
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

			UpdateStatusProgress(false);
			ClearData();
			SetForm();

			Properties.Settings.Default.PropertyChanged += (o, e) =>
			{
				Invalidate(true);
				Refresh();
			}; 

			// Prepare image dialog filters.
			var imageFiltersBuilder = new DialogFilterBuilder();
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
			foreach (var c in codecs)
			{
				var codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
				imageFiltersBuilder.Add(codecName, c.FilenameExtension);
			}
			imageFiltersBuilder.Add("All Files", "*.*");
			addImagesDialog.Filter = imageFiltersBuilder.Filters;

			// Prepare tilemap dialog filters.
			var tilemapFiltersBuilder = new DialogFilterBuilder();
			tilemapFiltersBuilder.Add("GBA Tile Map", "*.map");
			tilemapFiltersBuilder.Add("STM Tile Map", "*.stm");
			tilemapFiltersBuilder.Add("Text Tile Map", "*.txm;*.txt");
			tilemapFiltersBuilder.Add("All Files", "*.*");
			addTilemapsDialog.Filter = tilemapFiltersBuilder.Filters;

			// Prepare project dialogs filters.
			var projectFiltersBuilder = new DialogFilterBuilder();
			projectFiltersBuilder.Add("Project Files", "*.xml");
			projectFiltersBuilder.Add("All Files", "*.*");
			projectOpenDialog.Filter = projectFiltersBuilder.Filters;
			projectSaveDialog.Filter = projectFiltersBuilder.Filters;

#if DEBUG_WINDOW
			debugForm = new DebugForm();
			debugForm.Show();
#endif

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
			Model.BlocksBitmap.Clear();
			Model.CharsBitmap.Clear();
#if DEBUG_WINDOW
			debugForm.ClearImage();
#endif

			Invoke(new Action(() =>
			{
				blocksPictureBox.Invalidate(true);
				blocksPictureBox.Update();

				charsPictureBox.Invalidate(true);
				charsPictureBox.Update();

				UpdateStatusProgress(true);
			}));
		}

		public void OnRemapUpdated()
		{
			Invoke(new Action(() =>
			{
				blocksPictureBox.Invalidate(true);
				blocksPictureBox.Update();

				charsPictureBox.Invalidate(true);
				charsPictureBox.Update();
			}));
		}

		public void OnRemapCompleted(bool success)
		{
			Invoke(new Action(() =>
			{
				foreach (Form child in MdiChildren)
				{
					if (child is ImageForm)
					{
						child.Invalidate(true);
						child.Update();
					}
				}

				charsPictureBox.Invalidate(true);
				charsPictureBox.Update();

				blocksPictureBox.Invalidate(true);
				blocksPictureBox.Update();

				UpdateStatusProgress(false);

#if DEBUG_WINDOW
				debugForm.Invalidate(true);
				debugForm.Update();
#endif
			}));
		}

		public void OnRemapDisplayChar(Point position, IndexedBitmap bitmap)
		{
			Invoke(new Action(() =>
			{
				bitmap.CopyTo(Model.CharsBitmap, Model.Palette, position);
#if DEBUG_WINDOW
				debugForm.CopyImage(Model.Palette, position, bitmap);
#endif
			}));
		}

		public void OnRemapDisplayBlock(Point position, IndexedBitmap bitmap)
		{
			Invoke(new Action(() =>
			{
				bitmap.CopyTo(Model.BlocksBitmap, Model.Palette, position);
			}));
		}

		public void OnRemapDisplayCharactersCount(int count, int transparentCount)
		{
			Invoke(new Action(() =>
			{
				if (Model.OutputType == OutputType.Tiles)
				{
					statusSpritesLabel.Text = $"Characters ({count}), Transparent ({transparentCount})";
				}
				else
				{
					statusSpritesLabel.Text = $"Sprites ({count})";
				}
			}));
		}

		public void OnRemapDisplayBlocksCount(int count)
		{
			Invoke(new Action(() =>
			{
				if (Model.OutputType == OutputType.Tiles)
				{
					statusBlocksLabel.Text = $"Blocks ({count})";
				}
				else
				{
					statusBlocksLabel.Text = $"Objects ({count})";
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

						ExportData($"{parentDirectory}\\Output\\{Model.Name.ToLower()}.{Model.ExportAssemblerFileExtension}");
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
			// Note: settings form is non-modal so it will automatically get disposed upon closing.
			FormHelpers.ShowOrCreateNewModelessInstance<SettingsForm>(form =>
			{
				form.StartPosition = FormStartPosition.CenterParent;
				form.Model = Model;
			});
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
			var gridWidth = Model.DefaultItemWidth();
			var gridHeight = Model.DefaultItemHeight();

			e.Graphics.DrawGrid(charsPictureBox.Image, gridWidth, gridHeight);
		}

		private void charsPictureBox_Click(object sender, EventArgs e)
		{
			var title = "Characters";

			this.ShowOrCreateNewMdiChildInstance<ImageForm>(
				form => form.Text == title,	// we have multiple child `ImageForm`s, so title identifies the exact instance
				form =>
				{
					form.Text = title;

					form.CopyImage(Model.CharsBitmap, new ImageForm.Parameters
					{
						GridWidth = () => Model.DefaultItemWidth(),
						GridHeight = () => Model.DefaultItemHeight()
					});

					MoveResizeAsMdiChild(form);
				});
		}

		private void blocksPictureBox_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawGrid(blocksPictureBox.Image, Model.GridWidth, Model.GridHeight);
		}

		private void blocksPictureBox_Click(object sender, EventArgs e)
		{
			var title = "Blocks";

			this.ShowOrCreateNewMdiChildInstance<ImageForm>(
				form => form.Text == title, // we have multiple child `ImageForm`s, so title identifies the exact instance
				form =>
				{
					form.Text = title;

					form.CopyImage(Model.BlocksBitmap, new ImageForm.Parameters
					{
						GridWidth = () => Model.GridWidth,
						GridHeight = () => Model.GridHeight
					});

					MoveResizeAsMdiChild(form);
				});
		}

		#endregion

		#region Project tool strip

		private void moveDownImageToolStripButton_Click(object sender, EventArgs e)
		{
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
			DisposeSourceImageForms();

			Model.ForEachSourceImage((image, idx) =>
			{
				image.Reload();
				imageForms.Add(new ImageForm { MdiParent = this });
			});

			Model.ForEachSourceTilemap((tilemap, idx) =>
			{
				tilemap.Reload();
				tilemapForms.Add(new ImageForm { MdiParent = this });
			});
		}

		private void projectListBox_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			int index = projectListBox.IndexFromPoint(e.Location);
			if (index == ListBox.NoMatches) return;

			var sourceIndex = index - 1;
			var imageFormIndex = sourceIndex;
			var tilemapFormIndex = sourceIndex - Model.SourceImagesCount;	// this is used to access 0-bound `tilemapForma` list

			if (index == 0)
			{
				// Project name.
				if (ShowProjectNameDialog())
				{
					projectListBox.Items[0] = Model.Name.ToProjectItemTitle();
				}
			}
			else if (Model.Sources[sourceIndex] is SourceImage image)
			{
				// Source images.
				if (imageForms[imageFormIndex] == null || !imageForms[imageFormIndex].Visible)
				{
					imageForms[imageFormIndex] = CreateImageFormForSource(image);
				}

				imageForms[imageFormIndex].LoadImage(image.Filename, Model.GridWidth, Model.GridHeight);
				imageForms[imageFormIndex].BringToFront();

				MoveResizeAsMdiChild(imageForms[imageFormIndex], show: true);
			}
			else if (Model.Sources[sourceIndex] is SourceTilemap tilemap)
			{
				// Source tilemaps. Note how source index is different from tilemap form index. Also since tilemap images need to be reloaded after mapping, we add parameters.
				if (tilemapForms[tilemapFormIndex] == null || !tilemapForms[tilemapFormIndex].Visible)
				{
					tilemapForms[tilemapFormIndex] = CreateImageFormForSource(tilemap);
				}

				RenderingOptions CreateOptions()
				{
					return new RenderingOptions
					{
						RenderTileIndex = Properties.Settings.Default.TilemapRenderTileIndex
					};
				}

				Bitmap TilemapImage(double scale)
				{
					return tilemap.CreateBitmap(Exporter, CreateOptions());
				};

				void TilemapOverlay(Graphics g, Size size, double scale)
				{
					tilemap.RenderOverlay(g, size, scale, Exporter, CreateOptions());
				}

				// Note: with initial image we establish desired image form size.
				tilemapForms[tilemapFormIndex].SetImage(TilemapImage(1.0), new ImageForm.Parameters
				{
					GridWidth = () => Model.GridWidth,
					GridHeight = () => Model.GridHeight,
					ImageProvider = TilemapImage,
					OverlayProvider = TilemapOverlay,
				});

				tilemapForms[tilemapFormIndex].BringToFront();

				MoveResizeAsMdiChild(tilemapForms[tilemapFormIndex], show: true);
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
			DisposeSourceImageForms();
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
				LoadProjectFromFile(projectOpenDialog.FileName);
			}
		}

		/// <summary>
		/// Loads project from the given file.
		/// </summary>
		private void LoadProjectFromFile(string filename)
		{
			// We need to clear data first, before assigning anything else to avoid new values being reset unintentionally.
			ClearData();

			projectPath = filename;
			SetParentFolder(Path.GetFullPath(filename));

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
			Model.BlocksBitmap.Clear();
			blocksPictureBox.Image = Model.BlocksBitmap;
			blocksPictureBox.Height = Model.BlocksBitmap.Height;
			blocksPictureBox.Width = Model.BlocksBitmap.Width;
			blocksPictureBox.Invalidate(true);
			blocksPictureBox.Refresh();

			Model.CharsBitmap.Clear();
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
			ReloadModelSourcesIfNeeded();

			if (completed != null && isPaletteSet)
			{
				completed();
				return;
			}

			paletteForm.StartPosition = FormStartPosition.CenterParent;
			paletteForm.Model = Model;
			paletteForm.Exporter = Exporter;
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
			// Note: after remapping, we want to reset the flags so next time remap is required, we will reload model sources again.
			ReloadModelSourcesIfNeeded(requestReloadNextTime: true);

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
			// Note: we don't have to reload sources before exporting. If they were changed, we'd have to remap anyway since sources affect the outcome. Additionally, most often than not, remapping is performed as batch operation just before exporting.

			if (sourceFilename == null)
			{
				outputFilesDialog.FileName = Model.Name.ToLower();
				outputFilesDialog.Filter = $"Machine Code (*.{Model.ExportAssemblerFileExtension})|*.{Model.ExportAssemblerFileExtension}|All Files (*.*)|*.*";
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
				ExportPaths = new ExportPathProvider(sourceFilename, Model);

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

			DisposeSourceImageForms();

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
					imageForms.Add(CreateImageFormForSource(source));
				}
				else if (source is SourceTilemap)
				{
					tilemapForms.Add(CreateImageFormForSource(source));
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
		/// Updates block width from UI to <see cref="Model"/>.
		/// </summary>
		private void UpdateBlockWidth()
		{
			if (int.TryParse(blockWidthTextBox.Text, out int size))
			{
				// Make sure arbitrary value is constrained within allowed range.
				Model.GridWidth = size;

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
				Model.GridHeight = size;

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
		/// Hides or shows progress status controls.
		/// </summary>
		private void UpdateStatusProgress(bool active)
		{
			statusToolStripProgressBar.Minimum = 0;
			statusToolStripProgressBar.Maximum = active ? 10000 : 0;
		}

		ImageForm CreateImageFormForSource(ISourceFile source)
		{
			return new ImageForm
			{
				Text = Path.GetFileName(source.Filename),
				MdiParent = this,
			};
		}

		private void DisposeSourceImageForms()
		{
			foreach (ImageForm form in imageForms)
			{
				form.Dispose();
			}

			imageForms.Clear();

			foreach (ImageForm form in tilemapForms)
			{
				form.Dispose();
			}

			tilemapForms.Clear();
		}

		/// <summary>
		/// Reloads all model sources and manages reload-needed flag.
		/// </summary>
		private void ReloadModelSourcesIfNeeded(bool requestReloadNextTime = false)
		{
			if (!isDataReloaded)
			{
				Model.ReloadSources();

				isDataReloaded = true;
			}

			if (requestReloadNextTime)
			{
				isDataReloaded = false;
			}
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
				var messageBuilder = new StringBuilder();
				messageBuilder.AppendLine(exception.Message);
				messageBuilder.AppendLine();
				messageBuilder.AppendLine("Exception details have been copied to clipboard as convenience!");

				Clipboard.SetText(exception.StackTrace);

				MessageBox.Show(messageBuilder.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
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