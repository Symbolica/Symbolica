using Symbolica.Computation.Values;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal readonly record struct Offset<TSize>(
    TSize AggregateSize,
    string AggregateType,
    TSize FieldSize,
    IValue Value)
{
    public Offset<TSize> Negate()
    {
        return new(
            AggregateSize,
            AggregateType,
            FieldSize,
            Multiply.Create(Value, ConstantUnsigned.Create(Value.Size, -1)));
    }

    internal Offset<TSize> Add(IValue value)
    {
        return new(AggregateSize, AggregateType, FieldSize, Values.Add.Create(Value, value));
    }

    internal Offset<TSize> TryMakeConstant(IAssertions assertions)
    {
        return new(AggregateSize, AggregateType, FieldSize, Value.TryMakeConstant(assertions));
    }
}

internal static class OffsetExtensions
{
    internal static Offset<Bits> BitCast(this Offset<Bits> offset, Bits targetSize)
    {
        return new Offset<Bits>(
            offset.AggregateSize,
            offset.AggregateType,
            targetSize,
            offset.Value);
    }

    internal static Offset<Bytes> BitCast(this Offset<Bytes> offset, Bits targetSize)
    {
        return new Offset<Bytes>(
            offset.AggregateSize,
            offset.AggregateType,
            targetSize.ToBytes(),
            offset.Value);
    }

    public static Offset<Bits> Multiply(this Offset<Bits> offset, uint value)
    {
        return new Offset<Bits>(
            (Bits) ((uint) offset.AggregateSize * value),
            offset.AggregateType,
            (Bits) ((uint) offset.FieldSize * value),
            Values.Multiply.Create(offset.Value, ConstantUnsigned.Create(offset.Value.Size, value)));
    }

    public static Offset<Bytes> Multiply(this Offset<Bytes> offset, uint value)
    {
        return new Offset<Bytes>(
            (Bytes) ((uint) offset.AggregateSize * value),
            offset.AggregateType,
            (Bytes) ((uint) offset.FieldSize * value),
            Values.Multiply.Create(offset.Value, ConstantUnsigned.Create(offset.Value.Size, value)));
    }

    public static Offset<Bits> ToBits(this Offset<Bytes> offset)
    {
        return new Offset<Bits>(
            offset.AggregateSize.ToBits(),
            offset.AggregateType,
            offset.FieldSize.ToBits(),
            Values.Multiply.Create(offset.Value, ConstantUnsigned.Create(offset.Value.Size, (uint) Bytes.One.ToBits())));
    }
}
