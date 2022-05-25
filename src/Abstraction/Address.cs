using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public sealed class Address : IAddress
{
    private readonly IExpressionFactory _exprFactory;
    private readonly (IType, IExpression)[] _offsets;
    private readonly Lazy<int> _equivalencyHash;
    private readonly Lazy<int> _mergeHash;
    private IExpression Value => _offsets.Select(o => o.Item2).Aggregate((a, o) => a.Add(o));

    private Address(IExpressionFactory exprFactory, (IType, IExpression)[] offsets)
    {
        _exprFactory = exprFactory;
        _offsets = offsets;
        _equivalencyHash = new(EquivalencyHash(false));
        _mergeHash = new(EquivalencyHash(true));

        Func<int> EquivalencyHash(bool includeSubs)
        {
            return () =>
            {
                var offsetsHash = new HashCode();
                foreach (var offset in _offsets)
                    offsetsHash.Add(
                        HashCode.Combine(offset.Item1.GetEquivalencyHash(includeSubs),
                        offset.Item2.GetEquivalencyHash(includeSubs)));
                return offsetsHash.ToHashCode();
            };
        }
    }

    public IType IndexedType => _offsets.First().Item1;
    public IExpression BaseAddress => _offsets.First().Item2;
    public IEnumerable<(IType, IExpression)> Offsets => _offsets.Skip(1);

    public Bits Size => BaseAddress.Size;

    public bool IsTheOneThatIWant => Value.IsTheOneThatIWant;
    public bool IsSymbolic => Value.IsSymbolic;

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
            : new Address(
                _exprFactory,
                _offsets
                    .SkipLast(1)
                    .Append((_offsets.Last().Item1, _offsets.Last().Item2.Add(expression)))
                    .ToArray());
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

    public IExpression PointerToInteger(Bits size)
    {
        return Value.PointerToInteger(size);
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
        return expression is Address a
            ? Value.Subtract(a.Value)
            : new Address(
                _exprFactory,
                _offsets.SkipLast(1)
                .Append((_offsets.Last().Item1, _offsets.Last().Item2.Subtract(expression)))
                .ToArray());
    }

    public IAddress? SubtractBase(ISpace space, IExpression expression)
    {
        if (expression is IAddress)
            throw new Exception("base");

        IExpression baseAddress = BaseAddress.Subtract(expression);
        var isZero = baseAddress.Equal(_exprFactory.CreateZero(baseAddress.Size));
        using var proposition = isZero.GetProposition(space);

        var offsets = (proposition.CanBeFalse() ? new[] { (IndexedType, baseAddress) } : Enumerable.Empty<(IType, IExpression)>()).Concat(Offsets);
        return offsets.Any() ? new Address(_exprFactory, offsets.ToArray()) : null;
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

    public IAddress AddImplicitOffsets()
    {
        static IEnumerable<IType> GetImplicitTypes(IType type)
        {
            if (type.Types.Any())
            {
                var nextType = type.Types.First();
                return new[] { nextType }.Concat(GetImplicitTypes(nextType));
            }
            return Enumerable.Empty<IType>();
        }

        var lastOffset = _offsets.Last();
        var newOffsets = GetImplicitTypes(lastOffset.Item1).Select(t => (t, _exprFactory.CreateZero(_exprFactory.PointerSize)));
        return new Address(_exprFactory, _offsets.Concat(newOffsets).ToArray());
    }

    public static Address Create(IExpressionFactory exprFactory, IExpression baseAddress)
    {
        return Create(exprFactory, null!, baseAddress, Enumerable.Empty<(IType, IExpression)>());
    }

    public static Address Create(IExpressionFactory exprFactory, IType indexedType,
        IExpression baseAddress, IEnumerable<(IType, IExpression)> offsets)
    {
        return baseAddress is Address a
            ? new Address(exprFactory, a._offsets.Concat(offsets).ToArray())
            : new Address(exprFactory, new[] { (indexedType, baseAddress) }.Concat(offsets).ToArray());
    }

    public (HashSet<(IExpression, IExpression)> subs, bool) IsEquivalentTo(IExpression other)
    {
        return other is Address a
            ? _offsets.IsSequenceEquivalentTo(
                    a._offsets,
                    (x, y) => x.Item2.IsEquivalentTo(y.Item2).And(x.Item1.IsEquivalentTo(y.Item1)))
            : (new(), false);
    }

    public object ToJson()
    {
        return new
        {
            Type = GetType().Name,
            Offsets = _offsets.Select(o => o.Item2.ToJson()).ToArray()
        };
    }

    public int GetEquivalencyHash(bool includeSubs)
    {
        return includeSubs
            ? _mergeHash.Value
            : _equivalencyHash.Value;
    }
}
