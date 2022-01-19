namespace NextGraphics
{
	partial class ImageForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.sourceImagePanel = new System.Windows.Forms.Panel();
			this.sourcePictureBox = new System.Windows.Forms.PictureBox();
			this.scaleHScrollBar = new System.Windows.Forms.HScrollBar();
			this.scaleLabel = new System.Windows.Forms.Label();
			this.sourceImagePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sourcePictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// sourceImagePanel
			// 
			this.sourceImagePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.sourceImagePanel.AutoScroll = true;
			this.sourceImagePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.sourceImagePanel.Controls.Add(this.sourcePictureBox);
			this.sourceImagePanel.Location = new System.Drawing.Point(6, 39);
			this.sourceImagePanel.Name = "sourceImagePanel";
			this.sourceImagePanel.Size = new System.Drawing.Size(1047, 678);
			this.sourceImagePanel.TabIndex = 1;
			this.sourceImagePanel.Resize += new System.EventHandler(this.sourceImagePanel_Resize);
			// 
			// sourcePictureBox
			// 
			this.sourcePictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.sourcePictureBox.Location = new System.Drawing.Point(-1, 3);
			this.sourcePictureBox.Name = "sourcePictureBox";
			this.sourcePictureBox.Size = new System.Drawing.Size(1043, 670);
			this.sourcePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.sourcePictureBox.TabIndex = 0;
			this.sourcePictureBox.TabStop = false;
			this.sourcePictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.sourcePictureBox_Paint);
			// 
			// scaleHScrollBar
			// 
			this.scaleHScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.scaleHScrollBar.Location = new System.Drawing.Point(61, 9);
			this.scaleHScrollBar.Minimum = 10;
			this.scaleHScrollBar.Name = "scaleHScrollBar";
			this.scaleHScrollBar.Size = new System.Drawing.Size(992, 17);
			this.scaleHScrollBar.TabIndex = 2;
			this.scaleHScrollBar.Value = 25;
			this.scaleHScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scaleHScrollBar_Scroll);
			// 
			// scaleLabel
			// 
			this.scaleLabel.AutoSize = true;
			this.scaleLabel.Location = new System.Drawing.Point(10, 13);
			this.scaleLabel.Name = "scaleLabel";
			this.scaleLabel.Size = new System.Drawing.Size(34, 13);
			this.scaleLabel.TabIndex = 3;
			this.scaleLabel.Text = "Scale";
			// 
			// ImageForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1062, 729);
			this.Controls.Add(this.scaleLabel);
			this.Controls.Add(this.sourceImagePanel);
			this.Controls.Add(this.scaleHScrollBar);
			this.Name = "ImageForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "SrcImage";
			this.Resize += new System.EventHandler(this.sourceImagePanel_Resize);
			this.sourceImagePanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.sourcePictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		
		private System.Windows.Forms.Label scaleLabel;
		private System.Windows.Forms.HScrollBar scaleHScrollBar;
		private System.Windows.Forms.Panel sourceImagePanel;
		private System.Windows.Forms.PictureBox sourcePictureBox;
	}
}