using nxDumpFuse.Extensions;
using nxDumpFuse.Interfaces;
using nxDumpFuse.ViewModels;
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
                resolver.GetRequiredService<IDialogService>()));
            services.RegisterLazySingleton<IAboutViewModel>(() => new AboutViewModel());
        }
    }
}
