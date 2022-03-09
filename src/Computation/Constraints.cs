using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Exceptions;

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
        var status = _context.Check(assertion.AsBool(_context));

        return status switch
        {
            Status.UNSATISFIABLE => false,
            Status.SATISFIABLE => true,
            Status.UNKNOWN => throw new UnexpectedSatisfiabilityException(status),
            _ => throw new UnexpectedSatisfiabilityException(status)
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

    public static IConstraints Create(IEnumerable<IValue> assertions)
    {
        var context = ContextFactory.Create();
        context.Assert(assertions.Select(a => a.AsBool(context)));

        return new Constraints(context);
    }
}
