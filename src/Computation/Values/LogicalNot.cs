using System;
using System.Collections.Generic;
using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed record LogicalNot : Bool
{
    private readonly Logical _value;

    private LogicalNot(Logical value)
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
        if (_value.Value is Bool assertion && value is Bool given)
            if (IsUnsatisfiable(given, assertion))
            {
                // This value asserts some negation that is already covered by the domain of the incoming assertion
                merged = given;
                return true;
            }
            else if (assertion.Equals(given))
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

    public static IValue Create(IValue value)
    {
        return value switch
        {
            IConstantValue v => v.AsBool().Not(),
            LogicalNot v => v._value,
            Logical v => new LogicalNot(v),
            _ => new LogicalNot(new Logical(value))
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

    public override int GetEquivalencyHash(bool includeSubs)
    {
        return HashCode.Combine(
            GetType().Name,
            _value.GetEquivalencyHash(includeSubs));
    }

    private sealed record Logical : Bool
    {
        public Logical(IValue value)
        {
            Value = value;
        }

        internal IValue Value { get; }

        public override ISet<IValue> Symbols => Value.Symbols;

        public override BoolExpr AsBool(ISolver solver)
        {
            return Value.AsBool(solver);
        }

        public override bool Equals(IValue? other)
        {
            return Equals(other as Logical);
        }

        public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
        {
            return other is Logical v
                ? Value.IsEquivalentTo(v.Value)
                : (new(), false);
        }

        public override bool TryMerge(IValue value, out IValue? merged)
        {
            if (value is Logical logical && Value.TryMerge(logical.Value, out var mergedLogical) && mergedLogical is not null)
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
                : Create(Value.Substitute(subs));
        }

        public override object ToJson()
        {
            return new
            {
                Type = GetType().Name,
                Size = (uint) Size,
                Value = Value.ToJson()
            };
        }

        public override int GetEquivalencyHash(bool includeSubs)
        {
            return HashCode.Combine(
                GetType().Name,
                Value.GetEquivalencyHash(includeSubs));
        }
    }
}
