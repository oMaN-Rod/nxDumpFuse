using nxDumpFuse.Extensions;
using nxDumpFuse.Services;
using nxDumpFuse.ViewModels;
using nxDumpFuse.ViewModels.Interfaces;
using Splat;

namespace nxDumpFuse.DependencyInjection
{
    public static class ViewModelBootstrapper
    {
        public static void RegisterViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IMainWindowViewModel>(() => new MainWindowViewModel(
                resolver.GetRequiredService<IFuseViewModel>(),
                resolver.GetRequiredService<IAboutViewModel>()
            ));

            services.RegisterLazySingleton<IFuseViewModel>(() => new FuseViewModel(
                resolver.GetRequiredService<IDialogService>(),
                resolver.GetRequiredService<IFuseService>()));
            services.RegisterLazySingleton<IAboutViewModel>(() => new AboutViewModel());
        }
    }
}
