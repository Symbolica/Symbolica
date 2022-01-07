using System;
using System.Linq;
using System.Numerics;
using Symbolica.Collection;
using Symbolica.Computation.Exceptions;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation
{
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
                (l, r) => ConstantUnsigned.Create(Size, (BigInteger) l.AsUnsigned() + r.AsUnsigned()),
                (l, r) => l.Add(r));
        }

        public IExpression And(IExpression expression)
        {
            return Binary(expression,
                (l, r) => ConstantUnsigned.Create(Size, (BigInteger) l.AsUnsigned() & r.AsUnsigned()),
                (l, r) => l.And(r));
        }

        public IExpression ArithmeticShiftRight(IExpression expression)
        {
            return Binary(expression,
                (l, r) => ConstantSigned.Create(Size, (BigInteger) l.AsSigned() >> (int) r.AsUnsigned()),
                (l, r) => l.ArithmeticShiftRight(r));
        }

        public IExpression Equal(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new ConstantBool((BigInteger) l.AsUnsigned() == r.AsUnsigned()),
                (l, r) => l.Equal(r));
        }

        public IExpression FloatAdd(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new ConstantSingle(l + r),
                (l, r) => new ConstantDouble(l + r),
                (l, r) => l.FloatAdd(r));
        }

        public IExpression FloatCeiling()
        {
            return Unary(
                v => new ConstantSingle(MathF.Ceiling(v)),
                v => new ConstantDouble(Math.Ceiling(v)),
                e => e.FloatCeiling());
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
                e => e.FloatConvert(size));
        }

        public IExpression FloatDivide(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new ConstantSingle(l / r),
                (l, r) => new ConstantDouble(l / r),
                (l, r) => l.FloatDivide(r));
        }

        public IExpression FloatEqual(IExpression expression)
        {
            return Binary(expression,
                // ReSharper disable CompareOfFloatsByEqualityOperator
                (l, r) => new ConstantBool(l == r),
                (l, r) => new ConstantBool(l == r),
                // ReSharper restore CompareOfFloatsByEqualityOperator
                (l, r) => l.FloatEqual(r));
        }

        public IExpression FloatFloor()
        {
            return Unary(
                v => new ConstantSingle(MathF.Floor(v)),
                v => new ConstantDouble(Math.Floor(v)),
                e => e.FloatFloor());
        }

        public IExpression FloatGreater(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new ConstantBool(l > r),
                (l, r) => new ConstantBool(l > r),
                (l, r) => l.FloatGreater(r));
        }

        public IExpression FloatGreaterOrEqual(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new ConstantBool(l >= r),
                (l, r) => new ConstantBool(l >= r),
                (l, r) => l.FloatGreaterOrEqual(r));
        }

        public IExpression FloatLess(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new ConstantBool(l < r),
                (l, r) => new ConstantBool(l < r),
                (l, r) => l.FloatLess(r));
        }

        public IExpression FloatLessOrEqual(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new ConstantBool(l <= r),
                (l, r) => new ConstantBool(l <= r),
                (l, r) => l.FloatLessOrEqual(r));
        }

        public IExpression FloatMultiply(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new ConstantSingle(l * r),
                (l, r) => new ConstantDouble(l * r),
                (l, r) => l.FloatMultiply(r));
        }

        public IExpression FloatNegate()
        {
            return Unary(
                v => new ConstantSingle(-v),
                v => new ConstantDouble(-v),
                e => e.FloatNegate());
        }

        public IExpression FloatNotEqual(IExpression expression)
        {
            return Binary(expression,
                // ReSharper disable CompareOfFloatsByEqualityOperator
                (l, r) => new ConstantBool(l != r),
                (l, r) => new ConstantBool(l != r),
                // ReSharper restore CompareOfFloatsByEqualityOperator
                (l, r) => l.FloatNotEqual(r));
        }

        public IExpression FloatOrdered(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new ConstantBool(!(float.IsNaN(l) || float.IsNaN(r))),
                (l, r) => new ConstantBool(!(double.IsNaN(l) || double.IsNaN(r))),
                (l, r) => l.FloatOrdered(r));
        }

        public IExpression FloatPower(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new ConstantSingle(MathF.Pow(l, r)),
                (l, r) => new ConstantDouble(Math.Pow(l, r)),
                (l, r) => l.FloatPower(r));
        }

        public IExpression FloatRemainder(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new ConstantSingle(MathF.IEEERemainder(l, r)),
                (l, r) => new ConstantDouble(Math.IEEERemainder(l, r)),
                (l, r) => l.FloatRemainder(r));
        }

        public IExpression FloatSubtract(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new ConstantSingle(l - r),
                (l, r) => new ConstantDouble(l - r),
                (l, r) => l.FloatSubtract(r));
        }

        public IExpression FloatToSigned(Bits size)
        {
            return Unary(
                v => ConstantSigned.Create(size, (BigInteger) v),
                v => ConstantSigned.Create(size, (BigInteger) v),
                e => e.FloatToSigned(size));
        }

        public IExpression FloatToUnsigned(Bits size)
        {
            return Unary(
                v => ConstantUnsigned.Create(size, (BigInteger) v),
                v => ConstantUnsigned.Create(size, (BigInteger) v),
                e => e.FloatToUnsigned(size));
        }

        public IExpression FloatUnordered(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new ConstantBool(float.IsNaN(l) || float.IsNaN(r)),
                (l, r) => new ConstantBool(double.IsNaN(l) || double.IsNaN(r)),
                (l, r) => l.FloatUnordered(r));
        }

        public IExpression LogicalShiftRight(IExpression expression)
        {
            return Binary(expression,
                (l, r) => ConstantUnsigned.Create(Size, (BigInteger) l.AsUnsigned() >> (int) r.AsUnsigned()),
                (l, r) => l.LogicalShiftRight(r));
        }

        public IExpression Multiply(IExpression expression)
        {
            return Binary(expression,
                (l, r) => ConstantUnsigned.Create(Size, (BigInteger) l.AsUnsigned() * r.AsUnsigned()),
                (l, r) => l.Multiply(r));
        }

        public IExpression Not()
        {
            return Unary(v => ConstantUnsigned.Create(Size, ~(BigInteger) v.AsUnsigned()));
        }

        public IExpression NotEqual(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new ConstantBool((BigInteger) l.AsUnsigned() != r.AsUnsigned()),
                (l, r) => l.NotEqual(r));
        }

        public IExpression Or(IExpression expression)
        {
            return Binary(expression,
                (l, r) => ConstantUnsigned.Create(Size, (BigInteger) l.AsUnsigned() | r.AsUnsigned()),
                (l, r) => l.Or(r));
        }

        public IExpression Read(IExpression offset, Bits size)
        {
            return Binary(offset,
                (b, o) => b.AsBitVector(_collectionFactory)
                    .Read(o.AsUnsigned(), size),
                (b, o) => b.Read(o, size));
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
                (l, r) => ConstantUnsigned.Create(Size, (BigInteger) l.AsUnsigned() << (int) r.AsUnsigned()),
                (l, r) => l.ShiftLeft(r));
        }

        public IExpression SignedDivide(IExpression expression)
        {
            return Binary(expression,
                (l, r) => ConstantSigned.Create(Size, (BigInteger) l.AsSigned() / r.AsSigned()),
                (l, r) => l.SignedDivide(r));
        }

        public IExpression SignedGreater(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new ConstantBool((BigInteger) l.AsSigned() > r.AsSigned()),
                (l, r) => l.SignedGreater(r));
        }

        public IExpression SignedGreaterOrEqual(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new ConstantBool((BigInteger) l.AsSigned() >= r.AsSigned()),
                (l, r) => l.SignedGreaterOrEqual(r));
        }

        public IExpression SignedLess(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new ConstantBool((BigInteger) l.AsSigned() < r.AsSigned()),
                (l, r) => l.SignedLess(r));
        }

        public IExpression SignedLessOrEqual(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new ConstantBool((BigInteger) l.AsSigned() <= r.AsSigned()),
                (l, r) => l.SignedLessOrEqual(r));
        }

        public IExpression SignedRemainder(IExpression expression)
        {
            return Binary(expression,
                (l, r) => ConstantSigned.Create(Size, (BigInteger) l.AsSigned() % r.AsSigned()),
                (l, r) => l.SignedRemainder(r));
        }

        public IExpression SignedToFloat(Bits size)
        {
            return Unary(
                v => (uint) size switch
                {
                    32U => new ConstantSingle((float) (BigInteger) v.AsSigned()),
                    64U => new ConstantDouble((double) (BigInteger) v.AsSigned()),
                    _ => null
                },
                e => e.SignedToFloat(size));
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
                (l, r) => ConstantUnsigned.Create(Size, (BigInteger) l.AsUnsigned() - r.AsUnsigned()),
                (l, r) => l.Subtract(r));
        }

        public IExpression Truncate(Bits size)
        {
            return size < Size
                ? Unary(v => ConstantUnsigned.Create(size, v.AsUnsigned()))
                : this;
        }

        public IExpression UnsignedDivide(IExpression expression)
        {
            return Binary(expression,
                (l, r) => ConstantUnsigned.Create(Size, (BigInteger) l.AsUnsigned() / r.AsUnsigned()),
                (l, r) => l.UnsignedDivide(r));
        }

        public IExpression UnsignedGreater(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new ConstantBool((BigInteger) l.AsUnsigned() > r.AsUnsigned()),
                (l, r) => l.UnsignedGreater(r));
        }

        public IExpression UnsignedGreaterOrEqual(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new ConstantBool((BigInteger) l.AsUnsigned() >= r.AsUnsigned()),
                (l, r) => l.UnsignedGreaterOrEqual(r));
        }

        public IExpression UnsignedLess(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new ConstantBool((BigInteger) l.AsUnsigned() < r.AsUnsigned()),
                (l, r) => l.UnsignedLess(r));
        }

        public IExpression UnsignedLessOrEqual(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new ConstantBool((BigInteger) l.AsUnsigned() <= r.AsUnsigned()),
                (l, r) => l.UnsignedLessOrEqual(r));
        }

        public IExpression UnsignedRemainder(IExpression expression)
        {
            return Binary(expression,
                (l, r) => ConstantUnsigned.Create(Size, (BigInteger) l.AsUnsigned() % r.AsUnsigned()),
                (l, r) => l.UnsignedRemainder(r));
        }

        public IExpression UnsignedToFloat(Bits size)
        {
            return Unary(
                v => (uint) size switch
                {
                    32U => new ConstantSingle((float) (BigInteger) v.AsUnsigned()),
                    64U => new ConstantDouble((double) (BigInteger) v.AsUnsigned()),
                    _ => null
                },
                e => e.UnsignedToFloat(size));
        }

        public IExpression Write(IExpression offset, IExpression value)
        {
            return Size == offset.Size
                ? Ternary(offset, value,
                    (b, o, v) => b.AsBitVector(_collectionFactory)
                        .Write(o.AsUnsigned(), v.AsBitVector(_collectionFactory)),
                    (b, o, v) => b.Write(o, v))
                : throw new InconsistentExpressionSizesException(Size, offset.Size);
        }

        public IExpression Xor(IExpression expression)
        {
            return Binary(expression,
                (l, r) => ConstantUnsigned.Create(Size, (BigInteger) l.AsUnsigned() ^ r.AsUnsigned()),
                (l, r) => l.Xor(r));
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
            Func<IExpression, IExpression> symbolic)
        {
            return constant == null
                ? symbolic(AsSymbolic())
                : new ConstantExpression(_contextFactory, _collectionFactory,
                    constant);
        }

        private IExpression Unary(
            Func<IConstantValue, IConstantValue?> constant,
            Func<IExpression, IExpression> symbolic)
        {
            return Unary(
                constant(_value),
                symbolic);
        }

        private IExpression Unary(
            Func<float, IConstantValue?> constantSingle,
            Func<double, IConstantValue?> constantDouble,
            Func<IExpression, IExpression> symbolic)
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
            Func<IExpression, IExpression, IExpression> symbolic)
        {
            return constant == null
                ? symbolic(AsSymbolic(), y)
                : new ConstantExpression(_contextFactory, _collectionFactory,
                    constant);
        }

        private IExpression Binary(IExpression y,
            Func<IConstantValue, IConstantValue, IConstantValue?> constant,
            Func<IExpression, IExpression, IExpression> symbolic)
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
            Func<IExpression, IExpression, IExpression> symbolic)
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
            Func<IExpression, IExpression, IExpression, IExpression> symbolic)
        {
            return constant == null
                ? symbolic(AsSymbolic(), y, z)
                : new ConstantExpression(_contextFactory, _collectionFactory,
                    constant);
        }

        private IExpression Ternary(IExpression y, IExpression z,
            Func<IConstantValue, IConstantValue, IConstantValue, IConstantValue> constant,
            Func<IExpression, IExpression, IExpression, IExpression> symbolic)
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
}
