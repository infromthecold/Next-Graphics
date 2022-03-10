using NextGraphics.Exporting;
using NextGraphics.Exporting.PaletteMapping;
using NextGraphics.Models;

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;


namespace NextGraphics
{
	public partial class ImageSelectForm : Form
	{
		public MainModel Model { get; set; }
		public Exporter Exporter { get; set; }
		public bool IsUsingModel { get; set; } = false;

		public PaletteMapper.Palette LoadedPalette { get; private set; } = new PaletteMapper.Palette();

		public int FromIndex { get; set; } = 0;
		public int ToIndex { get; set; } = 0;
		public int ColoursCount { get; set; } = 0;
		public Color CopiedColour { get; set; }

		private Colour thatColour = new Colour();
		private Colour thisColour = new Colour();
		private Button[] colourButtons = new Button[256];
		private Button clickedColourButton;

		private readonly List<string> fullNames = new List<string>();

		#region Initialization & Disposal

		public ImageSelectForm()
		{
			InitializeComponent();
			CreateColourButtons();
		}

		#endregion

		#region Events

		private void imagesListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (imagesListBox.SelectedIndex >= 0)
			{
				Application.UseWaitCursor = true;
				Cursor.Current = Cursors.WaitCursor;

				if (IsUsingModel)
				{
					LoadPaletteFromModel();
				}
				else
				{
					LoadPaletteFromFile();
				}

				Cursor.Current = Cursors.Default;
				Application.UseWaitCursor = false;
			}

			UpdateImportButton();
		}

		private void colourButton_Click(object sender, EventArgs e)
		{
			clickedColourButton = (Button)sender;

			Point lowerLeft = new Point(0, clickedColourButton.Height);
			lowerLeft = clickedColourButton.PointToScreen(lowerLeft);

			copyMenu.Show(lowerLeft);
		}

		private void openMixerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			colorDialog.Color = clickedColourButton.BackColor;

			if (colorDialog.ShowDialog() == DialogResult.OK)
			{
				var colourIndex = int.Parse(clickedColourButton.Name);
				clickedColourButton.BackColor = colorDialog.Color;
				LoadedPalette[colourIndex].CopyFromColor(clickedColourButton.BackColor);
			}
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CopiedColour = clickedColourButton.BackColor;
		}

		private void cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int colourIndex = int.Parse(clickedColourButton.Name);
			clickedColourButton.BackColor = SystemColors.Control;
			LoadedPalette[colourIndex].CopyFromColor(clickedColourButton.BackColor);
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int colourIndex = int.Parse(clickedColourButton.Name);
			clickedColourButton.BackColor = CopiedColour;
			LoadedPalette[colourIndex].CopyFromColor(CopiedColour);
		}

		private void fromTextBox_TextChanged(object sender, EventArgs e)
		{
			FromIndex = IntegerFromControl(sender);
		}

		private void toTextBox_TextChanged(object sender, EventArgs e)
		{
			ToIndex = IntegerFromControl(sender);
		}

		private void countTextBox_TextChanged(object sender, EventArgs e)
		{
			ColoursCount = IntegerFromControl(sender);
		}

		private void importButton_Click(object sender, EventArgs e)
		{
			ToIndex = IntegerFromControl(toTextBox);
			FromIndex = IntegerFromControl(fromTextBox);
			ColoursCount = IntegerFromControl(countTextBox);
		}

		#endregion

		#region Public

		public void FillFilenamesFromModel()
		{
			FillFilenames(Model.SourceImages().Select(image => $" {image.Filename}"));
		}

		public void FillFilenames(IEnumerable<string> filenames)
		{
			fullNames.Clear();
			fullNames.AddRange(filenames);

			imagesListBox.Items.Clear();
			imagesListBox.Items.AddRange(fullNames.Select(n => Path.GetFileName(n)).ToArray());

			UpdateImportButton();
		}

		#endregion

		#region Helpers

		private void CreateColourButtons()
		{
			int across = 0;
			int down = 0;

			for (int c = 0; c < colourButtons.Length; c++)
			{
				SetUnusedColour(c);

				colourButtons[c] = new Button
				{
					Text = "",
					Name = c.ToString(),
					FlatStyle = FlatStyle.Flat,
					Location = new Point(10 + (across * 20), 15 + (down * 20)),
					Size = new Size(22, 22),
				};

				colourButtons[c].Click += colourButton_Click;
				colourButtons[c].FlatAppearance.BorderColor = SystemColors.ControlDark;
				colourButtons[c].FlatAppearance.BorderSize = 1;

				Controls.Add(colourButtons[c]);

				across++;
				if (across > 15)
				{
					across = 0;
					down++;
				}
			}
		}

		private void UpdateImportButton()
		{
			importButton.Enabled = (imagesListBox.Items.Count > 0 && imagesListBox.SelectedIndex >= 0);
		}

		private void LoadPaletteFromModel()
		{
			UpdatePalette(() =>
			{
				var images = Model.SourceImages().ToList();
				var source = (Bitmap)images[imagesListBox.SelectedIndex].Data.Clone();

				return Exporter.MapPalette(source);
			});
		}

		private void LoadPaletteFromFile()
		{
			UpdatePalette(() =>
			{
				var filename = fullNames[imagesListBox.SelectedIndex];

				return Exporter.LoadPalette(filename);
			});
		}

		private void UpdatePalette(Func<PaletteMapper.Palette> handler)
		{
			try
			{
				LoadedPalette = handler();

				countTextBox.Text = LoadedPalette.UsedCount.ToString();
				fromTextBox.Text = "0";
				toTextBox.Text = "0";

				// Show all used colours.
				for (int c = 0; c < LoadedPalette.UsedCount; c++)
				{
					colourButtons[c].BackColor = LoadedPalette[c].ToColor();
				}

				// All remaining buttons should show unused colour.
				SetUnusedColour(LoadedPalette.UsedCount, colourButtons.Length, (i, r, g, b) =>
				{
					colourButtons[i].BackColor = Color.FromArgb(r, g, b);
				});
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private int IntegerFromControl(object control)
		{
			TextBox textBox = (TextBox)control;
			try
			{
				return int.Parse(textBox.Text);
			}
			catch
			{
				return 0;
			}
		}

		private void SetUnusedColour(int index, int count = 1, Action<int, byte, byte, byte> handler = null) {
			var max = Math.Min(index + count, LoadedPalette.UsedCount);
			var colour = SystemColors.Control;

			for (int i = index; i < max; i++)
			{
				LoadedPalette[i].CopyFromColor(colour);

				if (handler != null)
				{
					handler(i, colour.R, colour.G, colour.B);
				}
			}
		}

		#endregion
	}
}
