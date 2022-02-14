using System;
using System.Collections.Generic;
using Microsoft.Z3;

namespace Symbolica.Computation;

internal sealed class Context<TContextHandle> : IContext
    where TContextHandle : IContextHandle, new()
{
    private readonly TContextHandle _contextHandle;
    private readonly ISet<string> _names;
    private readonly Lazy<Solver> _solver;

    public Context()
    {
        _contextHandle = new TContextHandle();
        _names = new HashSet<string>();
        _solver = new Lazy<Solver>(CreateSolver);
    }

    public Solver Solver => _solver.Value;

    public void Dispose()
    {
        _contextHandle.Dispose();
    }

    public void Assert(IEnumerable<BoolExpr> assertions)
    {
        Solver.Add(assertions);
    }

    public void Assert(string name, IEnumerable<BoolExpr> assertions)
    {
        if (_names.Contains(name))
            return;

        _names.Add(name);
        Assert(assertions);
    }

    public TSort CreateSort<TSort>(Func<Context, TSort> func)
        where TSort : Sort
    {
        return Create(func);
    }

    public TExpr CreateExpr<TExpr>(Func<Context, TExpr> func)
        where TExpr : Expr
    {
        return Create(func);
    }

    private Solver CreateSolver()
    {
        return Create(c => c.MkSolver(c.MkTactic("smt")));
    }

    private TZ3Object Create<TZ3Object>(Func<Context, TZ3Object> func)
        where TZ3Object : Z3Object
    {
        return func(_contextHandle.Context);
    }
}
