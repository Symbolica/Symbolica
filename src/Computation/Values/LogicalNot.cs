using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record LogicalNot : Bool
{
    private readonly Logical _value;

    private LogicalNot(Logical value)
    {
        _value = value;
    }

    public override BoolExpr AsBool(ISolver solver)
    {
        using var value = _value.AsBool(solver);
        return solver.Context.MkNot(value);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as LogicalNot);
    }

    public override bool TryMerge(IValue value, [MaybeNullWhen(false)] out IValue merged)
    {
        if (value is LogicalNot not && _value.TryMerge(not._value, out var mergedLogical))
        {
            merged = Create(mergedLogical);
            return true;
        }
        if (_value.Value is Bool assertion && value is Bool given && IsUnsatisfiable(given, assertion))
        {
            merged = given;
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

    private sealed record Logical : Bool
    {
        public Logical(IValue value)
        {
            Value = value;
        }

        internal IValue Value { get; }

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

        public override bool TryMerge(IValue value, [MaybeNullWhen(false)] out IValue merged)
        {
            if (value is Logical logical && Value.TryMerge(logical.Value, out var mergedLogical))
            {
                merged = new Logical(mergedLogical);
                return true;
            }
            merged = null;
            return false;
        }
    }
}
