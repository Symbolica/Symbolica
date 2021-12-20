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

namespace Symbolica.Computation
{
    internal sealed class Expression : IExpression
    {
        private readonly ICollectionFactory _collectionFactory;
        private readonly IValue[]? _constraints;
        private readonly IContextFactory _contextFactory;
        private readonly IValue _value;

        private Expression(IContextFactory contextFactory, ICollectionFactory collectionFactory,
            IValue value, IValue[]? constraints)
        {
            _contextFactory = contextFactory;
            _collectionFactory = collectionFactory;
            _value = value;
            _constraints = constraints;
        }

        public Bits Size => _value.Size;

        public BigInteger Constant => _value is IConstantValue c
            ? c.AsUnsigned()
            : AsConstant(_contextFactory, _value);

        public IExpression GetValue(ISpace space)
        {
            return Unary(
                v => GetValue((IPersistentSpace) space, v, _constraints ?? Array.Empty<IValue>()),
                v => v);
        }

        public IProposition GetProposition(ISpace space)
        {
            return _value is IConstantValue c
                ? ConstantProposition.Create(space, c.AsBool())
                : SymbolicProposition.Create((IPersistentSpace) space, _value, _constraints ?? Array.Empty<IValue>());
        }

        public IExpression Add(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new Add(l, r),
                (l, r) => ConstantUnsigned.Create(Size, (BigInteger) l.AsUnsigned() + r.AsUnsigned()));
        }

        public IExpression And(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new And(l, r),
                (l, r) => ConstantUnsigned.Create(Size, (BigInteger) l.AsUnsigned() & r.AsUnsigned()));
        }

        public IExpression ArithmeticShiftRight(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new ArithmeticShiftRight(l, r),
                (l, r) => ConstantSigned.Create(Size, (BigInteger) l.AsSigned() >> (int) r.AsUnsigned()));
        }

        public IExpression Equal(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new Equal(l, r),
                (l, r) => new ConstantBool((BigInteger) l.AsUnsigned() == r.AsUnsigned()));
        }

        public IExpression FloatAdd(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new FloatAdd(l, r),
                (l, r) => new ConstantSingle(l + r),
                (l, r) => new ConstantDouble(l + r));
        }

        public IExpression FloatCeiling()
        {
            return Unary(
                v => new FloatCeiling(v),
                v => new ConstantSingle(MathF.Ceiling(v)),
                v => new ConstantDouble(Math.Ceiling(v)));
        }

        public IExpression FloatConvert(Bits size)
        {
            return Unary(
                v => v is IRealValue r
                    ? new RealConvert(size, r)
                    : new FloatConvert(size, v),
                v => (uint) size switch
                {
                    32U => _value,
                    64U => new ConstantDouble(v),
                    _ => new FloatConvert(size, _value)
                },
                v => (uint) size switch
                {
                    32U => new ConstantSingle((float) v),
                    64U => _value,
                    _ => new FloatConvert(size, _value)
                });
        }

        public IExpression FloatDivide(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new FloatDivide(l, r),
                (l, r) => new ConstantSingle(l / r),
                (l, r) => new ConstantDouble(l / r));
        }

        public IExpression FloatEqual(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new FloatEqual(l, r),
                // ReSharper disable CompareOfFloatsByEqualityOperator
                (l, r) => new ConstantBool(l == r),
                (l, r) => new ConstantBool(l == r)
                // ReSharper restore CompareOfFloatsByEqualityOperator
            );
        }

        public IExpression FloatFloor()
        {
            return Unary(
                v => new FloatFloor(v),
                v => new ConstantSingle(MathF.Floor(v)),
                v => new ConstantDouble(Math.Floor(v)));
        }

        public IExpression FloatGreater(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new FloatGreater(l, r),
                (l, r) => new ConstantBool(l > r),
                (l, r) => new ConstantBool(l > r));
        }

        public IExpression FloatGreaterOrEqual(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new FloatGreaterOrEqual(l, r),
                (l, r) => new ConstantBool(l >= r),
                (l, r) => new ConstantBool(l >= r));
        }

        public IExpression FloatLess(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new FloatLess(l, r),
                (l, r) => new ConstantBool(l < r),
                (l, r) => new ConstantBool(l < r));
        }

        public IExpression FloatLessOrEqual(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new FloatLessOrEqual(l, r),
                (l, r) => new ConstantBool(l <= r),
                (l, r) => new ConstantBool(l <= r));
        }

        public IExpression FloatMultiply(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new FloatMultiply(l, r),
                (l, r) => new ConstantSingle(l * r),
                (l, r) => new ConstantDouble(l * r));
        }

        public IExpression FloatNegate()
        {
            return Unary(
                v => new FloatNegate(v),
                v => new ConstantSingle(-v),
                v => new ConstantDouble(-v));
        }

        public IExpression FloatNotEqual(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new Not(new FloatEqual(l, r)),
                // ReSharper disable CompareOfFloatsByEqualityOperator
                (l, r) => new ConstantBool(l != r),
                (l, r) => new ConstantBool(l != r)
                // ReSharper restore CompareOfFloatsByEqualityOperator
            );
        }

        public IExpression FloatOrdered(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new Not(new FloatUnordered(l, r)),
                (l, r) => new ConstantBool(!(float.IsNaN(l) || float.IsNaN(r))),
                (l, r) => new ConstantBool(!(double.IsNaN(l) || double.IsNaN(r))));
        }

        public IExpression FloatPower(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new FloatPower(l, r),
                (l, r) => new ConstantSingle(MathF.Pow(l, r)),
                (l, r) => new ConstantDouble(Math.Pow(l, r)));
        }

        public IExpression FloatRemainder(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new FloatRemainder(l, r),
                (l, r) => new ConstantSingle(MathF.IEEERemainder(l, r)),
                (l, r) => new ConstantDouble(Math.IEEERemainder(l, r)));
        }

        public IExpression FloatSubtract(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new FloatSubtract(l, r),
                (l, r) => new ConstantSingle(l - r),
                (l, r) => new ConstantDouble(l - r));
        }

        public IExpression FloatToSigned(Bits size)
        {
            return Unary(
                v => v is IRealValue r
                    ? new RealToSigned(size, r)
                    : new FloatToSigned(size, v),
                v => ConstantSigned.Create(size, (BigInteger) v),
                v => ConstantSigned.Create(size, (BigInteger) v));
        }

        public IExpression FloatToUnsigned(Bits size)
        {
            return Unary(
                v => new FloatToUnsigned(size, v),
                v => ConstantUnsigned.Create(size, (BigInteger) v),
                v => ConstantUnsigned.Create(size, (BigInteger) v));
        }

        public IExpression FloatUnordered(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new FloatUnordered(l, r),
                (l, r) => new ConstantBool(float.IsNaN(l) || float.IsNaN(r)),
                (l, r) => new ConstantBool(double.IsNaN(l) || double.IsNaN(r)));
        }

        public IExpression LogicalShiftRight(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new LogicalShiftRight(l, r),
                (l, r) => ConstantUnsigned.Create(Size, (BigInteger) l.AsUnsigned() >> (int) r.AsUnsigned()));
        }

        public IExpression Multiply(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new Multiply(l, r),
                (l, r) => ConstantUnsigned.Create(Size, (BigInteger) l.AsUnsigned() * r.AsUnsigned()));
        }

        public IExpression NotEqual(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new Not(new Equal(l, r)),
                (l, r) => new ConstantBool((BigInteger) l.AsUnsigned() != r.AsUnsigned()));
        }

        public IExpression Or(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new Or(l, r),
                (l, r) => ConstantUnsigned.Create(Size, (BigInteger) l.AsUnsigned() | r.AsUnsigned()));
        }

        public IExpression Read(IExpression offset, Bits size)
        {
            return Binary(offset,
                (b, o) => new Truncate(size, new LogicalShiftRight(b, o)),
                (b, o) => b.AsBitVector(_collectionFactory)
                    .Read(o.AsUnsigned(), size));
        }

        public IExpression Select(IExpression trueValue, IExpression falseValue)
        {
            if (trueValue.Size != falseValue.Size)
                throw new InconsistentExpressionSizesException(trueValue.Size, falseValue.Size);

            return _value is IConstantValue c
                ? c.AsBool() ? trueValue : falseValue
                : Ternary(trueValue, falseValue,
                    (p, t, f) => new Select(p, t, f),
                    (p, t, f) => p.AsBool() ? t : f);
        }

        public IExpression ShiftLeft(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new ShiftLeft(l, r),
                (l, r) => ConstantUnsigned.Create(Size, (BigInteger) l.AsUnsigned() << (int) r.AsUnsigned()));
        }

        public IExpression SignedDivide(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new SignedDivide(l, r),
                (l, r) => ConstantSigned.Create(Size, (BigInteger) l.AsSigned() / r.AsSigned()));
        }

        public IExpression SignedGreater(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new SignedGreater(l, r),
                (l, r) => new ConstantBool((BigInteger) l.AsSigned() > r.AsSigned()));
        }

        public IExpression SignedGreaterOrEqual(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new SignedGreaterOrEqual(l, r),
                (l, r) => new ConstantBool((BigInteger) l.AsSigned() >= r.AsSigned()));
        }

        public IExpression SignedLess(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new SignedLess(l, r),
                (l, r) => new ConstantBool((BigInteger) l.AsSigned() < r.AsSigned()));
        }

        public IExpression SignedLessOrEqual(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new SignedLessOrEqual(l, r),
                (l, r) => new ConstantBool((BigInteger) l.AsSigned() <= r.AsSigned()));
        }

        public IExpression SignedRemainder(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new SignedRemainder(l, r),
                (l, r) => ConstantSigned.Create(Size, (BigInteger) l.AsSigned() % r.AsSigned()));
        }

        public IExpression SignedToFloat(Bits size)
        {
            return Unary(
                v => new SignedToFloat(size, v),
                v => (uint) size switch
                {
                    32U => new ConstantSingle((float) (BigInteger) v.AsSigned()),
                    64U => new ConstantDouble((double) (BigInteger) v.AsSigned()),
                    _ => new SignedToFloat(size, v)
                });
        }

        public IExpression SignExtend(Bits size)
        {
            return size > Size
                ? Unary(
                    v => new SignExtend(size, v),
                    v => v.AsSigned().Extend(size))
                : this;
        }

        public IExpression Subtract(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new Subtract(l, r),
                (l, r) => ConstantUnsigned.Create(Size, (BigInteger) l.AsUnsigned() - r.AsUnsigned()));
        }

        public IExpression Truncate(Bits size)
        {
            return size < Size
                ? Unary(
                    v => new Truncate(size, v),
                    v => ConstantUnsigned.Create(size, v.AsUnsigned()))
                : this;
        }

        public IExpression UnsignedDivide(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new UnsignedDivide(l, r),
                (l, r) => ConstantUnsigned.Create(Size, (BigInteger) l.AsUnsigned() / r.AsUnsigned()));
        }

        public IExpression UnsignedGreater(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new UnsignedGreater(l, r),
                (l, r) => new ConstantBool((BigInteger) l.AsUnsigned() > r.AsUnsigned()));
        }

        public IExpression UnsignedGreaterOrEqual(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new UnsignedGreaterOrEqual(l, r),
                (l, r) => new ConstantBool((BigInteger) l.AsUnsigned() >= r.AsUnsigned()));
        }

        public IExpression UnsignedLess(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new UnsignedLess(l, r),
                (l, r) => new ConstantBool((BigInteger) l.AsUnsigned() < r.AsUnsigned()));
        }

        public IExpression UnsignedLessOrEqual(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new UnsignedLessOrEqual(l, r),
                (l, r) => new ConstantBool((BigInteger) l.AsUnsigned() <= r.AsUnsigned()));
        }

        public IExpression UnsignedRemainder(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new UnsignedRemainder(l, r),
                (l, r) => ConstantUnsigned.Create(Size, (BigInteger) l.AsUnsigned() % r.AsUnsigned()));
        }

        public IExpression UnsignedToFloat(Bits size)
        {
            return Unary(
                v => new UnsignedToFloat(size, v),
                v => (uint) size switch
                {
                    32U => new ConstantSingle((float) (BigInteger) v.AsUnsigned()),
                    64U => new ConstantDouble((double) (BigInteger) v.AsUnsigned()),
                    _ => new UnsignedToFloat(size, v)
                });
        }

        public IExpression Write(IExpression offset, IExpression value)
        {
            if (Size != offset.Size)
                throw new InconsistentExpressionSizesException(Size, offset.Size);

            return Ternary(offset, value,
                (b, o, v) => new Or(
                    new And(b, new Not(new ShiftLeft(new ZeroExtend(b.Size, ConstantUnsigned.CreateOnes(v.Size)), o))),
                    new ShiftLeft(new ZeroExtend(b.Size, v), o)),
                (b, o, v) => b.AsBitVector(_collectionFactory)
                    .Write(o.AsUnsigned(), v.AsBitVector(_collectionFactory)));
        }

        public IExpression Xor(IExpression expression)
        {
            return Binary(expression,
                (l, r) => new Xor(l, r),
                (l, r) => ConstantUnsigned.Create(Size, (BigInteger) l.AsUnsigned() ^ r.AsUnsigned()));
        }

        public IExpression ZeroExtend(Bits size)
        {
            return size > Size
                ? Unary(
                    v => new ZeroExtend(size, v),
                    v => v.AsUnsigned().Extend(size))
                : this;
        }

        private static BigInteger AsConstant(IContextFactory contextFactory, IValue value)
        {
            using var handle = contextFactory.Create();

            return value is Float && value.AsFloat(handle.Context).Simplify().IsFPNaN
                ? value.Size.GetNan(handle.Context)
                : AsConstant(handle.Context, value);
        }

        private static BigInteger AsConstant(Context context, IValue value)
        {
            var expr = value.AsBitVector(context).Simplify();

            return expr.IsNumeral
                ? ((BitVecNum) expr).BigInteger
                : throw new IrreducibleSymbolicExpressionException();
        }

        private static IValue GetValue(IPersistentSpace space, IValue value, IValue[] constraints)
        {
            using var model = space.GetModel(constraints);

            return ConstantUnsigned.Create(value.Size, model.Evaluate(value));
        }

        public static IExpression Create(IContextFactory contextFactory, ICollectionFactory collectionFactory,
            IValue value, IEnumerable<Func<IExpression, IExpression>>? constraints)
        {
            var unconstrained = new Expression(contextFactory, collectionFactory,
                value, null);

            return constraints == null
                ? unconstrained
                : new Expression(contextFactory, collectionFactory,
                    value, constraints
                        .Select(c => ((Expression) c(unconstrained))._value)
                        .ToArray());
        }

        private IExpression Unary(
            Func<IValue, IValue> symbolic,
            Func<IConstantValue, IValue> constant)
        {
            return _value is IConstantValue cx
                ? new Expression(_contextFactory, _collectionFactory,
                    constant(cx), null)
                : new Expression(_contextFactory, _collectionFactory,
                    symbolic(_value), _constraints);
        }

        private IExpression Unary(
            Func<IValue, IValue> symbolic,
            Func<float, IValue> constantSingle,
            Func<double, IValue> constantDouble)
        {
            return Unary(
                symbolic,
                x => (uint) Size switch
                {
                    32U => constantSingle(x.AsSingle()),
                    64U => constantDouble(x.AsDouble()),
                    _ => symbolic(x)
                });
        }

        private IExpression Binary(IExpression other,
            Func<IValue, IValue, IValue> symbolic,
            Func<IConstantValue, IConstantValue, IValue> constant)
        {
            if (Size != other.Size)
                throw new InconsistentExpressionSizesException(Size, other.Size);

            var y = (Expression) other;

            return _value is IConstantValue cx && y._value is IConstantValue cy
                ? new Expression(_contextFactory, _collectionFactory,
                    constant(cx, cy), null)
                : new Expression(_contextFactory, _collectionFactory,
                    symbolic(_value, y._value), Concat(_constraints, y._constraints));
        }

        private IExpression Binary(IExpression other,
            Func<IValue, IValue, IValue> symbolic,
            Func<float, float, IValue> constantSingle,
            Func<double, double, IValue> constantDouble)
        {
            return Binary(other,
                symbolic,
                (x, y) => (uint) Size switch
                {
                    32U => constantSingle(x.AsSingle(), y.AsSingle()),
                    64U => constantDouble(x.AsDouble(), y.AsDouble()),
                    _ => symbolic(x, y)
                });
        }

        private IExpression Ternary(IExpression second, IExpression third,
            Func<IValue, IValue, IValue, IValue> symbolic,
            Func<IConstantValue, IConstantValue, IConstantValue, IValue> constant)
        {
            var y = (Expression) second;
            var z = (Expression) third;

            return _value is IConstantValue cx && y._value is IConstantValue cy && z._value is IConstantValue cz
                ? new Expression(_contextFactory, _collectionFactory,
                    constant(cx, cy, cz), null)
                : new Expression(_contextFactory, _collectionFactory,
                    symbolic(_value, y._value, z._value), Concat(_constraints, Concat(y._constraints, z._constraints)));
        }

        private static IValue[]? Concat(IValue[]? left, IValue[]? right)
        {
            return left == null
                ? right
                : right == null
                    ? left
                    : left.Concat(right).ToArray();
        }
    }
}
