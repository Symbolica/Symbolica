using System;
using System.Numerics;
using Symbolica.Collection;

namespace Symbolica.Expression.Values.Constants;

public sealed record ConstantSingle : IConstantFloat
{
    private readonly float _value;

    public ConstantSingle(float value)
    {
        Type = Float.Single();
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
        return ConstantSigned.Create(Type.Size, BitConverter.SingleToInt32Bits(_value));
    }

    public ConstantBool AsBool()
    {
        return AsUnsigned().AsBool();
    }

    public ConstantSingle AsSingle()
    {
        return this;
    }

    public ConstantDouble AsDouble()
    {
        return ConstantDouble.Create(AsSigned());
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

    public static implicit operator float(ConstantSingle value)
    {
        return value._value;
    }

    public static ConstantSingle Create(ConstantSigned value)
    {
        return value.Type.Size == (Bits) 32U
            ? new ConstantSingle(BitConverter.Int32BitsToSingle((int) (BigInteger) value))
            : throw new InconsistentExpressionSizesException(value.Type.Size, (Bits) 32U);
    }
}
