using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation;

public readonly record struct Offset(
    Bytes AggregateSize,
    string AggregateType,
    Bytes FieldSize,
    IOperand Value);
