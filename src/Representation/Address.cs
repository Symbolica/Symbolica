using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation;

internal sealed class Address : IAddress
{
    private readonly IExpression[] _offsets;
    private IExpression Value => _offsets.Aggregate((a, o) => a.Add(o));

    private Address(IType indexedType, IExpression[] offsets)
    {
        IndexedType = indexedType;
        _offsets = offsets;
    }

    public IType IndexedType { get; }
    public IExpression BaseAddress => _offsets.First();
    public IEnumerable<IExpression> Offsets => _offsets.Skip(1);

    public Bits Size => BaseAddress.Size;

    public TExpression As<TExpression>()
        where TExpression : class, IExpression
    {
        return Value.As<TExpression>();
    }

    public BigInteger GetSingleValue(ISpace space)
    {
        return Value.GetSingleValue(space);
    }

    public BigInteger GetExampleValue(ISpace space)
    {
        return Value.GetExampleValue(space);
    }

    public IProposition GetProposition(ISpace space)
    {
        return Value.GetProposition(space);
    }

    public IExpression Add(IExpression expression)
    {
        return expression is IAddress // a && a.IndexedType.Size > IndexedType.Size
            ? throw new Exception("add") // expression.Add(this)
            : new Address(IndexedType, _offsets.SkipLast(1).Append(_offsets.Last().Add(expression)).ToArray());
    }

    public IExpression And(IExpression expression)
    {
        return Value.And(expression);
    }

    public IExpression ArithmeticShiftRight(IExpression expression)
    {
        return Value.ArithmeticShiftRight(expression);
    }

    public IExpression Equal(IExpression expression)
    {
        return Value.Equal(expression);
    }

    public IExpression FloatAdd(IExpression expression)
    {
        return Value.FloatAdd(expression);
    }

    public IExpression FloatCeiling()
    {
        return Value.FloatCeiling();
    }

    public IExpression FloatConvert(Bits size)
    {
        return Value.FloatConvert(size);
    }

    public IExpression FloatDivide(IExpression expression)
    {
        return Value.FloatDivide(expression);
    }

    public IExpression FloatEqual(IExpression expression)
    {
        return Value.FloatEqual(expression);
    }

    public IExpression FloatFloor()
    {
        return Value.FloatFloor();
    }

    public IExpression FloatGreater(IExpression expression)
    {
        return Value.FloatGreater(expression);
    }

    public IExpression FloatGreaterOrEqual(IExpression expression)
    {
        return Value.FloatGreaterOrEqual(expression);
    }

    public IExpression FloatLess(IExpression expression)
    {
        return Value.FloatLess(expression);
    }

    public IExpression FloatLessOrEqual(IExpression expression)
    {
        return Value.FloatLessOrEqual(expression);
    }

    public IExpression FloatMultiply(IExpression expression)
    {
        return Value.FloatMultiply(expression);
    }

    public IExpression FloatNegate()
    {
        return Value.FloatNegate();
    }

    public IExpression FloatNotEqual(IExpression expression)
    {
        return Value.FloatNotEqual(expression);
    }

    public IExpression FloatOrdered(IExpression expression)
    {
        return Value.FloatOrdered(expression);
    }

    public IExpression FloatPower(IExpression expression)
    {
        return Value.FloatPower(expression);
    }

    public IExpression FloatRemainder(IExpression expression)
    {
        return Value.FloatRemainder(expression);
    }

    public IExpression FloatSubtract(IExpression expression)
    {
        return Value.FloatSubtract(expression);
    }

    public IExpression FloatToSigned(Bits size)
    {
        return Value.FloatToSigned(size);
    }

    public IExpression FloatToUnsigned(Bits size)
    {
        return Value.FloatToUnsigned(size);
    }

    public IExpression FloatUnordered(IExpression expression)
    {
        return Value.FloatUnordered(expression);
    }

    public IExpression LogicalShiftRight(IExpression expression)
    {
        return Value.LogicalShiftRight(expression);
    }

    public IExpression Multiply(IExpression expression)
    {
        return Value.Multiply(expression);
    }

    public IExpression NotEqual(IExpression expression)
    {
        return Value.NotEqual(expression);
    }

    public IExpression Or(IExpression expression)
    {
        return Value.Or(expression);
    }

    public IExpression Read(ISpace space, IExpression offset, Bits size)
    {
        return Value.Read(space, offset, size);
    }

    public IExpression Select(IExpression trueValue, IExpression falseValue)
    {
        return Value.Select(trueValue, falseValue);
    }

    public IExpression ShiftLeft(IExpression expression)
    {
        return Value.ShiftLeft(expression);
    }

    public IExpression SignedDivide(IExpression expression)
    {
        return Value.SignedDivide(expression);
    }

    public IExpression SignedGreater(IExpression expression)
    {
        return Value.SignedGreater(expression);
    }

    public IExpression SignedGreaterOrEqual(IExpression expression)
    {
        return Value.SignedGreaterOrEqual(expression);
    }

    public IExpression SignedLess(IExpression expression)
    {
        return Value.SignedLess(expression);
    }

    public IExpression SignedLessOrEqual(IExpression expression)
    {
        return Value.SignedLessOrEqual(expression);
    }

    public IExpression SignedRemainder(IExpression expression)
    {
        return Value.SignedRemainder(expression);
    }

    public IExpression SignedToFloat(Bits size)
    {
        return Value.SignedToFloat(size);
    }

    public IExpression SignExtend(Bits size)
    {
        return size == Size
            ? this
            : throw new Exception("sext");
    }

    public IExpression Subtract(IExpression expression)
    {
        return expression is IAddress
            ? throw new Exception("sub")
            : new Address(IndexedType, _offsets.SkipLast(1).Append(_offsets.Last().Subtract(expression)).ToArray());
    }
    public IAddress SubtractBase(IExpression baseAddress)
    {
        return baseAddress is IAddress
            ? throw new Exception("base")
            : new Address(IndexedType, new[] { BaseAddress.Subtract(baseAddress) }.Concat(Offsets).ToArray());
    }

    public IExpression Truncate(Bits size)
    {
        return size == Size
            ? this
            : throw new Exception("trunc");
    }

    public IExpression UnsignedDivide(IExpression expression)
    {
        return Value.UnsignedDivide(expression);
    }

    public IExpression UnsignedGreater(IExpression expression)
    {
        return Value.UnsignedGreater(expression);
    }

    public IExpression UnsignedGreaterOrEqual(IExpression expression)
    {
        return Value.UnsignedGreaterOrEqual(expression);
    }

    public IExpression UnsignedLess(IExpression expression)
    {
        return Value.UnsignedLess(expression);
    }

    public IExpression UnsignedLessOrEqual(IExpression expression)
    {
        return Value.UnsignedLessOrEqual(expression);
    }

    public IExpression UnsignedRemainder(IExpression expression)
    {
        return Value.UnsignedRemainder(expression);
    }

    public IExpression UnsignedToFloat(Bits size)
    {
        return Value.UnsignedToFloat(size);
    }

    public IExpression Write(ISpace space, IExpression offset, IExpression value)
    {
        return Value.Write(space, offset, value);
    }

    public IExpression Xor(IExpression expression)
    {
        return Value.Xor(expression);
    }

    public IExpression ZeroExtend(Bits size)
    {
        return size == Size
            ? this
            : throw new Exception("zext");
    }

    public static Address Create(IType indexedType, IExpression baseAddress, IEnumerable<IExpression> offsets)
    {
        return baseAddress is Address a
            ? new Address(a.IndexedType, a._offsets.Concat(offsets).ToArray())
            : new Address(indexedType, new[] { baseAddress }.Concat(offsets).ToArray());
    }
}
