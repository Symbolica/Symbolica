using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation.Values.Constants;

internal sealed record ConstantUnsigned : BitVector, IConstantValue
{
    private readonly BigInteger _value;

    private ConstantUnsigned(Bits size, BigInteger value)
        : base(size)
    {
        _value = value;
    }

    public bool IsZero => _value.IsZero;
    public bool IsOne => _value.IsOne;

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        return solver.Context.MkBV(_value.ToString(), (uint) Size);
    }

    public ConstantBitVector AsBitVector(ICollectionFactory collectionFactory)
    {
        return ConstantBitVector.Create(collectionFactory, this);
    }

    public ConstantUnsigned AsUnsigned()
    {
        return this;
    }

    public ConstantSigned AsSigned()
    {
        return ConstantSigned.Create(Size, _value);
    }

    public ConstantBool AsBool()
    {
        return new ConstantBool(!_value.IsZero);
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
        return other is IConstantValue v && Equal(v.AsUnsigned());
    }

    public ConstantUnsigned Add(ConstantUnsigned value)
    {
        return Create(Size, _value + value._value);
    }

    public ConstantUnsigned And(ConstantUnsigned value)
    {
        return Create(Size, _value & value._value);
    }

    public ConstantUnsigned Divide(ConstantUnsigned value)
    {
        return Create(Size, _value / value._value);
    }

    public ConstantBool Equal(ConstantUnsigned value)
    {
        return new ConstantBool(_value == value._value);
    }

    public ConstantUnsigned Extend(Bits size)
    {
        return new ConstantUnsigned(size, _value);
    }

    public ConstantBool Greater(ConstantUnsigned value)
    {
        return new ConstantBool(_value > value._value);
    }

    public ConstantBool GreaterOrEqual(ConstantUnsigned value)
    {
        return new ConstantBool(_value >= value._value);
    }

    public ConstantBool Less(ConstantUnsigned value)
    {
        return new ConstantBool(_value < value._value);
    }

    public ConstantBool LessOrEqual(ConstantUnsigned value)
    {
        return new ConstantBool(_value <= value._value);
    }

    public ConstantUnsigned Multiply(ConstantUnsigned value)
    {
        return Create(Size, _value * value._value);
    }

    public ConstantUnsigned Not()
    {
        return Create(Size, ~_value);
    }

    public ConstantUnsigned Or(ConstantUnsigned value)
    {
        return Create(Size, _value | value._value);
    }

    public ConstantUnsigned Remainder(ConstantUnsigned value)
    {
        return Create(Size, _value % value._value);
    }

    public ConstantUnsigned ShiftLeft(ConstantUnsigned value)
    {
        return Create(Size, _value << (int) value._value);
    }

    public ConstantUnsigned ShiftRight(ConstantUnsigned value)
    {
        return Create(Size, _value >> (int) value._value);
    }

    public ConstantUnsigned Subtract(ConstantUnsigned value)
    {
        return Create(Size, _value - value._value);
    }

    public ConstantDouble ToDouble()
    {
        return new ConstantDouble((double) _value);
    }

    public ConstantSingle ToSingle()
    {
        return new ConstantSingle((float) _value);
    }

    public ConstantUnsigned Truncate(Bits size)
    {
        return Create(size, _value);
    }

    public ConstantUnsigned Xor(ConstantUnsigned value)
    {
        return Create(Size, _value ^ value._value);
    }

    public static implicit operator BigInteger(ConstantUnsigned value)
    {
        return value._value;
    }

    public static ConstantUnsigned CreateZero(Bits size)
    {
        return new ConstantUnsigned(size, BigInteger.Zero);
    }

    public static ConstantUnsigned Create(Bits size, BigInteger value)
    {
        return new ConstantUnsigned(size, value.IsZero || value.Sign > 0 && value.GetBitLength() <= (uint) size
            ? value
            : Normalize(size, value));
    }

    private static BigInteger Normalize(Bits size, BigInteger value)
    {
        return value & ((BigInteger.One << (int) (uint) size) - BigInteger.One);
    }
}
