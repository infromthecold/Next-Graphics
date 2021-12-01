namespace NextGraphics
{
	partial class settingsPanel
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
			this.mirrorX = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.Accuracy = new System.Windows.Forms.TextBox();
			this.Transparent = new System.Windows.Forms.CheckBox();
			this.Repeats = new System.Windows.Forms.CheckBox();
			this.mirrorY = new System.Windows.Forms.CheckBox();
			this.rotations = new System.Windows.Forms.CheckBox();
			this.sortTransparent = new System.Windows.Forms.CheckBox();
			this.button1 = new System.Windows.Forms.Button();
			this.comments = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.reduce = new System.Windows.Forms.CheckBox();
			this.binaryBlocks = new System.Windows.Forms.CheckBox();
			this.FourBit = new System.Windows.Forms.CheckBox();
			this.binaryOut = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.tilesOut = new System.Windows.Forms.CheckBox();
			this.blocksOut = new System.Windows.Forms.CheckBox();
			this.tilesAcross = new System.Windows.Forms.TextBox();
			this.blocksFormat = new System.Windows.Forms.ComboBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.BR = new System.Windows.Forms.RadioButton();
			this.BC = new System.Windows.Forms.RadioButton();
			this.BL = new System.Windows.Forms.RadioButton();
			this.MR = new System.Windows.Forms.RadioButton();
			this.MC = new System.Windows.Forms.RadioButton();
			this.ML = new System.Windows.Forms.RadioButton();
			this.TR = new System.Windows.Forms.RadioButton();
			this.TC = new System.Windows.Forms.RadioButton();
			this.TL = new System.Windows.Forms.RadioButton();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.transTile = new System.Windows.Forms.CheckBox();
			this.transBlock = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textFlips = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.SuspendLayout();
			// 
			// mirrorX
			// 
			this.mirrorX.AutoSize = true;
			this.mirrorX.Location = new System.Drawing.Point(6, 50);
			this.mirrorX.Name = "mirrorX";
			this.mirrorX.Size = new System.Drawing.Size(74, 17);
			this.mirrorX.TabIndex = 0;
			this.mirrorX.Text = "Mirrored-X";
			this.mirrorX.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.Accuracy);
			this.groupBox1.Controls.Add(this.Transparent);
			this.groupBox1.Controls.Add(this.Repeats);
			this.groupBox1.Controls.Add(this.mirrorY);
			this.groupBox1.Controls.Add(this.mirrorX);
			this.groupBox1.Controls.Add(this.rotations);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(130, 143);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Ignore repeats of ";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(111, 28);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(15, 13);
			this.label3.TabIndex = 31;
			this.label3.Text = "%";
			// 
			// Accuracy
			// 
			this.Accuracy.Location = new System.Drawing.Point(86, 24);
			this.Accuracy.Name = "Accuracy";
			this.Accuracy.Size = new System.Drawing.Size(24, 20);
			this.Accuracy.TabIndex = 30;
			this.Accuracy.Text = "100";
			this.Accuracy.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			// 
			// Transparent
			// 
			this.Transparent.AutoSize = true;
			this.Transparent.Location = new System.Drawing.Point(6, 119);
			this.Transparent.Name = "Transparent";
			this.Transparent.Size = new System.Drawing.Size(113, 17);
			this.Transparent.TabIndex = 5;
			this.Transparent.Text = "Transparent Pixels";
			this.Transparent.UseVisualStyleBackColor = true;
			// 
			// Repeats
			// 
			this.Repeats.AutoSize = true;
			this.Repeats.Location = new System.Drawing.Point(6, 27);
			this.Repeats.Name = "Repeats";
			this.Repeats.Size = new System.Drawing.Size(58, 17);
			this.Repeats.TabIndex = 4;
			this.Repeats.Text = "Copies";
			this.Repeats.UseVisualStyleBackColor = true;
			// 
			// mirrorY
			// 
			this.mirrorY.AutoSize = true;
			this.mirrorY.Location = new System.Drawing.Point(6, 73);
			this.mirrorY.Name = "mirrorY";
			this.mirrorY.Size = new System.Drawing.Size(74, 17);
			this.mirrorY.TabIndex = 2;
			this.mirrorY.Text = "Mirrored-Y";
			this.mirrorY.UseVisualStyleBackColor = true;
			// 
			// rotations
			// 
			this.rotations.AutoSize = true;
			this.rotations.Location = new System.Drawing.Point(6, 96);
			this.rotations.Name = "rotations";
			this.rotations.Size = new System.Drawing.Size(73, 17);
			this.rotations.TabIndex = 3;
			this.rotations.Text = "Rotatated";
			this.rotations.UseVisualStyleBackColor = true;
			// 
			// sortTransparent
			// 
			this.sortTransparent.AutoSize = true;
			this.sortTransparent.Location = new System.Drawing.Point(6, 27);
			this.sortTransparent.Name = "sortTransparent";
			this.sortTransparent.Size = new System.Drawing.Size(105, 17);
			this.sortTransparent.TabIndex = 6;
			this.sortTransparent.Text = "Transparent First";
			this.sortTransparent.UseVisualStyleBackColor = true;
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new System.Drawing.Point(139, 312);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 2;
			this.button1.Text = "Close";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// comments
			// 
			this.comments.FormattingEnabled = true;
			this.comments.Items.AddRange(new object[] {
            "No comments",
            "Full comments"});
			this.comments.Location = new System.Drawing.Point(6, 111);
			this.comments.Name = "comments";
			this.comments.Size = new System.Drawing.Size(163, 21);
			this.comments.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 95);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Comment Level";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.textFlips);
			this.groupBox2.Controls.Add(this.reduce);
			this.groupBox2.Controls.Add(this.binaryBlocks);
			this.groupBox2.Controls.Add(this.FourBit);
			this.groupBox2.Controls.Add(this.binaryOut);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.sortTransparent);
			this.groupBox2.Controls.Add(this.comments);
			this.groupBox2.Location = new System.Drawing.Point(156, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(221, 143);
			this.groupBox2.TabIndex = 7;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Output Settings";
			// 
			// reduce
			// 
			this.reduce.AutoSize = true;
			this.reduce.Location = new System.Drawing.Point(114, 50);
			this.reduce.Name = "reduce";
			this.reduce.Size = new System.Drawing.Size(70, 17);
			this.reduce.TabIndex = 22;
			this.reduce.Text = "Reduced";
			this.reduce.UseVisualStyleBackColor = true;
			// 
			// binaryBlocks
			// 
			this.binaryBlocks.AutoSize = true;
			this.binaryBlocks.Location = new System.Drawing.Point(114, 73);
			this.binaryBlocks.Name = "binaryBlocks";
			this.binaryBlocks.Size = new System.Drawing.Size(103, 17);
			this.binaryBlocks.TabIndex = 21;
			this.binaryBlocks.Text = "Frames/Blocks+";
			this.binaryBlocks.UseVisualStyleBackColor = true;
			// 
			// FourBit
			// 
			this.FourBit.AutoSize = true;
			this.FourBit.Location = new System.Drawing.Point(6, 50);
			this.FourBit.Name = "FourBit";
			this.FourBit.Size = new System.Drawing.Size(82, 17);
			this.FourBit.TabIndex = 17;
			this.FourBit.Text = "4 Bit Sprites";
			this.FourBit.UseVisualStyleBackColor = true;
			// 
			// binaryOut
			// 
			this.binaryOut.AutoSize = true;
			this.binaryOut.Location = new System.Drawing.Point(6, 73);
			this.binaryOut.Name = "binaryOut";
			this.binaryOut.Size = new System.Drawing.Size(61, 17);
			this.binaryOut.TabIndex = 20;
			this.binaryOut.Text = "Binary+";
			this.binaryOut.UseVisualStyleBackColor = true;
			this.binaryOut.CheckedChanged += new System.EventHandler(this.binaryOut_CheckedChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(73, 74);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(101, 13);
			this.label4.TabIndex = 26;
			this.label4.Text = "Blocks/Tiles Across";
			// 
			// tilesOut
			// 
			this.tilesOut.AutoSize = true;
			this.tilesOut.Location = new System.Drawing.Point(9, 48);
			this.tilesOut.Name = "tilesOut";
			this.tilesOut.Size = new System.Drawing.Size(117, 17);
			this.tilesOut.TabIndex = 22;
			this.tilesOut.Text = "Tiles/Sprites Image";
			this.tilesOut.UseVisualStyleBackColor = true;
			// 
			// blocksOut
			// 
			this.blocksOut.AutoSize = true;
			this.blocksOut.Location = new System.Drawing.Point(9, 25);
			this.blocksOut.Name = "blocksOut";
			this.blocksOut.Size = new System.Drawing.Size(104, 17);
			this.blocksOut.TabIndex = 21;
			this.blocksOut.Text = "Blocks as Image";
			this.blocksOut.UseVisualStyleBackColor = true;
			// 
			// tilesAcross
			// 
			this.tilesAcross.Location = new System.Drawing.Point(9, 71);
			this.tilesAcross.Name = "tilesAcross";
			this.tilesAcross.Size = new System.Drawing.Size(58, 20);
			this.tilesAcross.TabIndex = 24;
			this.tilesAcross.Text = "1";
			// 
			// blocksFormat
			// 
			this.blocksFormat.FormattingEnabled = true;
			this.blocksFormat.Items.AddRange(new object[] {
            "BMP",
            "PNG",
            "JPG"});
			this.blocksFormat.Location = new System.Drawing.Point(9, 97);
			this.blocksFormat.Name = "blocksFormat";
			this.blocksFormat.Size = new System.Drawing.Size(58, 21);
			this.blocksFormat.TabIndex = 23;
			this.blocksFormat.Text = "Format";
			// 
			// groupBox3
			// 
			this.groupBox3.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox3.Controls.Add(this.BR);
			this.groupBox3.Controls.Add(this.BC);
			this.groupBox3.Controls.Add(this.BL);
			this.groupBox3.Controls.Add(this.MR);
			this.groupBox3.Controls.Add(this.MC);
			this.groupBox3.Controls.Add(this.ML);
			this.groupBox3.Controls.Add(this.TR);
			this.groupBox3.Controls.Add(this.TC);
			this.groupBox3.Controls.Add(this.TL);
			this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.groupBox3.Location = new System.Drawing.Point(12, 161);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(130, 145);
			this.groupBox3.TabIndex = 8;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Sprite Centre";
			// 
			// BR
			// 
			this.BR.AutoSize = true;
			this.BR.Location = new System.Drawing.Point(102, 120);
			this.BR.Name = "BR";
			this.BR.Size = new System.Drawing.Size(14, 13);
			this.BR.TabIndex = 8;
			this.BR.TabStop = true;
			this.BR.UseVisualStyleBackColor = true;
			// 
			// BC
			// 
			this.BC.AutoSize = true;
			this.BC.Location = new System.Drawing.Point(59, 120);
			this.BC.Name = "BC";
			this.BC.Size = new System.Drawing.Size(14, 13);
			this.BC.TabIndex = 7;
			this.BC.TabStop = true;
			this.BC.UseVisualStyleBackColor = true;
			// 
			// BL
			// 
			this.BL.AutoSize = true;
			this.BL.Location = new System.Drawing.Point(16, 120);
			this.BL.Name = "BL";
			this.BL.Size = new System.Drawing.Size(14, 13);
			this.BL.TabIndex = 6;
			this.BL.TabStop = true;
			this.BL.UseVisualStyleBackColor = true;
			// 
			// MR
			// 
			this.MR.AutoSize = true;
			this.MR.Location = new System.Drawing.Point(102, 74);
			this.MR.Name = "MR";
			this.MR.Size = new System.Drawing.Size(14, 13);
			this.MR.TabIndex = 5;
			this.MR.TabStop = true;
			this.MR.UseVisualStyleBackColor = true;
			// 
			// MC
			// 
			this.MC.AutoSize = true;
			this.MC.Location = new System.Drawing.Point(59, 74);
			this.MC.Name = "MC";
			this.MC.Size = new System.Drawing.Size(14, 13);
			this.MC.TabIndex = 4;
			this.MC.TabStop = true;
			this.MC.UseVisualStyleBackColor = true;
			// 
			// ML
			// 
			this.ML.AutoSize = true;
			this.ML.Location = new System.Drawing.Point(16, 74);
			this.ML.Name = "ML";
			this.ML.Size = new System.Drawing.Size(14, 13);
			this.ML.TabIndex = 3;
			this.ML.TabStop = true;
			this.ML.UseVisualStyleBackColor = true;
			// 
			// TR
			// 
			this.TR.AutoSize = true;
			this.TR.Location = new System.Drawing.Point(102, 29);
			this.TR.Name = "TR";
			this.TR.Size = new System.Drawing.Size(14, 13);
			this.TR.TabIndex = 2;
			this.TR.TabStop = true;
			this.TR.UseVisualStyleBackColor = true;
			// 
			// TC
			// 
			this.TC.AutoSize = true;
			this.TC.Location = new System.Drawing.Point(59, 29);
			this.TC.Name = "TC";
			this.TC.Size = new System.Drawing.Size(14, 13);
			this.TC.TabIndex = 1;
			this.TC.TabStop = true;
			this.TC.UseVisualStyleBackColor = true;
			// 
			// TL
			// 
			this.TL.AutoSize = true;
			this.TL.Location = new System.Drawing.Point(16, 29);
			this.TL.Name = "TL";
			this.TL.Size = new System.Drawing.Size(14, 13);
			this.TL.TabIndex = 0;
			this.TL.TabStop = true;
			this.TL.UseVisualStyleBackColor = true;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.transTile);
			this.groupBox4.Controls.Add(this.transBlock);
			this.groupBox4.Controls.Add(this.label2);
			this.groupBox4.Controls.Add(this.blocksOut);
			this.groupBox4.Controls.Add(this.label4);
			this.groupBox4.Controls.Add(this.tilesOut);
			this.groupBox4.Controls.Add(this.tilesAcross);
			this.groupBox4.Controls.Add(this.blocksFormat);
			this.groupBox4.Location = new System.Drawing.Point(156, 161);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(221, 145);
			this.groupBox4.TabIndex = 27;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Mapping Blocks";
			// 
			// transTile
			// 
			this.transTile.AutoSize = true;
			this.transTile.Location = new System.Drawing.Point(123, 48);
			this.transTile.Name = "transTile";
			this.transTile.Size = new System.Drawing.Size(98, 17);
			this.transTile.TabIndex = 29;
			this.transTile.Text = "Transparent (0)";
			this.transTile.UseVisualStyleBackColor = true;
			// 
			// transBlock
			// 
			this.transBlock.AutoSize = true;
			this.transBlock.Location = new System.Drawing.Point(123, 25);
			this.transBlock.Name = "transBlock";
			this.transBlock.Size = new System.Drawing.Size(98, 17);
			this.transBlock.TabIndex = 28;
			this.transBlock.Text = "Transparent (0)";
			this.transBlock.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(73, 100);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(55, 13);
			this.label2.TabIndex = 27;
			this.label2.Text = "File format";
			// 
			// textFlips
			// 
			this.textFlips.AutoSize = true;
			this.textFlips.Location = new System.Drawing.Point(114, 28);
			this.textFlips.Name = "textFlips";
			this.textFlips.Size = new System.Drawing.Size(96, 17);
			this.textFlips.TabIndex = 30;
			this.textFlips.Text = "Text Flips Rots";
			this.textFlips.UseVisualStyleBackColor = true;
			// 
			// settingsPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(385, 342);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupBox2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "settingsPanel";
			this.Text = "Options";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button button1;
		public System.Windows.Forms.CheckBox mirrorX;
		public System.Windows.Forms.CheckBox mirrorY;
		public System.Windows.Forms.CheckBox rotations;
		public System.Windows.Forms.CheckBox Repeats;
		public System.Windows.Forms.CheckBox Transparent;
		public System.Windows.Forms.ComboBox comments;
		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.CheckBox sortTransparent;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		public System.Windows.Forms.RadioButton BR;
		public System.Windows.Forms.RadioButton BC;
		public System.Windows.Forms.RadioButton BL;
		public System.Windows.Forms.RadioButton MR;
		public System.Windows.Forms.RadioButton MC;
		public System.Windows.Forms.RadioButton ML;
		public System.Windows.Forms.RadioButton TR;
		public System.Windows.Forms.RadioButton TC;
		public System.Windows.Forms.RadioButton TL;
		public System.Windows.Forms.CheckBox binaryOut;
		public System.Windows.Forms.ComboBox blocksFormat;
		public System.Windows.Forms.CheckBox tilesOut;
		public System.Windows.Forms.CheckBox blocksOut;
		public System.Windows.Forms.CheckBox FourBit;
		public System.Windows.Forms.Label label4;
		public System.Windows.Forms.TextBox tilesAcross;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label label2;
		public System.Windows.Forms.CheckBox transBlock;
		public System.Windows.Forms.CheckBox transTile;
		public System.Windows.Forms.CheckBox binaryBlocks;
		public System.Windows.Forms.TextBox Accuracy;
		private System.Windows.Forms.Label label3;
		public System.Windows.Forms.CheckBox reduce;
		public System.Windows.Forms.CheckBox textFlips;
	}
}