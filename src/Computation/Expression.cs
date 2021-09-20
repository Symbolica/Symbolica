using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class Expression : IExpression
    {
        private readonly ICollectionFactory _collectionFactory;
        private readonly IBool[]? _constraints;
        private readonly Lazy<BigInteger> _integer;
        private readonly IValue _value;

        private Expression(ICollectionFactory collectionFactory,
            IValue value, IBool[]? constraints)
        {
            _collectionFactory = collectionFactory;
            _value = value;
            _constraints = constraints;
            _integer = new Lazy<BigInteger>(GetConstant);
        }

        public Bits Size => _value.Size;
        public BigInteger Integer => _integer.Value;

        public IExpression GetValue(ISpace space)
        {
            return Create(c => c.GetValue((IPersistentSpace) space, _constraints ?? Array.Empty<IBool>()));
        }

        public IProposition GetProposition(ISpace space)
        {
            return _value.AsBool().GetProposition((IPersistentSpace) space, _constraints ?? Array.Empty<IBool>());
        }

        public IExpression Add(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsUnsigned().Add(r.AsUnsigned()));
        }

        public IExpression And(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsBitwise().And(r.AsBitwise()));
        }

        public IExpression ArithmeticShiftRight(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsSigned().ArithmeticShiftRight(r.AsUnsigned()));
        }

        public IExpression Equal(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsBitwise().Equal(r.AsBitwise()));
        }

        public IExpression FloatAdd(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsFloat().Add(r.AsFloat()));
        }

        public IExpression FloatCeiling()
        {
            return Create(c => c.AsFloat().Ceiling());
        }

        public IExpression FloatConvert(Bits size)
        {
            return Create(c => c.AsFloat().Convert(size));
        }

        public IExpression FloatDivide(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsFloat().Divide(r.AsFloat()));
        }

        public IExpression FloatEqual(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsFloat().Equal(r.AsFloat()));
        }

        public IExpression FloatFloor()
        {
            return Create(c => c.AsFloat().Floor());
        }

        public IExpression FloatGreater(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsFloat().Greater(r.AsFloat()));
        }

        public IExpression FloatGreaterOrEqual(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsFloat().GreaterOrEqual(r.AsFloat()));
        }

        public IExpression FloatLess(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsFloat().Less(r.AsFloat()));
        }

        public IExpression FloatLessOrEqual(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsFloat().LessOrEqual(r.AsFloat()));
        }

        public IExpression FloatMultiply(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsFloat().Multiply(r.AsFloat()));
        }

        public IExpression FloatNegate()
        {
            return Create(c => c.AsFloat().Negate());
        }

        public IExpression FloatNotEqual(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsFloat().NotEqual(r.AsFloat()));
        }

        public IExpression FloatOrdered(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsFloat().Ordered(r.AsFloat()));
        }

        public IExpression FloatRemainder(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsFloat().Remainder(r.AsFloat()));
        }

        public IExpression FloatSubtract(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsFloat().Subtract(r.AsFloat()));
        }

        public IExpression FloatToSigned(Bits size)
        {
            return Create(c => c.AsFloat().ToSigned(size));
        }

        public IExpression FloatToUnsigned(Bits size)
        {
            return Create(c => c.AsFloat().ToUnsigned(size));
        }

        public IExpression FloatUnordered(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsFloat().Unordered(r.AsFloat()));
        }

        public IExpression LogicalShiftRight(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsUnsigned().LogicalShiftRight(r.AsUnsigned()));
        }

        public IExpression Multiply(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsUnsigned().Multiply(r.AsUnsigned()));
        }

        public IExpression NotEqual(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsBitwise().NotEqual(r.AsBitwise()));
        }

        public IExpression Or(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsBitwise().Or(r.AsBitwise()));
        }

        public IExpression Read(IExpression offset, Bits size)
        {
            return Create(offset, (c, o) =>
                c.AsBitVector(_collectionFactory).Read(o.AsUnsigned(), size));
        }

        public IExpression Select(IExpression trueValue, IExpression falseValue)
        {
            return Create(trueValue, falseValue, (p, t, f) => p.AsBool().Select(t, f));
        }

        public IExpression ShiftLeft(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsUnsigned().ShiftLeft(r.AsUnsigned()));
        }

        public IExpression SignedDivide(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsSigned().Divide(r.AsSigned()));
        }

        public IExpression SignedGreater(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsSigned().Greater(r.AsSigned()));
        }

        public IExpression SignedGreaterOrEqual(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsSigned().GreaterOrEqual(r.AsSigned()));
        }

        public IExpression SignedLess(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsSigned().Less(r.AsSigned()));
        }

        public IExpression SignedLessOrEqual(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsSigned().LessOrEqual(r.AsSigned()));
        }

        public IExpression SignedRemainder(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsSigned().Remainder(r.AsSigned()));
        }

        public IExpression SignedToFloat(Bits size)
        {
            return Create(c => c.AsSigned().SignedToFloat(size));
        }

        public IExpression SignExtend(Bits size)
        {
            return size > Size
                ? Create(c => c.AsSigned().SignExtend(size))
                : this;
        }

        public IExpression Subtract(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsUnsigned().Subtract(r.AsUnsigned()));
        }

        public IExpression Truncate(Bits size)
        {
            return size < Size
                ? Create(c => c.AsUnsigned().Truncate(size))
                : this;
        }

        public IExpression UnsignedDivide(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsUnsigned().Divide(r.AsUnsigned()));
        }

        public IExpression UnsignedGreater(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsUnsigned().Greater(r.AsUnsigned()));
        }

        public IExpression UnsignedGreaterOrEqual(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsUnsigned().GreaterOrEqual(r.AsUnsigned()));
        }

        public IExpression UnsignedLess(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsUnsigned().Less(r.AsUnsigned()));
        }

        public IExpression UnsignedLessOrEqual(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsUnsigned().LessOrEqual(r.AsUnsigned()));
        }

        public IExpression UnsignedRemainder(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsUnsigned().Remainder(r.AsUnsigned()));
        }

        public IExpression UnsignedToFloat(Bits size)
        {
            return Create(c => c.AsUnsigned().UnsignedToFloat(size));
        }

        public IExpression Write(IExpression offset, IExpression value)
        {
            return Create(offset, value, (c, o, v) =>
                c.AsBitVector(_collectionFactory).Write(o.AsUnsigned(), v.AsBitVector(_collectionFactory)));
        }

        public IExpression Xor(IExpression expression)
        {
            return Create(expression, (l, r) => l.AsBitwise().Xor(r.AsBitwise()));
        }

        public IExpression ZeroExtend(Bits size)
        {
            return size > Size
                ? Create(c => c.AsUnsigned().ZeroExtend(size))
                : this;
        }

        private BigInteger GetConstant()
        {
            return _value.AsUnsigned() is IConstantInteger c
                ? c.Constant
                : AsConstant();
        }

        private BigInteger AsConstant()
        {
            using var context = new Context();
            return _value.AsConstant(context);
        }

        public static IExpression Create(ICollectionFactory collectionFactory,
            IValue value, IEnumerable<Func<IExpression, IExpression>>? constraints)
        {
            var unconstrained = new Expression(collectionFactory,
                value, null);

            return constraints == null
                ? unconstrained
                : new Expression(collectionFactory,
                    value, constraints
                        .Select(c => ((Expression) c(unconstrained))._value.AsBool())
                        .ToArray());
        }

        private IExpression Create(Func<IValue, IValue> func)
        {
            return new Expression(_collectionFactory,
                func(_value), _constraints);
        }

        private IExpression Create(IExpression other,
            Func<IValue, IValue, IValue> func)
        {
            var y = (Expression) other;

            return new Expression(_collectionFactory,
                func(_value, y._value), Concat(_constraints, y._constraints));
        }

        private IExpression Create(IExpression second, IExpression third,
            Func<IValue, IValue, IValue, IValue> func)
        {
            var y = (Expression) second;
            var z = (Expression) third;

            return new Expression(_collectionFactory,
                func(_value, y._value, z._value), Concat(_constraints, Concat(y._constraints, z._constraints)));
        }

        private static IBool[]? Concat(IBool[]? left, IBool[]? right)
        {
            return left == null
                ? right
                : right == null
                    ? left
                    : left.Concat(right).ToArray();
        }
    }
}
