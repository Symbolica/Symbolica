using System.Diagnostics.CodeAnalysis;
using Microsoft.Z3;

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
        merged = value;
        return false;
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

    private sealed record Logical : Bool
    {
        private readonly IValue _value;

        public Logical(IValue value)
        {
            _value = value;
        }

        public override BoolExpr AsBool(ISolver solver)
        {
            return _value.AsBool(solver);
        }

        public override bool Equals(IValue? other)
        {
            return Equals(other as Logical);
        }

        public override bool TryMerge(IValue value, [MaybeNullWhen(false)] out IValue merged)
        {
            if (value is Logical logical && _value.TryMerge(logical._value, out var mergedLogical))
            {
                merged = new Logical(mergedLogical);
                return true;
            }
            merged = value;
            return false;
        }
    }
}
