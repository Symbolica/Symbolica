using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal abstract record Integer : IValue
{
    protected Integer(Bits size)
    {
        Size = size;
    }

    public Bits Size { get; }
    public abstract IEnumerable<IValue> Children { get; }
    public abstract string? PrintedValue { get; }

    public abstract BitVecExpr AsBitVector(ISolver solver);
    public abstract BoolExpr AsBool(ISolver solver);
    public abstract bool Equals(IValue? other);

    public FPExpr AsFloat(ISolver solver)
    {
        using var bitVector = AsBitVector(solver);
        using var sort = Size.GetSort(solver);
        return solver.Context.MkFPToFP(bitVector, sort);
    }

    public virtual IValue BitCast(Bits targetSize) => this;

    public virtual IValue ToBits() => Values.Multiply.Create(this, ConstantUnsigned.Create(Size, (uint) Bytes.One.ToBits()));
}
