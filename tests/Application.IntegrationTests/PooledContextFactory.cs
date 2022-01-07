using Symbolica.Computation;

namespace Symbolica
{
    internal sealed class PooledContextFactory : IContextFactory
    {
        public IContextHandle Create()
        {
            return PooledContextHandle.Create();
        }
    }
}
