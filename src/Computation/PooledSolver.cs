using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Exceptions;
using Symbolica.Expression;
using Symbolica.Expression.Values;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Computation;

internal sealed class PooledSolver : ISolver
{
    private static readonly ConcurrentBag<Context> Contexts = new();
    private readonly ISet<string> _names;
    private readonly Solver _solver;

    private PooledSolver(Context context, Solver solver)
    {
        Context = context;
        _solver = solver;
        _names = new HashSet<string>();
    }

    public Context Context { get; }

    public void Dispose()
    {
        _solver.Dispose();
        Contexts.Add(Context);
    }

    public void Assert(IEnumerable<IExpression<IType>> assertions)
    {
        foreach (var assertion in assertions)
        {
            using var expr = assertion.Map(new AsBool(this));
            _solver.Add(expr);
        }
    }

    public void Assert(string name, IEnumerable<IExpression<IType>> assertions)
    {
        if (_names.Add(name))
            Assert(assertions);
    }

    public bool IsSatisfiable(IExpression<IType> assertion)
    {
        using var expr = assertion.Map(new AsBool(this));
        var status = _solver.Check(expr);

        return status switch
        {
            Status.UNSATISFIABLE => false,
            Status.SATISFIABLE => true,
            Status.UNKNOWN => throw new UnexpectedSatisfiabilityException(status),
            _ => throw new UnexpectedSatisfiabilityException(status)
        };
    }

    public BigInteger GetSingleValue(IExpression<IType> value)
    {
        var constant = GetExampleValue(value);

        return IsSatisfiable(NotEqual.Create(value, ConstantUnsigned.Create(value.Size, constant)))
            ? throw new IrreducibleSymbolicExpressionException()
            : constant;
    }

    public BigInteger GetExampleValue(IExpression<IType> value)
    {
        using var bitVector = value.Map(new AsBitVector(this));
        using var model = CreateModel();
        using var expr = model.Eval(bitVector, true);
        return ((BitVecNum) expr).BigInteger;
    }

    public Example GetExample()
    {
        using var model = CreateModel();
        return new Example(model.Consts
            .Select(CreateExample)
            .ToArray());
    }

    private Model CreateModel()
    {
        var status = _solver.Check();

        return status == Status.SATISFIABLE
            ? _solver.Model
            : throw new InvalidModelException(status);
    }

    private static KeyValuePair<string, string> CreateExample(KeyValuePair<FuncDecl, Expr> pair)
    {
        using var func = pair.Key;
        using var expr = pair.Value;
        return new KeyValuePair<string, string>(func.Name.ToString(), expr.ToString());
    }

    private static ISolver Create(Context context)
    {
        using var tactic = context.MkTactic("smt");
        return new PooledSolver(context, context.MkSolver(tactic));
    }

    public static ISolver Create()
    {
        return Create(Contexts.TryTake(out var context)
            ? context
            : new Context());
    }
}
