namespace NextGraphics
{
	partial class ImageSelectForm
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
			this.imagesListBox = new System.Windows.Forms.ListBox();
			this.cancelButton = new System.Windows.Forms.Button();
			this.importButton = new System.Windows.Forms.Button();
			this.fromTextBox = new System.Windows.Forms.TextBox();
			this.fromLabel = new System.Windows.Forms.Label();
			this.countLabel = new System.Windows.Forms.Label();
			this.countTextBox = new System.Windows.Forms.TextBox();
			this.toLabel = new System.Windows.Forms.Label();
			this.toTextBox = new System.Windows.Forms.TextBox();
			this.importSettingsGroupBox = new System.Windows.Forms.GroupBox();
			this.copyMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openMixerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.copyMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// imagesListBox
			// 
			this.imagesListBox.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			this.imagesListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.imagesListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this.imagesListBox.FormattingEnabled = true;
			this.imagesListBox.ItemHeight = 16;
			this.imagesListBox.Location = new System.Drawing.Point(343, 15);
			this.imagesListBox.Margin = new System.Windows.Forms.Padding(10);
			this.imagesListBox.Name = "imagesListBox";
			this.imagesListBox.ScrollAlwaysVisible = true;
			this.imagesListBox.Size = new System.Drawing.Size(156, 322);
			this.imagesListBox.TabIndex = 1;
			this.imagesListBox.SelectedIndexChanged += new System.EventHandler(this.imagesListBox_SelectedIndexChanged);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(342, 366);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 8;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// importButton
			// 
			this.importButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.importButton.Location = new System.Drawing.Point(424, 367);
			this.importButton.Name = "importButton";
			this.importButton.Size = new System.Drawing.Size(75, 23);
			this.importButton.TabIndex = 7;
			this.importButton.Text = "Import";
			this.importButton.UseVisualStyleBackColor = true;
			this.importButton.Click += new System.EventHandler(this.importButton_Click);
			// 
			// fromTextBox
			// 
			this.fromTextBox.Location = new System.Drawing.Point(59, 368);
			this.fromTextBox.Name = "fromTextBox";
			this.fromTextBox.Size = new System.Drawing.Size(39, 20);
			this.fromTextBox.TabIndex = 9;
			this.fromTextBox.TextChanged += new System.EventHandler(this.fromTextBox_TextChanged);
			// 
			// fromLabel
			// 
			this.fromLabel.AutoSize = true;
			this.fromLabel.Location = new System.Drawing.Point(27, 371);
			this.fromLabel.Name = "fromLabel";
			this.fromLabel.Size = new System.Drawing.Size(30, 13);
			this.fromLabel.TabIndex = 10;
			this.fromLabel.Text = "From";
			// 
			// countLabel
			// 
			this.countLabel.AutoSize = true;
			this.countLabel.Location = new System.Drawing.Point(115, 371);
			this.countLabel.Name = "countLabel";
			this.countLabel.Size = new System.Drawing.Size(35, 13);
			this.countLabel.TabIndex = 12;
			this.countLabel.Text = "Count";
			// 
			// countTextBox
			// 
			this.countTextBox.Location = new System.Drawing.Point(152, 368);
			this.countTextBox.Name = "countTextBox";
			this.countTextBox.Size = new System.Drawing.Size(38, 20);
			this.countTextBox.TabIndex = 11;
			this.countTextBox.TextChanged += new System.EventHandler(this.countTextBox_TextChanged);
			// 
			// toLabel
			// 
			this.toLabel.AutoSize = true;
			this.toLabel.Location = new System.Drawing.Point(201, 371);
			this.toLabel.Name = "toLabel";
			this.toLabel.Size = new System.Drawing.Size(51, 13);
			this.toLabel.TabIndex = 14;
			this.toLabel.Text = "Where to";
			// 
			// toTextBox
			// 
			this.toTextBox.Location = new System.Drawing.Point(254, 368);
			this.toTextBox.Name = "toTextBox";
			this.toTextBox.Size = new System.Drawing.Size(47, 20);
			this.toTextBox.TabIndex = 13;
			this.toTextBox.TextChanged += new System.EventHandler(this.toTextBox_TextChanged);
			// 
			// importSettingsGroupBox
			// 
			this.importSettingsGroupBox.Location = new System.Drawing.Point(13, 347);
			this.importSettingsGroupBox.Name = "importSettingsGroupBox";
			this.importSettingsGroupBox.Size = new System.Drawing.Size(302, 54);
			this.importSettingsGroupBox.TabIndex = 15;
			this.importSettingsGroupBox.TabStop = false;
			this.importSettingsGroupBox.Text = "Import Settings";
			// 
			// copyMenu
			// 
			this.copyMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.openMixerToolStripMenuItem});
			this.copyMenu.Name = "copyMenu";
			this.copyMenu.Size = new System.Drawing.Size(137, 92);
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
			// openMixerToolStripMenuItem
			// 
			this.openMixerToolStripMenuItem.Name = "openMixerToolStripMenuItem";
			this.openMixerToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.openMixerToolStripMenuItem.Text = "Open Mixer";
			this.openMixerToolStripMenuItem.Click += new System.EventHandler(this.openMixerToolStripMenuItem_Click);
			// 
			// ImageSelectForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
			this.ClientSize = new System.Drawing.Size(513, 405);
			this.Controls.Add(this.toLabel);
			this.Controls.Add(this.toTextBox);
			this.Controls.Add(this.countLabel);
			this.Controls.Add(this.countTextBox);
			this.Controls.Add(this.fromLabel);
			this.Controls.Add(this.fromTextBox);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.importButton);
			this.Controls.Add(this.imagesListBox);
			this.Controls.Add(this.importSettingsGroupBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MinimizeBox = false;
			this.Name = "ImageSelectForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Import Palette";
			this.copyMenu.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button importButton;
		private System.Windows.Forms.ListBox imagesListBox;
		private System.Windows.Forms.Label fromLabel;
		private System.Windows.Forms.Label countLabel;
		private System.Windows.Forms.Label toLabel;
		private System.Windows.Forms.GroupBox importSettingsGroupBox;
		private System.Windows.Forms.ContextMenuStrip copyMenu;
		private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openMixerToolStripMenuItem;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private System.Windows.Forms.TextBox fromTextBox;
		private System.Windows.Forms.TextBox countTextBox;
		private System.Windows.Forms.TextBox toTextBox;
    }
}