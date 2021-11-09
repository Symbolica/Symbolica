using System;
using System.Reflection;
using System.Threading.Tasks;
using Symbolica.Application.Collection;
using Symbolica.Application.Computation;
using Symbolica.Computation;
using Symbolica.Expression;

namespace Symbolica.Application
{
    internal sealed class SpaceFactoryProxy : ISpaceFactory, IDisposable
    {
        private readonly UnmanagedLoadContext _unmanagedLoadContext;
        private IsolatedLoadContext? _isolatedLoadContext;
        private ISpaceFactory? _spaceFactory;

        public SpaceFactoryProxy()
        {
            _unmanagedLoadContext = new UnmanagedLoadContext();
            _isolatedLoadContext = new IsolatedLoadContext(_unmanagedLoadContext, typeof(ISpaceFactory).Assembly);
            _spaceFactory = (ISpaceFactory) _isolatedLoadContext
                .LoadFromAssemblyName(Assembly.GetExecutingAssembly().GetName())
                .CreateInstance(typeof(IsolatedSpaceFactory).FullName!)!;
        }

        public void Dispose()
        {
            _isolatedLoadContext?.Unload();

            var weakReference = new WeakReference(_isolatedLoadContext);
            _isolatedLoadContext = null;
            _spaceFactory = null;

            Task.Run(() =>
            {
                while (weakReference.IsAlive)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

                _unmanagedLoadContext.Unload();
            });
        }

        public ISpace CreateInitial(Bits pointerSize, bool useSymbolicGarbage)
        {
            return _spaceFactory?.CreateInitial(pointerSize, useSymbolicGarbage)
                   ?? throw new ObjectDisposedException(nameof(SpaceFactoryProxy));
        }

        private sealed class IsolatedSpaceFactory : ISpaceFactory
        {
            private readonly ISpaceFactory _spaceFactory;

            public IsolatedSpaceFactory()
            {
                _spaceFactory = new SpaceFactory(
                    new SymbolFactory(), new ModelFactory(),
                    new NonDisposedContextFactory(), new CollectionFactory());
            }

            public ISpace CreateInitial(Bits pointerSize, bool useSymbolicGarbage)
            {
                return _spaceFactory.CreateInitial(pointerSize, useSymbolicGarbage);
            }
        }
    }
}
