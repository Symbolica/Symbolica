using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation.Values.Constants;

internal sealed class ConstantUnsigned : BitVector, IConstantValue
{
    private readonly BigInteger _value;

    private ConstantUnsigned(Bits size, BigInteger value)
        : base(size)
    {
        _value = value;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return context.MkBV(_value.ToString(), (uint) Size);
    }

    public ConstantBitVector AsBitVector(ICollectionFactory collectionFactory)
    {
        return ConstantBitVector.Create(collectionFactory, Size, this);
    }

    public ConstantUnsigned AsUnsigned()
    {
        return this;
    }

    public ConstantSigned AsSigned()
    {
        return ConstantSigned.Create(Size, _value);
    }

    public ConstantBool AsBool()
    {
        return new ConstantBool(!_value.IsZero);
    }

    public ConstantSingle AsSingle()
    {
        return AsSigned().AsSingle();
    }

    public ConstantDouble AsDouble()
    {
        return AsSigned().AsDouble();
    }

    public ConstantUnsigned Extend(Bits size)
    {
        return new ConstantUnsigned(size, _value);
    }

    public static implicit operator BigInteger(ConstantUnsigned value)
    {
        return value._value;
    }

    public static explicit operator int(ConstantUnsigned value)
    {
        return (int) (BigInteger) value;
    }

    public static ConstantUnsigned Create(Bits size, BigInteger value)
    {
        return new ConstantUnsigned(size, value.IsZero || value.Sign > 0 && value.GetBitLength() <= (uint) size
            ? value
            : Normalize(size, value));
    }

    private static BigInteger Normalize(Bits size, BigInteger value)
    {
        return value & ((BigInteger.One << (int) (uint) size) - BigInteger.One);
    }
}
