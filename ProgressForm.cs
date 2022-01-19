using System;
using System.Windows.Forms;

namespace NextGraphics
{
	public partial class ProgressForm : Form
	{
		private bool inProgress = true;

		#region Initialization & Disposal

		public ProgressForm()
		{
			InitializeComponent();
		}

		#endregion

		#region Events

		private void cancelButton_Click(object sender, EventArgs e)
		{
			inProgress = false;
		}

		#endregion

		#region Public

		public bool Progress(bool steps)
		{
			if (progressBar.Value == progressBar.Maximum)
			{
				progressBar.Value = 0;
			}
			else if (steps)
			{
				progressBar.PerformStep();
			}

			Validate(true);
			Update();
			Application.DoEvents();

			return inProgress;
		}

		public void ResetProgress()
		{
			progressBar.Value = 0;
		}

		#endregion
	}
}