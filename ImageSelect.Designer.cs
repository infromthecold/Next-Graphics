namespace NextGraphics
{
	partial class ImageSelect
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
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.outCancel = new System.Windows.Forms.Button();
			this.outOk = new System.Windows.Forms.Button();
			this.importFrom = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.importCount = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.importToo = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.SuspendLayout();
			// 
			// listBox1
			// 
			this.listBox1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			this.listBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this.listBox1.FormattingEnabled = true;
			this.listBox1.ItemHeight = 16;
			this.listBox1.Location = new System.Drawing.Point(343, 15);
			this.listBox1.Margin = new System.Windows.Forms.Padding(10);
			this.listBox1.Name = "listBox1";
			this.listBox1.ScrollAlwaysVisible = true;
			this.listBox1.Size = new System.Drawing.Size(156, 322);
			this.listBox1.TabIndex = 1;
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			// 
			// outCancel
			// 
			this.outCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.outCancel.Location = new System.Drawing.Point(342, 366);
			this.outCancel.Name = "outCancel";
			this.outCancel.Size = new System.Drawing.Size(75, 23);
			this.outCancel.TabIndex = 8;
			this.outCancel.Text = "Cancel";
			this.outCancel.UseVisualStyleBackColor = true;
			// 
			// outOk
			// 
			this.outOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.outOk.Location = new System.Drawing.Point(424, 367);
			this.outOk.Name = "outOk";
			this.outOk.Size = new System.Drawing.Size(75, 23);
			this.outOk.TabIndex = 7;
			this.outOk.Text = "Import";
			this.outOk.UseVisualStyleBackColor = true;
			// 
			// importFrom
			// 
			this.importFrom.Location = new System.Drawing.Point(59, 368);
			this.importFrom.Name = "importFrom";
			this.importFrom.Size = new System.Drawing.Size(39, 20);
			this.importFrom.TabIndex = 9;
			this.importFrom.TextChanged += new System.EventHandler(this.importFrom_TextChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(27, 371);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(30, 13);
			this.label1.TabIndex = 10;
			this.label1.Text = "From";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(115, 371);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(35, 13);
			this.label2.TabIndex = 12;
			this.label2.Text = "Count";
			// 
			// importCount
			// 
			this.importCount.Location = new System.Drawing.Point(152, 368);
			this.importCount.Name = "importCount";
			this.importCount.Size = new System.Drawing.Size(38, 20);
			this.importCount.TabIndex = 11;
			this.importCount.TextChanged += new System.EventHandler(this.importCount_TextChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(201, 371);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(51, 13);
			this.label3.TabIndex = 14;
			this.label3.Text = "Where to";
			// 
			// importToo
			// 
			this.importToo.Location = new System.Drawing.Point(254, 368);
			this.importToo.Name = "importToo";
			this.importToo.Size = new System.Drawing.Size(47, 20);
			this.importToo.TabIndex = 13;
			this.importToo.TextChanged += new System.EventHandler(this.importToo_TextChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Location = new System.Drawing.Point(13, 347);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(302, 54);
			this.groupBox1.TabIndex = 15;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Import Settings";
			// 
			// ImageSelect
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
			this.ClientSize = new System.Drawing.Size(513, 405);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.importToo);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.importCount);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.importFrom);
			this.Controls.Add(this.outCancel);
			this.Controls.Add(this.outOk);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MinimizeBox = false;
			this.Name = "ImageSelect";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Import Palette";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Button outCancel;
		private System.Windows.Forms.Button outOk;
		public System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.TextBox importFrom;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox importCount;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox importToo;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}