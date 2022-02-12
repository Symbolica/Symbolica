using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class Symbol : BitVector
{
    private readonly string _name;

    private Symbol(Bits size, string name)
        : base(size)
    {
        _name = name;
    }

    public override BitVecExpr AsBitVector(IContext context)
    {
        return context.CreateExpr(c => c.MkBVConst(_name, (uint) Size));
    }

    public static IValue Create(Bits size, string? name)
    {
        return new Symbol(size, name ?? Guid.NewGuid().ToString());
    }
}
