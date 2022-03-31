using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed record FloatNegate : Float
{
    private readonly IValue _value;

    private FloatNegate(IValue value)
        : base(value.Size)
    {
        _value = value;
    }

    public override IEnumerable<IValue> Children => new[] { _value };

    public override string? PrintedValue => null;

    public override FPExpr AsFloat(ISolver solver)
    {
        using var value = _value.AsFloat(solver);
        return solver.Context.MkFPNeg(value);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as FloatNegate);
    }

    public static IValue Create(IValue value)
    {
        return Unary(value,
            v => new ConstantSingle(-v),
            v => new ConstantDouble(-v),
            v => new FloatNegate(v));
    }
}
