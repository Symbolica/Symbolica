using System;
using System.Numerics;
using Symbolica.Collection;

namespace Symbolica.Expression.Values.Constants;

public sealed record ConstantDouble : IConstantFloat
{
    private readonly double _value;

    public ConstantDouble(double value)
    {
        Type = Float.Double();
        _value = value;
    }

    public Float Type { get; }

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
        return ConstantSigned.Create(Type.Size, BitConverter.DoubleToInt64Bits(_value));
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

    public bool Equals(IExpression<IType>? other)
    {
        return AsUnsigned().Equals(other);
    }

    public T Map<T>(IArityMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(ITypeMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IConstantMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IFloatMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public static implicit operator double(ConstantDouble value)
    {
        return value._value;
    }

    public static ConstantDouble Create(ConstantSigned value)
    {
        return value.Type.Size == (Bits) 64U
            ? new ConstantDouble(BitConverter.Int64BitsToDouble((long) (BigInteger) value))
            : throw new InconsistentExpressionSizesException(value.Type.Size, (Bits) 64U);
    }
}
