using System.Windows.Forms;

namespace NextGraphics
{
	public partial class NewProjectForm : Form
	{
		public string ProjectName
		{
			get => projectNameTextBox.Text;
			set => projectNameTextBox.Text = value;
		}

		#region Initialization & Disposal

		public NewProjectForm()
		{
			InitializeComponent();

			AcceptButton = okButton;	// This will accept enter in text box as dialog confirmation
		}

		#endregion
	}
}
