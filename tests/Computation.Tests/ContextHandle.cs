using Microsoft.Z3;

namespace Symbolica.Computation;

internal sealed class ContextHandle : IContextHandle
{
    public ContextHandle()
    {
        Context = new Context();
    }

    public Context Context { get; }

    public long RefCount => throw new System.NotImplementedException();

    public void Dispose()
    {
        Context.Dispose();
    }
}
