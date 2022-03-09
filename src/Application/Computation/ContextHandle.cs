using System.Collections.Concurrent;
using Microsoft.Z3;

namespace Symbolica.Computation;

internal sealed class ContextHandle : IContextHandle
{
    private static readonly ConcurrentBag<Context> Contexts = new();

    public ContextHandle()
    {
        Context = Contexts.TryTake(out var context)
            ? context
            : new Context();
    }

    public Context Context { get; }

    public void Dispose()
    {
        Contexts.Add(Context);
    }
}
