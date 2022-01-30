using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;

namespace Symbolica.Computation;

internal sealed class Model : IModel
{
    private readonly IContextHandle _handle;
    private readonly Solver _solver;

    private Model(IContextHandle handle, Solver solver)
    {
        _handle = handle;
        _solver = solver;
    }

    public void Dispose()
    {
        _handle.Dispose();
    }

    public bool IsSatisfiable(IValue assertion)
    {
        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        return _solver.Check(assertion.AsBool(_handle.Context)) switch
        {
            Status.UNSATISFIABLE => false,
            Status.SATISFIABLE => true,
            _ => throw new Exception("Satisfiability is unknown.")
        };
    }

    public BigInteger Evaluate(IValue value)
    {
        return _solver.Check() == Status.SATISFIABLE
            ? ((BitVecNum) _solver.Model.Eval(value.AsBitVector(_handle.Context), true)).BigInteger
            : throw new Exception("The model cannot be evaluated.");
    }

    public IEnumerable<KeyValuePair<string, string>> Evaluate()
    {
        return _solver.Check() == Status.SATISFIABLE
            ? _solver.Model.Consts.Select(
                p => new KeyValuePair<string, string>(p.Key.Name.ToString(), p.Value.ToString()))
            : Enumerable.Empty<KeyValuePair<string, string>>();
    }

    public static IModel Create(IContextFactory contextFactory, IEnumerable<IValue> assertions)
    {
        var handle = contextFactory.Create();
        var solver = handle.Context.MkSolver(handle.Context.MkTactic("smt"));
        solver.Assert(assertions.Select(a => a.AsBool(handle.Context)).ToArray());

        return new Model(handle, solver);
    }
}
