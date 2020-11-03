namespace NextGraphics
{
	partial class centerPanel
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
			this.TL = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.BR = new System.Windows.Forms.RadioButton();
			this.BC = new System.Windows.Forms.RadioButton();
			this.BL = new System.Windows.Forms.RadioButton();
			this.MR = new System.Windows.Forms.RadioButton();
			this.MC = new System.Windows.Forms.RadioButton();
			this.ML = new System.Windows.Forms.RadioButton();
			this.TR = new System.Windows.Forms.RadioButton();
			this.TC = new System.Windows.Forms.RadioButton();
			this.button1 = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
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
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.BR);
			this.groupBox1.Controls.Add(this.BC);
			this.groupBox1.Controls.Add(this.BL);
			this.groupBox1.Controls.Add(this.MR);
			this.groupBox1.Controls.Add(this.MC);
			this.groupBox1.Controls.Add(this.ML);
			this.groupBox1.Controls.Add(this.TR);
			this.groupBox1.Controls.Add(this.TC);
			this.groupBox1.Controls.Add(this.TL);
			this.groupBox1.Location = new System.Drawing.Point(13, 13);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(134, 145);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Centre";
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
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new System.Drawing.Point(41, 164);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 3;
			this.button1.Text = "Close";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.okButonClick);
			// 
			// centerPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(164, 202);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "centerPanel";
			this.Text = "Centre";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button button1;
		public System.Windows.Forms.RadioButton TL;
		public System.Windows.Forms.RadioButton BR;
		public System.Windows.Forms.RadioButton BC;
		public System.Windows.Forms.RadioButton BL;
		public System.Windows.Forms.RadioButton MR;
		public System.Windows.Forms.RadioButton MC;
		public System.Windows.Forms.RadioButton ML;
		public System.Windows.Forms.RadioButton TR;
		public System.Windows.Forms.RadioButton TC;
	}
}