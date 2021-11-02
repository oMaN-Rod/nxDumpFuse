using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using nxDumpFuse.Extensions;
using nxDumpFuse.ViewModels;
using nxDumpFuse.ViewModels.Interfaces;
using nxDumpFuse.Views;
using Splat;

namespace nxDumpFuse
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                DataContext = GetRequiredService<IMainWindowViewModel>();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = DataContext
                };
            }

            var theme = new Avalonia.Themes.Default.DefaultTheme();
            theme.TryGetResource("Button", out _);

            base.OnFrameworkInitializationCompleted();
        }

        private static T GetRequiredService<T>() => Locator.Current.GetRequiredService<T>();
    }
}
