using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed record AggregateOffset : Integer
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

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        return Aggregate().AsBitVector(solver);
    }

    public override BoolExpr AsBool(ISolver solver)
    {
        return Aggregate().AsBool(solver);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as AggregateOffset);
    }

    internal IValue Multiply(IConstantValue value)
    {
        return Create(
            Values.Multiply.Create(_baseAddress, value),
            _offsets.Select(o => (value.AsUnsigned() * o.Item1, Values.Multiply.Create(o.Item2, value))).ToArray());
    }

    internal IValue Subtract(IValue value)
    {
        return Create(Values.Subtract.Create(_baseAddress, value), _offsets);
    }

    private bool IsConstant => _baseAddress is IConstantValue && _offsets.All(o => o.Item2 is IConstantValue);

    private IValue Aggregate()
    {
        return _offsets.Aggregate(_baseAddress, (l, r) => Add.Create(l, r.Item2));
    }

    public static IValue Create(IValue baseAddress, (BigInteger, IValue)[] offsets)
    {
        var aggregateOffset = new AggregateOffset(baseAddress, offsets);
        // TODO: Remove this nonsense once we're exploting AOs in Writes, but right now it's too slow without this as everything appears "symbolic".
        return aggregateOffset.IsConstant ? aggregateOffset.Aggregate() : aggregateOffset;
    }
}
