namespace NextGraphics
{
	partial class ProgressForm
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
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.processingLabel = new System.Windows.Forms.Label();
			this.cancelButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// progressBar
			// 
			this.progressBar.Location = new System.Drawing.Point(96, 22);
			this.progressBar.MarqueeAnimationSpeed = 50;
			this.progressBar.Maximum = 388;
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(388, 25);
			this.progressBar.Step = 1;
			this.progressBar.TabIndex = 0;
			// 
			// processingLabel
			// 
			this.processingLabel.AutoSize = true;
			this.processingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this.processingLabel.Location = new System.Drawing.Point(12, 26);
			this.processingLabel.Name = "processingLabel";
			this.processingLabel.Size = new System.Drawing.Size(78, 17);
			this.processingLabel.TabIndex = 1;
			this.processingLabel.Text = "Processing";
			// 
			// cancelButton
			// 
			this.cancelButton.Location = new System.Drawing.Point(212, 67);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(89, 27);
			this.cancelButton.TabIndex = 2;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// ProgressForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(496, 112);
			this.ControlBox = false;
			this.Controls.Add(this.processingLabel);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.progressBar);
			this.MaximumSize = new System.Drawing.Size(512, 151);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(512, 151);
			this.Name = "ProgressForm";
			this.Text = "Progress";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label processingLabel;
		private System.Windows.Forms.Button cancelButton;
		public System.Windows.Forms.ProgressBar progressBar;
	}
}