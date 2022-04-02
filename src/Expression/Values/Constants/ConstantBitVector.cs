using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Collection;

namespace Symbolica.Expression.Values.Constants;

public sealed record ConstantBitVector : IConstantBitVector
{
    private readonly IPersistentList<byte> _value;

    private ConstantBitVector(Bits size, IPersistentList<byte> value)
    {
        Type = new BitVector(size);
        _value = value;
    }

    public BitVector Type { get; }

    IInteger IExpression<IInteger>.Type => Type;

    public ConstantBitVector AsBitVector(ICollectionFactory collectionFactory)
    {
        return this;
    }

    public ConstantUnsigned AsUnsigned()
    {
        return ConstantUnsigned.Create(Type.Size, new BigInteger(_value.ToArray(), true));
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

    public bool Equals(IExpression<IType>? other)
    {
        return AsUnsigned().Equals(other);
    }

    public T Map<T>(IArityMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(ITypeMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IConstantMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IIntegerMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IBitVectorMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public ConstantBitVector Read(ConstantUnsigned offset, Bits size)
    {
        return new(size, _value.GetRange(GetIndex(offset), GetCount(size)));
    }

    public ConstantBitVector Write(ConstantUnsigned offset, ConstantBitVector value)
    {
        return new(Type.Size, _value.SetRange(GetIndex(offset), value._value));
    }

    public static ConstantBitVector Create(ICollectionFactory collectionFactory, ConstantUnsigned value)
    {
        return new ConstantBitVector(value.Type.Size,
            collectionFactory.CreatePersistentList<byte>().AddRange(GetBytes(value.Type.Size, value)));
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
