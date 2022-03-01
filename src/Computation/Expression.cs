using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Collection;
using Symbolica.Computation.Exceptions;
using Symbolica.Computation.Values;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class Expression : IExpression
{
    private readonly ICollectionFactory _collectionFactory;
    private readonly IValue _value;

    public Expression(ICollectionFactory collectionFactory,
        IValue value)
    {
        _collectionFactory = collectionFactory;
        _value = value;
    }

    public Bits Size => _value.Size;

    public BigInteger GetSingleValue(ISpace space)
    {
        using var solver = ((IPersistentSpace) space).CreateSolver();

        return solver.GetSingleValue(_value);
    }

    public IEnumerable<(IExpression, Bits)> GetAddresses(Bits length)
    {
        return _value switch
        {
            Address<Bits> a => a.GetAddresses(length).Select(x => (Create(_ => x.Item1), x.Item2)),
            Address<Bytes> => throw new Exception($"Cannot get addresses for an {nameof(Address<Bytes>)}."),
            _ => new[] { (Create(v => v), length) }
        };
    }

    public IEnumerable<(IExpression, Bytes)> GetAddresses(Bytes length)
    {
        return _value switch
        {
            Address<Bytes> a => a.GetAddresses(length).Select(x => (Create(_ => x.Item1), x.Item2)),
            Address<Bits> => throw new Exception($"Cannot get addresses for an {nameof(Address<Bits>)}."),
            _ => new[] { (Create(v => v), length) }
        };
    }

    public BigInteger GetExampleValue(ISpace space)
    {
        using var solver = ((IPersistentSpace) space).CreateSolver();

        return solver.GetExampleValue(
            _value switch
            {
                Address<Bits> a => a.Aggregate(),
                Address<Bytes> a => a.Aggregate(),
                _ => _value
            });
    }

    public IProposition GetProposition(ISpace space)
    {
        return Proposition.Create((IPersistentSpace) space, _value);
    }

    public IExpression Add(IExpression expression)
    {
        return Create(expression, Values.Add.Create);
    }

    public IExpression And(IExpression expression)
    {
        return Create(expression, (l, r) => l is Bool || r is Bool
            ? LogicalAnd.Create(l, r)
            : Values.And.Create(l, r));
    }

    public IExpression ArithmeticShiftRight(IExpression expression)
    {
        return Create(expression, Values.ArithmeticShiftRight.Create);
    }

    public IExpression Equal(IExpression expression)
    {
        return Create(expression, Values.Equal.Create);
    }

    public IExpression FloatAdd(IExpression expression)
    {
        return Create(expression, Values.FloatAdd.Create);
    }

    public IExpression FloatCeiling()
    {
        return Create(Values.FloatCeiling.Create);
    }

    public IExpression FloatConvert(Bits size)
    {
        return Create(v => Values.FloatConvert.Create(size, v));
    }

    public IExpression FloatDivide(IExpression expression)
    {
        return Create(expression, Values.FloatDivide.Create);
    }

    public IExpression FloatEqual(IExpression expression)
    {
        return Create(expression, Values.FloatEqual.Create);
    }

    public IExpression FloatFloor()
    {
        return Create(Values.FloatFloor.Create);
    }

    public IExpression FloatGreater(IExpression expression)
    {
        return Create(expression, Values.FloatGreater.Create);
    }

    public IExpression FloatGreaterOrEqual(IExpression expression)
    {
        return Create(expression, Values.FloatGreaterOrEqual.Create);
    }

    public IExpression FloatLess(IExpression expression)
    {
        return Create(expression, Values.FloatLess.Create);
    }

    public IExpression FloatLessOrEqual(IExpression expression)
    {
        return Create(expression, Values.FloatLessOrEqual.Create);
    }

    public IExpression FloatMultiply(IExpression expression)
    {
        return Create(expression, Values.FloatMultiply.Create);
    }

    public IExpression FloatNegate()
    {
        return Create(Values.FloatNegate.Create);
    }

    public IExpression FloatNotEqual(IExpression expression)
    {
        return Create(expression, Values.FloatNotEqual.Create);
    }

    public IExpression FloatOrdered(IExpression expression)
    {
        return Create(expression, Values.FloatOrdered.Create);
    }

    public IExpression FloatPower(IExpression expression)
    {
        return Create(expression, Values.FloatPower.Create);
    }

    public IExpression FloatRemainder(IExpression expression)
    {
        return Create(expression, Values.FloatRemainder.Create);
    }

    public IExpression FloatSubtract(IExpression expression)
    {
        return Create(expression, Values.FloatSubtract.Create);
    }

    public IExpression FloatToSigned(Bits size)
    {
        return Create(v => Values.FloatToSigned.Create(size, v));
    }

    public IExpression FloatToUnsigned(Bits size)
    {
        return Create(v => Values.FloatToUnsigned.Create(size, v));
    }

    public IExpression FloatUnordered(IExpression expression)
    {
        return Create(expression, Values.FloatUnordered.Create);
    }

    public IExpression LogicalShiftRight(IExpression expression)
    {
        return Create(expression, Values.LogicalShiftRight.Create);
    }

    public IExpression Multiply(IExpression expression)
    {
        return Create(expression, Values.Multiply.Create);
    }

    public IExpression NotEqual(IExpression expression)
    {
        return Create(expression, Values.NotEqual.Create);
    }

    public IExpression Or(IExpression expression)
    {
        return Create(expression, (l, r) => l is Bool || r is Bool
            ? LogicalOr.Create(l, r)
            : Values.Or.Create(l, r));
    }

    public IExpression Read(ISpace space, IExpression offset, Bits size)
    {
        using var solver = ((IPersistentSpace) space).CreateSolver();
        return new Expression(
            _collectionFactory,
            Values.Read.Create(_collectionFactory, solver, _value, ((Expression) offset)._value, size));
    }

    public IExpression Select(IExpression trueValue, IExpression falseValue)
    {
        return trueValue.Size == falseValue.Size
            ? Create(trueValue, falseValue, Values.Select.Create)
            : throw new InconsistentExpressionSizesException(trueValue.Size, falseValue.Size);
    }

    public IExpression ShiftLeft(IExpression expression)
    {
        return Create(expression, Values.ShiftLeft.Create);
    }

    public IExpression SignedDivide(IExpression expression)
    {
        return Create(expression, Values.SignedDivide.Create);
    }

    public IExpression SignedGreater(IExpression expression)
    {
        return Create(expression, Values.SignedGreater.Create);
    }

    public IExpression SignedGreaterOrEqual(IExpression expression)
    {
        return Create(expression, Values.SignedGreaterOrEqual.Create);
    }

    public IExpression SignedLess(IExpression expression)
    {
        return Create(expression, Values.SignedLess.Create);
    }

    public IExpression SignedLessOrEqual(IExpression expression)
    {
        return Create(expression, Values.SignedLessOrEqual.Create);
    }

    public IExpression SignedRemainder(IExpression expression)
    {
        return Create(expression, Values.SignedRemainder.Create);
    }

    public IExpression SignedToFloat(Bits size)
    {
        return Create(v => Values.SignedToFloat.Create(size, v));
    }

    public IExpression SignExtend(Bits size)
    {
        return Create(v => Values.SignExtend.Create(size, v));
    }

    public IExpression Subtract(IExpression expression)
    {
        return Create(expression, Values.Subtract.Create);
    }

    public IExpression Truncate(Bits size)
    {
        return Create(v => Values.Truncate.Create(size, v));
    }

    public IExpression ToBits()
    {
        return Create(v => v.ToBits());
    }

    public IExpression UnsignedDivide(IExpression expression)
    {
        return Create(expression, Values.UnsignedDivide.Create);
    }

    public IExpression UnsignedGreater(IExpression expression)
    {
        return Create(expression, Values.UnsignedGreater.Create);
    }

    public IExpression UnsignedGreaterOrEqual(IExpression expression)
    {
        return Create(expression, Values.UnsignedGreaterOrEqual.Create);
    }

    public IExpression UnsignedLess(IExpression expression)
    {
        return Create(expression, Values.UnsignedLess.Create);
    }

    public IExpression UnsignedLessOrEqual(IExpression expression)
    {
        return Create(expression, Values.UnsignedLessOrEqual.Create);
    }

    public IExpression UnsignedRemainder(IExpression expression)
    {
        return Create(expression, Values.UnsignedRemainder.Create);
    }

    public IExpression UnsignedToFloat(Bits size)
    {
        return Create(v => Values.UnsignedToFloat.Create(size, v));
    }

    public IExpression Write(ISpace space, IExpression offset, IExpression value)
    {
        using var solver = ((IPersistentSpace) space).CreateSolver();
        return Create(offset, value, (b, o, v) =>
                AggregateWrite.Create(_collectionFactory, solver, b, o, v));
    }

    public IExpression Xor(IExpression expression)
    {
        return Create(expression, (l, r) => l is Bool || r is Bool
            ? LogicalXor.Create(l, r)
            : Values.Xor.Create(l, r));
    }

    public IExpression ZeroExtend(Bits size)
    {
        return Create(v => Values.ZeroExtend.Create(size, v));
    }

    private IExpression Create(Func<IValue, IValue> func)
    {
        return new Expression(_collectionFactory,
            func(_value));
    }

    private IExpression Create(IExpression y, Func<IValue, IValue, IValue> func)
    {
        return Size == y.Size
            ? new Expression(_collectionFactory,
                func(_value, ((Expression) y)._value))
            : throw new InconsistentExpressionSizesException(Size, y.Size);
    }

    private IExpression Create(IExpression y, IExpression z, Func<IValue, IValue, IValue, IValue> func)
    {
        return new Expression(_collectionFactory,
            func(_value, ((Expression) y)._value, ((Expression) z)._value));
    }

    public static IExpression CreateAddress(ICollectionFactory collectionFactory,
        IExpression baseAddress, Offset[] offsets)
    {
        return new Expression(collectionFactory,
            Address<Bytes>.Create(
                ((Expression) baseAddress)._value,
                offsets.Select(
                    o => new Offset<Bytes>(
                        o.AggregateSize,
                        o.AggregateType,
                        o.FieldSize,
                        ((Expression) o.Value)._value)).ToArray()));
    }

    public static IExpression CreateSymbolic(ICollectionFactory collectionFactory,
        Bits size, string name, IEnumerable<Func<IExpression, IExpression>> assertions)
    {
        return new Expression(collectionFactory,
            Symbol.Create(size, name, assertions.Select(a => new Func<IValue, IValue>(
                v => ((Expression) a(new Expression(collectionFactory, v)))._value))));
    }
}
