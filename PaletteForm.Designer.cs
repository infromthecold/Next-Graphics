namespace NextGraphics
{
	partial class PaletteForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.label1 = new System.Windows.Forms.Label();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.loadPaletteButton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this.customTypeRadioButton = new System.Windows.Forms.RadioButton();
			this.label4 = new System.Windows.Forms.Label();
			this.next512TypeRadioButton = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this.next256TypeRadioButton = new System.Windows.Forms.RadioButton();
			this.selectFromImageButton = new System.Windows.Forms.Button();
			this.transparentColourPickButton = new System.Windows.Forms.Button();
			this.messageLabel = new System.Windows.Forms.Label();
			this.savePaletteButton = new System.Windows.Forms.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.colourCountTextBox = new System.Windows.Forms.TextBox();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.tColourIndex1 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.hexColourTextBox = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.copyMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openMixerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.hexValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.paletteRowUpButton = new System.Windows.Forms.Button();
			this.paletteRowDownButton = new System.Windows.Forms.Button();
			this.startIndexTextBox = new System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			this.copyMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(19, 286);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(97, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Transparent Colour";
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(325, 357);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 6;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(415, 357);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 5;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// loadPaletteButton
			// 
			this.loadPaletteButton.Cursor = System.Windows.Forms.Cursors.Default;
			this.loadPaletteButton.Location = new System.Drawing.Point(12, 201);
			this.loadPaletteButton.Name = "loadPaletteButton";
			this.loadPaletteButton.Size = new System.Drawing.Size(59, 23);
			this.loadPaletteButton.TabIndex = 7;
			this.loadPaletteButton.Text = "Load";
			this.loadPaletteButton.UseVisualStyleBackColor = true;
			this.loadPaletteButton.Click += new System.EventHandler(this.loadPaletteButton_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.customTypeRadioButton);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.next512TypeRadioButton);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.next256TypeRadioButton);
			this.groupBox1.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(133, 105);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Colour Mapping";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Cursor = System.Windows.Forms.Cursors.Default;
			this.label5.Location = new System.Drawing.Point(29, 84);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(78, 13);
			this.label5.TabIndex = 5;
			this.label5.Text = "Custom Palette";
			// 
			// customTypeRadioButton
			// 
			this.customTypeRadioButton.AutoSize = true;
			this.customTypeRadioButton.Location = new System.Drawing.Point(10, 84);
			this.customTypeRadioButton.Name = "customTypeRadioButton";
			this.customTypeRadioButton.Size = new System.Drawing.Size(14, 13);
			this.customTypeRadioButton.TabIndex = 4;
			this.customTypeRadioButton.TabStop = true;
			this.customTypeRadioButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.customTypeRadioButton.UseVisualStyleBackColor = true;
			this.customTypeRadioButton.CheckedChanged += new System.EventHandler(this.customTypeRadioButton_CheckedChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Cursor = System.Windows.Forms.Cursors.Default;
			this.label4.Location = new System.Drawing.Point(30, 54);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(86, 13);
			this.label4.TabIndex = 3;
			this.label4.Text = "512 Next Palette";
			// 
			// next512TypeRadioButton
			// 
			this.next512TypeRadioButton.AutoSize = true;
			this.next512TypeRadioButton.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.next512TypeRadioButton.Location = new System.Drawing.Point(10, 54);
			this.next512TypeRadioButton.Name = "next512TypeRadioButton";
			this.next512TypeRadioButton.Size = new System.Drawing.Size(14, 13);
			this.next512TypeRadioButton.TabIndex = 2;
			this.next512TypeRadioButton.TabStop = true;
			this.next512TypeRadioButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.next512TypeRadioButton.UseVisualStyleBackColor = true;
			this.next512TypeRadioButton.CheckedChanged += new System.EventHandler(this.next512TypeRadioButton_CheckedChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Cursor = System.Windows.Forms.Cursors.Default;
			this.label2.Location = new System.Drawing.Point(29, 26);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(86, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "256 Next Palette";
			// 
			// next256TypeRadioButton
			// 
			this.next256TypeRadioButton.AutoSize = true;
			this.next256TypeRadioButton.Cursor = System.Windows.Forms.Cursors.Default;
			this.next256TypeRadioButton.Location = new System.Drawing.Point(10, 26);
			this.next256TypeRadioButton.Name = "next256TypeRadioButton";
			this.next256TypeRadioButton.Size = new System.Drawing.Size(14, 13);
			this.next256TypeRadioButton.TabIndex = 0;
			this.next256TypeRadioButton.TabStop = true;
			this.next256TypeRadioButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.next256TypeRadioButton.UseVisualStyleBackColor = true;
			this.next256TypeRadioButton.CheckedChanged += new System.EventHandler(this.next256TypeRadioButton_CheckedChanged);
			// 
			// selectFromImageButton
			// 
			this.selectFromImageButton.Cursor = System.Windows.Forms.Cursors.Default;
			this.selectFromImageButton.Location = new System.Drawing.Point(45, 145);
			this.selectFromImageButton.Name = "selectFromImageButton";
			this.selectFromImageButton.Size = new System.Drawing.Size(75, 23);
			this.selectFromImageButton.TabIndex = 9;
			this.selectFromImageButton.Text = "Select";
			this.selectFromImageButton.UseVisualStyleBackColor = true;
			this.selectFromImageButton.Click += new System.EventHandler(this.selectPaletteButton_Click);
			// 
			// transparentColourPickButton
			// 
			this.transparentColourPickButton.Cursor = System.Windows.Forms.Cursors.Default;
			this.transparentColourPickButton.Location = new System.Drawing.Point(77, 302);
			this.transparentColourPickButton.Name = "transparentColourPickButton";
			this.transparentColourPickButton.Size = new System.Drawing.Size(57, 23);
			this.transparentColourPickButton.TabIndex = 9;
			this.transparentColourPickButton.Text = "Pick";
			this.transparentColourPickButton.UseVisualStyleBackColor = true;
			this.transparentColourPickButton.Click += new System.EventHandler(this.transparentColourButton_Click);
			// 
			// messageLabel
			// 
			this.messageLabel.AutoSize = true;
			this.messageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
			this.messageLabel.Location = new System.Drawing.Point(167, 339);
			this.messageLabel.Name = "messageLabel";
			this.messageLabel.Size = new System.Drawing.Size(311, 13);
			this.messageLabel.TabIndex = 10;
			this.messageLabel.Text = "Please note: Only every other colour is shown for the 512 Palette";
			// 
			// savePaletteButton
			// 
			this.savePaletteButton.Cursor = System.Windows.Forms.Cursors.Default;
			this.savePaletteButton.Location = new System.Drawing.Point(77, 201);
			this.savePaletteButton.Name = "savePaletteButton";
			this.savePaletteButton.Size = new System.Drawing.Size(59, 23);
			this.savePaletteButton.TabIndex = 10;
			this.savePaletteButton.Text = "Save";
			this.savePaletteButton.UseVisualStyleBackColor = true;
			this.savePaletteButton.Click += new System.EventHandler(this.savePaletteButton_Click);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(9, 229);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(64, 13);
			this.label8.TabIndex = 11;
			this.label8.Text = "Palette Shift";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(19, 334);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(74, 13);
			this.label3.TabIndex = 12;
			this.label3.Text = "Colours to use";
			// 
			// colourCountTextBox
			// 
			this.colourCountTextBox.Location = new System.Drawing.Point(93, 354);
			this.colourCountTextBox.Name = "colourCountTextBox";
			this.colourCountTextBox.Size = new System.Drawing.Size(41, 20);
			this.colourCountTextBox.TabIndex = 13;
			this.colourCountTextBox.Text = "255";
			this.colourCountTextBox.TextChanged += new System.EventHandler(this.colourCountTextBox_TextChanged);
			this.colourCountTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.colourCountTextBox_KeyUp);
			// 
			// tColourIndex1
			// 
			this.tColourIndex1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.tColourIndex1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tColourIndex1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
			this.tColourIndex1.Location = new System.Drawing.Point(20, 304);
			this.tColourIndex1.Name = "tColourIndex1";
			this.tColourIndex1.Size = new System.Drawing.Size(41, 20);
			this.tColourIndex1.TabIndex = 15;
			this.tColourIndex1.Text = "0";
			this.tColourIndex1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(167, 361);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(59, 13);
			this.label9.TabIndex = 16;
			this.label9.Text = "Hex Colour";
			// 
			// hexColourTextBox
			// 
			this.hexColourTextBox.Location = new System.Drawing.Point(234, 358);
			this.hexColourTextBox.Name = "hexColourTextBox";
			this.hexColourTextBox.Size = new System.Drawing.Size(62, 20);
			this.hexColourTextBox.TabIndex = 17;
			this.hexColourTextBox.TextChanged += new System.EventHandler(this.hexColourTextBox_TextChanged);
			this.hexColourTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.hexColourTextBox_TextChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Cursor = System.Windows.Forms.Cursors.Default;
			this.label6.Location = new System.Drawing.Point(10, 123);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(128, 13);
			this.label6.TabIndex = 18;
			this.label6.Text = "Select colours from image";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Cursor = System.Windows.Forms.Cursors.Default;
			this.label10.Location = new System.Drawing.Point(10, 180);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(61, 13);
			this.label10.TabIndex = 19;
			this.label10.Text = "Palette files";
			// 
			// copyMenu
			// 
			this.copyMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.selectToolStripMenuItem,
            this.openMixerToolStripMenuItem,
            this.hexValueToolStripMenuItem});
			this.copyMenu.Name = "copyMenu";
			this.copyMenu.Size = new System.Drawing.Size(137, 136);
			// 
			// cutToolStripMenuItem
			// 
			this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
			this.cutToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.cutToolStripMenuItem.Text = "Clear";
			this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.copyToolStripMenuItem.Text = "Copy";
			this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
			// 
			// pasteToolStripMenuItem
			// 
			this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
			this.pasteToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.pasteToolStripMenuItem.Text = "Paste";
			this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
			// 
			// selectToolStripMenuItem
			// 
			this.selectToolStripMenuItem.Name = "selectToolStripMenuItem";
			this.selectToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.selectToolStripMenuItem.Text = "Select";
			// 
			// openMixerToolStripMenuItem
			// 
			this.openMixerToolStripMenuItem.Name = "openMixerToolStripMenuItem";
			this.openMixerToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.openMixerToolStripMenuItem.Text = "Open Mixer";
			this.openMixerToolStripMenuItem.Click += new System.EventHandler(this.ColourToolStripMenuItemOpenMixer);
			// 
			// hexValueToolStripMenuItem
			// 
			this.hexValueToolStripMenuItem.Name = "hexValueToolStripMenuItem";
			this.hexValueToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.hexValueToolStripMenuItem.Text = "Hex Value";
			this.hexValueToolStripMenuItem.Click += new System.EventHandler(this.hexValueToolStripMenuItem_Click);
			// 
			// paletteRowUpButton
			// 
			this.paletteRowUpButton.Location = new System.Drawing.Point(14, 251);
			this.paletteRowUpButton.Name = "paletteRowUpButton";
			this.paletteRowUpButton.Size = new System.Drawing.Size(57, 23);
			this.paletteRowUpButton.TabIndex = 21;
			this.paletteRowUpButton.Text = "Up";
			this.paletteRowUpButton.UseVisualStyleBackColor = true;
			this.paletteRowUpButton.Click += new System.EventHandler(this.paletteRowUpButton_Click);
			// 
			// paletteRowDownButton
			// 
			this.paletteRowDownButton.Location = new System.Drawing.Point(77, 251);
			this.paletteRowDownButton.Name = "paletteRowDownButton";
			this.paletteRowDownButton.Size = new System.Drawing.Size(57, 23);
			this.paletteRowDownButton.TabIndex = 22;
			this.paletteRowDownButton.Text = "Down";
			this.paletteRowDownButton.UseVisualStyleBackColor = true;
			this.paletteRowDownButton.Click += new System.EventHandler(this.paletteRowDownButton_Click);
			// 
			// startIndexTextBox
			// 
			this.startIndexTextBox.Location = new System.Drawing.Point(20, 354);
			this.startIndexTextBox.Name = "startIndexTextBox";
			this.startIndexTextBox.Size = new System.Drawing.Size(41, 20);
			this.startIndexTextBox.TabIndex = 23;
			this.startIndexTextBox.Text = "255";
			this.startIndexTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.startIndexTextBox_KeyDown);
			// 
			// PaletteForm
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(508, 393);
			this.Controls.Add(this.startIndexTextBox);
			this.Controls.Add(this.paletteRowDownButton);
			this.Controls.Add(this.paletteRowUpButton);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.loadPaletteButton);
			this.Controls.Add(this.selectFromImageButton);
			this.Controls.Add(this.hexColourTextBox);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.tColourIndex1);
			this.Controls.Add(this.colourCountTextBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.savePaletteButton);
			this.Controls.Add(this.messageLabel);
			this.Controls.Add(this.transparentColourPickButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "PaletteForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Palette";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PaletteForm_FormClosing);
			this.Load += new System.EventHandler(this.PaletteForm_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.copyMenu.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button loadPaletteButton;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton next256TypeRadioButton;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.RadioButton customTypeRadioButton;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.RadioButton next512TypeRadioButton;
		private System.Windows.Forms.Button selectFromImageButton;
		private System.Windows.Forms.Button transparentColourPickButton;
		private System.Windows.Forms.Label messageLabel;
		private System.Windows.Forms.Button savePaletteButton;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox colourCountTextBox;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private System.Windows.Forms.Label tColourIndex1;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox hexColourTextBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.ContextMenuStrip copyMenu;
		private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openMixerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hexValueToolStripMenuItem;
        private System.Windows.Forms.Button paletteRowUpButton;
        private System.Windows.Forms.Button paletteRowDownButton;
        private System.Windows.Forms.TextBox startIndexTextBox;
    }
}