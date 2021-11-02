using Splat;

namespace nxDumpFuse.DependencyInjection
{
    public static class Bootstrapper
    {
        public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            FuseServiceBootstrapper.RegisterFuseService(services, resolver);
            DialogServiceBootstrapper.RegisterDialogService(services, resolver);
            ViewModelBootstrapper.RegisterViewModels(services, resolver);
        }
    }
}
