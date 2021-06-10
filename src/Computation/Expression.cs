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
        private readonly SymbolicBool[]? _constraints;
        private readonly Lazy<BigInteger> _integer;
        private readonly IValue _value;

        private Expression(ICollectionFactory collectionFactory,
            IValue value, SymbolicBool[]? constraints)
        {
            _collectionFactory = collectionFactory;
            _value = value;
            _constraints = constraints;
            _integer = new Lazy<BigInteger>(ComputeInteger);
        }

        public Bits Size => _value.Size;
        public BigInteger Integer => _integer.Value;

        public IExpression GetValue(ISpace space)
        {
            return Create(c => c.GetValue((IPersistentSpace) space, _constraints ?? Array.Empty<SymbolicBool>()));
        }

        public IProposition GetProposition(ISpace space)
        {
            return _value.GetProposition((IPersistentSpace) space, _constraints ?? Array.Empty<SymbolicBool>());
        }

        public IExpression Add(IExpression expression)
        {
            return Create(expression, (l, r) => l.Add(r));
        }

        public IExpression And(IExpression expression)
        {
            return Create(expression, (l, r) => l.And(r));
        }

        public IExpression ArithmeticShiftRight(IExpression expression)
        {
            return Create(expression, (l, r) => l.ArithmeticShiftRight(r));
        }

        public IExpression Equal(IExpression expression)
        {
            return Create(expression, (l, r) => l.Equal(r));
        }

        public IExpression FloatAdd(IExpression expression)
        {
            return Create(expression, (l, r) => l.FloatAdd(r));
        }

        public IExpression FloatCeiling()
        {
            return Create(c => c.FloatCeiling());
        }

        public IExpression FloatConvert(Bits size)
        {
            return Create(c => c.FloatConvert(size));
        }

        public IExpression FloatDivide(IExpression expression)
        {
            return Create(expression, (l, r) => l.FloatDivide(r));
        }

        public IExpression FloatEqual(IExpression expression)
        {
            return Create(expression, (l, r) => l.FloatEqual(r));
        }

        public IExpression FloatFloor()
        {
            return Create(c => c.FloatFloor());
        }

        public IExpression FloatGreater(IExpression expression)
        {
            return Create(expression, (l, r) => l.FloatGreater(r));
        }

        public IExpression FloatGreaterOrEqual(IExpression expression)
        {
            return Create(expression, (l, r) => l.FloatGreaterOrEqual(r));
        }

        public IExpression FloatLess(IExpression expression)
        {
            return Create(expression, (l, r) => l.FloatLess(r));
        }

        public IExpression FloatLessOrEqual(IExpression expression)
        {
            return Create(expression, (l, r) => l.FloatLessOrEqual(r));
        }

        public IExpression FloatMultiply(IExpression expression)
        {
            return Create(expression, (l, r) => l.FloatMultiply(r));
        }

        public IExpression FloatNegate()
        {
            return Create(c => c.FloatNegate());
        }

        public IExpression FloatNotEqual(IExpression expression)
        {
            return Create(expression, (l, r) => l.FloatNotEqual(r));
        }

        public IExpression FloatOrdered(IExpression expression)
        {
            return Create(expression, (l, r) => l.FloatOrdered(r));
        }

        public IExpression FloatRemainder(IExpression expression)
        {
            return Create(expression, (l, r) => l.FloatRemainder(r));
        }

        public IExpression FloatSubtract(IExpression expression)
        {
            return Create(expression, (l, r) => l.FloatSubtract(r));
        }

        public IExpression FloatToSigned(Bits size)
        {
            return Create(c => c.FloatToSigned(size));
        }

        public IExpression FloatToUnsigned(Bits size)
        {
            return Create(c => c.FloatToUnsigned(size));
        }

        public IExpression FloatUnordered(IExpression expression)
        {
            return Create(expression, (l, r) => l.FloatUnordered(r));
        }

        public IExpression LogicalShiftRight(IExpression expression)
        {
            return Create(expression, (l, r) => l.LogicalShiftRight(r));
        }

        public IExpression Multiply(IExpression expression)
        {
            return Create(expression, (l, r) => l.Multiply(r));
        }

        public IExpression NotEqual(IExpression expression)
        {
            return Create(expression, (l, r) => l.NotEqual(r));
        }

        public IExpression Or(IExpression expression)
        {
            return Create(expression, (l, r) => l.Or(r));
        }

        public IExpression Read(IExpression offset, Bits size)
        {
            return Create(offset, (c, o) => c.Read(_collectionFactory, o, size));
        }

        public IExpression Select(IExpression trueValue, IExpression falseValue)
        {
            return Create(trueValue, falseValue, (p, t, f) => p.Select(t, f));
        }

        public IExpression ShiftLeft(IExpression expression)
        {
            return Create(expression, (l, r) => l.ShiftLeft(r));
        }

        public IExpression SignedDivide(IExpression expression)
        {
            return Create(expression, (l, r) => l.SignedDivide(r));
        }

        public IExpression SignedGreater(IExpression expression)
        {
            return Create(expression, (l, r) => l.SignedGreater(r));
        }

        public IExpression SignedGreaterOrEqual(IExpression expression)
        {
            return Create(expression, (l, r) => l.SignedGreaterOrEqual(r));
        }

        public IExpression SignedLess(IExpression expression)
        {
            return Create(expression, (l, r) => l.SignedLess(r));
        }

        public IExpression SignedLessOrEqual(IExpression expression)
        {
            return Create(expression, (l, r) => l.SignedLessOrEqual(r));
        }

        public IExpression SignedRemainder(IExpression expression)
        {
            return Create(expression, (l, r) => l.SignedRemainder(r));
        }

        public IExpression SignedToFloat(Bits size)
        {
            return Create(c => c.SignedToFloat(size));
        }

        public IExpression SignExtend(Bits size)
        {
            return size > Size
                ? Create(c => c.SignExtend(size))
                : this;
        }

        public IExpression Subtract(IExpression expression)
        {
            return Create(expression, (l, r) => l.Subtract(r));
        }

        public IExpression Truncate(Bits size)
        {
            return size < Size
                ? Create(c => c.Truncate(size))
                : this;
        }

        public IExpression UnsignedDivide(IExpression expression)
        {
            return Create(expression, (l, r) => l.UnsignedDivide(r));
        }

        public IExpression UnsignedGreater(IExpression expression)
        {
            return Create(expression, (l, r) => l.UnsignedGreater(r));
        }

        public IExpression UnsignedGreaterOrEqual(IExpression expression)
        {
            return Create(expression, (l, r) => l.UnsignedGreaterOrEqual(r));
        }

        public IExpression UnsignedLess(IExpression expression)
        {
            return Create(expression, (l, r) => l.UnsignedLess(r));
        }

        public IExpression UnsignedLessOrEqual(IExpression expression)
        {
            return Create(expression, (l, r) => l.UnsignedLessOrEqual(r));
        }

        public IExpression UnsignedRemainder(IExpression expression)
        {
            return Create(expression, (l, r) => l.UnsignedRemainder(r));
        }

        public IExpression UnsignedToFloat(Bits size)
        {
            return Create(c => c.UnsignedToFloat(size));
        }

        public IExpression Write(IExpression offset, IExpression value)
        {
            return Create(offset, value, (c, o, v) => c.Write(_collectionFactory, o, v));
        }

        public IExpression Xor(IExpression expression)
        {
            return Create(expression, (l, r) => l.Xor(r));
        }

        public IExpression ZeroExtend(Bits size)
        {
            return size > Size
                ? Create(c => c.ZeroExtend(size))
                : this;
        }

        private BigInteger ComputeInteger()
        {
            using var context = new Context();
            return _value.GetInteger(context);
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
                        .Select(c => ((Expression) c(unconstrained))._value.ToSymbolicBool())
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

        private static SymbolicBool[]? Concat(SymbolicBool[]? left, SymbolicBool[]? right)
        {
            return left == null
                ? right
                : right == null
                    ? left
                    : left.Concat(right).ToArray();
        }
    }
}
