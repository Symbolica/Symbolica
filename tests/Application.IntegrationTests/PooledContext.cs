using System;
using System.Collections.Concurrent;
using Microsoft.Z3;
using Symbolica.Computation;

namespace Symbolica;

internal sealed class PooledContext : IContext
{
    private static readonly ConcurrentBag<IContext> Contexts = new();
    private readonly IContext _context;

    public PooledContext()
    {
        _context = Contexts.TryTake(out var context)
            ? context
            : new DisposableContext();
    }

    public void Dispose()
    {
        Contexts.Add(_context);
    }

    public TResult Execute<TResult>(Func<Context, TResult> func)
        where TResult : Z3Object
    {
        return _context.Execute(func);
    }
}
