using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Z3;
using Symbolica.Expression;

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

    public override ISet<IValue> Symbols => _value.Symbols.Union(_range.Symbols).ToHashSet();

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

    public override bool TryMerge(IValue value, out IValue? merged)
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

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return other is InRange v
            ? _value.IsEquivalentTo(v._value)
                .And(_range.IsEquivalentTo(v._range))
            : (new(), false);
    }

    public override IValue Substitute(IReadOnlyDictionary<IValue, IValue> subs)
    {
        return subs.TryGetValue(this, out var sub)
            ? sub
            : new InRange(_value.Substitute(subs), _range.Substitute(subs));
    }

    public override object ToJson()
    {
        return new
        {
            Type = GetType().Name,
            Size = (uint) Size,
            Range = _range.ToJson(),
            Value = _value.ToJson()
        };
    }

    public override int GetEquivalencyHash(bool includeSubs)
    {
        return HashCode.Combine(
            GetType().Name,
            _value.GetEquivalencyHash(includeSubs),
            _range.GetEquivalencyHash(includeSubs));
    }

    public override IValue RenameSymbols(Func<string, string> renamer)
    {
        return new InRange(_value.RenameSymbols(renamer), _range.RenameSymbols(renamer));
    }
}
