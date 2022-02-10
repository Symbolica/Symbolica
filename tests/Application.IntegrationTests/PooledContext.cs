using System;
using System.Collections.Concurrent;
using Microsoft.Z3;
using Symbolica.Computation;

namespace Symbolica;

internal sealed class PooledContext : IContext
{
    private static readonly ConcurrentBag<IContext> Contexts = new();
    private readonly IContext _context;

    private PooledContext(IContext context)
    {
        _context = context;
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

    public static IContext Create()
    {
        return new PooledContext(Contexts.TryTake(out var context)
            ? context
            : DisposableContext.Create());
    }
}
