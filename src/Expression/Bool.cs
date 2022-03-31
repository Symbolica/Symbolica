namespace Symbolica.Expression;

public abstract record Bool : IExpression
{
    public Bits Size => Bits.One;

    public abstract bool Equals(IExpression? other);
    public abstract T Map<T>(IExprMapper<T> mapper);
}
