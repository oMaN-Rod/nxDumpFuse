using System.Diagnostics;
using System.Reactive;
using System.Runtime.InteropServices;
using System.Text;
using nxDumpFuse.ViewModels.Interfaces;
using ReactiveUI;

namespace nxDumpFuse.ViewModels
{
    public class AboutViewModel : ViewModelBase, IAboutViewModel
    {
        private const string GitHubUrl = "https://github.com/oMaN-Rod/nxDumpFuse";

        public AboutViewModel()
        {
            OpenGithubCommand = ReactiveCommand.Create(OpenGithub);
        }

        public ReactiveCommand<Unit, Unit> OpenGithubCommand { get; }

        public string UsageText => BuildUsageText();

        public string AuthorInfo => "Made for fun by oMaN-Rod, check me out on -->";

        private static void OpenGithub()
        {
            try
            {
                Process.Start("explorer.exe", GitHubUrl);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", GitHubUrl);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", GitHubUrl);
                }
            }
        }

        private static string BuildUsageText()
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
