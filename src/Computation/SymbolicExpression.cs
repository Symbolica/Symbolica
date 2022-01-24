﻿using System;
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
    internal sealed class SymbolicExpression : ISymbolicExpression
    {
        private readonly ICollectionFactory _collectionFactory;
        private readonly IContextFactory _contextFactory;
        private readonly IReader _reader;

        private SymbolicExpression(IContextFactory contextFactory, ICollectionFactory collectionFactory, IReader reader,
            IValue value, IValue[] constraints)
        {
            _contextFactory = contextFactory;
            _collectionFactory = collectionFactory;
            _reader = reader;
            Value = value;
            Constraints = constraints;
        }

        public Bits Size => Value.Size;
        public BigInteger Constant => AsConstant();
        public IValue Value { get; }
        public IValue[] Constraints { get; }

        public IExpression GetValue(ISpace space)
        {
            return Evaluate((IPersistentSpace) space);
        }

        public IProposition GetProposition(ISpace space)
        {
            return SymbolicProposition.Create((IPersistentSpace) space, Value, Constraints);
        }

        public IExpression Add(IExpression expression)
        {
            return Binary(expression, (l, r) => new Add(l, r));
        }

        IExpression IExpression.And(IExpression expression)
        {
            return And(expression);
        }

        public IExpression ArithmeticShiftRight(IExpression expression)
        {
            return Binary(expression, (l, r) => new ArithmeticShiftRight(l, r));
        }

        public IExpression Equal(IExpression expression)
        {
            return Binary(expression, (l, r) => new Equal(l, r));
        }

        public IExpression FloatAdd(IExpression expression)
        {
            return Binary(expression, (l, r) => new FloatAdd(l, r));
        }

        public IExpression FloatCeiling()
        {
            return Unary(v => new FloatCeiling(v));
        }

        public IExpression FloatConvert(Bits size)
        {
            return Unary(v => v is IRealValue r
                ? new RealConvert(size, r)
                : new FloatConvert(size, v));
        }

        public IExpression FloatDivide(IExpression expression)
        {
            return Binary(expression, (l, r) => new FloatDivide(l, r));
        }

        public IExpression FloatEqual(IExpression expression)
        {
            return Binary(expression, (l, r) => new FloatEqual(l, r));
        }

        public IExpression FloatFloor()
        {
            return Unary(v => new FloatFloor(v));
        }

        public IExpression FloatGreater(IExpression expression)
        {
            return Binary(expression, (l, r) => new FloatGreater(l, r));
        }

        public IExpression FloatGreaterOrEqual(IExpression expression)
        {
            return Binary(expression, (l, r) => new FloatGreaterOrEqual(l, r));
        }

        public IExpression FloatLess(IExpression expression)
        {
            return Binary(expression, (l, r) => new FloatLess(l, r));
        }

        public IExpression FloatLessOrEqual(IExpression expression)
        {
            return Binary(expression, (l, r) => new FloatLessOrEqual(l, r));
        }

        public IExpression FloatMultiply(IExpression expression)
        {
            return Binary(expression, (l, r) => new FloatMultiply(l, r));
        }

        public IExpression FloatNegate()
        {
            return Unary(v => new FloatNegate(v));
        }

        public IExpression FloatNotEqual(IExpression expression)
        {
            return FloatEqual(expression).Not();
        }

        public IExpression FloatOrdered(IExpression expression)
        {
            return FloatUnordered(expression).Not();
        }

        public IExpression FloatPower(IExpression expression)
        {
            return Binary(expression, (l, r) => new FloatPower(l, r));
        }

        public IExpression FloatRemainder(IExpression expression)
        {
            return Binary(expression, (l, r) => new FloatRemainder(l, r));
        }

        public IExpression FloatSubtract(IExpression expression)
        {
            return Binary(expression, (l, r) => new FloatSubtract(l, r));
        }

        public IExpression FloatToSigned(Bits size)
        {
            return Unary(v => v is IRealValue r
                ? new RealToSigned(size, r)
                : new FloatToSigned(size, v));
        }

        public IExpression FloatToUnsigned(Bits size)
        {
            return Unary(v => new FloatToUnsigned(size, v));
        }

        public IExpression FloatUnordered(IExpression expression)
        {
            return Binary(expression, (l, r) => new FloatUnordered(l, r));
        }

        public IExpression LogicalShiftRight(IExpression expression)
        {
            return Binary(expression, (l, r) => new LogicalShiftRight(l, r));
        }

        public IExpression Multiply(IExpression expression)
        {
            return Binary(expression, (l, r) => new Multiply(l, r));
        }

        public IExpression Not()
        {
            return Unary(v => new Not(v));
        }

        public IExpression NotEqual(IExpression expression)
        {
            return Equal(expression).Not();
        }

        IExpression IExpression.Or(IExpression expression)
        {
            return Or(expression);
        }

        public IExpression Read(IExpression offset, Bits size)
        {
            return _reader.Read(this, offset, size);
        }

        public IExpression Select(IExpression trueValue, IExpression falseValue)
        {
            return Ternary(trueValue, falseValue, (p, t, f) => new Select(p, t, f));
        }

        public IExpression ShiftLeft(IExpression expression)
        {
            return Binary(expression, (l, r) => new ShiftLeft(l, r));
        }

        public IExpression SignedDivide(IExpression expression)
        {
            return Binary(expression, (l, r) => new SignedDivide(l, r));
        }

        public IExpression SignedGreater(IExpression expression)
        {
            return Binary(expression, (l, r) => new SignedGreater(l, r));
        }

        public IExpression SignedGreaterOrEqual(IExpression expression)
        {
            return Binary(expression, (l, r) => new SignedGreaterOrEqual(l, r));
        }

        public IExpression SignedLess(IExpression expression)
        {
            return Binary(expression, (l, r) => new SignedLess(l, r));
        }

        public IExpression SignedLessOrEqual(IExpression expression)
        {
            return Binary(expression, (l, r) => new SignedLessOrEqual(l, r));
        }

        public IExpression SignedRemainder(IExpression expression)
        {
            return Binary(expression, (l, r) => new SignedRemainder(l, r));
        }

        public IExpression SignedToFloat(Bits size)
        {
            return Unary(v => new SignedToFloat(size, v));
        }

        public IExpression SignExtend(Bits size)
        {
            return size > Size
                ? Unary(v => new SignExtend(size, v))
                : this;
        }

        public IExpression Subtract(IExpression expression)
        {
            return Binary(expression, (l, r) => new Subtract(l, r));
        }

        public IExpression Truncate(Bits size)
        {
            return size < Size
                ? Unary(v => new Truncate(size, v))
                : this;
        }

        public IExpression UnsignedDivide(IExpression expression)
        {
            return Binary(expression, (l, r) => new UnsignedDivide(l, r));
        }

        public IExpression UnsignedGreater(IExpression expression)
        {
            return Binary(expression, (l, r) => new UnsignedGreater(l, r));
        }

        public IExpression UnsignedGreaterOrEqual(IExpression expression)
        {
            return Binary(expression, (l, r) => new UnsignedGreaterOrEqual(l, r));
        }

        public IExpression UnsignedLess(IExpression expression)
        {
            return Binary(expression, (l, r) => new UnsignedLess(l, r));
        }

        public IExpression UnsignedLessOrEqual(IExpression expression)
        {
            return Binary(expression, (l, r) => new UnsignedLessOrEqual(l, r));
        }

        public IExpression UnsignedRemainder(IExpression expression)
        {
            return Binary(expression, (l, r) => new UnsignedRemainder(l, r));
        }

        public IExpression UnsignedToFloat(Bits size)
        {
            return Unary(v => new UnsignedToFloat(size, v));
        }

        public IExpression Write(IExpression offset, IExpression value)
        {
            var mask = Mask(offset, value.Size);
            var reader = new WriteReader(new DefaultReader(), this, mask, value);
            var buffer = And(mask.Not()).Or(value.ZeroExtend(Size).ShiftLeft(offset));

            return new SymbolicExpression(buffer._contextFactory, buffer._collectionFactory, reader,
                buffer.Value, buffer.Constraints);
        }

        public IExpression Xor(IExpression expression)
        {
            return Binary(expression, (l, r) => new Xor(l, r));
        }

        public IExpression ZeroExtend(Bits size)
        {
            return size > Size
                ? Unary(v => new ZeroExtend(size, v))
                : this;
        }

        public IExpression Mask(IExpression offset, Bits size)
        {
            var zero = new ConstantExpression(_contextFactory, _collectionFactory,
                ConstantUnsigned.Create(size, BigInteger.Zero));

            return zero.Not().ZeroExtend(Size).ShiftLeft(offset);
        }

        public SymbolicExpression And(IExpression expression)
        {
            return Binary(expression, (l, r) => new And(l, r));
        }

        public SymbolicExpression Or(IExpression expression)
        {
            return Binary(expression, (l, r) => new Or(l, r));
        }

        private SymbolicExpression Unary(Func<IValue, IValue> func)
        {
            return new(_contextFactory, _collectionFactory, new DefaultReader(),
                func(Value), Constraints);
        }

        private SymbolicExpression Binary(IExpression b, Func<IValue, IValue, IValue> func)
        {
            var y = (IValueExpression) b;

            return new SymbolicExpression(_contextFactory, _collectionFactory, new DefaultReader(),
                func(Value, y.Value), Constraints.Concat(y.Constraints).ToArray());
        }

        private SymbolicExpression Ternary(IExpression b, IExpression c, Func<IValue, IValue, IValue, IValue> func)
        {
            var y = (IValueExpression) b;
            var z = (IValueExpression) c;

            return new SymbolicExpression(_contextFactory, _collectionFactory, new DefaultReader(),
                func(Value, y.Value, z.Value), Constraints.Concat(y.Constraints.Concat(z.Constraints)).ToArray());
        }

        private IExpression Evaluate(IPersistentSpace space)
        {
            using var model = space.GetModel(Constraints);

            return new ConstantExpression(_contextFactory, _collectionFactory,
                ConstantUnsigned.Create(Value.Size, model.Evaluate(Value)));
        }

        private BigInteger AsConstant()
        {
            using var handle = _contextFactory.Create();

            return Value is Float && Value.AsFloat(handle.Context).Simplify().IsFPNaN
                ? Value.Size.GetNan(handle.Context)
                : AsConstant(Value.AsBitVector(handle.Context).Simplify());
        }

        private static BigInteger AsConstant(Expr expr)
        {
            return expr.IsNumeral
                ? ((BitVecNum) expr).BigInteger
                : throw new IrreducibleSymbolicExpressionException();
        }

        public static SymbolicExpression Create(IContextFactory contextFactory, ICollectionFactory collectionFactory,
            IValue value, IEnumerable<Func<IExpression, IExpression>> constraints)
        {
            var unconstrained = new SymbolicExpression(contextFactory, collectionFactory, new DefaultReader(),
                value, Array.Empty<IValue>());

            return new SymbolicExpression(contextFactory, collectionFactory, new DefaultReader(),
                value, constraints
                    .Select(c => ((IValueExpression) c(unconstrained)).Value)
                    .ToArray());
        }
    }
}
