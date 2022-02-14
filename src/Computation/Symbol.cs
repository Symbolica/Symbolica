using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class Symbol : BitVector
{
    private readonly IValue[] _assertions;
    private readonly string _name;

    private Symbol(Bits size, string name, IValue[] assertions)
        : base(size)
    {
        _name = name;
        _assertions = assertions;
    }

    public override BitVecExpr AsBitVector(IContext context)
    {
        context.Assert(_name, _assertions.Select(a => a.AsBool(context)));

        return context.CreateExpr(c => c.MkBVConst(_name, (uint) Size));
    }

    public static IValue Create(Bits size, string name, IEnumerable<Func<IValue, IValue>> assertions)
    {
        var unconstrained = new Symbol(size, name, Array.Empty<IValue>());

        return new Symbol(size, name, assertions
            .Select(a => a(unconstrained))
            .ToArray());
    }
}
