using nxDumpFuse.Services;
using Splat;

namespace nxDumpFuse.DependencyInjection
{
    public static class FuseServiceBootstrapper
    {
        public static void RegisterFuseService(IMutableDependencyResolver services,
            IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IFuseService>(() => new FuseService());
        }
    }
}
