namespace NextGraphics
{
	partial class Progress
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
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.label1 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// progressBar1
			// 
			this.progressBar1.Location = new System.Drawing.Point(96, 22);
			this.progressBar1.MarqueeAnimationSpeed = 50;
			this.progressBar1.Maximum = 388;
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(388, 25);
			this.progressBar1.Step = 1;
			this.progressBar1.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this.label1.Location = new System.Drawing.Point(12, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(78, 17);
			this.label1.TabIndex = 1;
			this.label1.Text = "Processing";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(212, 67);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(89, 27);
			this.button1.TabIndex = 2;
			this.button1.Text = "Cancel";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// Progress
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(496, 112);
			this.ControlBox = false;
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.progressBar1);
			this.MaximumSize = new System.Drawing.Size(512, 151);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(512, 151);
			this.Name = "Progress";
			this.Text = "Progress";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button1;
		public System.Windows.Forms.ProgressBar progressBar1;
	}
}