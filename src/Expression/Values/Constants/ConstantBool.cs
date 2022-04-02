using System.Numerics;
using Symbolica.Collection;

namespace Symbolica.Expression.Values.Constants;

public sealed record ConstantBool : IConstantBool
{
    private readonly bool _value;

    public ConstantBool(bool value)
    {
        _value = value;
    }

    public Bool Type => Bool.Type;

    IInteger IExpression<IInteger>.Type => Type;

    public ConstantBitVector AsBitVector(ICollectionFactory collectionFactory)
    {
        return AsUnsigned().AsBitVector(collectionFactory);
    }

    public ConstantUnsigned AsUnsigned()
    {
        return ConstantUnsigned.Create(Type.Size, _value ? BigInteger.One : BigInteger.Zero);
    }

    public ConstantSigned AsSigned()
    {
        return ConstantSigned.Create(Type.Size, _value ? BigInteger.One : BigInteger.Zero);
    }

    public ConstantBool AsBool()
    {
        return this;
    }

    public ConstantSingle AsSingle()
    {
        return AsSigned().AsSingle();
    }

    public ConstantDouble AsDouble()
    {
        return AsSigned().AsDouble();
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

    public T Map<T>(IIntegerMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IBoolMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public ConstantBool Not()
    {
        return new ConstantBool(!_value);
    }

    public static implicit operator bool(ConstantBool value)
    {
        return value._value;
    }
}
