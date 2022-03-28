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
			this.scaleLabel = new System.Windows.Forms.Label();
			this.controlsPanel = new System.Windows.Forms.Panel();
			this.scaleNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.scaleTrackBar = new System.Windows.Forms.TrackBar();
			this.sourceImagePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sourcePictureBox)).BeginInit();
			this.controlsPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.scaleNumericUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.scaleTrackBar)).BeginInit();
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
			this.sourceImagePanel.Location = new System.Drawing.Point(6, 6);
			this.sourceImagePanel.Name = "sourceImagePanel";
			this.sourceImagePanel.Size = new System.Drawing.Size(855, 584);
			this.sourceImagePanel.TabIndex = 1;
			this.sourceImagePanel.Resize += new System.EventHandler(this.sourceImagePanel_Resize);
			// 
			// sourcePictureBox
			// 
			this.sourcePictureBox.Location = new System.Drawing.Point(0, 0);
			this.sourcePictureBox.Margin = new System.Windows.Forms.Padding(0);
			this.sourcePictureBox.Name = "sourcePictureBox";
			this.sourcePictureBox.Size = new System.Drawing.Size(547, 413);
			this.sourcePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.sourcePictureBox.TabIndex = 0;
			this.sourcePictureBox.TabStop = false;
			this.sourcePictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.sourcePictureBox_Paint);
			// 
			// scaleLabel
			// 
			this.scaleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.scaleLabel.AutoSize = true;
			this.scaleLabel.Location = new System.Drawing.Point(1, 2);
			this.scaleLabel.Name = "scaleLabel";
			this.scaleLabel.Size = new System.Drawing.Size(34, 13);
			this.scaleLabel.TabIndex = 3;
			this.scaleLabel.Text = "Scale";
			this.scaleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// controlsPanel
			// 
			this.controlsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.controlsPanel.Controls.Add(this.scaleNumericUpDown);
			this.controlsPanel.Controls.Add(this.scaleTrackBar);
			this.controlsPanel.Controls.Add(this.scaleLabel);
			this.controlsPanel.Location = new System.Drawing.Point(6, 595);
			this.controlsPanel.Name = "controlsPanel";
			this.controlsPanel.Size = new System.Drawing.Size(851, 20);
			this.controlsPanel.TabIndex = 4;
			// 
			// scaleNumericUpDown
			// 
			this.scaleNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.scaleNumericUpDown.BackColor = System.Drawing.SystemColors.Menu;
			this.scaleNumericUpDown.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.scaleNumericUpDown.DecimalPlaces = 1;
			this.scaleNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.scaleNumericUpDown.Location = new System.Drawing.Point(41, 2);
			this.scaleNumericUpDown.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.scaleNumericUpDown.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            65536});
			this.scaleNumericUpDown.Name = "scaleNumericUpDown";
			this.scaleNumericUpDown.Size = new System.Drawing.Size(40, 16);
			this.scaleNumericUpDown.TabIndex = 5;
			this.scaleNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.scaleNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.scaleNumericUpDown.ValueChanged += new System.EventHandler(this.scaleNumericUpDown_ValueChanged);
			// 
			// scaleTrackBar
			// 
			this.scaleTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.scaleTrackBar.AutoSize = false;
			this.scaleTrackBar.LargeChange = 10;
			this.scaleTrackBar.Location = new System.Drawing.Point(80, 0);
			this.scaleTrackBar.Maximum = 100;
			this.scaleTrackBar.Minimum = 5;
			this.scaleTrackBar.Name = "scaleTrackBar";
			this.scaleTrackBar.Size = new System.Drawing.Size(771, 20);
			this.scaleTrackBar.TabIndex = 4;
			this.scaleTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
			this.scaleTrackBar.Value = 10;
			this.scaleTrackBar.ValueChanged += new System.EventHandler(this.scaleTrackBar_ValueChanged);
			// 
			// ImageForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(866, 617);
			this.Controls.Add(this.controlsPanel);
			this.Controls.Add(this.sourceImagePanel);
			this.Name = "ImageForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "SrcImage";
			this.Resize += new System.EventHandler(this.sourceImagePanel_Resize);
			this.sourceImagePanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.sourcePictureBox)).EndInit();
			this.controlsPanel.ResumeLayout(false);
			this.controlsPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.scaleNumericUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.scaleTrackBar)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion
		
		private System.Windows.Forms.Label scaleLabel;
		private System.Windows.Forms.Panel sourceImagePanel;
		private System.Windows.Forms.PictureBox sourcePictureBox;
		private System.Windows.Forms.Panel controlsPanel;
		private System.Windows.Forms.TrackBar scaleTrackBar;
		private System.Windows.Forms.NumericUpDown scaleNumericUpDown;
	}
}