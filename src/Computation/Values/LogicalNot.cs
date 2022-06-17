using System;
using System.Collections.Generic;
using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed record LogicalNot : Bool
{
    private readonly Bool _value;

    private LogicalNot(Bool value)
    {
        _value = value;
    }

    public override ISet<IValue> Symbols => _value.Symbols;

    public override BoolExpr AsBool(ISolver solver)
    {
        using var value = _value.AsBool(solver);
        return solver.Context.MkNot(value);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as LogicalNot);
    }

    public override bool TryMerge(IValue value, out IValue? merged)
    {
        if (value is LogicalNot not && _value.TryMerge(not._value, out var mergedLogical) && mergedLogical is not null)
        {
            merged = Create(mergedLogical);
            return true;
        }
        if (value is Bool given)
            if (IsUnsatisfiable(given, _value))
            {
                // This value asserts some negation that is already covered by the domain of the incoming assertion
                merged = given;
                return true;
            }
            else if (_value.Equals(given))
            {
                // This not is the opposite of the incoming value so they cancel out
                merged = null;
                return true;
            }
        merged = null;
        return false;
    }

    private static bool IsUnsatisfiable(Bool given, Bool assertion)
    {
        return given switch
        {
            Equal equal => equal.AsRange(true).IsUnsatisfiable(assertion) || equal.AsRange(false).IsUnsatisfiable(assertion),
            InRange inRange => inRange.IsUnsatisfiable(assertion),
            _ => false
        };
    }

    public static Bool Create(IValue value)
    {
        return value switch
        {
            IConstantValue v => v.AsBool().Not(),
            LogicalNot v => v._value,
            LogicalAnd a => a.DeMorgan(),
            LogicalOr o => o.DeMorgan(),
            _ => new LogicalNot(Logical.Create(value))
        };
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return other is LogicalNot v
            ? _value.IsEquivalentTo(v._value)
            : (new(), false);
    }

    public override IValue Substitute(IReadOnlyDictionary<IValue, IValue> subs)
    {
        return subs.TryGetValue(this, out var sub)
            ? sub
            : Create(_value.Substitute(subs));
    }

    public override object ToJson()
    {
        return new
        {
            Type = GetType().Name,
            Size = (uint) Size,
            Value = _value.ToJson()
        };
    }

    public override int GetEquivalencyHash()
    {
        return HashCode.Combine(
            GetType().Name,
            _value.GetEquivalencyHash());
    }

    public override IValue RenameSymbols(Func<string, string> renamer)
    {
        return Create(_value.RenameSymbols(renamer));
    }
}

internal sealed record Logical : Bool
{
    private readonly IValue _value;

    private Logical(IValue value)
    {
        _value = value;
    }

    public override ISet<IValue> Symbols => _value.Symbols;

    public override BoolExpr AsBool(ISolver solver)
    {
        return _value.AsBool(solver);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as Logical);
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return other is Logical v
            ? _value.IsEquivalentTo(v._value)
            : (new(), false);
    }

    public override bool TryMerge(IValue value, out IValue? merged)
    {
        if (value is Logical logical && _value.TryMerge(logical._value, out var mergedLogical) && mergedLogical is not null)
        {
            merged = new Logical(mergedLogical);
            return true;
        }
        merged = null;
        return false;
    }

    public override IValue Substitute(IReadOnlyDictionary<IValue, IValue> subs)
    {
        return subs.TryGetValue(this, out var sub)
            ? sub
            : Create(_value.Substitute(subs));
    }

    public override object ToJson()
    {
        return new
        {
            Type = GetType().Name,
            Size = (uint) Size,
            Value = _value.ToJson()
        };
    }

    public override int GetEquivalencyHash()
    {
        return HashCode.Combine(
            GetType().Name,
            _value.GetEquivalencyHash());
    }

    public override IValue RenameSymbols(Func<string, string> renamer)
    {
        return Create(_value.RenameSymbols(renamer));
    }

    public static Bool Create(IValue value)
    {
        return value switch
        {
            IConstantValue c => c.AsBool(),
            Bool b => b,
            _ => new Logical(value)
        };
    }
}
