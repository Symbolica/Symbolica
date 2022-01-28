using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation;

internal sealed record SymbolicUnsigned : BitVector
{
    private readonly BigInteger _value;

    public SymbolicUnsigned(ConstantUnsigned value)
        : base(value.Size)
    {
        _value = value;
    }

    public override IEnumerable<IValue> Children => Enumerable.Empty<IValue>();

    public override string? PrintedValue => _value.ToString();

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        return solver.Context.MkBV(_value.ToString(), (uint) Size);
    }

    public override bool Equals(IValue? other) => Equals(other as SymbolicUnsigned);
}
