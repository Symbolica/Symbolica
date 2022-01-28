using System.Collections.Generic;
using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed class LogicalNot : Bool
{
    private readonly Logical _value;

    private LogicalNot(Logical value)
    {
        _value = value;
    }

    public override IEnumerable<IValue> Children => new[] { _value };

    public override string? PrintedValue => null;

    public override BoolExpr AsBool(IContext context)
    {
        return context.CreateExpr(c => c.MkNot(_value.AsBool(context)));
    }

    public static IValue Create(IValue value)
    {
        return value switch
        {
            IConstantValue v => v.AsBool().Not(),
            LogicalNot v => v._value,
            Logical v => new LogicalNot(v),
            _ => new LogicalNot(new Logical(value))
        };
    }

    private sealed class Logical : Bool
    {
        private readonly IValue _value;

        public Logical(IValue value)
        {
            _value = value;
        }

        public override IEnumerable<IValue> Children => new[] { _value };

        public override string? PrintedValue => null;

        public override BoolExpr AsBool(IContext context)
        {
            return _value.AsBool(context);
        }
    }
}
