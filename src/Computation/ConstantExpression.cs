using System;
using System.Linq;
using System.Numerics;
using Symbolica.Collection;
using Symbolica.Computation.Exceptions;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class ConstantExpression : IValueExpression
{
    private readonly ICollectionFactory _collectionFactory;
    private readonly IContextFactory _contextFactory;
    private readonly IConstantValue _value;

    public ConstantExpression(IContextFactory contextFactory, ICollectionFactory collectionFactory,
        IConstantValue value)
    {
        _contextFactory = contextFactory;
        _collectionFactory = collectionFactory;
        _value = value;
    }

    public Bits Size => _value.Size;
    public BigInteger Constant => _value.AsUnsigned();
    public IValue Value => _value;
    public IValue[] Constraints => Array.Empty<IValue>();

    public IExpression GetValue(ISpace space)
    {
        return this;
    }

    public IProposition GetProposition(ISpace space)
    {
        return ConstantProposition.Create(space, _value.AsBool());
    }

    public IExpression Add(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().Add(r.AsUnsigned()),
            (l, r) => l.AsSymbolic().Add(r));
    }

    public IExpression And(IExpression expression)
    {
        return Constant.IsZero
            ? this
            : Not().Constant.IsZero
                ? expression
                : Binary(expression,
                    (l, r) => l.AsUnsigned().And(r.AsUnsigned()),
                    (l, r) => l.AsSymbolic().And(r));
    }

    public IExpression ArithmeticShiftRight(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsSigned().ShiftRight(r.AsUnsigned()),
            (l, r) => l.AsSymbolic().ArithmeticShiftRight(r));
    }

    public IExpression Equal(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().Equal(r.AsUnsigned()),
            (l, r) => l.AsSymbolic().Equal(r));
    }

    public IExpression FloatAdd(IExpression expression)
    {
        return Binary(expression,
            (l, r) => new ConstantSingle(l + r),
            (l, r) => new ConstantDouble(l + r),
            (l, r) => l.AsSymbolic().FloatAdd(r));
    }

    public IExpression FloatCeiling()
    {
        return Unary(
            v => new ConstantSingle(MathF.Ceiling(v)),
            v => new ConstantDouble(Math.Ceiling(v)),
            e => e.AsSymbolic().FloatCeiling());
    }

    public IExpression FloatConvert(Bits size)
    {
        return Unary(
            v => (uint) size switch
            {
                32U => _value,
                64U => new ConstantDouble(v),
                _ => null
            },
            v => (uint) size switch
            {
                32U => new ConstantSingle((float) v),
                64U => _value,
                _ => null
            },
            e => e.AsSymbolic().FloatConvert(size));
    }

    public IExpression FloatDivide(IExpression expression)
    {
        return Binary(expression,
            (l, r) => new ConstantSingle(l / r),
            (l, r) => new ConstantDouble(l / r),
            (l, r) => l.AsSymbolic().FloatDivide(r));
    }

    public IExpression FloatEqual(IExpression expression)
    {
        return Binary(expression,
            // ReSharper disable CompareOfFloatsByEqualityOperator
            (l, r) => new ConstantBool(l == r),
            (l, r) => new ConstantBool(l == r),
            // ReSharper restore CompareOfFloatsByEqualityOperator
            (l, r) => l.AsSymbolic().FloatEqual(r));
    }

    public IExpression FloatFloor()
    {
        return Unary(
            v => new ConstantSingle(MathF.Floor(v)),
            v => new ConstantDouble(Math.Floor(v)),
            e => e.AsSymbolic().FloatFloor());
    }

    public IExpression FloatGreater(IExpression expression)
    {
        return Binary(expression,
            (l, r) => new ConstantBool(l > r),
            (l, r) => new ConstantBool(l > r),
            (l, r) => l.AsSymbolic().FloatGreater(r));
    }

    public IExpression FloatGreaterOrEqual(IExpression expression)
    {
        return Binary(expression,
            (l, r) => new ConstantBool(l >= r),
            (l, r) => new ConstantBool(l >= r),
            (l, r) => l.AsSymbolic().FloatGreaterOrEqual(r));
    }

    public IExpression FloatLess(IExpression expression)
    {
        return Binary(expression,
            (l, r) => new ConstantBool(l < r),
            (l, r) => new ConstantBool(l < r),
            (l, r) => l.AsSymbolic().FloatLess(r));
    }

    public IExpression FloatLessOrEqual(IExpression expression)
    {
        return Binary(expression,
            (l, r) => new ConstantBool(l <= r),
            (l, r) => new ConstantBool(l <= r),
            (l, r) => l.AsSymbolic().FloatLessOrEqual(r));
    }

    public IExpression FloatMultiply(IExpression expression)
    {
        return Binary(expression,
            (l, r) => new ConstantSingle(l * r),
            (l, r) => new ConstantDouble(l * r),
            (l, r) => l.AsSymbolic().FloatMultiply(r));
    }

    public IExpression FloatNegate()
    {
        return Unary(
            v => new ConstantSingle(-v),
            v => new ConstantDouble(-v),
            e => e.AsSymbolic().FloatNegate());
    }

    public IExpression FloatNotEqual(IExpression expression)
    {
        return Binary(expression,
            // ReSharper disable CompareOfFloatsByEqualityOperator
            (l, r) => new ConstantBool(l != r),
            (l, r) => new ConstantBool(l != r),
            // ReSharper restore CompareOfFloatsByEqualityOperator
            (l, r) => l.AsSymbolic().FloatNotEqual(r));
    }

    public IExpression FloatOrdered(IExpression expression)
    {
        return Binary(expression,
            (l, r) => new ConstantBool(!(float.IsNaN(l) || float.IsNaN(r))),
            (l, r) => new ConstantBool(!(double.IsNaN(l) || double.IsNaN(r))),
            (l, r) => l.AsSymbolic().FloatOrdered(r));
    }

    public IExpression FloatPower(IExpression expression)
    {
        return Binary(expression,
            (l, r) => new ConstantSingle(MathF.Pow(l, r)),
            (l, r) => new ConstantDouble(Math.Pow(l, r)),
            (l, r) => l.AsSymbolic().FloatPower(r));
    }

    public IExpression FloatRemainder(IExpression expression)
    {
        return Binary(expression,
            (l, r) => new ConstantSingle(MathF.IEEERemainder(l, r)),
            (l, r) => new ConstantDouble(Math.IEEERemainder(l, r)),
            (l, r) => l.AsSymbolic().FloatRemainder(r));
    }

    public IExpression FloatSubtract(IExpression expression)
    {
        return Binary(expression,
            (l, r) => new ConstantSingle(l - r),
            (l, r) => new ConstantDouble(l - r),
            (l, r) => l.AsSymbolic().FloatSubtract(r));
    }

    public IExpression FloatToSigned(Bits size)
    {
        return Unary(
            v => ConstantSigned.Create(size, (BigInteger) v),
            v => ConstantSigned.Create(size, (BigInteger) v),
            e => e.AsSymbolic().FloatToSigned(size));
    }

    public IExpression FloatToUnsigned(Bits size)
    {
        return Unary(
            v => ConstantUnsigned.Create(size, (BigInteger) v),
            v => ConstantUnsigned.Create(size, (BigInteger) v),
            e => e.AsSymbolic().FloatToUnsigned(size));
    }

    public IExpression FloatUnordered(IExpression expression)
    {
        return Binary(expression,
            (l, r) => new ConstantBool(float.IsNaN(l) || float.IsNaN(r)),
            (l, r) => new ConstantBool(double.IsNaN(l) || double.IsNaN(r)),
            (l, r) => l.AsSymbolic().FloatUnordered(r));
    }

    public IExpression LogicalShiftRight(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().ShiftRight(r.AsUnsigned()),
            (l, r) => l.AsSymbolic().LogicalShiftRight(r));
    }

    public IExpression Multiply(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().Multiply(r.AsUnsigned()),
            (l, r) => l.AsSymbolic().Multiply(r));
    }

    public IExpression Not()
    {
        return Unary(v => v.AsUnsigned().Not());
    }

    public IExpression NotEqual(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().NotEqual(r.AsUnsigned()),
            (l, r) => l.AsSymbolic().NotEqual(r));
    }

    public IExpression Or(IExpression expression)
    {
        return Constant.IsZero
            ? expression
            : Not().Constant.IsZero
                ? this
                : Binary(expression,
                    (l, r) => l.AsUnsigned().Or(r.AsUnsigned()),
                    (l, r) => l.AsSymbolic().Or(r));
    }

    public IExpression Read(IExpression offset, Bits size)
    {
        return Binary(offset,
            (b, o) => b.AsBitVector(_collectionFactory).Read(o.AsUnsigned(), size),
            (b, o) => b.AsSymbolic().Read(o, size));
    }

    public IExpression Select(IExpression trueValue, IExpression falseValue)
    {
        return trueValue.Size == falseValue.Size
            ? _value.AsBool()
                ? trueValue
                : falseValue
            : throw new InconsistentExpressionSizesException(trueValue.Size, falseValue.Size);
    }

    public IExpression ShiftLeft(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().ShiftLeft(r.AsUnsigned()),
            (l, r) => l.AsSymbolic().ShiftLeft(r));
    }

    public IExpression SignedDivide(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsSigned().Divide(r.AsSigned()),
            (l, r) => l.AsSymbolic().SignedDivide(r));
    }

    public IExpression SignedGreater(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsSigned().Greater(r.AsSigned()),
            (l, r) => l.AsSymbolic().SignedGreater(r));
    }

    public IExpression SignedGreaterOrEqual(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsSigned().GreaterOrEqual(r.AsSigned()),
            (l, r) => l.AsSymbolic().SignedGreaterOrEqual(r));
    }

    public IExpression SignedLess(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsSigned().Less(r.AsSigned()),
            (l, r) => l.AsSymbolic().SignedLess(r));
    }

    public IExpression SignedLessOrEqual(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsSigned().LessOrEqual(r.AsSigned()),
            (l, r) => l.AsSymbolic().SignedLessOrEqual(r));
    }

    public IExpression SignedRemainder(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsSigned().Remainder(r.AsSigned()),
            (l, r) => l.AsSymbolic().SignedRemainder(r));
    }

    public IExpression SignedToFloat(Bits size)
    {
        return Unary(
            v => (uint) size switch
            {
                32U => v.AsSigned().ToSingle(),
                64U => v.AsSigned().ToDouble(),
                _ => null
            },
            e => e.AsSymbolic().SignedToFloat(size));
    }

    public IExpression SignExtend(Bits size)
    {
        return size > Size
            ? Unary(v => v.AsSigned().Extend(size))
            : this;
    }

    public IExpression Subtract(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().Subtract(r.AsUnsigned()),
            (l, r) => l.AsSymbolic().Subtract(r));
    }

    public IExpression Truncate(Bits size)
    {
        return size < Size
            ? Unary(v => v.AsUnsigned().Truncate(size))
            : this;
    }

    public IExpression UnsignedDivide(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().Divide(r.AsUnsigned()),
            (l, r) => l.AsSymbolic().UnsignedDivide(r));
    }

    public IExpression UnsignedGreater(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().Greater(r.AsUnsigned()),
            (l, r) => l.AsSymbolic().UnsignedGreater(r));
    }

    public IExpression UnsignedGreaterOrEqual(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().GreaterOrEqual(r.AsUnsigned()),
            (l, r) => l.AsSymbolic().UnsignedGreaterOrEqual(r));
    }

    public IExpression UnsignedLess(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().Less(r.AsUnsigned()),
            (l, r) => l.AsSymbolic().UnsignedLess(r));
    }

    public IExpression UnsignedLessOrEqual(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().LessOrEqual(r.AsUnsigned()),
            (l, r) => l.AsSymbolic().UnsignedLessOrEqual(r));
    }

    public IExpression UnsignedRemainder(IExpression expression)
    {
        return Binary(expression,
            (l, r) => l.AsUnsigned().Remainder(r.AsUnsigned()),
            (l, r) => l.AsSymbolic().UnsignedRemainder(r));
    }

    public IExpression UnsignedToFloat(Bits size)
    {
        return Unary(
            v => (uint) size switch
            {
                32U => v.AsUnsigned().ToSingle(),
                64U => v.AsUnsigned().ToDouble(),
                _ => null
            },
            e => e.AsSymbolic().UnsignedToFloat(size));
    }

    public IExpression Write(IExpression offset, IExpression value)
    {
        return Size == offset.Size
            ? Ternary(offset, value,
                (b, o, v) => b.AsBitVector(_collectionFactory).Write(o.AsUnsigned(), v.AsBitVector(_collectionFactory)),
                (b, o, v) => new SymbolicWriteExpression(_contextFactory, _collectionFactory,
                    b, o, v))
            : throw new InconsistentExpressionSizesException(Size, offset.Size);
    }

    public IExpression Xor(IExpression expression)
    {
        return Constant.IsZero
            ? expression
            : Not().Constant.IsZero
                ? expression.Not()
                : Binary(expression,
                    (l, r) => l.AsUnsigned().Xor(r.AsUnsigned()),
                    (l, r) => l.AsSymbolic().Xor(r));
    }

    public IExpression ZeroExtend(Bits size)
    {
        return size > Size
            ? Unary(v => v.AsUnsigned().Extend(size))
            : this;
    }

    private IExpression Unary(Func<IConstantValue, IConstantValue> constant)
    {
        return new ConstantExpression(_contextFactory, _collectionFactory,
            constant(_value));
    }

    private IExpression Unary(
        IConstantValue? constant,
        Func<ConstantExpression, IExpression> symbolic)
    {
        return constant == null
            ? symbolic(this)
            : new ConstantExpression(_contextFactory, _collectionFactory,
                constant);
    }

    private IExpression Unary(
        Func<IConstantValue, IConstantValue?> constant,
        Func<ConstantExpression, IExpression> symbolic)
    {
        return Unary(
            constant(_value),
            symbolic);
    }

    private IExpression Unary(
        Func<float, IConstantValue?> constantSingle,
        Func<double, IConstantValue?> constantDouble,
        Func<ConstantExpression, IExpression> symbolic)
    {
        return Unary(
            x => (uint) Size switch
            {
                32U => constantSingle(x.AsSingle()),
                64U => constantDouble(x.AsDouble()),
                _ => null
            },
            symbolic);
    }

    private IExpression Binary(IExpression y,
        IConstantValue? constant,
        Func<ConstantExpression, IExpression, IExpression> symbolic)
    {
        return constant == null
            ? symbolic(this, y)
            : new ConstantExpression(_contextFactory, _collectionFactory,
                constant);
    }

    private IExpression Binary(IExpression y,
        Func<IConstantValue, IConstantValue, IConstantValue?> constant,
        Func<ConstantExpression, IExpression, IExpression> symbolic)
    {
        return Size == y.Size
            ? Binary(y,
                y is ConstantExpression cy
                    ? constant(_value, cy._value)
                    : null,
                symbolic)
            : throw new InconsistentExpressionSizesException(Size, y.Size);
    }

    private IExpression Binary(IExpression y,
        Func<float, float, IConstantValue> constantSingle,
        Func<double, double, IConstantValue> constantDouble,
        Func<ConstantExpression, IExpression, IExpression> symbolic)
    {
        return Binary(y,
            (a, b) => (uint) Size switch
            {
                32U => constantSingle(a.AsSingle(), b.AsSingle()),
                64U => constantDouble(a.AsDouble(), b.AsDouble()),
                _ => null
            },
            symbolic);
    }

    private IExpression Ternary(IExpression y, IExpression z,
        IConstantValue? constant,
        Func<ConstantExpression, IExpression, IExpression, IExpression> symbolic)
    {
        return constant == null
            ? symbolic(this, y, z)
            : new ConstantExpression(_contextFactory, _collectionFactory,
                constant);
    }

    private IExpression Ternary(IExpression y, IExpression z,
        Func<IConstantValue, IConstantValue, IConstantValue, IConstantValue> constant,
        Func<ConstantExpression, IExpression, IExpression, IExpression> symbolic)
    {
        return Ternary(y, z,
            y is ConstantExpression cy && z is ConstantExpression cz
                ? constant(_value, cy._value, cz._value)
                : null,
            symbolic);
    }

    private IExpression AsSymbolic()
    {
        return SymbolicExpression.Create(_contextFactory, _collectionFactory,
            Value, Enumerable.Empty<Func<IExpression, IExpression>>());
    }
}
