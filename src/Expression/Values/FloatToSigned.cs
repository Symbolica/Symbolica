using System.Numerics;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression.Values;

public sealed record FloatToSigned : IUnaryBitVectorExpression
{
    private FloatToSigned(Bits size, IExpression<IType> value)
    {
        Type = new BitVector(size);
        Value = value;
    }

    public IExpression<IType> Value { get; }

    public BitVector Type { get; }

    IInteger IExpression<IInteger>.Type => Type;

    public bool Equals(IExpression<IType>? other)
    {
        return Equals(other as FloatToSigned);
    }

    public T Map<T>(IArityMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(ITypeMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IUnaryMapper<T> mapper)
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

    public static IExpression<IType> Create(Bits size, IExpression<IType> value)
    {
        return Float.Unary(value,
            v => ConstantSigned.Create(size, (BigInteger) v),
            v => ConstantSigned.Create(size, (BigInteger) v),
            v => v is IExpression<Real> r
                ? new RealToSigned(size, r)
                : new FloatToSigned(size, v));
    }
}
