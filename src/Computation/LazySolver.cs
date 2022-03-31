using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class LazySolver : ISolver
{
    private readonly IList<Action<ISolver>> _actions;
    private ISolver? _solver;

    public LazySolver()
    {
        _solver = null;
        _actions = new List<Action<ISolver>>();
    }

    public Context Context => GetSolver().Context;

    public void Dispose()
    {
        _solver?.Dispose();
    }

    public void Assert(IEnumerable<IExpression> assertions)
    {
        _actions.Add(s => s.Assert(assertions));
    }

    public void Assert(string name, IEnumerable<IExpression> assertions)
    {
        _actions.Add(s => s.Assert(name, assertions));
    }

    public bool IsSatisfiable(IExpression assertion)
    {
        return assertion is IConstantValue v
            ? v.AsBool()
            : GetSolver().IsSatisfiable(assertion);
    }

    public BigInteger GetSingleValue(IExpression expression)
    {
        return expression is IConstantValue v
            ? v.AsUnsigned()
            : GetSolver().GetSingleValue(expression);
    }

    public BigInteger GetExampleValue(IExpression expression)
    {
        return expression is IConstantValue v
            ? v.AsUnsigned()
            : GetSolver().GetExampleValue(expression);
    }

    public IExample GetExample()
    {
        return GetSolver().GetExample();
    }

    public bool TryGetSingleValue(IExpression expression, out BigInteger constant)
    {
        if (expression is IConstantValue v)
        {
            constant = v.AsUnsigned();
            return true;
        }

        return GetSolver().TryGetSingleValue(expression, out constant);
    }

    private ISolver GetSolver()
    {
        _solver ??= PooledSolver.Create();

        foreach (var action in _actions)
            action(_solver);

        _actions.Clear();
        return _solver;
    }
}
