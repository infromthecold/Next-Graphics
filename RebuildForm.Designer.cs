namespace NextGraphics
{
	partial class RebuildForm
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
			this.blocksInRowLabel = new System.Windows.Forms.Label();
			this.blocksInRowTextBox = new System.Windows.Forms.TextBox();
			this.rebuildButton = new System.Windows.Forms.Button();
			this.blockFileTextBox = new System.Windows.Forms.TextBox();
			this.blockFileLabel = new System.Windows.Forms.Label();
			this.browseButton = new System.Windows.Forms.Button();
			this.saveAsButton = new System.Windows.Forms.Button();
			this.outputFileLabel = new System.Windows.Forms.Label();
			this.outputFileTextBox = new System.Windows.Forms.TextBox();
			this.zeroBlockCheckBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// blocksInRowLabel
			// 
			this.blocksInRowLabel.AutoSize = true;
			this.blocksInRowLabel.Location = new System.Drawing.Point(13, 81);
			this.blocksInRowLabel.Name = "blocksInRowLabel";
			this.blocksInRowLabel.Size = new System.Drawing.Size(85, 13);
			this.blocksInRowLabel.TabIndex = 0;
			this.blocksInRowLabel.Text = "Blocks in a row?";
			// 
			// blocksInRowTextBox
			// 
			this.blocksInRowTextBox.Location = new System.Drawing.Point(110, 78);
			this.blocksInRowTextBox.Name = "blocksInRowTextBox";
			this.blocksInRowTextBox.Size = new System.Drawing.Size(37, 20);
			this.blocksInRowTextBox.TabIndex = 1;
			this.blocksInRowTextBox.Text = "4";
			// 
			// rebuildButton
			// 
			this.rebuildButton.Location = new System.Drawing.Point(187, 103);
			this.rebuildButton.Name = "rebuildButton";
			this.rebuildButton.Size = new System.Drawing.Size(75, 23);
			this.rebuildButton.TabIndex = 2;
			this.rebuildButton.Text = "Rebuild";
			this.rebuildButton.UseVisualStyleBackColor = true;
			this.rebuildButton.Click += new System.EventHandler(this.rebuildButton_Click);
			// 
			// blockFileTextBox
			// 
			this.blockFileTextBox.Location = new System.Drawing.Point(110, 15);
			this.blockFileTextBox.Name = "blockFileTextBox";
			this.blockFileTextBox.Size = new System.Drawing.Size(244, 20);
			this.blockFileTextBox.TabIndex = 3;
			// 
			// blockFileLabel
			// 
			this.blockFileLabel.AutoSize = true;
			this.blockFileLabel.Location = new System.Drawing.Point(13, 18);
			this.blockFileLabel.Name = "blockFileLabel";
			this.blockFileLabel.Size = new System.Drawing.Size(76, 13);
			this.blockFileLabel.TabIndex = 4;
			this.blockFileLabel.Text = "Block file (.blk)";
			// 
			// browseButton
			// 
			this.browseButton.Location = new System.Drawing.Point(360, 13);
			this.browseButton.Name = "browseButton";
			this.browseButton.Size = new System.Drawing.Size(75, 23);
			this.browseButton.TabIndex = 5;
			this.browseButton.Text = "Browse";
			this.browseButton.UseVisualStyleBackColor = true;
			this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
			// 
			// saveAsButton
			// 
			this.saveAsButton.Location = new System.Drawing.Point(360, 46);
			this.saveAsButton.Name = "saveAsButton";
			this.saveAsButton.Size = new System.Drawing.Size(75, 23);
			this.saveAsButton.TabIndex = 8;
			this.saveAsButton.Text = "Save As";
			this.saveAsButton.UseVisualStyleBackColor = true;
			this.saveAsButton.Click += new System.EventHandler(this.saveAsButton_Click);
			// 
			// outputFileLabel
			// 
			this.outputFileLabel.AutoSize = true;
			this.outputFileLabel.Location = new System.Drawing.Point(13, 51);
			this.outputFileLabel.Name = "outputFileLabel";
			this.outputFileLabel.Size = new System.Drawing.Size(52, 13);
			this.outputFileLabel.TabIndex = 7;
			this.outputFileLabel.Text = "Ouput file";
			// 
			// outputFileTextBox
			// 
			this.outputFileTextBox.Location = new System.Drawing.Point(110, 48);
			this.outputFileTextBox.Name = "outputFileTextBox";
			this.outputFileTextBox.Size = new System.Drawing.Size(244, 20);
			this.outputFileTextBox.TabIndex = 6;
			// 
			// zeroBlockCheckBox
			// 
			this.zeroBlockCheckBox.AutoSize = true;
			this.zeroBlockCheckBox.Location = new System.Drawing.Point(164, 80);
			this.zeroBlockCheckBox.Name = "zeroBlockCheckBox";
			this.zeroBlockCheckBox.Size = new System.Drawing.Size(149, 17);
			this.zeroBlockCheckBox.TabIndex = 10;
			this.zeroBlockCheckBox.Text = "Output empty block Zero?";
			this.zeroBlockCheckBox.UseVisualStyleBackColor = true;
			// 
			// RebuildForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(451, 135);
			this.Controls.Add(this.zeroBlockCheckBox);
			this.Controls.Add(this.saveAsButton);
			this.Controls.Add(this.outputFileLabel);
			this.Controls.Add(this.outputFileTextBox);
			this.Controls.Add(this.browseButton);
			this.Controls.Add(this.blockFileLabel);
			this.Controls.Add(this.blockFileTextBox);
			this.Controls.Add(this.rebuildButton);
			this.Controls.Add(this.blocksInRowTextBox);
			this.Controls.Add(this.blocksInRowLabel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "RebuildForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Rebuild from blk file";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label blocksInRowLabel;
		public System.Windows.Forms.TextBox blocksInRowTextBox;
		private System.Windows.Forms.Button rebuildButton;
		private System.Windows.Forms.TextBox blockFileTextBox;
		private System.Windows.Forms.Label blockFileLabel;
		private System.Windows.Forms.Button browseButton;
		private System.Windows.Forms.Button saveAsButton;
		private System.Windows.Forms.Label outputFileLabel;
		private System.Windows.Forms.TextBox outputFileTextBox;
		private System.Windows.Forms.CheckBox zeroBlockCheckBox;
	}
}