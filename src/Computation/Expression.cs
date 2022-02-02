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
        return Binary(expression, Values.Symbolics.Add.Create);
    }

    public IExpression And(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.And.Create);
    }

    public IExpression ArithmeticShiftRight(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.ArithmeticShiftRight.Create);
    }

    public IExpression Equal(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.Equal.Create);
    }

    public IExpression FloatAdd(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.FloatAdd.Create);
    }

    public IExpression FloatCeiling()
    {
        return Unary(Values.Symbolics.FloatCeiling.Create);
    }

    public IExpression FloatConvert(Bits size)
    {
        return Unary(v => Values.Symbolics.FloatConvert.Create(size, v));
    }

    public IExpression FloatDivide(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.FloatDivide.Create);
    }

    public IExpression FloatEqual(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.FloatEqual.Create);
    }

    public IExpression FloatFloor()
    {
        return Unary(Values.Symbolics.FloatFloor.Create);
    }

    public IExpression FloatGreater(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.FloatGreater.Create);
    }

    public IExpression FloatGreaterOrEqual(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.FloatGreaterOrEqual.Create);
    }

    public IExpression FloatLess(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.FloatLess.Create);
    }

    public IExpression FloatLessOrEqual(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.FloatLessOrEqual.Create);
    }

    public IExpression FloatMultiply(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.FloatMultiply.Create);
    }

    public IExpression FloatNegate()
    {
        return Unary(Values.Symbolics.FloatNegate.Create);
    }

    public IExpression FloatNotEqual(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.FloatNotEqual.Create);
    }

    public IExpression FloatOrdered(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.FloatOrdered.Create);
    }

    public IExpression FloatPower(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.FloatPower.Create);
    }

    public IExpression FloatRemainder(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.FloatRemainder.Create);
    }

    public IExpression FloatSubtract(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.FloatSubtract.Create);
    }

    public IExpression FloatToSigned(Bits size)
    {
        return Unary(v => Values.Symbolics.FloatToSigned.Create(size, v));
    }

    public IExpression FloatToUnsigned(Bits size)
    {
        return Unary(v => Values.Symbolics.FloatToUnsigned.Create(size, v));
    }

    public IExpression FloatUnordered(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.FloatUnordered.Create);
    }

    public IExpression LogicalShiftRight(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.LogicalShiftRight.Create);
    }

    public IExpression Multiply(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.Multiply.Create);
    }

    public IExpression NotEqual(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.NotEqual.Create);
    }

    public IExpression Or(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.Or.Create);
    }

    public IExpression Read(IExpression offset, Bits size)
    {
        return Binary(offset, (b, o) => Values.Symbolics.Read.Create(_collectionFactory, b, o, size));
    }

    public IExpression Select(IExpression trueValue, IExpression falseValue)
    {
        return trueValue.Size == falseValue.Size
            ? _value is IConstantValue v
                ? v.AsBool()
                    ? trueValue
                    : falseValue
                : Ternary(trueValue, falseValue, Values.Symbolics.Select.Create)
            : throw new InconsistentExpressionSizesException(trueValue.Size, falseValue.Size);
    }

    public IExpression ShiftLeft(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.ShiftLeft.Create);
    }

    public IExpression SignedDivide(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.SignedDivide.Create);
    }

    public IExpression SignedGreater(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.SignedGreater.Create);
    }

    public IExpression SignedGreaterOrEqual(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.SignedGreaterOrEqual.Create);
    }

    public IExpression SignedLess(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.SignedLess.Create);
    }

    public IExpression SignedLessOrEqual(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.SignedLessOrEqual.Create);
    }

    public IExpression SignedRemainder(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.SignedRemainder.Create);
    }

    public IExpression SignedToFloat(Bits size)
    {
        return Unary(v => Values.Symbolics.SignedToFloat.Create(size, v));
    }

    public IExpression SignExtend(Bits size)
    {
        return size > Size
            ? Unary(v => Values.Symbolics.SignExtend.Create(size, v))
            : this;
    }

    public IExpression Subtract(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.Subtract.Create);
    }

    public IExpression Truncate(Bits size)
    {
        return size < Size
            ? Unary(v => Values.Symbolics.Truncate.Create(size, v))
            : this;
    }

    public IExpression UnsignedDivide(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.UnsignedDivide.Create);
    }

    public IExpression UnsignedGreater(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.UnsignedGreater.Create);
    }

    public IExpression UnsignedGreaterOrEqual(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.UnsignedGreaterOrEqual.Create);
    }

    public IExpression UnsignedLess(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.UnsignedLess.Create);
    }

    public IExpression UnsignedLessOrEqual(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.UnsignedLessOrEqual.Create);
    }

    public IExpression UnsignedRemainder(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.UnsignedRemainder.Create);
    }

    public IExpression UnsignedToFloat(Bits size)
    {
        return Unary(v => Values.Symbolics.UnsignedToFloat.Create(size, v));
    }

    public IExpression Write(IExpression offset, IExpression value)
    {
        return Size == offset.Size
            ? Ternary(offset, value, (b, o, v) => Values.Symbolics.Write.Create(_collectionFactory, b, o, v))
            : throw new InconsistentExpressionSizesException(Size, offset.Size);
    }

    public IExpression Xor(IExpression expression)
    {
        return Binary(expression, Values.Symbolics.Xor.Create);
    }

    public IExpression ZeroExtend(Bits size)
    {
        return size > Size
            ? Unary(v => Values.Symbolics.ZeroExtend.Create(size, v))
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
