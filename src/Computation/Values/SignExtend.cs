using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record SignExtend : BitVector
{
    private readonly IValue _value;

    private SignExtend(Size size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        using var value = _value.AsBitVector(solver);
        return solver.Context.MkSignExt((Size - _value.Size).Bits, value);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as SignExtend);
    }

    public static IValue Create(Size size, IValue value)
    {
        return size > value.Size
            ? value is IConstantValue v
                ? v.AsSigned().Extend(size)
                : new SignExtend(size, value)
            : value;
    }
}
