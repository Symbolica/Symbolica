using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation;

public struct Offset
{
    public Offset(Bytes aggregateSize, IOperand value)
    {
        AggregateSize = aggregateSize;
        Value = value;
    }

    public Bytes AggregateSize { get; }
    public IOperand Value { get; }
}
