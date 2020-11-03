namespace NextGraphics
{
	partial class IgnorePanel
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
			this.Transparent = new System.Windows.Forms.CheckBox();
			this.Repeats = new System.Windows.Forms.CheckBox();
			this.mirrorY = new System.Windows.Forms.CheckBox();
			this.rotations = new System.Windows.Forms.CheckBox();
			this.button1 = new System.Windows.Forms.Button();
			this.comments = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// mirrorX
			// 
			this.mirrorX.AutoSize = true;
			this.mirrorX.Location = new System.Drawing.Point(31, 43);
			this.mirrorX.Name = "mirrorX";
			this.mirrorX.Size = new System.Drawing.Size(62, 17);
			this.mirrorX.TabIndex = 0;
			this.mirrorX.Text = "Mirror-X";
			this.mirrorX.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.Transparent);
			this.groupBox1.Controls.Add(this.Repeats);
			this.groupBox1.Controls.Add(this.mirrorY);
			this.groupBox1.Controls.Add(this.mirrorX);
			this.groupBox1.Controls.Add(this.rotations);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(137, 136);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Ignore";
			// 
			// Transparent
			// 
			this.Transparent.AutoSize = true;
			this.Transparent.Location = new System.Drawing.Point(31, 112);
			this.Transparent.Name = "Transparent";
			this.Transparent.Size = new System.Drawing.Size(83, 17);
			this.Transparent.TabIndex = 5;
			this.Transparent.Text = "Transparent";
			this.Transparent.UseVisualStyleBackColor = true;
			// 
			// Repeats
			// 
			this.Repeats.AutoSize = true;
			this.Repeats.Location = new System.Drawing.Point(31, 20);
			this.Repeats.Name = "Repeats";
			this.Repeats.Size = new System.Drawing.Size(66, 17);
			this.Repeats.TabIndex = 4;
			this.Repeats.Text = "Repeats";
			this.Repeats.UseVisualStyleBackColor = true;
			// 
			// mirrorY
			// 
			this.mirrorY.AutoSize = true;
			this.mirrorY.Location = new System.Drawing.Point(31, 66);
			this.mirrorY.Name = "mirrorY";
			this.mirrorY.Size = new System.Drawing.Size(62, 17);
			this.mirrorY.TabIndex = 2;
			this.mirrorY.Text = "Mirror-Y";
			this.mirrorY.UseVisualStyleBackColor = true;
			// 
			// rotations
			// 
			this.rotations.AutoSize = true;
			this.rotations.Location = new System.Drawing.Point(31, 89);
			this.rotations.Name = "rotations";
			this.rotations.Size = new System.Drawing.Size(71, 17);
			this.rotations.TabIndex = 3;
			this.rotations.Text = "Rotations";
			this.rotations.UseVisualStyleBackColor = true;
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new System.Drawing.Point(43, 206);
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
			this.comments.Location = new System.Drawing.Point(12, 179);
			this.comments.Name = "comments";
			this.comments.Size = new System.Drawing.Size(137, 21);
			this.comments.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 160);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Comment Level";
			// 
			// IgnorePanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(161, 241);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comments);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "IgnorePanel";
			this.Text = "Ignore";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

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
	}
}