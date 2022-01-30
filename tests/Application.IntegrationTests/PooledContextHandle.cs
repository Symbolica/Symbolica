using System.Collections.Concurrent;
using Microsoft.Z3;
using Symbolica.Computation;

namespace Symbolica;

internal sealed class PooledContextHandle : IContextHandle
{
    private static readonly ConcurrentBag<Context> Contexts = new();

    private PooledContextHandle(Context context)
    {
        Context = context;
    }

    public Context Context { get; }

    public void Dispose()
    {
        Contexts.Add(Context);
    }

    public static IContextHandle Create()
    {
        return new PooledContextHandle(Contexts.TryTake(out var context)
            ? context
            : new Context());
    }
}
