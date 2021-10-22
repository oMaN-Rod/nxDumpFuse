using nxDumpFuse.Extensions;
using nxDumpFuse.Interfaces;
using nxDumpFuse.Model;
using nxDumpFuse.Services;
using Splat;

namespace nxDumpFuse.DependencyInjection
{
    public static class DialogServiceBootstrapper
    {
        public static void RegisterDialogService(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IMainWindowProvider>((() => new MainWindowProvider()));
            services.RegisterLazySingleton<IDialogService>(() => new DialogService(
                resolver.GetRequiredService<IMainWindowProvider>()));
        }
    }
}
