using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Exceptions;
using Symbolica.Expression;

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

    public void Assert(IEnumerable<IExpression> assertions)
    {
        foreach (var assertion in assertions)
        {
            using var expr = assertion.AsBool(this);
            _solver.Add(expr);
        }
    }

    public void Assert(string name, IEnumerable<IExpression> assertions)
    {
        if (_names.Add(name))
            Assert(assertions);
    }

    public bool IsSatisfiable(IExpression assertion)
    {
        // string PrintAssertion(IExpression assertion, int depth) => $"{System.Environment.NewLine}{string.Join("", Enumerable.Repeat(" ", depth * 2))}{assertion.GetType().Name}{(assertion.PrintedValue is null ? string.Empty : $" ({assertion.PrintedValue})")}{string.Join("", assertion.Children.Select(v => PrintAssertion(v, depth + 1)))}";

        // int AssertionCount(IExpression assertion) => 1 + assertion.Children.Sum(AssertionCount);

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

    public BigInteger GetSingleValue(IExpression value)
    {
        return TryGetSingleValue(value, out var constant)
            ? constant
            : throw new IrreducibleSymbolicExpressionException();
    }

    public BigInteger GetExampleValue(IExpression value)
    {
        using var bitVector = value.AsBitVector(this);
        using var model = CreateModel();
        using var expr = model.Eval(bitVector, true);
        return ((BitVecNum) expr).BigInteger;
    }

    public IExample GetExample()
    {
        using var model = CreateModel();
        return new Example(model.Consts
            .Select(CreateExample)
            .ToArray());
    }

    public bool TryGetSingleValue(IExpression value, out BigInteger constant)
    {
        constant = GetExampleValue(value);
        return !IsSatisfiable(
            Symbolica.Expression.Values.NotEqual.Create(
                value,
                Symbolica.Expression.Values.Constants.ConstantUnsigned.Create(value.Size, constant)));
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
