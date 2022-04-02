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

    public void Assert(IEnumerable<IExpression<IType>> assertions)
    {
        _actions.Add(s => s.Assert(assertions));
    }

    public void Assert(string name, IEnumerable<IExpression<IType>> assertions)
    {
        _actions.Add(s => s.Assert(name, assertions));
    }

    public bool IsSatisfiable(IExpression<IType> assertion)
    {
        return assertion is IConstantValue<IType> v
            ? v.AsBool()
            : GetSolver().IsSatisfiable(assertion);
    }

    public BigInteger GetSingleValue(IExpression<IType> expression)
    {
        return expression is IConstantValue<IType> v
            ? v.AsUnsigned()
            : GetSolver().GetSingleValue(expression);
    }

    public BigInteger GetExampleValue(IExpression<IType> expression)
    {
        return expression is IConstantValue<IType> v
            ? v.AsUnsigned()
            : GetSolver().GetExampleValue(expression);
    }

    public Example GetExample()
    {
        return GetSolver().GetExample();
    }

    public bool TryGetSingleValue(IExpression<IType> expression, out BigInteger constant)
    {
        if (expression is IConstantValue<IType> v)
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
