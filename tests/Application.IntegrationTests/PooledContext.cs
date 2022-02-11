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

    public Solver CreateSolver(Func<Context, Solver> func)
    {
        return _context.CreateSolver(func);
    }

    public TSort CreateSort<TSort>(Func<Context, TSort> func)
        where TSort : Sort
    {
        return _context.CreateSort(func);
    }

    public TExpr CreateExpr<TExpr>(Func<Context, TExpr> func)
        where TExpr : Expr
    {
        return _context.CreateExpr(func);
    }
}
