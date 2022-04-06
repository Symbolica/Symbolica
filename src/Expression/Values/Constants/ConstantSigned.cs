using System.Numerics;
using Symbolica.Collection;

namespace Symbolica.Expression.Values.Constants;

public sealed record ConstantSigned : IConstantBitVector
{
    private readonly BigInteger _value;

    private ConstantSigned(Bits size, BigInteger value)
    {
        Type = new BitVector(size);
        _value = value;
    }

    public BitVector Type { get; }

    IInteger IExpression<IInteger>.Type => Type;

    public ConstantBitVector AsBitVector(ICollectionFactory collectionFactory)
    {
        return AsUnsigned().AsBitVector(collectionFactory);
    }

    public ConstantUnsigned AsUnsigned()
    {
        return ConstantUnsigned.Create(Type.Size, _value);
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

    public bool Equals(IExpression<IType>? other)
    {
        return AsUnsigned().Equals(other);
    }

    public ConstantSigned Divide(ConstantSigned value)
    {
        return Create(Type.Size, _value / value._value);
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

    public T Map<T>(IIntegerMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IBitVectorMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public ConstantSigned Remainder(ConstantSigned value)
    {
        return Create(Type.Size, _value % value._value);
    }

    public ConstantSigned ShiftRight(ConstantUnsigned value)
    {
        return Create(Type.Size, _value >> (int) (BigInteger) value);
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
}
