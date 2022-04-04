using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression.Values;

public sealed record Address : IBitVectorExpression
{
    private Address(IExpression<IType> baseAddress, IEnumerable<Offset> offsets)
    {
        BaseAddress = baseAddress;
        Offsets = offsets;
        Type = new BitVector(BaseAddress.Size);
    }

    public IExpression<IType> BaseAddress { get; }

    public IEnumerable<Offset> Offsets { get; }

    public BitVector Type { get; }

    IInteger IExpression<IInteger>.Type => Type;

    public bool Equals(IExpression<IType>? other)
    {
        return Equals(other as Address);
    }

    public IExpression<IType> ToBitVector()
    {
        return Values.Multiply.Create(
            Offsets.Aggregate(BaseAddress, (l, r) => Values.Add.Create(l, r.Value)),
            ConstantUnsigned.Create(Type.Size, (uint) Bytes.One.ToBits()));
    }

    internal Address Add(IConstantValue<IType> value)
    {
        return new(Values.Add.Create(BaseAddress, value), Offsets);
    }

    public Address BitCast(Bits targetSize)
    {
        return ReplaceLastOffset(Offsets.Last().BitCast(targetSize));
    }

    public IEnumerable<(Address, Bytes)> GetAddresses(Bytes length)
    {
        var field = Offsets.Last();
        if ((uint) length > (uint) field.AggregateSize)
            throw new Exception("Can't generate addresses, the length would be out of bounds.");

        if ((uint) length % (uint) field.FieldSize != 0)
            throw new Exception("Can only deal with lengths that are whole field multiples right now.");

        var numFields = (uint) length / (uint) field.FieldSize;
        IExpression<IType> CreateFieldMultiple(int i)
        {
            return ConstantUnsigned.Create(field.Value.Size, (uint) i * (uint) field.FieldSize);
        }

        return Enumerable.Range(0, (int) numFields)
            .Select(i => (
                ReplaceLastOffset(field.Add(CreateFieldMultiple(i))),
                field.FieldSize));
    }

    public Address AppendOffsets(IEnumerable<Offset> offsets)
    {
        return AppendOffsets(offsets.ToArray());
    }

    public Address AppendOffsets(params Offset[] offsets)
    {
        return new Address(
            BaseAddress,
            Offsets.Concat(offsets.Select(
                o => o.Value is Address a
                    ? new Offset(o.AggregateSize, o.AggregateType, o.FieldSize, a.ToBitVector())
                    : o)));
    }

    public Address IncrementFinalOffset(Bytes offset)
    {
        var finalOffset = Offsets.Last();
        return ReplaceLastOffset(finalOffset.Add(ConstantUnsigned.Create(finalOffset.Value.Size, (uint) offset)));
    }

    public T Map<T>(IArityMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(ITypeMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public U Map<U>(IIntegerMapper<U> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IBitVectorMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    internal Address Multiply(IConstantValue<IType> value)
    {
        return new(
            Values.Multiply.Create(BaseAddress, value),
            Offsets.Select(o => o.Multiply((uint) (BigInteger) value.AsUnsigned())));
    }

    internal Address Negate()
    {
        static IExpression<IType> NegateValue(IExpression<IType> value) => Values.Multiply.Create(value, ConstantUnsigned.Create(value.Size, -1));
        return new(NegateValue(BaseAddress), Offsets.Select(o => o.Negate()));
    }

    internal Address ReplaceLastOffset(Offset offset)
    {
        return new(BaseAddress, Offsets.SkipLast(1).Append(offset));
    }

    internal Address Subtract(IExpression<IType> value)
    {
        return new(Values.Subtract.Create(BaseAddress, value), Offsets);
    }

    public static Address Create(Bits size, Bytes baseAddress)
    {
        return Create(ConstantUnsigned.Create(size, (uint) baseAddress));
    }

    public static Address CreateNull(Bits size)
    {
        return Create(ConstantUnsigned.CreateZero(size));
    }

    public static Address Create(IExpression<IType> baseAddress)
    {
        return baseAddress switch
        {
            Address a => Create(a.BaseAddress).AppendOffsets(a.Offsets),
            _ => new Address(baseAddress, Enumerable.Empty<Offset>())
        };
    }
}
