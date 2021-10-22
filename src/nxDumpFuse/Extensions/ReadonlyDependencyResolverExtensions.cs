using System;
using Splat;

namespace nxDumpFuse.Extensions
{
    public static class ReadonlyDependencyResolverExtensions
    {
        public static TService GetRequiredService<TService>(this IReadonlyDependencyResolver resolver)
        {
            var service = resolver.GetService<TService>();
            if (service == null)
            {
                throw new InvalidOperationException($"Failed to resolve object of type {typeof(TService)}");
            }
            return service;
        }
    }
}
