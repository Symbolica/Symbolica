using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation.Values.Constants;

internal sealed class ConstantSigned : BitVector, IConstantValue
{
    private readonly BigInteger _value;

    private ConstantSigned(Bits size, BigInteger value)
        : base(size)
    {
        _value = value;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return AsUnsigned().AsBitVector(context);
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

    public ConstantSigned Extend(Bits size)
    {
        return new ConstantSigned(size, _value);
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
}
