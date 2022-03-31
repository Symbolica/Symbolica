using System.Numerics;
using Symbolica.Collection;

namespace Symbolica.Expression.Values.Constants;

public sealed record ConstantBool : Bool, IConstantValue
{
    private readonly bool _value;

    public ConstantBool(bool value)
    {
        _value = value;
    }

    public ConstantBitVector AsBitVector(ICollectionFactory collectionFactory)
    {
        return AsUnsigned().AsBitVector(collectionFactory);
    }

    public ConstantUnsigned AsUnsigned()
    {
        return ConstantUnsigned.Create(Size, _value ? BigInteger.One : BigInteger.Zero);
    }

    public ConstantSigned AsSigned()
    {
        return ConstantSigned.Create(Size, _value ? BigInteger.One : BigInteger.Zero);
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

    public override bool Equals(IExpression? other)
    {
        return AsUnsigned().Equals(other);
    }

    public override T Map<T>(IExprMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IConstantMapper<T> mapper)
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
