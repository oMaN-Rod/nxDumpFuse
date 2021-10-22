using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using nxDumpFuse.ViewModels;

namespace nxDumpFuse.Views
{
    public class AboutView : UserControl
    {
        public AboutView()
        {
            InitializeComponent();
            DataContext = new AboutViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
