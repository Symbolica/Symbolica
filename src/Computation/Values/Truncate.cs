using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record Truncate : BitVector
{
    private readonly IValue _value;

    private Truncate(Bits size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        using var value = _value.AsBitVector(solver);
        return solver.Context.MkExtract((uint) (Size - Bits.One), 0U, value);
    }

    public override bool Equals(IValue? other) => Equals(other as Truncate);

    public static IValue Create(Bits size, IValue value)
    {
        return size < value.Size
            ? value is IConstantValue v
                ? v.AsUnsigned().Truncate(size)
                : new Truncate(size, value)
            : value;
    }
}
