﻿using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation.Values.Constants;

internal sealed class ConstantSigned : BitVector, IConstantValue
{
    private readonly BigInteger _value;

    private ConstantSigned(Bits size, BigInteger value)
        : base(size)
    {
        _value = value;
    }

    public override IEnumerable<IValue> Children => Enumerable.Empty<IValue>();

    public override string? PrintedValue => _value < BigInteger.Pow(2, 32) ? _value.ToString() : _value.ToString("E");

    public bool IsOne => _value == 1;

    public override BitVecExpr AsBitVector(IContext context)
    {
        return AsUnsigned().AsBitVector(context);
    }

    public ConstantBitVector AsBitVector(ICollectionFactory collectionFactory)
    {
        return AsUnsigned().AsBitVector(collectionFactory);
    }

    public ConstantUnsigned AsUnsigned()
    {
        return ConstantUnsigned.Create(Size, _value);
    }

    public ConstantSigned AsSigned()
    {
        return this;
    }

    public ConstantBool AsBool()
    {
        return AsUnsigned().AsBool();
    }

    public ConstantSingle AsSingle()
    {
        return ConstantSingle.Create(this);
    }

    public ConstantDouble AsDouble()
    {
        return ConstantDouble.Create(this);
    }

    public ConstantSigned Divide(ConstantSigned value)
    {
        return Create(Size, _value / value._value);
    }

    public ConstantSigned Extend(Bits size)
    {
        return new ConstantSigned(size, _value);
    }

    public ConstantBool Greater(ConstantSigned value)
    {
        return new ConstantBool(_value > value._value);
    }

    public ConstantBool GreaterOrEqual(ConstantSigned value)
    {
        return new ConstantBool(_value >= value._value);
    }

    public ConstantBool Less(ConstantSigned value)
    {
        return new ConstantBool(_value < value._value);
    }

    public ConstantBool LessOrEqual(ConstantSigned value)
    {
        return new ConstantBool(_value <= value._value);
    }

    public ConstantSigned Remainder(ConstantSigned value)
    {
        return Create(Size, _value % value._value);
    }

    public ConstantSigned ShiftRight(ConstantUnsigned value)
    {
        return Create(Size, _value >> (int) (BigInteger) value);
    }

    public ConstantDouble ToDouble()
    {
        return new ConstantDouble((double) _value);
    }

    public ConstantSingle ToSingle()
    {
        return new ConstantSingle((float) _value);
    }

    public static implicit operator BigInteger(ConstantSigned value)
    {
        return value._value;
    }

    public static ConstantSigned Create(Bits size, BigInteger value)
    {
        return new ConstantSigned(size, value.IsZero
            ? value
            : Normalize(size, value));
    }

    private static BigInteger Normalize(Bits size, BigInteger value)
    {
        var msb = BigInteger.One << ((int) (uint) size - 1);
        return (value & (msb - BigInteger.One)) - (value & msb);
    }
}
