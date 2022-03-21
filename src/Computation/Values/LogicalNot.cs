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
    }
}
