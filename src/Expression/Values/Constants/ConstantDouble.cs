using System;
using System.Numerics;
using Symbolica.Collection;

namespace Symbolica.Expression.Values.Constants;

public sealed record ConstantDouble : IFloat, IConstantValue
{
    private readonly double _value;

    public ConstantDouble(double value)
    {
        _value = value;
    }

    public Bits Size => (Bits) 64U;

    public ConstantBitVector AsBitVector(ICollectionFactory collectionFactory)
    {
        return AsUnsigned().AsBitVector(collectionFactory);
    }

    public ConstantUnsigned AsUnsigned()
    {
        return AsSigned().AsUnsigned();
    }

    public ConstantSigned AsSigned()
    {
        return ConstantSigned.Create(Size, BitConverter.DoubleToInt64Bits(_value));
    }

    public ConstantBool AsBool()
    {
        return AsUnsigned().AsBool();
    }

    public ConstantSingle AsSingle()
    {
        return ConstantSingle.Create(AsSigned());
    }

    public ConstantDouble AsDouble()
    {
        return this;
    }

    public bool Equals(IExpression? other)
    {
        return AsUnsigned().Equals(other);
    }

    public T Map<T>(IExprMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IConstantMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public static implicit operator double(ConstantDouble value)
    {
        return value._value;
    }

    public static ConstantDouble Create(ConstantSigned value)
    {
        return value.Size == (Bits) 64U
            ? new ConstantDouble(BitConverter.Int64BitsToDouble((long) (BigInteger) value))
            : throw new InconsistentExpressionSizesException(value.Size, (Bits) 64U);
    }
}
