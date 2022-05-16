using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record Truncate : BitVector
{
    private readonly IValue _value;

    private Truncate(Bits size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        using var value = _value.AsBitVector(solver);
        return solver.Context.MkExtract((uint) (Size - Bits.One), 0U, value);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as Truncate);
    }

    public static IValue Create(Bits size, IValue value)
    {
        return size < value.Size
            ? value is IConstantValue v
                ? v.AsUnsigned().Truncate(size)
                : new Truncate(size, value)
            : value;
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return other is Truncate v
            ? _value.IsEquivalentTo(v._value)
                .And((new(), Size == v.Size))
            : (new(), false);
    }

    public override IValue Substitute(IReadOnlyDictionary<IValue, IValue> subs)
    {
        return subs.TryGetValue(this, out var sub)
            ? sub
            : Create(Size, _value.Substitute(subs));
    }
}
