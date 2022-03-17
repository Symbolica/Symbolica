using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

    public bool IsSatisfiable(BoolExpr assertion)
    {
        var status = _solver.Check(assertion);

        return status switch
        {
            Status.UNSATISFIABLE => false,
            Status.SATISFIABLE => true,
            Status.UNKNOWN => throw new UnexpectedSatisfiabilityException(status),
            _ => throw new UnexpectedSatisfiabilityException(status)
        };
    }

    public BigInteger GetSingleValue(BitVecExpr variable)
    {
        var simplified = variable.Simplify();

        var value = simplified.IsNumeral
            ? ((BitVecNum) simplified).BigInteger
            : throw new IrreducibleSymbolicExpressionException();

        return value == GetExampleValue(variable)
            ? value
            : throw new IrreducibleSymbolicExpressionException();
    }

    public BigInteger GetExampleValue(BitVecExpr variable)
    {
        return ((BitVecNum) CreateModel().Eval(variable, true)).BigInteger;
    }

    public IEnumerable<KeyValuePair<string, string>> GetExampleValues()
    {
        return CreateModel().Consts
            .Select(p => new KeyValuePair<string, string>(p.Key.Name.ToString(), p.Value.ToString()));
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
