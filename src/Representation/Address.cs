using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation;

internal sealed class Address : IAddress
{
    private readonly IExpression _address;

    private Address(IType indexedType, IExpression baseAddress, IExpression address)
    {
        IndexedType = indexedType;
        BaseAddress = baseAddress;
        _address = address;
    }

    public IType IndexedType { get; }
    public IExpression BaseAddress { get; }
    public Bits Size => _address.Size;

    public static bool operator ==(Address? left, Address? right)
    {
        return left?.Equals(right) ?? right is null;
    }

    public static bool operator !=(Address? left, Address? right)
    {
        return !(left == right);
    }

    public override bool Equals(object? other)
    {
        return Equals(other as Address);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public bool Equals(Address? other)
    {
        return ReferenceEquals(this, other) || Value.Equals(other?.Value);
    }

    public bool Equals(IExpression? other)
    {
        return Equals(other as Address);
    }

    public TExpression As<TExpression>()
        where TExpression : class, IExpression
    {
        return _address.As<TExpression>();
    }

    public BigInteger GetSingleValue(ISpace space)
    {
        return _address.GetSingleValue(space);
    }

    public BigInteger GetExampleValue(ISpace space)
    {
        return _address.GetExampleValue(space);
    }

    public IProposition GetProposition(ISpace space)
    {
        return _address.GetProposition(space);
    }

    public IExpression Add(IExpression expression)
    {
        return expression is IAddress a && a.IndexedType.Size > IndexedType.Size
            ? new Address(a.IndexedType, a.BaseAddress, _address.Add(expression))
            : new Address(IndexedType, BaseAddress, _address.Add(expression));
    }

    public IExpression And(IExpression expression)
    {
        return _address.And(expression);
    }

    public IExpression ArithmeticShiftRight(IExpression expression)
    {
        return _address.ArithmeticShiftRight(expression);
    }

    public IExpression Equal(IExpression expression)
    {
        return _address.Equal(expression);
    }

    public IExpression FloatAdd(IExpression expression)
    {
        return _address.FloatAdd(expression);
    }

    public IExpression FloatCeiling()
    {
        return _address.FloatCeiling();
    }

    public IExpression FloatConvert(Bits size)
    {
        return _address.FloatConvert(size);
    }

    public IExpression FloatDivide(IExpression expression)
    {
        return _address.FloatDivide(expression);
    }

    public IExpression FloatEqual(IExpression expression)
    {
        return _address.FloatEqual(expression);
    }

    public IExpression FloatFloor()
    {
        return _address.FloatFloor();
    }

    public IExpression FloatGreater(IExpression expression)
    {
        return _address.FloatGreater(expression);
    }

    public IExpression FloatGreaterOrEqual(IExpression expression)
    {
        return _address.FloatGreaterOrEqual(expression);
    }

    public IExpression FloatLess(IExpression expression)
    {
        return _address.FloatLess(expression);
    }

    public IExpression FloatLessOrEqual(IExpression expression)
    {
        return _address.FloatLessOrEqual(expression);
    }

    public IExpression FloatMultiply(IExpression expression)
    {
        return _address.FloatMultiply(expression);
    }

    public IExpression FloatNegate()
    {
        return _address.FloatNegate();
    }

    public IExpression FloatNotEqual(IExpression expression)
    {
        return _address.FloatNotEqual(expression);
    }

    public IExpression FloatOrdered(IExpression expression)
    {
        return _address.FloatOrdered(expression);
    }

    public IExpression FloatPower(IExpression expression)
    {
        return _address.FloatPower(expression);
    }

    public IExpression FloatRemainder(IExpression expression)
    {
        return _address.FloatRemainder(expression);
    }

    public IExpression FloatSubtract(IExpression expression)
    {
        return _address.FloatSubtract(expression);
    }

    public IExpression FloatToSigned(Bits size)
    {
        return _address.FloatToSigned(size);
    }

    public IExpression FloatToUnsigned(Bits size)
    {
        return _address.FloatToUnsigned(size);
    }

    public IExpression FloatUnordered(IExpression expression)
    {
        return _address.FloatUnordered(expression);
    }

    public IExpression LogicalShiftRight(IExpression expression)
    {
        return _address.LogicalShiftRight(expression);
    }

    public IExpression Multiply(IExpression expression)
    {
        return _address.Multiply(expression);
    }

    public IExpression NotEqual(IExpression expression)
    {
        return _address.NotEqual(expression);
    }

    public IExpression Or(IExpression expression)
    {
        return _address.Or(expression);
    }

    public IExpression Read(ISpace space, IExpression offset, Bits size)
    {
        return _address.Read(space, offset, size);
    }

    public IExpression Select(IExpression trueValue, IExpression falseValue)
    {
        return _address.Select(trueValue, falseValue);
    }

    public IExpression ShiftLeft(IExpression expression)
    {
        return _address.ShiftLeft(expression);
    }

    public IExpression SignedDivide(IExpression expression)
    {
        return _address.SignedDivide(expression);
    }

    public IExpression SignedGreater(IExpression expression)
    {
        return _address.SignedGreater(expression);
    }

    public IExpression SignedGreaterOrEqual(IExpression expression)
    {
        return _address.SignedGreaterOrEqual(expression);
    }

    public IExpression SignedLess(IExpression expression)
    {
        return _address.SignedLess(expression);
    }

    public IExpression SignedLessOrEqual(IExpression expression)
    {
        return _address.SignedLessOrEqual(expression);
    }

    public IExpression SignedRemainder(IExpression expression)
    {
        return _address.SignedRemainder(expression);
    }

    public IExpression SignedToFloat(Bits size)
    {
        return _address.SignedToFloat(size);
    }

    public IExpression SignExtend(Bits size)
    {
        return new Address(IndexedType, BaseAddress, _address.SignExtend(size));
    }

    public IExpression Subtract(IExpression expression)
    {
        return expression is IAddress a && a.IndexedType.Size > IndexedType.Size
            ? new Address(a.IndexedType, a.BaseAddress, _address.Subtract(expression))
            : new Address(IndexedType, BaseAddress, _address.Subtract(expression));
    }

    public IExpression Truncate(Bits size)
    {
        return new Address(IndexedType, BaseAddress, _address.Truncate(size));
    }

    public IExpression UnsignedDivide(IExpression expression)
    {
        return _address.UnsignedDivide(expression);
    }

    public IExpression UnsignedGreater(IExpression expression)
    {
        return _address.UnsignedGreater(expression);
    }

    public IExpression UnsignedGreaterOrEqual(IExpression expression)
    {
        return _address.UnsignedGreaterOrEqual(expression);
    }

    public IExpression UnsignedLess(IExpression expression)
    {
        return _address.UnsignedLess(expression);
    }

    public IExpression UnsignedLessOrEqual(IExpression expression)
    {
        return _address.UnsignedLessOrEqual(expression);
    }

    public IExpression UnsignedRemainder(IExpression expression)
    {
        return _address.UnsignedRemainder(expression);
    }

    public IExpression UnsignedToFloat(Bits size)
    {
        return _address.UnsignedToFloat(size);
    }

    public IExpression Write(ISpace space, IExpression offset, IExpression value)
    {
        return _address.Write(space, offset, value);
    }

    public IExpression Xor(IExpression expression)
    {
        return _address.Xor(expression);
    }

    public IExpression ZeroExtend(Bits size)
    {
        return new Address(IndexedType, BaseAddress, _address.ZeroExtend(size));
    }

    public static Address Create(IType indexedType, IExpression baseAddress, IExpression address)
    {
        return baseAddress is IAddress a
            ? new Address(a.IndexedType, a.BaseAddress, address)
            : new Address(indexedType, baseAddress, address);
    }
}
