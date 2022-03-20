using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record SignExtend : BitVector
{
    private readonly IValue _value;

    private SignExtend(Bits size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        using var value = _value.AsBitVector(solver);
        return solver.Context.MkSignExt((uint) (Size - _value.Size), value);
    }

    public override bool Equals(IValue? other) => Equals(other as SignExtend);

    public static IValue Create(Bits size, IValue value)
    {
        return size > value.Size
            ? value is IConstantValue v
                ? v.AsSigned().Extend(size)
                : new SignExtend(size, value)
            : value;
    }
}
