using Symbolica.Computation;

namespace Symbolica.Application
{
    internal sealed class NonDisposedContextFactory : IContextFactory
    {
        public IContextHandle Create()
        {
            return new NonDisposedContextHandle();
        }
    }
}
