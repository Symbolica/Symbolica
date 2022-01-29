using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed class Symbol : BitVector
{
    private readonly string _name;
    private readonly ISymbolFactory _symbolFactory;

    private Symbol(ISymbolFactory symbolFactory, Bits size, string name)
        : base(size)
    {
        _symbolFactory = symbolFactory;
        _name = name;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return _symbolFactory.Create(context, Size, _name);
    }

    public static IValue Create(ISymbolFactory symbolFactory, Bits size, string? name)
    {
        return new Symbol(symbolFactory, size, name ?? Guid.NewGuid().ToString());
    }
}
