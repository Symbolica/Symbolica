using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Computation.Exceptions;

namespace Symbolica.Computation;

internal sealed class PooledContext : IContext
{
    private static readonly ConcurrentBag<Context> Contexts = new();
    private readonly Context _context;
    private readonly ISet<string> _names;
    private readonly Solver _solver;

    private PooledContext(Context context, Solver solver)
    {
        _context = context;
        _solver = solver;
        _names = new HashSet<string>();
    }

    public void Dispose()
    {
        Contexts.Add(_context);
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

    private static IContext Create(Context context)
    {
        return new PooledContext(context, context.MkSolver(context.MkTactic("smt")));
    }

    public static IContext Create()
    {
        return Create(Contexts.TryTake(out var context)
            ? context
            : new Context());
    }
}
