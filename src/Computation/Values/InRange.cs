using System.Diagnostics.CodeAnalysis;
using Microsoft.Z3;

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

    internal bool IsUnsatisfiable(Bool assertion)
    {
        return assertion switch
        {
            Equal equal => IsUnsatisfiable(equal.AsRange(true)) || IsUnsatisfiable(equal.AsRange(false)),
            InRange inRange => IsUnsatisfiable(inRange),
            _ => false
        };
    }

    private bool IsUnsatisfiable(InRange other)
    {
        return _range.IsDisjoint(other._range) && other._value.Equals(_value);
    }

    public override bool TryMerge(IValue value, [MaybeNullWhen(false)] out IValue merged)
    {
        merged = null;
        return value switch
        {
            Equal equal => equal.TryMerge(this, out merged),
            InRange inRange => TryMerge(inRange, out merged),
            _ => false
        };
    }

    private bool TryMerge(InRange inRange, [MaybeNullWhen(false)] out IValue merged)
    {
        if (_value.Equals(inRange._value))
        {
            var mergedRange = _range.TryMergeRange(inRange._range);
            if (mergedRange is not null)
            {
                merged = new InRange(_value, mergedRange);
                return true;
            }
        }
        merged = null;
        return false;
    }
}