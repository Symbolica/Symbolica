using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Computation.Exceptions;
using Symbolica.Computation.Values;
using Symbolica.Computation.Values.Constants;
using Symbolica.Computation.Values.Symbolics;
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
        return Binary(expression,
            (l, r) => l.AsUnsigned().Add(r.AsUnsigned()),
            (l, r) => new Add(l, r));
    }

    public IExpression And(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().And(r.AsUnsigned()),
            (l, r) => new And(l, r));
    }

    public IExpression ArithmeticShiftRight(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsSigned().ShiftRight(r.AsUnsigned()),
            (l, r) => new ArithmeticShiftRight(l, r));
    }

    public IExpression Equal(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().Equal(r.AsUnsigned()),
            (l, r) => new Equal(l, r));
    }

    public IExpression FloatAdd(IExpression expression)
    {
        return Binary(expression,
            (l, r) => new ConstantSingle(l + r),
            (l, r) => new ConstantDouble(l + r),
            (l, r) => new FloatAdd(l, r));
    }

    public IExpression FloatCeiling()
    {
        return Unary(
            v => new ConstantSingle(MathF.Ceiling(v)),
            v => new ConstantDouble(Math.Ceiling(v)),
            v => new FloatCeiling(v));
    }

    public IExpression FloatConvert(Bits size)
    {
        return Unary(
            v => (uint) size switch
            {
                32U => new ConstantSingle(v),
                64U => new ConstantDouble(v),
                _ => new FloatConvert(size, new ConstantSingle(v))
            },
            v => (uint) size switch
            {
                32U => new ConstantSingle((float) v),
                64U => new ConstantDouble(v),
                _ => new FloatConvert(size, new ConstantDouble(v))
            },
            v => v is IRealValue r
                ? new RealConvert(size, r)
                : new FloatConvert(size, v));
    }

    public IExpression FloatDivide(IExpression expression)
    {
        return Binary(expression,
            (l, r) => new ConstantSingle(l / r),
            (l, r) => new ConstantDouble(l / r),
            (l, r) => new FloatDivide(l, r));
    }

    public IExpression FloatEqual(IExpression expression)
    {
        return Binary(expression,
            // ReSharper disable CompareOfFloatsByEqualityOperator
            (l, r) => new ConstantBool(l == r),
            (l, r) => new ConstantBool(l == r),
            // ReSharper restore CompareOfFloatsByEqualityOperator
            (l, r) => new FloatEqual(l, r));
    }

    public IExpression FloatFloor()
    {
        return Unary(
            v => new ConstantSingle(MathF.Floor(v)),
            v => new ConstantDouble(Math.Floor(v)),
            v => new FloatFloor(v));
    }

    public IExpression FloatGreater(IExpression expression)
    {
        return Binary(expression,
            (l, r) => new ConstantBool(l > r),
            (l, r) => new ConstantBool(l > r),
            (l, r) => new FloatGreater(l, r));
    }

    public IExpression FloatGreaterOrEqual(IExpression expression)
    {
        return Binary(expression,
            (l, r) => new ConstantBool(l >= r),
            (l, r) => new ConstantBool(l >= r),
            (l, r) => new FloatGreaterOrEqual(l, r));
    }

    public IExpression FloatLess(IExpression expression)
    {
        return Binary(expression,
            (l, r) => new ConstantBool(l < r),
            (l, r) => new ConstantBool(l < r),
            (l, r) => new FloatLess(l, r));
    }

    public IExpression FloatLessOrEqual(IExpression expression)
    {
        return Binary(expression,
            (l, r) => new ConstantBool(l <= r),
            (l, r) => new ConstantBool(l <= r),
            (l, r) => new FloatLessOrEqual(l, r));
    }

    public IExpression FloatMultiply(IExpression expression)
    {
        return Binary(expression,
            (l, r) => new ConstantSingle(l * r),
            (l, r) => new ConstantDouble(l * r),
            (l, r) => new FloatMultiply(l, r));
    }

    public IExpression FloatNegate()
    {
        return Unary(
            v => new ConstantSingle(-v),
            v => new ConstantDouble(-v),
            v => new FloatNegate(v));
    }

    public IExpression FloatNotEqual(IExpression expression)
    {
        return Binary(expression,
            // ReSharper disable CompareOfFloatsByEqualityOperator
            (l, r) => new ConstantBool(l != r),
            (l, r) => new ConstantBool(l != r),
            // ReSharper restore CompareOfFloatsByEqualityOperator
            (l, r) => new Not(new FloatEqual(l, r)));
    }

    public IExpression FloatOrdered(IExpression expression)
    {
        return Binary(expression,
            (l, r) => new ConstantBool(!(float.IsNaN(l) || float.IsNaN(r))),
            (l, r) => new ConstantBool(!(double.IsNaN(l) || double.IsNaN(r))),
            (l, r) => new Not(new FloatUnordered(l, r)));
    }

    public IExpression FloatPower(IExpression expression)
    {
        return Binary(expression,
            (l, r) => new ConstantSingle(MathF.Pow(l, r)),
            (l, r) => new ConstantDouble(Math.Pow(l, r)),
            (l, r) => new FloatPower(l, r));
    }

    public IExpression FloatRemainder(IExpression expression)
    {
        return Binary(expression,
            (l, r) => new ConstantSingle(MathF.IEEERemainder(l, r)),
            (l, r) => new ConstantDouble(Math.IEEERemainder(l, r)),
            (l, r) => new FloatRemainder(l, r));
    }

    public IExpression FloatSubtract(IExpression expression)
    {
        return Binary(expression,
            (l, r) => new ConstantSingle(l - r),
            (l, r) => new ConstantDouble(l - r),
            (l, r) => new FloatSubtract(l, r));
    }

    public IExpression FloatToSigned(Bits size)
    {
        return Unary(
            v => ConstantSigned.Create(size, (BigInteger) v),
            v => ConstantSigned.Create(size, (BigInteger) v),
            v => v is IRealValue r
                ? new RealToSigned(size, r)
                : new FloatToSigned(size, v));
    }

    public IExpression FloatToUnsigned(Bits size)
    {
        return Unary(
            v => ConstantUnsigned.Create(size, (BigInteger) v),
            v => ConstantUnsigned.Create(size, (BigInteger) v),
            v => new FloatToUnsigned(size, v));
    }

    public IExpression FloatUnordered(IExpression expression)
    {
        return Binary(expression,
            (l, r) => new ConstantBool(float.IsNaN(l) || float.IsNaN(r)),
            (l, r) => new ConstantBool(double.IsNaN(l) || double.IsNaN(r)),
            (l, r) => new FloatUnordered(l, r));
    }

    public IExpression LogicalShiftRight(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().ShiftRight(r.AsUnsigned()),
            (l, r) => new LogicalShiftRight(l, r));
    }

    public IExpression Multiply(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().Multiply(r.AsUnsigned()),
            (l, r) => new Multiply(l, r));
    }

    public IExpression NotEqual(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().NotEqual(r.AsUnsigned()),
            (l, r) => new Not(new Equal(l, r)));
    }

    public IExpression Or(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().Or(r.AsUnsigned()),
            (l, r) => new Or(l, r));
    }

    public IExpression Read(IExpression offset, Bits size)
    {
        return Binary(offset,
            (b, o) => b.AsBitVector(_collectionFactory).Read(o.AsUnsigned(), size),
            (b, o) => b is Write w
                ? w.Read(o, size)
                : new Truncate(size, new LogicalShiftRight(b, o)));
    }

    public IExpression Select(IExpression trueValue, IExpression falseValue)
    {
        return trueValue.Size == falseValue.Size
            ? _value is IConstantValue v
                ? v.AsBool()
                    ? trueValue
                    : falseValue
                : Ternary(trueValue, falseValue,
                    (p, t, f) => p.AsBool() ? t : f,
                    (p, t, f) => new Select(p, t, f))
            : throw new InconsistentExpressionSizesException(trueValue.Size, falseValue.Size);
    }

    public IExpression ShiftLeft(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().ShiftLeft(r.AsUnsigned()),
            (l, r) => new ShiftLeft(l, r));
    }

    public IExpression SignedDivide(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsSigned().Divide(r.AsSigned()),
            (l, r) => new SignedDivide(l, r));
    }

    public IExpression SignedGreater(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsSigned().Greater(r.AsSigned()),
            (l, r) => new SignedGreater(l, r));
    }

    public IExpression SignedGreaterOrEqual(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsSigned().GreaterOrEqual(r.AsSigned()),
            (l, r) => new SignedGreaterOrEqual(l, r));
    }

    public IExpression SignedLess(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsSigned().Less(r.AsSigned()),
            (l, r) => new SignedLess(l, r));
    }

    public IExpression SignedLessOrEqual(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsSigned().LessOrEqual(r.AsSigned()),
            (l, r) => new SignedLessOrEqual(l, r));
    }

    public IExpression SignedRemainder(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsSigned().Remainder(r.AsSigned()),
            (l, r) => new SignedRemainder(l, r));
    }

    public IExpression SignedToFloat(Bits size)
    {
        return Unary(
            v => (uint) size switch
            {
                32U => v.AsSigned().ToSingle(),
                64U => v.AsSigned().ToDouble(),
                _ => new SignedToFloat(size, v)
            },
            v => new SignedToFloat(size, v));
    }

    public IExpression SignExtend(Bits size)
    {
        return size > Size
            ? Unary(
                v => v.AsSigned().Extend(size),
                v => new SignExtend(size, v))
            : this;
    }

    public IExpression Subtract(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().Subtract(r.AsUnsigned()),
            (l, r) => new Subtract(l, r));
    }

    public IExpression Truncate(Bits size)
    {
        return size < Size
            ? Unary(
                v => v.AsUnsigned().Truncate(size),
                v => new Truncate(size, v))
            : this;
    }

    public IExpression UnsignedDivide(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().Divide(r.AsUnsigned()),
            (l, r) => new UnsignedDivide(l, r));
    }

    public IExpression UnsignedGreater(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().Greater(r.AsUnsigned()),
            (l, r) => new UnsignedGreater(l, r));
    }

    public IExpression UnsignedGreaterOrEqual(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().GreaterOrEqual(r.AsUnsigned()),
            (l, r) => new UnsignedGreaterOrEqual(l, r));
    }

    public IExpression UnsignedLess(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().Less(r.AsUnsigned()),
            (l, r) => new UnsignedLess(l, r));
    }

    public IExpression UnsignedLessOrEqual(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().LessOrEqual(r.AsUnsigned()),
            (l, r) => new UnsignedLessOrEqual(l, r));
    }

    public IExpression UnsignedRemainder(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().Remainder(r.AsUnsigned()),
            (l, r) => new UnsignedRemainder(l, r));
    }

    public IExpression UnsignedToFloat(Bits size)
    {
        return Unary(
            v => (uint) size switch
            {
                32U => v.AsUnsigned().ToSingle(),
                64U => v.AsUnsigned().ToDouble(),
                _ => new UnsignedToFloat(size, v)
            },
            v => new UnsignedToFloat(size, v));
    }

    public IExpression Write(IExpression offset, IExpression value)
    {
        return Size == offset.Size
            ? Ternary(offset, value,
                (b, o, v) => b.AsBitVector(_collectionFactory).Write(o.AsUnsigned(), v.AsBitVector(_collectionFactory)),
                (b, o, v) => new Write(b, o, v))
            : throw new InconsistentExpressionSizesException(Size, offset.Size);
    }

    public IExpression Xor(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().Xor(r.AsUnsigned()),
            (l, r) => new Xor(l, r));
    }

    public IExpression ZeroExtend(Bits size)
    {
        return size > Size
            ? Unary(
                v => v.AsUnsigned().Extend(size),
                v => new ZeroExtend(size, v))
            : this;
    }

    private IExpression Unary(
        Func<float, IValue> constantSingle,
        Func<double, IValue> constantDouble,
        Func<IValue, IValue> symbolic)
    {
        return Unary(
            a => (uint) Size switch
            {
                32U => constantSingle(a.AsSingle()),
                64U => constantDouble(a.AsDouble()),
                _ => symbolic(a)
            },
            symbolic);
    }

    private IExpression Unary(
        Func<IConstantValue, IValue> constant,
        Func<IValue, IValue> symbolic)
    {
        var value = Unary(_value, constant, symbolic);

        return value is IConstantValue
            ? Create(_contextFactory, _collectionFactory,
                value)
            : Create(_contextFactory, _collectionFactory,
                value, _constraints);
    }

    private static IValue Unary(IValue x,
        Func<IConstantValue, IValue> constant,
        Func<IValue, IValue> symbolic)
    {
        return x is IConstantValue cx
            ? constant(cx)
            : symbolic(x);
    }

    private IExpression Binary(IExpression y,
        Func<float, float, IValue> constantSingle,
        Func<double, double, IValue> constantDouble,
        Func<IValue, IValue, IValue> symbolic)
    {
        return Binary(y,
            (a, b) => (uint) Size switch
            {
                32U => constantSingle(a.AsSingle(), b.AsSingle()),
                64U => constantDouble(a.AsDouble(), b.AsDouble()),
                _ => symbolic(a, b)
            },
            symbolic);
    }

    private IExpression Binary(IExpression y,
        Func<IConstantValue, IConstantValue, IValue> constant,
        Func<IValue, IValue, IValue> symbolic)
    {
        return Binary((Expression) y, constant, symbolic);
    }

    private IExpression Binary(Expression y,
        Func<IConstantValue, IConstantValue, IValue> constant,
        Func<IValue, IValue, IValue> symbolic)
    {
        var value = Binary(_value, y._value, constant, symbolic);

        return value is IConstantValue
            ? Create(_contextFactory, _collectionFactory,
                value)
            : Create(_contextFactory, _collectionFactory,
                value, _constraints.Concat(y._constraints).ToArray());
    }

    private static IValue Binary(IValue x, IValue y,
        Func<IConstantValue, IConstantValue, IValue> constant,
        Func<IValue, IValue, IValue> symbolic)
    {
        return x is IConstantValue cx && y is IConstantValue cy
            ? constant(cx, cy)
            : symbolic(x, y);
    }

    private IExpression Ternary(IExpression y, IExpression z,
        Func<IConstantValue, IConstantValue, IConstantValue, IValue> constant,
        Func<IValue, IValue, IValue, IValue> symbolic)
    {
        return Ternary((Expression) y, (Expression) z, constant, symbolic);
    }

    private IExpression Ternary(Expression y, Expression z,
        Func<IConstantValue, IConstantValue, IConstantValue, IValue> constant,
        Func<IValue, IValue, IValue, IValue> symbolic)
    {
        var value = Ternary(_value, y._value, z._value, constant, symbolic);

        return value is IConstantValue
            ? Create(_contextFactory, _collectionFactory,
                value)
            : Create(_contextFactory, _collectionFactory,
                value, _constraints.Concat(y._constraints.Concat(z._constraints)).ToArray());
    }

    private static IValue Ternary(IValue x, IValue y, IValue z,
        Func<IConstantValue, IConstantValue, IConstantValue, IValue> constant,
        Func<IValue, IValue, IValue, IValue> symbolic)
    {
        return x is IConstantValue cx && y is IConstantValue cy && z is IConstantValue cz
            ? constant(cx, cy, cz)
            : symbolic(x, y, z);
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
