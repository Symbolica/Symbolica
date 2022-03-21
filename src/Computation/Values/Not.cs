using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed record Not : BitVector
{
    private readonly IValue _value;

    private Not(IValue value)
        : base(value.Size)
    {
        _value = value;
    }

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        using var value = _value.AsBitVector(solver);
        return solver.Context.MkBVNot(value);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as Not);
    }

    public static IValue Create(IValue value)
    {
        return value switch
        {
            IConstantValue v => v.AsUnsigned().Not(),
            Not v => v._value,
            _ => new Not(value)
        };
    }
}
