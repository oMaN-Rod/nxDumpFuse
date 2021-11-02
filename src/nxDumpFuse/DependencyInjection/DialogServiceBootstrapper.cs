using nxDumpFuse.Extensions;
using nxDumpFuse.Services;
using nxDumpFuse.ViewModels.Interfaces;
using Splat;

namespace nxDumpFuse.DependencyInjection
{
    public static class DialogServiceBootstrapper
    {
        public static void RegisterDialogService(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IMainWindowProvider>(() => new MainWindowProvider());
            services.RegisterLazySingleton<IDialogService>(() => new DialogService(
                resolver.GetRequiredService<IMainWindowProvider>()));
        }
    }
}
