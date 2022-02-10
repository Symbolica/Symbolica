namespace Symbolica.Computation;

internal sealed class ContextFactory : IContextFactory
{
    public IContext Create()
    {
        return DisposableContext.Create();
    }
}
