namespace NextGraphics
{
	partial class DebugForm
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
			this.debugPictureBox = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.debugPictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// DEBUG_PICTURE
			// 
			this.debugPictureBox.Location = new System.Drawing.Point(3, 1);
			this.debugPictureBox.Name = "DEBUG_PICTURE";
			this.debugPictureBox.Size = new System.Drawing.Size(504, 360);
			this.debugPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.debugPictureBox.TabIndex = 0;
			this.debugPictureBox.TabStop = false;
			this.debugPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.debugPictureBox_Paint);
			// 
			// DEBUGFORM
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(510, 364);
			this.Controls.Add(this.debugPictureBox);
			this.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DEBUGFORM";
			this.ShowIcon = false;
			this.Text = "Debug";
			((System.ComponentModel.ISupportInitialize)(this.debugPictureBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox debugPictureBox;
	}
}