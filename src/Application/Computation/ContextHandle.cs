using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Z3;

namespace Symbolica.Computation;

internal sealed class ContextHandle : IContextHandle
{
    // private static readonly ConcurrentBag<Context> Contexts = new();

    public ContextHandle()
    {
        // Context = Contexts.TryTake(out var context)
        //     ? context
        //     : new Context();
        Context = new Context();
    }

    public long RefCount => (long) Context.GetType().GetField("refCount", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(Context)!;

    public Context Context { get; }

    public void Dispose()
    {
        Context.BoolSort.Dispose();
        Context.CharSort.Dispose();
        Context.IntSort.Dispose();
        Context.RealSort.Dispose();
        Context.StringSort.Dispose();
        if (RefCount != 0)
            Debugger.Break();
        Context.Dispose();
        // Contexts.Add(Context);
    }
}
