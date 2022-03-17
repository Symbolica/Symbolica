using System;
using System.Collections.Generic;
using System.Linq;
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
        _solver.Dispose();
        _contextHandle.Dispose();
    }

    public void Assert(ICollection<BoolExpr> assertions)
    {
        try
        {
            _solver.Add(assertions);
        }
        finally
        {
            foreach (var assertion in assertions)
                assertion.Dispose();
        }
    }

    public void Assert(string name, ICollection<BoolExpr> assertions)
    {
        if (_names.Contains(name))
            return;

        _names.Add(name);
        Assert(assertions);
    }

    public Status Check(BoolExpr assertion)
    {
        return _solver.Check(assertion);
    }

    public BitVecNum Evaluate(BitVecExpr variable)
    {
        using var model = CreateModel();
        return (BitVecNum) model.Eval(variable, true);
    }

    public ICollection<KeyValuePair<FuncDecl, Expr>> Evaluate()
    {
        using var model = CreateModel();
        return model.Consts.ToList();
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

    public BitVecNum MkBV(uint v, uint size)
    {
        using var sort = CreateSort(c => c.MkBitVecSort(size));
        return (BitVecNum) Create(c => c.MkNumeral(v, sort));
    }

    public BitVecNum MkBV(string v, uint size)
    {
        using var sort = CreateSort(c => c.MkBitVecSort(size));
        return (BitVecNum) Create(c => c.MkNumeral(v, sort));
    }

    public BitVecExpr MkBVConst(string name, uint size)
    {
        using var symbol = Create(c => c.MkSymbol(name));
        using var sort = CreateSort(c => c.MkBitVecSort(size));
        return (BitVecExpr) Create(c => c.MkConst(symbol, sort));
    }

    private Solver CreateSolver()
    {
        using var tactic = Create(c => c.MkTactic("smt"));
        return Create(c => c.MkSolver(tactic));
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
