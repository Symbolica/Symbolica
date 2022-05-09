using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed record InRange : Bool
{
    private readonly IValue _value;
    private readonly Range _range;

    public InRange(IValue value, Range range)
    {
        _value = value;
        _range = range;
    }

    public override BoolExpr AsBool(ISolver solver)
    {
        return LogicalAnd.Create(
            UnsignedGreaterOrEqual.Create(_value, _range.Min),
            UnsignedLessOrEqual.Create(_value, _range.Max)).AsBool(solver);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as InRange);
    }

    public override bool TryMerge(IValue value, [MaybeNullWhen(false)] out IValue merged)
    {
        merged = null;
        return value switch
        {
            Equal equal => equal.TryMerge(this, out merged),
            InRange inRange when _value.Equals(inRange._value) => TryMerge(inRange._range, out merged),
            _ => false
        };
    }

    private bool TryMerge(Range range, [MaybeNullWhen(false)] out IValue merged)
    {
        merged = null;
        return (_range, range) switch
        {
            ({ Min: IConstantValue min1, Max: IConstantValue max1 }, { Min: IConstantValue min2, Max: IConstantValue max2 })
                => TryMergeConstantRanges(min1.AsUnsigned(), max1.AsUnsigned(), min2.AsUnsigned(), max2.AsUnsigned(), out merged),
            _ => false
        };
    }

    private bool TryMergeConstantRanges(ConstantUnsigned min1, ConstantUnsigned max1, ConstantUnsigned min2, ConstantUnsigned max2, [MaybeNullWhen(false)] out IValue merged)
    {
        var (min, maxMin) = (BigInteger) min1 < (BigInteger) min2
            ? (min1, min2)
            : (min2, min1);

        var (max, minMax) = (BigInteger) max1 > (BigInteger) max2
            ? (max1, max2)
            : (max2, max1);

        if (((BigInteger) minMax + 1) >= maxMin)
        {
            merged = new InRange(_value, new(min, max));
            return true;
        }

        merged = null;
        return false;
    }
}
