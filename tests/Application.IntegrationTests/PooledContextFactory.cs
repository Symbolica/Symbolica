using Symbolica.Computation;

namespace Symbolica.Application
{
    internal sealed class PooledContextFactory : IContextFactory
    {
        public IContextHandle Create()
        {
            return PooledContextHandle.Create();
        }
    }
}
