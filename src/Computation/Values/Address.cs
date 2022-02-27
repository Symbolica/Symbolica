using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed class Address<TSize> : Integer
{
    private Address(IValue baseAddress, IEnumerable<Offset<TSize>> offsets)
        : base(baseAddress.Size)
    {
        BaseAddress = baseAddress;
        Offsets = offsets;
    }

    public IValue BaseAddress { get; }

    public IEnumerable<Offset<TSize>> Offsets { get; }

    public override IEnumerable<IValue> Children => new[] { BaseAddress }.Concat(Offsets.Select(x => x.Value));

    public override string? PrintedValue => null;

    public override BitVecExpr AsBitVector(IContext context)
    {
        return Aggregate().AsBitVector(context);
    }

    public override BoolExpr AsBool(IContext context)
    {
        return Aggregate().AsBool(context);
    }

    public override IValue ToBits()
    {
        return Offsets switch
        {
            IEnumerable<Offset<Bytes>> byteOffsets => Address<Bits>.Create(BaseAddress.ToBits(), byteOffsets.Select(o => o.ToBits())),
            IEnumerable<Offset<Bits>> => this,
            _ => throw new Exception($"{typeof(TSize)} is not a supported size.")
        };
    }

    internal IValue Aggregate()
    {
        return Offsets
            .Aggregate(BaseAddress, (l, r) => Values.Add.Create(l, r.Value));
    }

    internal IValue Add(IConstantValue value)
    {
        return Create(Values.Add.Create(BaseAddress, value), Offsets);
    }

    internal IValue Subtract(IValue value)
    {
        return Create(Values.Subtract.Create(BaseAddress, value), Offsets);
    }

    internal Address<TSize> Negate()
    {
        static IValue NegateValue(IValue value) => Multiply.Create(value, ConstantUnsigned.Create(value.Size, -1));
        return Create(
            NegateValue(BaseAddress),
            Offsets.Select(o => o.Negate()));
    }

    public static Address<TSize> Create(IValue baseAddress, IEnumerable<Offset<TSize>> offsets)
    {
        Address<TSize> Merge(Address<TSize> baseAddress)
        {
            return Create(baseAddress.BaseAddress, baseAddress.Offsets.Concat(offsets));
        }

        if (!offsets.Any())
            throw new Exception($"{nameof(Address<TSize>)} must have at least one offset.");

        if (offsets.Any(o => o.Value is Address<Bits> || o.Value is Address<Bytes>))
            Debugger.Break();

        return baseAddress switch
        {
            Address<TSize> b => Merge(b),
            _ => new Address<TSize>(baseAddress, offsets)
        };
    }
}
