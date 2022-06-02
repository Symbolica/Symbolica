using System.Collections.Generic;
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

    public override ISet<IValue> Symbols => new HashSet<IValue>();

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        return solver.Context.MkBV(_value.ToString(), (uint) Size);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as SymbolicUnsigned);
    }

    public override int GetEquivalencyHash()
    {
        throw new System.NotImplementedException();
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        throw new System.NotImplementedException();
    }

    public override IValue Substitute(IReadOnlyDictionary<IValue, IValue> subs)
    {
        throw new System.NotImplementedException();
    }

    public override object ToJson()
    {
        throw new System.NotImplementedException();
    }
}
