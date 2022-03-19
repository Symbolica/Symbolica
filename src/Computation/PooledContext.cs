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
        _solver.Dispose();
        Contexts.Add(_context);
    }

    public void Assert(IEnumerable<IValue> assertions)
    {
        foreach (var assertion in assertions)
        {
            using var expr = assertion.AsBool(this);
            _solver.Add(expr);
        }
    }

    public void Assert(string name, IEnumerable<IValue> assertions)
    {
        if (_names.Add(name))
            Assert(assertions);
    }

    public bool IsSatisfiable(IValue assertion)
    {
        using var expr = assertion.AsBool(this);
        var status = _solver.Check(expr);

        return status switch
        {
            Status.UNSATISFIABLE => false,
            Status.SATISFIABLE => true,
            Status.UNKNOWN => throw new UnexpectedSatisfiabilityException(status),
            _ => throw new UnexpectedSatisfiabilityException(status)
        };
    }

    public BigInteger GetSingleValue(IValue variable)
    {
        using var bitVector = variable.AsBitVector(this);
        using var simplified = bitVector.Simplify();

        var value = simplified.IsNumeral
            ? ((BitVecNum) simplified).BigInteger
            : throw new IrreducibleSymbolicExpressionException();

        return value == GetExampleValue(variable)
            ? value
            : throw new IrreducibleSymbolicExpressionException();
    }

    public BigInteger GetExampleValue(IValue variable)
    {
        using var bitVector = variable.AsBitVector(this);
        using var model = CreateModel();
        using var expr = model.Eval(bitVector, true);
        return ((BitVecNum) expr).BigInteger;
    }

    public IEnumerable<KeyValuePair<string, string>> GetExampleValues()
    {
        using var model = CreateModel();
        return model.Consts.ToList().Select(p =>
        {
            using var func = p.Key;
            using var expr = p.Value;
            return new KeyValuePair<string, string>(func.Name.ToString(), expr.ToString());
        });
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
        using var tactic = context.MkTactic("smt");
        return new PooledContext(context, context.MkSolver(tactic));
    }

    public static IContext Create()
    {
        return Create(Contexts.TryTake(out var context)
            ? context
            : new Context());
    }
}
