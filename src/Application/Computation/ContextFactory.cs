namespace Symbolica.Computation;

internal sealed class ContextFactory : IContextFactory
{
    public IContextHandle Create()
    {
        return new ContextHandle();
    }
}
