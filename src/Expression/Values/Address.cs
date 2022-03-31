using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression.Values;

public sealed record Address : IExpression
{
    private Address(IExpression baseAddress, IEnumerable<Offset> offsets)
    {
        BaseAddress = baseAddress;
        Offsets = offsets;
    }

    public IExpression BaseAddress { get; }

    public IEnumerable<Offset> Offsets { get; }

    public Bits Size => BaseAddress.Size;

    public bool Equals(IExpression? other)
    {
        return Equals(other as Address);
    }

    public IExpression BitCast(Bits targetSize)
    {
        return ReplaceLastOffset(Offsets.Last().BitCast(targetSize));
    }

    public IExpression Aggregate()
    {
        return Offsets
            .Aggregate(BaseAddress, (l, r) => Values.Add.Create(l, r.Value));
    }

    internal IExpression Add(IConstantValue value)
    {
        return Create(Values.Add.Create(BaseAddress, value), Offsets);
    }

    public IEnumerable<(Address, Bytes)> GetAddresses(Bytes length)
    {
        var field = Offsets.Last();
        if ((uint) length > (uint) field.AggregateSize)
            throw new Exception("Can't generate addresses, the length would be out of bounds.");

        if ((uint) length % (uint) field.FieldSize != 0)
            throw new Exception("Can only deal with lengths that are whole field multiples right now.");

        var numFields = (uint) length / (uint) field.FieldSize;
        IExpression CreateFieldMultiple(int i)
        {
            return ConstantUnsigned.Create(field.Value.Size, (uint) i * (uint) field.FieldSize);
        }

        return Enumerable.Range(0, (int) numFields)
            .Select(i => (
                ReplaceLastOffset(field.Add(CreateFieldMultiple(i))),
                field.FieldSize));
    }

    internal Address IncrementFinalOffset(Bytes offset)
    {
        var finalOffset = Offsets.Last();
        return ReplaceLastOffset(finalOffset.Add(ConstantUnsigned.Create(finalOffset.Value.Size, (uint) offset)));
    }

    public T Map<T>(IExprMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    internal IExpression Multiply(IConstantValue value)
    {
        return Create(
            Values.Multiply.Create(BaseAddress, value),
            Offsets.Select(o => o.Multiply((uint) (BigInteger) value.AsUnsigned())));
    }

    internal Address Negate()
    {
        static IExpression NegateValue(IExpression value) => Values.Multiply.Create(value, ConstantUnsigned.Create(value.Size, -1));
        return Create(
            NegateValue(BaseAddress),
            Offsets.Select(o => o.Negate()));
    }

    internal IExpression Subtract(IExpression value)
    {
        return Create(Values.Subtract.Create(BaseAddress, value), Offsets);
    }

    internal Address ReplaceLastOffset(Offset offset)
    {
        return Create(BaseAddress, Offsets.SkipLast(1).Append(offset));
    }

    public static Address Create(IExpression baseAddress)
    {
        return Create(baseAddress, Enumerable.Empty<Offset>());
    }

    public static Address Create(IExpression baseAddress, IEnumerable<Offset> offsets)
    {
        Address Merge(Address baseAddress)
        {
            return Create(baseAddress.BaseAddress, baseAddress.Offsets.Concat(offsets));
        }

        return baseAddress switch
        {
            Address b => Merge(b),
            _ => new Address(
                baseAddress,
                offsets.Select(
                    o => o.Value is Address a
                        ? new Offset(o.AggregateSize, o.AggregateType, o.FieldSize, a.Aggregate())
                        : o))
        };
    }
}
