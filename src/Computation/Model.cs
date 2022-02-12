using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;

namespace Symbolica.Computation;

internal sealed class Model : IModel
{
    private readonly IContext _context;
    private readonly Solver _solver;

    private Model(IContext context, Solver solver)
    {
        _context = context;
        _solver = solver;
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public bool IsSatisfiable(IValue assertion)
    {
        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        return _solver.Check(assertion.AsBool(_context)) switch
        {
            Status.UNSATISFIABLE => false,
            Status.SATISFIABLE => true,
            _ => throw new Exception("Satisfiability is unknown.")
        };
    }

    public BigInteger Evaluate(IValue value)
    {
        return _solver.Check() == Status.SATISFIABLE
            ? ((BitVecNum) _solver.Model.Eval(value.AsBitVector(_context), true)).BigInteger
            : throw new Exception("The model cannot be evaluated.");
    }

    public IEnumerable<KeyValuePair<string, string>> Evaluate()
    {
        return _solver.Check() == Status.SATISFIABLE
            ? _solver.Model.Consts.Select(
                p => new KeyValuePair<string, string>(p.Key.Name.ToString(), p.Value.ToString()))
            : Enumerable.Empty<KeyValuePair<string, string>>();
    }

    public static IModel Create<TContext>(IEnumerable<IValue> assertions)
        where TContext : IContext, new()
    {
        var context = new TContext();
        var solver = context.CreateSolver();
        solver.Assert(assertions.Select(a => a.AsBool(context)).ToArray());

        return new Model(context, solver);
    }
}
