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

    public void Assert(IEnumerable<IValue> assertions)
    {
        _context.Assert(assertions.Select(a => a.AsBool(_context)));
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

    public BigInteger GetSingleValue(IValue value)
    {
        return value is Float && value.AsFloat(_context).Simplify().IsFPNaN
            ? value.Size.GetNan(_context)
            : AsConstant(value);
    }

    public BigInteger GetExampleValue(IValue value)
    {
        return _context.Evaluate(value.AsBitVector(_context)).BigInteger;
    }

    public IEnumerable<KeyValuePair<string, string>> GetExampleValues()
    {
        return _context.Evaluate().Select(p =>
            new KeyValuePair<string, string>(p.Key.Name.ToString(), p.Value.ToString()));
    }

    private BigInteger AsConstant(IValue value)
    {
        var expr = value.AsBitVector(_context).Simplify();

        var constant = expr.IsNumeral
            ? ((BitVecNum) expr).BigInteger
            : throw new IrreducibleSymbolicExpressionException();

        return constant == GetExampleValue(value)
            ? constant
            : throw new IrreducibleSymbolicExpressionException();
    }

    public static IConstraints Create<TContext>()
        where TContext : IContext, new()
    {
        return new Constraints(new TContext());
    }
}
