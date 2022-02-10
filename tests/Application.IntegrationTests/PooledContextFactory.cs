using Symbolica.Computation;

namespace Symbolica;

internal sealed class PooledContextFactory : IContextFactory
{
    public IContext Create()
    {
        return PooledContext.Create();
    }
}
