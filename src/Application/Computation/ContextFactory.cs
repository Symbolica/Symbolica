using Symbolica.Computation;

namespace Symbolica.Application.Computation
{
    internal sealed class ContextFactory : IContextFactory
    {
        public IContextHandle Create()
        {
            return new ContextHandle();
        }
    }
}
