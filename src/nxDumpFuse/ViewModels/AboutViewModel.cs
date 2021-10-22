using System.Diagnostics;
using System.Reactive;
using System.Runtime.InteropServices;
using System.Text;
using nxDumpFuse.Interfaces;
using ReactiveUI;

namespace nxDumpFuse.ViewModels
{
    public class AboutViewModel : ViewModelBase, IAboutViewModel
    {
        private string _gitHubUrl = "https://github.com/oMaN-Rod/nxDumpFuse";
        private string _explorer = "explorer.exe";

        public AboutViewModel()
        {
            OpenGithubCommand = ReactiveCommand.Create(OpenGithub);
        }

        public ReactiveCommand<Unit, Unit> OpenGithubCommand { get; }

        public string UsageText => BuildUsageText();
        public string AuthorInfo => "Made for fun by oMaN-Rod, check me out on -->";

        private void OpenGithub()
        {
            try
            {
                Process.Start(_explorer, _gitHubUrl);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", _gitHubUrl);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", _gitHubUrl);
                }
            }
        }

        private string BuildUsageText()
        {
            var sb = new StringBuilder();
            sb.AppendLine("To fuse files make sure all file parts are alone in a directory with no other files, i.e");
            sb.AppendLine("├── _dir");
            sb.AppendLine("│   ├── nxDumpPart0");
            sb.AppendLine("│   ├── nxDumpPart1");
            sb.AppendLine("│   ├── ...........");
            return sb.ToString();
        }
    }
}
