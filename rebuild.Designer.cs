namespace NextGraphics
{
	partial class rebuild
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
			this.label1 = new System.Windows.Forms.Label();
			this.blocksRow = new System.Windows.Forms.TextBox();
			this.rebuildButton = new System.Windows.Forms.Button();
			this.inPath = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.Browse = new System.Windows.Forms.Button();
			this.SaveAs = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.outPath = new System.Windows.Forms.TextBox();
			this.zeroBlock = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 81);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(85, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Blocks in a row?";
			// 
			// blocksRow
			// 
			this.blocksRow.Location = new System.Drawing.Point(110, 78);
			this.blocksRow.Name = "blocksRow";
			this.blocksRow.Size = new System.Drawing.Size(37, 20);
			this.blocksRow.TabIndex = 1;
			this.blocksRow.Text = "4";
			// 
			// rebuildButton
			// 
			this.rebuildButton.Location = new System.Drawing.Point(187, 103);
			this.rebuildButton.Name = "rebuildButton";
			this.rebuildButton.Size = new System.Drawing.Size(75, 23);
			this.rebuildButton.TabIndex = 2;
			this.rebuildButton.Text = "Rebuild";
			this.rebuildButton.UseVisualStyleBackColor = true;
			this.rebuildButton.Click += new System.EventHandler(this.doRebuildClick);
			// 
			// inPath
			// 
			this.inPath.Location = new System.Drawing.Point(110, 15);
			this.inPath.Name = "inPath";
			this.inPath.Size = new System.Drawing.Size(244, 20);
			this.inPath.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 18);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(76, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Block file (.blk)";
			// 
			// Browse
			// 
			this.Browse.Location = new System.Drawing.Point(360, 13);
			this.Browse.Name = "Browse";
			this.Browse.Size = new System.Drawing.Size(75, 23);
			this.Browse.TabIndex = 5;
			this.Browse.Text = "Browse";
			this.Browse.UseVisualStyleBackColor = true;
			this.Browse.Click += new System.EventHandler(this.Browse_Click);
			// 
			// SaveAs
			// 
			this.SaveAs.Location = new System.Drawing.Point(360, 46);
			this.SaveAs.Name = "SaveAs";
			this.SaveAs.Size = new System.Drawing.Size(75, 23);
			this.SaveAs.TabIndex = 8;
			this.SaveAs.Text = "Save As";
			this.SaveAs.UseVisualStyleBackColor = true;
			this.SaveAs.Click += new System.EventHandler(this.SaveAs_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 51);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(52, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Ouput file";
			// 
			// outPath
			// 
			this.outPath.Location = new System.Drawing.Point(110, 48);
			this.outPath.Name = "outPath";
			this.outPath.Size = new System.Drawing.Size(244, 20);
			this.outPath.TabIndex = 6;
			// 
			// zeroBlock
			// 
			this.zeroBlock.AutoSize = true;
			this.zeroBlock.Location = new System.Drawing.Point(164, 80);
			this.zeroBlock.Name = "zeroBlock";
			this.zeroBlock.Size = new System.Drawing.Size(149, 17);
			this.zeroBlock.TabIndex = 10;
			this.zeroBlock.Text = "Output empty block Zero?";
			this.zeroBlock.UseVisualStyleBackColor = true;
			// 
			// rebuild
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(451, 135);
			this.Controls.Add(this.zeroBlock);
			this.Controls.Add(this.SaveAs);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.outPath);
			this.Controls.Add(this.Browse);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.inPath);
			this.Controls.Add(this.rebuildButton);
			this.Controls.Add(this.blocksRow);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "rebuild";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Rebuild from blk file";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.TextBox blocksRow;
		private System.Windows.Forms.Button rebuildButton;
		private System.Windows.Forms.TextBox inPath;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button Browse;
		private System.Windows.Forms.Button SaveAs;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox outPath;
		private System.Windows.Forms.CheckBox zeroBlock;
	}
}