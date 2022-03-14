using System.Collections.Concurrent;
using Microsoft.Z3;
using Symbolica.Computation;

namespace Symbolica;

internal sealed class PooledContextHandle : IContextHandle
{
    private static readonly ConcurrentBag<Context> Contexts = new();

    public PooledContextHandle()
    {
        Context = Contexts.TryTake(out var context)
            ? context
            : new Context();
    }

    public Context Context { get; }

    public long RefCount => throw new System.NotImplementedException();

    public void Dispose()
    {
        Contexts.Add(Context);
    }
}
