namespace Symbolica.Expression.Values.Constants;

public sealed record NormalFloat : IFloatExpression
{
    public NormalFloat(Bits size, string value)
    {
        Type = new Float(size);
        Value = value;
    }

    public Float Type { get; }

    public string Value { get; }

    public bool Equals(IExpression<IType>? other)
    {
        return Equals(other as NormalFloat);
    }

    public U Map<U>(IArityMapper<U> mapper)
    {
        return mapper.Map(this);
    }

    public U Map<U>(ITypeMapper<U> mapper)
    {
        return mapper.Map(this);
    }

    public U Map<U>(IFloatMapper<U> mapper)
    {
        return mapper.Map(this);
    }
}
