namespace NextGraphics
{
	partial class InfoForm
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
			this.infoTextBox = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// infoTextBox
			// 
			this.infoTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.infoTextBox.Location = new System.Drawing.Point(13, 13);
			this.infoTextBox.Name = "infoTextBox";
			this.infoTextBox.Size = new System.Drawing.Size(689, 562);
			this.infoTextBox.TabIndex = 0;
			this.infoTextBox.Text = "";
			// 
			// infoWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(714, 587);
			this.Controls.Add(this.infoTextBox);
			this.Name = "infoWindow";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "Info";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.RichTextBox infoTextBox;
	}
}