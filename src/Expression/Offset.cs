namespace Symbolica.Expression;

public struct Offset
{
    public Offset(Bytes elementSize, IExpression value)
    {
        AggregateSize = elementSize;
        Value = value;
    }

    public Bytes AggregateSize { get; }
    public IExpression Value { get; }
}
