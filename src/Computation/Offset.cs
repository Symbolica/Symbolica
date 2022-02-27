using Symbolica.Computation.Values;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal struct Offset<TSize>
{
    public Offset(TSize aggregateSize, IValue value)
    {
        AggregateSize = aggregateSize;
        Value = value;
    }

    public TSize AggregateSize { get; }
    public IValue Value { get; }

    public Offset<TSize> Negate()
    {
        return new(AggregateSize, Multiply.Create(Value, ConstantUnsigned.Create(Value.Size, -1)));
    }
}

internal static class OffsetExtensions
{
    public static Offset<Bits> ToBits(this Offset<Bytes> offset)
    {
        return new Offset<Bits>(
            offset.AggregateSize.ToBits(),
            Multiply.Create(offset.Value, ConstantUnsigned.Create(offset.Value.Size, (uint) Bytes.One.ToBits())));
    }
}
