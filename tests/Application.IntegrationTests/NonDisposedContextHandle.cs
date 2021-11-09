using Microsoft.Z3;
using Symbolica.Computation;

namespace Symbolica.Application
{
    internal sealed class NonDisposedContextHandle : IContextHandle
    {
        public NonDisposedContextHandle()
        {
            Context = new Context();
        }

        public Context Context { get; }

        public void Dispose()
        {
        }
    }
}
