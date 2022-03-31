using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression;

public readonly record struct Offset(
    Bytes AggregateSize,
    string AggregateType,
    Bytes FieldSize,
    IExpression Value)
{
    internal Offset Add(IExpression value)
    {
        return new(AggregateSize, AggregateType, FieldSize, Values.Add.Create(Value, value));
    }

    internal Offset BitCast(Bits targetSize)
    {
        return new Offset(
            AggregateSize,
            AggregateType,
            targetSize.ToBytes(),
            Value);
    }

    internal Offset Multiply(uint value)
    {
        return new Offset(
            (Bytes) ((uint) AggregateSize * value),
            AggregateType,
            (Bytes) ((uint) FieldSize * value),
            Values.Multiply.Create(Value, ConstantUnsigned.Create(Value.Size, value)));
    }

    internal Offset Negate()
    {
        return new(
            AggregateSize,
            AggregateType,
            FieldSize,
            Values.Multiply.Create(Value, ConstantUnsigned.Create(Value.Size, -1)));
    }
}
