using System;
using Microsoft.Z3;

namespace Symbolica.Computation;

public sealed class DisposableContext : IContext
{
    private readonly Context _context;

    public DisposableContext()
    {
        _context = new Context();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public Solver CreateSolver(Func<Context, Solver> func)
    {
        return func(_context);
    }

    public TSort CreateSort<TSort>(Func<Context, TSort> func)
        where TSort : Sort
    {
        return func(_context);
    }

    public TExpr CreateExpr<TExpr>(Func<Context, TExpr> func)
        where TExpr : Expr
    {
        return func(_context);
    }
}
