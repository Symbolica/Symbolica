using Microsoft.Z3;
using Symbolica.Computation;

namespace Symbolica.Application.Computation
{
    internal sealed class ContextHandle : IContextHandle
    {
        public ContextHandle()
        {
            Context = new Context();
        }

        public Context Context { get; }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
