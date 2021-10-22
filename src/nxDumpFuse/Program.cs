using Avalonia;
using Avalonia.ReactiveUI;
using System;
using nxDumpFuse.DependencyInjection;
using Splat;

namespace nxDumpFuse
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            RegisterDependencies();
            
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

        private static void RegisterDependencies() => Bootstrapper.Register(Locator.CurrentMutable, Locator.Current);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .With(new SkiaOptions { MaxGpuResourceSizeBytes = 8096000 })
                .With(new Win32PlatformOptions { AllowEglInitialization = true })
                .LogToTrace()
                .UseReactiveUI();
    }
}
