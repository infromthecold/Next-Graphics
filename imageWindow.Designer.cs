namespace NextGraphics
{
	partial class imageWindow
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
			this.srcPicture = new System.Windows.Forms.PictureBox();
			this.panel1 = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.srcPicture)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// srcPicture
			// 
			this.srcPicture.Location = new System.Drawing.Point(3, 3);
			this.srcPicture.Name = "srcPicture";
			this.srcPicture.Size = new System.Drawing.Size(780, 419);
			this.srcPicture.TabIndex = 0;
			this.srcPicture.TabStop = false;
			this.srcPicture.Paint += new System.Windows.Forms.PaintEventHandler(this.srcWindowPaint);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.AutoScroll = true;
			this.panel1.Controls.Add(this.srcPicture);
			this.panel1.Location = new System.Drawing.Point(13, 13);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(789, 425);
			this.panel1.TabIndex = 1;
			this.panel1.Resize += new System.EventHandler(this.resize);
			// 
			// imageWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(814, 447);
			this.Controls.Add(this.panel1);
			this.Name = "imageWindow";
			this.Text = "SrcImage";
			((System.ComponentModel.ISupportInitialize)(this.srcPicture)).EndInit();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox srcPicture;
		private System.Windows.Forms.Panel panel1;
	}
}