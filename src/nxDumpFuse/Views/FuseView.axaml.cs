using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using nxDumpFuse.ViewModels;

namespace nxDumpFuse.Views
{
    public class FuseView : UserControl
    {
        public FuseView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
