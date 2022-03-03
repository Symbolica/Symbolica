using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

    public override IValue BitCast(Bits targetSize)
    {
        return this switch
        {
            Address<Bits> a => a.ReplaceLastOffset(a.Offsets.Last().BitCast(targetSize)),
            Address<Bytes> a => a.ReplaceLastOffset(a.Offsets.Last().BitCast(targetSize)),
            _ => throw new Exception($"{typeof(TSize)} is not a supported size for bit casting.")
        };
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

        return baseAddress switch
        {
            Address<TSize> b => Merge(b),
            _ => new Address<TSize>(
                baseAddress,
                offsets.Select(
                    o => o.Value is Address<TSize> a
                        ? new Offset<TSize>(o.AggregateSize, o.AggregateType, o.FieldSize, a.Aggregate())
                        : o))
        };
    }

    internal Address<TSize> ReplaceLastOffset(Offset<TSize> offset)
    {
        return Create(BaseAddress, Offsets.SkipLast(1).Append(offset));
    }
}

internal static class AddressExtensions
{
    internal static IEnumerable<(Address<Bits>, Bits)> GetAddresses(this Address<Bits> address, Bits length)
    {
        return address.GetAddresses(length, x => (uint) x);
    }

    internal static IEnumerable<(Address<Bytes>, Bytes)> GetAddresses(this Address<Bytes> address, Bytes length)
    {
        return address.GetAddresses(length, x => (uint) x);
    }

    private static IEnumerable<(Address<TSize>, TSize)> GetAddresses<TSize>(
        this Address<TSize> address,
        TSize length,
        Func<TSize, uint> ToUint)
    {
        var field = address.Offsets.Last();
        if (ToUint(length) > ToUint(field.AggregateSize))
            throw new Exception("Can't generate addresses, the length would be out of bounds.");

        if (ToUint(length) % ToUint(field.FieldSize) != 0)
            throw new Exception("Can only deal with lengths that are whole field multiples right now.");

        var numFields = ToUint(length) / ToUint(field.FieldSize);
        IValue CreateFieldMultiple(int i)
        {
            return ConstantUnsigned.Create(field.Value.Size, (uint) i * ToUint(field.FieldSize));
        }

        return Enumerable.Range(0, (int) numFields)
            .Select(i => (
                address.ReplaceLastOffset(field.Add(CreateFieldMultiple(i))),
                field.FieldSize));
    }

    internal static IValue Multiply(this Address<Bits> address, IConstantValue value)
    {
        return Address<Bits>.Create(
            Values.Multiply.Create(address.BaseAddress, value),
            address.Offsets.Select(o => o.Multiply((uint) (BigInteger) value.AsUnsigned())));
    }

    internal static IValue Multiply(this Address<Bytes> address, IConstantValue value)
    {
        return Address<Bytes>.Create(
            Values.Multiply.Create(address.BaseAddress, value),
            address.Offsets.Select(o => o.Multiply((uint) (BigInteger) value.AsUnsigned())));
    }
}
