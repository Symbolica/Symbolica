using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation.Values.Constants;

internal sealed record ConstantBitVector : BitVector, IConstantValue
{
    private readonly IPersistentList<byte> _value;

    private ConstantBitVector(Size size, IPersistentList<byte> value)
        : base(size)
    {
        _value = value;
    }

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        return AsUnsigned().AsBitVector(solver);
    }

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

    public override bool Equals(IValue? other)
    {
        return AsUnsigned().Equals(other);
    }

    public ConstantBitVector Read(ConstantUnsigned offset, Size size)
    {
        return new ConstantBitVector(size, _value.GetRange(GetIndex(offset), GetCount(size)));
    }

    public ConstantBitVector Write(ConstantUnsigned offset, ConstantBitVector value)
    {
        return new ConstantBitVector(Size, _value.SetRange(GetIndex(offset), value._value));
    }

    public static ConstantBitVector Create(ICollectionFactory collectionFactory, ConstantUnsigned value)
    {
        return new ConstantBitVector(value.Size,
            collectionFactory.CreatePersistentList<byte>().AddRange(GetBytes(value.Size, value)));
    }

    private static IEnumerable<byte> GetBytes(Size size, BigInteger value)
    {
        var bytes = new byte[GetCount(size)];
        value.TryWriteBytes(bytes, out _, true);

        return bytes;
    }

    private static int GetIndex(BigInteger offset)
    {
        return (int) Size.FromBits(offset).Bytes;
    }

    private static int GetCount(Size size)
    {
        return (int) size.Bytes;
    }
}
