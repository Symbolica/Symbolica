namespace Symbolica.Expression;

public readonly record struct Offset(
    Bytes AggregateSize,
    string AggregateType,
    Bytes FieldSize,
    IExpression Value);
