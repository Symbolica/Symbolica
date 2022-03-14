using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed class Truncate : BitVector
{
    private readonly IValue _value;

    private Truncate(Bits size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override IEnumerable<IValue> Children => new[] { _value };

    public override string? PrintedValue => null;

    public override BitVecExpr AsBitVector(IContext context)
    {
        using var t = _value.AsBitVector(context);
        return context.CreateExpr(c => c.MkExtract((uint) (Size - Bits.One), 0U, t));
    }

    public static IValue Create(Bits size, IValue value)
    {
        return size < value.Size
            ? value is IConstantValue v
                ? v.AsUnsigned().Truncate(size)
                : new Truncate(size, value)
            : value;
    }
}
