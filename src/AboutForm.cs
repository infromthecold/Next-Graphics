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
			// Note: .NET Core doesn't include ApplicationDeployment class so we always use assembly version. The two should match though...
			return Assembly.GetExecutingAssembly().GetName().Version;
		}
	}
}
