using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class LazySolver : ISolver
{
    private readonly IList<Action<ISolver>> _actions;
    private readonly ISolver? _solver;

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

    public void Assert(IEnumerable<IValue> assertions)
    {
        _actions.Add(s => s.Assert(assertions));
    }

    public void Assert(string name, IEnumerable<IValue> assertions)
    {
        _actions.Add(s => s.Assert(name, assertions));
    }

    public bool IsSatisfiable(IValue assertion)
    {
        return assertion is IConstantValue v
            ? v.AsBool()
            : GetSolver().IsSatisfiable(assertion);
    }

    public BigInteger GetSingleValue(IValue value)
    {
        return value is IConstantValue v
            ? v.AsUnsigned()
            : GetSolver().GetSingleValue(value);
    }

    public BigInteger GetExampleValue(IValue value)
    {
        return value is IConstantValue v
            ? v.AsUnsigned()
            : GetSolver().GetExampleValue(value);
    }

    public IExample GetExample()
    {
        return GetSolver().GetExample();
    }

    private ISolver GetSolver()
    {
        var solver = _solver ?? PooledSolver.Create();

        foreach (var action in _actions)
            action(solver);

        _actions.Clear();
        return solver;
    }
}
