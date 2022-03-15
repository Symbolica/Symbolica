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
    private readonly Solver _solver;

    public Context()
    {
        _names = new HashSet<string>();
        _contextHandle = new TContextHandle();
        _solver = CreateSolver();
    }

    public void Dispose()
    {
        _contextHandle.Dispose();
    }

    public void Assert(IEnumerable<BoolExpr> assertions)
    {
        _solver.Add(assertions);
    }

    public void Assert(string name, IEnumerable<BoolExpr> assertions)
    {
        if (_names.Add(name))
            Assert(assertions);
    }

    public Status Check(BoolExpr assertion)
    {
        return _solver.Check(assertion);
    }

    public BitVecNum Evaluate(BitVecExpr variable)
    {
        return (BitVecNum) CreateModel().Eval(variable, true);
    }

    public IEnumerable<KeyValuePair<FuncDecl, Expr>> Evaluate()
    {
        return CreateModel().Consts;
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

    private Model CreateModel()
    {
        var status = _solver.Check();

        return status == Status.SATISFIABLE
            ? _solver.Model
            : throw new InvalidModelException(status);
    }
}
