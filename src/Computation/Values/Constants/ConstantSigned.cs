using System.Collections.Generic;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation.Values.Constants;

internal sealed record ConstantSigned : BitVector, IConstantValue
{
    private readonly BigInteger _value;

    private ConstantSigned(Bits size, BigInteger value)
        : base(size)
    {
        _value = value;
    }

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        return AsUnsigned().AsBitVector(solver);
    }

    public ConstantBitVector AsBitVector(ICollectionFactory collectionFactory)
    {
        return AsUnsigned().AsBitVector(collectionFactory);
    }

    public ConstantUnsigned AsUnsigned()
    {
        return ConstantUnsigned.Create(Size, _value);
    }

    public ConstantSigned AsSigned()
    {
        return this;
    }

    public ConstantBool AsBool()
    {
        return AsUnsigned().AsBool();
    }

    public ConstantSingle AsSingle()
    {
        return ConstantSingle.Create(this);
    }

    public ConstantDouble AsDouble()
    {
        return ConstantDouble.Create(this);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as IConstantValue);
    }

    public bool Equals(IConstantValue? other)
    {
        return AsUnsigned().Equals(other);
    }

    public bool Equals(ConstantSigned? other)
    {
        return Equals(other as IConstantValue);
    }

    public override int GetHashCode()
    {
        return AsUnsigned().GetHashCode();
    }

    public ConstantSigned Divide(ConstantSigned value)
    {
        return Create(Size, _value / value._value);
    }

    public ConstantSigned Extend(Bits size)
    {
        return new ConstantSigned(size, _value);
    }

    public ConstantBool Greater(ConstantSigned value)
    {
        return new ConstantBool(_value > value._value);
    }

    public ConstantBool GreaterOrEqual(ConstantSigned value)
    {
        return new ConstantBool(_value >= value._value);
    }

    public ConstantBool Less(ConstantSigned value)
    {
        return new ConstantBool(_value < value._value);
    }

    public ConstantBool LessOrEqual(ConstantSigned value)
    {
        return new ConstantBool(_value <= value._value);
    }

    public ConstantSigned Remainder(ConstantSigned value)
    {
        return Create(Size, _value % value._value);
    }

    public ConstantSigned ShiftRight(ConstantUnsigned value)
    {
        return Create(Size, _value >> (int) (BigInteger) value);
    }

    public ConstantDouble ToDouble()
    {
        return new ConstantDouble((double) _value);
    }

    public ConstantSingle ToSingle()
    {
        return new ConstantSingle((float) _value);
    }

    public static implicit operator BigInteger(ConstantSigned value)
    {
        return value._value;
    }

    public static ConstantSigned Create(Bits size, BigInteger value)
    {
        return new ConstantSigned(size, value.IsZero
            ? value
            : Normalize(size, value));
    }

    private static BigInteger Normalize(Bits size, BigInteger value)
    {
        var msb = BigInteger.One << ((int) (uint) size - 1);
        return (value & (msb - BigInteger.One)) - (value & msb);
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return (new(), Equals(other));
    }

    public override IValue Substitute(IReadOnlyDictionary<IValue, IValue> subs)
    {
        return subs.TryGetValue(this, out var sub)
            ? sub
            : this;
    }
}
