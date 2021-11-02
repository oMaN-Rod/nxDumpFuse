using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using nxDumpFuse.ViewModels.Interfaces;

namespace nxDumpFuse
{
    public class MainWindowProvider : IMainWindowProvider
    {
        public Window GetMainWindow()
        {
            var lifetime = (IClassicDesktopStyleApplicationLifetime) Application.Current.ApplicationLifetime;

            return lifetime.MainWindow;
        }
    }
}
