using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;

namespace Symbolica.Computation;

internal sealed class Constraints : IConstraints
{
    private readonly IContext _context;

    private Constraints(IContext context)
    {
        _context = context;
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public bool IsSatisfiable(IValue assertion)
    {
        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        return _context.Solver.Check(assertion.AsBool(_context)) switch
        {
            Status.UNSATISFIABLE => false,
            Status.SATISFIABLE => true,
            _ => throw new Exception("Satisfiability is unknown.")
        };
    }

    public BigInteger Evaluate(IValue value)
    {
        var expr = value.AsBitVector(_context);

        return _context.Solver.Check() == Status.SATISFIABLE
            ? ((BitVecNum) _context.Solver.Model.Eval(expr, true)).BigInteger
            : throw new Exception("The model cannot be evaluated.");
    }

    public IEnumerable<KeyValuePair<string, string>> Evaluate()
    {
        return _context.Solver.Check() == Status.SATISFIABLE
            ? _context.Solver.Model.Consts.Select(
                p => new KeyValuePair<string, string>(p.Key.Name.ToString(), p.Value.ToString()))
            : Enumerable.Empty<KeyValuePair<string, string>>();
    }

    public static IConstraints Create<TContext>(IEnumerable<IValue> assertions)
        where TContext : IContext, new()
    {
        var context = new TContext();
        context.Assert(assertions.Select(a => a.AsBool(context)));

        return new Constraints(context);
    }
}
