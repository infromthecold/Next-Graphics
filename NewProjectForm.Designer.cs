namespace NextGraphics
{
	partial class NewProjectForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewProjectForm));
			this.label1 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.outCancel = new System.Windows.Forms.Button();
			this.outOk = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(71, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Project Name";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(90, 20);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(232, 20);
			this.textBox1.TabIndex = 1;
			this.textBox1.Text = "Next graphics project name";
			// 
			// outCancel
			// 
			this.outCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.outCancel.Location = new System.Drawing.Point(9, 61);
			this.outCancel.Name = "outCancel";
			this.outCancel.Size = new System.Drawing.Size(75, 23);
			this.outCancel.TabIndex = 6;
			this.outCancel.Text = "Cancel";
			this.outCancel.UseVisualStyleBackColor = true;
			// 
			// outOk
			// 
			this.outOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.outOk.Location = new System.Drawing.Point(247, 61);
			this.outOk.Name = "outOk";
			this.outOk.Size = new System.Drawing.Size(75, 23);
			this.outOk.TabIndex = 5;
			this.outOk.Text = "OK";
			this.outOk.UseVisualStyleBackColor = true;
			// 
			// newProject
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(336, 96);
			this.Controls.Add(this.outCancel);
			this.Controls.Add(this.outOk);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "newProject";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "New Project ";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button outCancel;
		private System.Windows.Forms.Button outOk;
		public System.Windows.Forms.TextBox textBox1;
	}
}