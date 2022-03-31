using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Collection;

namespace Symbolica.Expression.Values.Constants;

public sealed record ConstantBitVector : IBitVector, IConstantValue
{
    private readonly IPersistentList<byte> _value;

    private ConstantBitVector(Bits size, IPersistentList<byte> value)
    {
        Size = size;
        _value = value;
    }

    public Bits Size { get; }

    public ConstantBitVector AsBitVector(ICollectionFactory collectionFactory)
    {
        return this;
    }

    public ConstantUnsigned AsUnsigned()
    {
        return ConstantUnsigned.Create(Size, new BigInteger(_value.ToArray(), true));
    }

    public ConstantSigned AsSigned()
    {
        return AsUnsigned().AsSigned();
    }

    public ConstantBool AsBool()
    {
        return AsUnsigned().AsBool();
    }

    public ConstantSingle AsSingle()
    {
        return AsSigned().AsSingle();
    }

    public ConstantDouble AsDouble()
    {
        return AsSigned().AsDouble();
    }

    public bool Equals(IExpression? other)
    {
        return AsUnsigned().Equals(other);
    }

    public T Map<T>(IExprMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IConstantMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public ConstantBitVector Read(ConstantUnsigned offset, Bits size)
    {
        return new(size, _value.GetRange(GetIndex(offset), GetCount(size)));
    }

    public ConstantBitVector Write(ConstantUnsigned offset, ConstantBitVector value)
    {
        return new(Size, _value.SetRange(GetIndex(offset), value._value));
    }

    public static ConstantBitVector Create(ICollectionFactory collectionFactory, ConstantUnsigned value)
    {
        return new ConstantBitVector(value.Size,
            collectionFactory.CreatePersistentList<byte>().AddRange(GetBytes(value.Size, value)));
    }

    private static IEnumerable<byte> GetBytes(Bits size, BigInteger value)
    {
        var bytes = new byte[GetCount(size)];
        value.TryWriteBytes(bytes, out _, true);

        return bytes;
    }

    private static int GetIndex(BigInteger offset)
    {
        return (int) (uint) ((Bits) (uint) offset).ToBytes();
    }

    private static int GetCount(Bits size)
    {
        return (int) (uint) size.ToBytes();
    }
}
