using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using nxDumpFuse.Model;
using nxDumpFuse.Model.Enums;

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

        private void FuseSimpleLog_OnLoadingRow(object? sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is FuseSimpleLog { Type: FuseSimpleLogType.Error })
                e.Row.Background = Brushes.Red;
        }
    }
}
