
namespace NextGraphics
{
	partial class palOffset
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
			this.paletteOffset = new System.Windows.Forms.NumericUpDown();
			this.button1 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.paletteOffset)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 34);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(71, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Palette Offset";
			// 
			// paletteOffset
			// 
			this.paletteOffset.Increment = new decimal(new int[] {
            16,
            0,
            0,
            0});
			this.paletteOffset.Location = new System.Drawing.Point(103, 32);
			this.paletteOffset.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
			this.paletteOffset.Name = "paletteOffset";
			this.paletteOffset.Size = new System.Drawing.Size(57, 20);
			this.paletteOffset.TabIndex = 1;
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new System.Drawing.Point(55, 75);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 2;
			this.button1.Text = "Close";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// palOffset
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(180, 110);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.paletteOffset);
			this.Controls.Add(this.label1);
			this.Name = "palOffset";
			this.Text = "Palette Offset";
			((System.ComponentModel.ISupportInitialize)(this.paletteOffset)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.NumericUpDown paletteOffset;
		private System.Windows.Forms.Button button1;
	}
}