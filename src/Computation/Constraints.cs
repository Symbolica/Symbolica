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
        return _context.Check(assertion.AsBool(_context)) switch
        {
            Status.UNSATISFIABLE => false,
            Status.SATISFIABLE => true,
            _ => throw new Exception("Satisfiability is unknown.")
        };
    }

    public BigInteger Evaluate(IValue value)
    {
        return _context.Evaluate(value.AsBitVector(_context)).BigInteger;
    }

    public IEnumerable<KeyValuePair<string, string>> Evaluate()
    {
        return _context.Evaluate().Select(p =>
            new KeyValuePair<string, string>(p.Key.Name.ToString(), p.Value.ToString()));
    }

    public static IConstraints Create<TContext>(IEnumerable<IValue> assertions)
        where TContext : IContext, new()
    {
        var context = new TContext();
        context.Assert(assertions.Select(a => a.AsBool(context)));

        return new Constraints(context);
    }
}
