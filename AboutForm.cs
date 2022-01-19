using System.Reflection;
using System.Windows.Forms;

namespace NextGraphics
{
	public partial class AboutForm : Form
	{
		public AboutForm()
		{
			InitializeComponent();

			versionLabel.Text = $"Version {VersionName()}";
		}

		private object VersionName()
		{
			// This will use deployment version if available (should be on released versions), otherwise fall-down to assembly version (for debug builds).
			try
			{
				return System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
			}
			catch
			{
				return Assembly.GetExecutingAssembly().GetName().Version;
			}
		}
	}
}
