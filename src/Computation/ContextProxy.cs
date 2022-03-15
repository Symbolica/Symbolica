using System;
using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Computation.Exceptions;

namespace Symbolica.Computation;

internal sealed class ContextProxy : IContext
{
    private readonly Context _context;
    private readonly ISet<string> _names;
    private readonly Solver _solver;

    private ContextProxy(Context context, Solver solver)
    {
        _context = context;
        _solver = solver;
        _names = new HashSet<string>();
    }

    public void Dispose()
    {
        _context.Dispose();
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
        return func(_context);
    }

    public TExpr CreateExpr<TExpr>(Func<Context, TExpr> func)
        where TExpr : Expr
    {
        return func(_context);
    }

    private Model CreateModel()
    {
        var status = _solver.Check();

        return status == Status.SATISFIABLE
            ? _solver.Model
            : throw new InvalidModelException(status);
    }

    public static IContext Create()
    {
        var context = new Context();

        return new ContextProxy(context, context.MkSolver(context.MkTactic("smt")));
    }
}
