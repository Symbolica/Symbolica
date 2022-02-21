using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed class AggregateOffset : Integer
{
    private readonly IValue _baseAddress;
    private readonly (BigInteger, IValue)[] _offsets;

    private AggregateOffset(IValue baseAddress, (BigInteger, IValue)[] offsets)
        : base(baseAddress.Size)
    {
        _baseAddress = baseAddress;
        _offsets = offsets;
    }

    public override IEnumerable<IValue> Children => new[] { _baseAddress }.Concat(_offsets.Select(x => x.Item2));

    public override string? PrintedValue => null;

    public override BitVecExpr AsBitVector(IContext context)
    {
        return Aggregate().AsBitVector(context);
    }

    public override BoolExpr AsBool(IContext context)
    {
        return Aggregate().AsBool(context);
    }

    internal AggregateOffset Multiply(IConstantValue value)
    {
        return Create(
            Values.Multiply.Create(_baseAddress, value),
            _offsets.Select(o => (value.AsUnsigned() * o.Item1, Values.Multiply.Create(o.Item2, value))).ToArray());
    }

    internal AggregateOffset Subtract(IValue value)
    {
        return Create(Values.Subtract.Create(_baseAddress, value), _offsets);
    }

    private IValue Aggregate()
    {
        return _offsets.Aggregate(_baseAddress, (l, r) => Add.Create(l, r.Item2));
    }

    public static AggregateOffset Create(IValue baseAddress, (BigInteger, IValue)[] offsets)
    {
        return new AggregateOffset(baseAddress, offsets);
    }
}
