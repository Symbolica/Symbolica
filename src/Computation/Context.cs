using System;
using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Computation.Exceptions;

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

    public void Dispose()
    {
        _contextHandle.Dispose();
    }

    public void Assert(IEnumerable<BoolExpr> assertions)
    {
        _solver.Value.Add(assertions);
    }

    public void Assert(string name, IEnumerable<BoolExpr> assertions)
    {
        if (_names.Contains(name))
            return;

        _names.Add(name);
        Assert(assertions);
    }

    public Status Check(BoolExpr assertion)
    {
        return _solver.Value.Check(assertion);
    }

    public BitVecNum Evaluate(BitVecExpr variable)
    {
        return (BitVecNum) CreateModel(_solver.Value).Eval(variable, true);
    }

    public IEnumerable<KeyValuePair<FuncDecl, Expr>> Evaluate()
    {
        return CreateModel(_solver.Value).Consts;
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

    private TResult Create<TResult>(Func<Context, TResult> func)
    {
        return func(_contextHandle.Context);
    }

    private static Model CreateModel(Solver solver)
    {
        var status = solver.Check();

        return status == Status.SATISFIABLE
            ? solver.Model
            : throw new InvalidModelException(status);
    }
}
