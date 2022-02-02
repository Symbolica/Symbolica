using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Computation.Exceptions;
using Symbolica.Computation.Values;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class Expression : IExpression
{
    private readonly ICollectionFactory _collectionFactory;
    private readonly IValue[] _constraints;
    private readonly IContextFactory _contextFactory;
    private readonly IValue _value;

    private Expression(IContextFactory contextFactory, ICollectionFactory collectionFactory,
        IValue value, IValue[] constraints)
    {
        _contextFactory = contextFactory;
        _collectionFactory = collectionFactory;
        _value = value;
        _constraints = constraints;
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
            : SymbolicProposition.Create((IPersistentSpace) space, _value, _constraints);
    }

    public IExpression Add(IExpression expression)
    {
        return Binary(expression, BitVector.Add);
    }

    public IExpression And(IExpression expression)
    {
        return Binary(expression, Integer.And);
    }

    public IExpression ArithmeticShiftRight(IExpression expression)
    {
        return Binary(expression, BitVector.ArithmeticShiftRight);
    }

    public IExpression Equal(IExpression expression)
    {
        return Binary(expression, BitVector.Equal);
    }

    public IExpression FloatAdd(IExpression expression)
    {
        return Binary(expression, Float.Add);
    }

    public IExpression FloatCeiling()
    {
        return Unary(Float.Ceiling);
    }

    public IExpression FloatConvert(Bits size)
    {
        return Unary(v => Float.Convert(size, v));
    }

    public IExpression FloatDivide(IExpression expression)
    {
        return Binary(expression, Float.Divide);
    }

    public IExpression FloatEqual(IExpression expression)
    {
        return Binary(expression, Float.Equal);
    }

    public IExpression FloatFloor()
    {
        return Unary(Float.Floor);
    }

    public IExpression FloatGreater(IExpression expression)
    {
        return Binary(expression, Float.Greater);
    }

    public IExpression FloatGreaterOrEqual(IExpression expression)
    {
        return Binary(expression, Float.GreaterOrEqual);
    }

    public IExpression FloatLess(IExpression expression)
    {
        return Binary(expression, Float.Less);
    }

    public IExpression FloatLessOrEqual(IExpression expression)
    {
        return Binary(expression, Float.LessOrEqual);
    }

    public IExpression FloatMultiply(IExpression expression)
    {
        return Binary(expression, Float.Multiply);
    }

    public IExpression FloatNegate()
    {
        return Unary(Float.Negate);
    }

    public IExpression FloatNotEqual(IExpression expression)
    {
        return Binary(expression, Float.NotEqual);
    }

    public IExpression FloatOrdered(IExpression expression)
    {
        return Binary(expression, Float.Ordered);
    }

    public IExpression FloatPower(IExpression expression)
    {
        return Binary(expression, Float.Power);
    }

    public IExpression FloatRemainder(IExpression expression)
    {
        return Binary(expression, Float.Remainder);
    }

    public IExpression FloatSubtract(IExpression expression)
    {
        return Binary(expression, Float.Subtract);
    }

    public IExpression FloatToSigned(Bits size)
    {
        return Unary(v => Float.ToSigned(size, v));
    }

    public IExpression FloatToUnsigned(Bits size)
    {
        return Unary(v => Float.ToUnsigned(size, v));
    }

    public IExpression FloatUnordered(IExpression expression)
    {
        return Binary(expression, Float.Unordered);
    }

    public IExpression LogicalShiftRight(IExpression expression)
    {
        return Binary(expression, BitVector.LogicalShiftRight);
    }

    public IExpression Multiply(IExpression expression)
    {
        return Binary(expression, BitVector.Multiply);
    }

    public IExpression NotEqual(IExpression expression)
    {
        return Binary(expression, BitVector.NotEqual);
    }

    public IExpression Or(IExpression expression)
    {
        return Binary(expression, Integer.Or);
    }

    public IExpression Read(IExpression offset, Bits size)
    {
        return Binary(offset, (b, o) => BitVector.Read(_collectionFactory, b, o, size));
    }

    public IExpression Select(IExpression trueValue, IExpression falseValue)
    {
        return trueValue.Size == falseValue.Size
            ? _value is IConstantValue v
                ? v.AsBool()
                    ? trueValue
                    : falseValue
                : Ternary(trueValue, falseValue, Value.Select)
            : throw new InconsistentExpressionSizesException(trueValue.Size, falseValue.Size);
    }

    public IExpression ShiftLeft(IExpression expression)
    {
        return Binary(expression, BitVector.ShiftLeft);
    }

    public IExpression SignedDivide(IExpression expression)
    {
        return Binary(expression, BitVector.SignedDivide);
    }

    public IExpression SignedGreater(IExpression expression)
    {
        return Binary(expression, BitVector.SignedGreater);
    }

    public IExpression SignedGreaterOrEqual(IExpression expression)
    {
        return Binary(expression, BitVector.SignedGreaterOrEqual);
    }

    public IExpression SignedLess(IExpression expression)
    {
        return Binary(expression, BitVector.SignedLess);
    }

    public IExpression SignedLessOrEqual(IExpression expression)
    {
        return Binary(expression, BitVector.SignedLessOrEqual);
    }

    public IExpression SignedRemainder(IExpression expression)
    {
        return Binary(expression, BitVector.SignedRemainder);
    }

    public IExpression SignedToFloat(Bits size)
    {
        return Unary(v => BitVector.SignedToFloat(size, v));
    }

    public IExpression SignExtend(Bits size)
    {
        return size > Size
            ? Unary(v => BitVector.SignExtend(size, v))
            : this;
    }

    public IExpression Subtract(IExpression expression)
    {
        return Binary(expression, BitVector.Subtract);
    }

    public IExpression Truncate(Bits size)
    {
        return size < Size
            ? Unary(v => BitVector.Truncate(size, v))
            : this;
    }

    public IExpression UnsignedDivide(IExpression expression)
    {
        return Binary(expression, BitVector.UnsignedDivide);
    }

    public IExpression UnsignedGreater(IExpression expression)
    {
        return Binary(expression, BitVector.UnsignedGreater);
    }

    public IExpression UnsignedGreaterOrEqual(IExpression expression)
    {
        return Binary(expression, BitVector.UnsignedGreaterOrEqual);
    }

    public IExpression UnsignedLess(IExpression expression)
    {
        return Binary(expression, BitVector.UnsignedLess);
    }

    public IExpression UnsignedLessOrEqual(IExpression expression)
    {
        return Binary(expression, BitVector.UnsignedLessOrEqual);
    }

    public IExpression UnsignedRemainder(IExpression expression)
    {
        return Binary(expression, BitVector.UnsignedRemainder);
    }

    public IExpression UnsignedToFloat(Bits size)
    {
        return Unary(v => BitVector.UnsignedToFloat(size, v));
    }

    public IExpression Write(IExpression offset, IExpression value)
    {
        return Size == offset.Size
            ? Ternary(offset, value, (b, o, v) => BitVector.Write(_collectionFactory, b, o, v))
            : throw new InconsistentExpressionSizesException(Size, offset.Size);
    }

    public IExpression Xor(IExpression expression)
    {
        return Binary(expression, Integer.Xor);
    }

    public IExpression ZeroExtend(Bits size)
    {
        return size > Size
            ? Unary(v => BitVector.ZeroExtend(size, v))
            : this;
    }

    private IExpression Unary(Func<IValue, IValue> func)
    {
        var value = func(_value);

        return value is IConstantValue
            ? Create(_contextFactory, _collectionFactory,
                value)
            : Create(_contextFactory, _collectionFactory,
                value, _constraints);
    }

    private IExpression Binary(IExpression y, Func<IValue, IValue, IValue> func)
    {
        return Size == y.Size
            ? Binary((Expression) y, func)
            : throw new InconsistentExpressionSizesException(Size, y.Size);
    }

    private IExpression Binary(Expression y, Func<IValue, IValue, IValue> func)
    {
        var value = func(_value, y._value);

        return value is IConstantValue
            ? Create(_contextFactory, _collectionFactory,
                value)
            : Create(_contextFactory, _collectionFactory,
                value, _constraints.Concat(y._constraints).ToArray());
    }

    private IExpression Ternary(IExpression y, IExpression z, Func<IValue, IValue, IValue, IValue> func)
    {
        return Ternary((Expression) y, (Expression) z, func);
    }

    private IExpression Ternary(Expression y, Expression z, Func<IValue, IValue, IValue, IValue> func)
    {
        var value = func(_value, y._value, z._value);

        return value is IConstantValue
            ? Create(_contextFactory, _collectionFactory,
                value)
            : Create(_contextFactory, _collectionFactory,
                value, _constraints.Concat(y._constraints.Concat(z._constraints)).ToArray());
    }

    private BigInteger AsConstant()
    {
        using var handle = _contextFactory.Create();

        return _value is Float && _value.AsFloat(handle.Context).Simplify().IsFPNaN
            ? Size.GetNan(handle.Context)
            : AsConstant(_value.AsBitVector(handle.Context).Simplify());
    }

    private static BigInteger AsConstant(Expr expr)
    {
        return expr.IsNumeral
            ? ((BitVecNum) expr).BigInteger
            : throw new IrreducibleSymbolicExpressionException();
    }

    private IExpression Evaluate(IPersistentSpace space)
    {
        using var model = space.GetModel(_constraints);

        return Create(_contextFactory, _collectionFactory,
            ConstantUnsigned.Create(Size, model.Evaluate(_value)));
    }

    private static IExpression Create(IContextFactory contextFactory, ICollectionFactory collectionFactory,
        IValue value)
    {
        return Create(contextFactory, collectionFactory,
            value, Array.Empty<IValue>());
    }

    private static IExpression Create(IContextFactory contextFactory, ICollectionFactory collectionFactory,
        IValue value, IValue[] constraints)
    {
        return new Expression(contextFactory, collectionFactory,
            value, constraints);
    }

    public static IExpression Create(IContextFactory contextFactory, ICollectionFactory collectionFactory,
        IValue value, IEnumerable<Func<IExpression, IExpression>> constraints)
    {
        var unconstrained = Create(contextFactory, collectionFactory,
            value);

        return Create(contextFactory, collectionFactory,
            value, constraints
                .Select(c => ((Expression) c(unconstrained))._value)
                .ToArray());
    }
}
