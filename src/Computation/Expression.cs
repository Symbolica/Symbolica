using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Collection;
using Symbolica.Computation.Exceptions;
using Symbolica.Computation.Values;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class Expression<TContext> : IExpression
    where TContext : IContext, new()
{
    private readonly IValue[] _assertions;
    private readonly ICollectionFactory _collectionFactory;
    private readonly IValue _value;

    private Expression(ICollectionFactory collectionFactory,
        IValue value, IValue[] assertions)
    {
        _collectionFactory = collectionFactory;
        _value = value;
        _assertions = assertions;
    }

    public Bits Size => _value.Size;

    public BigInteger Constant => _value is IConstantValue v
        ? v.AsUnsigned()
        : AsConstant();

    public IExpression GetValue(ISpace space)
    {
        return _value is IConstantValue
            ? this
            : Evaluate((IPersistentSpace) space);
    }

    public IProposition GetProposition(ISpace space)
    {
        return _value is IConstantValue v
            ? ConstantProposition.Create(space, v)
            : SymbolicProposition.Create((IPersistentSpace) space, _value, _assertions);
    }

    public IExpression Add(IExpression expression)
    {
        return Binary(expression, Values.Add.Create);
    }

    public IExpression And(IExpression expression)
    {
        return Binary(expression, Values.And.Create);
    }

    public IExpression ArithmeticShiftRight(IExpression expression)
    {
        return Binary(expression, Values.ArithmeticShiftRight.Create);
    }

    public IExpression Equal(IExpression expression)
    {
        return Binary(expression, Values.Equal.Create);
    }

    public IExpression FloatAdd(IExpression expression)
    {
        return Binary(expression, Values.FloatAdd.Create);
    }

    public IExpression FloatCeiling()
    {
        return Unary(Values.FloatCeiling.Create);
    }

    public IExpression FloatConvert(Bits size)
    {
        return Unary(v => Values.FloatConvert.Create(size, v));
    }

    public IExpression FloatDivide(IExpression expression)
    {
        return Binary(expression, Values.FloatDivide.Create);
    }

    public IExpression FloatEqual(IExpression expression)
    {
        return Binary(expression, Values.FloatEqual.Create);
    }

    public IExpression FloatFloor()
    {
        return Unary(Values.FloatFloor.Create);
    }

    public IExpression FloatGreater(IExpression expression)
    {
        return Binary(expression, Values.FloatGreater.Create);
    }

    public IExpression FloatGreaterOrEqual(IExpression expression)
    {
        return Binary(expression, Values.FloatGreaterOrEqual.Create);
    }

    public IExpression FloatLess(IExpression expression)
    {
        return Binary(expression, Values.FloatLess.Create);
    }

    public IExpression FloatLessOrEqual(IExpression expression)
    {
        return Binary(expression, Values.FloatLessOrEqual.Create);
    }

    public IExpression FloatMultiply(IExpression expression)
    {
        return Binary(expression, Values.FloatMultiply.Create);
    }

    public IExpression FloatNegate()
    {
        return Unary(Values.FloatNegate.Create);
    }

    public IExpression FloatNotEqual(IExpression expression)
    {
        return Binary(expression, Values.FloatNotEqual.Create);
    }

    public IExpression FloatOrdered(IExpression expression)
    {
        return Binary(expression, Values.FloatOrdered.Create);
    }

    public IExpression FloatPower(IExpression expression)
    {
        return Binary(expression, Values.FloatPower.Create);
    }

    public IExpression FloatRemainder(IExpression expression)
    {
        return Binary(expression, Values.FloatRemainder.Create);
    }

    public IExpression FloatSubtract(IExpression expression)
    {
        return Binary(expression, Values.FloatSubtract.Create);
    }

    public IExpression FloatToSigned(Bits size)
    {
        return Unary(v => Values.FloatToSigned.Create(size, v));
    }

    public IExpression FloatToUnsigned(Bits size)
    {
        return Unary(v => Values.FloatToUnsigned.Create(size, v));
    }

    public IExpression FloatUnordered(IExpression expression)
    {
        return Binary(expression, Values.FloatUnordered.Create);
    }

    public IExpression LogicalShiftRight(IExpression expression)
    {
        return Binary(expression, Values.LogicalShiftRight.Create);
    }

    public IExpression Multiply(IExpression expression)
    {
        return Binary(expression, Values.Multiply.Create);
    }

    public IExpression NotEqual(IExpression expression)
    {
        return Binary(expression, Values.NotEqual.Create);
    }

    public IExpression Or(IExpression expression)
    {
        return Binary(expression, Values.Or.Create);
    }

    public IExpression Read(IExpression offset, Bits size)
    {
        return Binary(offset, (b, o) => Values.Read.Create(_collectionFactory, b, o, size));
    }

    public IExpression Select(IExpression trueValue, IExpression falseValue)
    {
        return trueValue.Size == falseValue.Size
            ? _value is IConstantValue v
                ? v.AsBool()
                    ? trueValue
                    : falseValue
                : Ternary(trueValue, falseValue, (p, t, f) => new Select(p, t, f))
            : throw new InconsistentExpressionSizesException(trueValue.Size, falseValue.Size);
    }

    public IExpression ShiftLeft(IExpression expression)
    {
        return Binary(expression, Values.ShiftLeft.Create);
    }

    public IExpression SignedDivide(IExpression expression)
    {
        return Binary(expression, Values.SignedDivide.Create);
    }

    public IExpression SignedGreater(IExpression expression)
    {
        return Binary(expression, Values.SignedGreater.Create);
    }

    public IExpression SignedGreaterOrEqual(IExpression expression)
    {
        return Binary(expression, Values.SignedGreaterOrEqual.Create);
    }

    public IExpression SignedLess(IExpression expression)
    {
        return Binary(expression, Values.SignedLess.Create);
    }

    public IExpression SignedLessOrEqual(IExpression expression)
    {
        return Binary(expression, Values.SignedLessOrEqual.Create);
    }

    public IExpression SignedRemainder(IExpression expression)
    {
        return Binary(expression, Values.SignedRemainder.Create);
    }

    public IExpression SignedToFloat(Bits size)
    {
        return Unary(v => Values.SignedToFloat.Create(size, v));
    }

    public IExpression SignExtend(Bits size)
    {
        return size > Size
            ? Unary(v => Values.SignExtend.Create(size, v))
            : this;
    }

    public IExpression Subtract(IExpression expression)
    {
        return Binary(expression, Values.Subtract.Create);
    }

    public IExpression Truncate(Bits size)
    {
        return size < Size
            ? Unary(v => Values.Truncate.Create(size, v))
            : this;
    }

    public IExpression UnsignedDivide(IExpression expression)
    {
        return Binary(expression, Values.UnsignedDivide.Create);
    }

    public IExpression UnsignedGreater(IExpression expression)
    {
        return Binary(expression, Values.UnsignedGreater.Create);
    }

    public IExpression UnsignedGreaterOrEqual(IExpression expression)
    {
        return Binary(expression, Values.UnsignedGreaterOrEqual.Create);
    }

    public IExpression UnsignedLess(IExpression expression)
    {
        return Binary(expression, Values.UnsignedLess.Create);
    }

    public IExpression UnsignedLessOrEqual(IExpression expression)
    {
        return Binary(expression, Values.UnsignedLessOrEqual.Create);
    }

    public IExpression UnsignedRemainder(IExpression expression)
    {
        return Binary(expression, Values.UnsignedRemainder.Create);
    }

    public IExpression UnsignedToFloat(Bits size)
    {
        return Unary(v => Values.UnsignedToFloat.Create(size, v));
    }

    public IExpression Write(IExpression offset, IExpression value)
    {
        return Size == offset.Size
            ? Ternary(offset, value, (b, o, v) => Values.Write.Create(_collectionFactory, b, o, v))
            : throw new InconsistentExpressionSizesException(Size, offset.Size);
    }

    public IExpression Xor(IExpression expression)
    {
        return Binary(expression, Values.Xor.Create);
    }

    public IExpression ZeroExtend(Bits size)
    {
        return size > Size
            ? Unary(v => Values.ZeroExtend.Create(size, v))
            : this;
    }

    private IExpression Unary(Func<IValue, IValue> func)
    {
        var value = func(_value);

        return value is IConstantValue
            ? Create(_collectionFactory,
                value)
            : Create(_collectionFactory,
                value, _assertions);
    }

    private IExpression Binary(IExpression y, Func<IValue, IValue, IValue> func)
    {
        return Size == y.Size
            ? Binary((Expression<TContext>) y, func)
            : throw new InconsistentExpressionSizesException(Size, y.Size);
    }

    private IExpression Binary(Expression<TContext> y, Func<IValue, IValue, IValue> func)
    {
        var value = func(_value, y._value);

        return value is IConstantValue
            ? Create(_collectionFactory,
                value)
            : Create(_collectionFactory,
                value, _assertions.Concat(y._assertions).ToArray());
    }

    private IExpression Ternary(IExpression y, IExpression z, Func<IValue, IValue, IValue, IValue> func)
    {
        return Ternary((Expression<TContext>) y, (Expression<TContext>) z, func);
    }

    private IExpression Ternary(Expression<TContext> y, Expression<TContext> z, Func<IValue, IValue, IValue, IValue> func)
    {
        var value = func(_value, y._value, z._value);

        return value is IConstantValue
            ? Create(_collectionFactory,
                value)
            : Create(_collectionFactory,
                value, _assertions.Concat(y._assertions.Concat(z._assertions)).ToArray());
    }

    private BigInteger AsConstant()
    {
        using var context = new TContext();

        return _value.AsConstant(context);
    }

    private IExpression Evaluate(IPersistentSpace space)
    {
        using var constraints = space.GetConstraints(_assertions);

        return Create(_collectionFactory,
            ConstantUnsigned.Create(Size, constraints.Evaluate(_value)));
    }

    private static IExpression Create(ICollectionFactory collectionFactory,
        IValue value)
    {
        return Create(collectionFactory,
            value, Array.Empty<IValue>());
    }

    private static IExpression Create(ICollectionFactory collectionFactory,
        IValue value, IValue[] assertions)
    {
        return new Expression<TContext>(collectionFactory,
            value, assertions);
    }

    public static IExpression Create(ICollectionFactory collectionFactory,
        IValue value, IEnumerable<Func<IExpression, IExpression>> assertions)
    {
        var unconstrained = Create(collectionFactory,
            value);

        return Create(collectionFactory,
            value, assertions
                .Select(c => ((Expression<TContext>) c(unconstrained))._value)
                .ToArray());
    }
}
