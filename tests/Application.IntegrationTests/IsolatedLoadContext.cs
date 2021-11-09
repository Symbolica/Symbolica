using System;
using System.Reflection;
using System.Runtime.Loader;

namespace Symbolica.Application
{
    internal sealed class IsolatedLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;
        private readonly Assembly _sharedAssembly;
        private readonly UnmanagedLoadContext _unmanagedLoadContext;

        public IsolatedLoadContext(UnmanagedLoadContext unmanagedLoadContext, Assembly sharedAssembly)
            : base(true)
        {
            _resolver = new AssemblyDependencyResolver(Assembly.GetExecutingAssembly().Location);
            _unmanagedLoadContext = unmanagedLoadContext;
            _sharedAssembly = sharedAssembly;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            return libraryPath == null
                ? IntPtr.Zero
                : _unmanagedLoadContext.LoadUnmanagedDll(libraryPath);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            return AssemblyName.ReferenceMatchesDefinition(_sharedAssembly.GetName(), assemblyName)
                ? null
                : LoadFromAssemblyPath(assemblyName);
        }

        private Assembly? LoadFromAssemblyPath(AssemblyName assemblyName)
        {
            var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);

            return assemblyPath == null
                ? null
                : LoadFromAssemblyPath(assemblyPath);
        }
    }
}
