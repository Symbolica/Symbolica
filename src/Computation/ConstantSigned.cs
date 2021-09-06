using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Computation.Exceptions;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class ConstantSigned : ConstantInteger
    {
        private readonly BigInteger _value;

        private ConstantSigned(Bits size, BigInteger value)
            : base(size)
        {
            _value = value;
        }

        public override BigInteger Integer => ToConstantUnsigned().Integer;

        public override IProposition GetProposition(IPersistentSpace space, SymbolicBool[] constraints)
        {
            return ToConstantBool().GetProposition(space, constraints);
        }

        public override IValue Add(IValue value)
        {
            return ToConstantUnsigned().Add(value);
        }

        public override IValue And(IValue value)
        {
            return ToConstantUnsigned().And(value);
        }

        public override IValue ArithmeticShiftRight(IValue value)
        {
            return Create(value, (l, r) => l.ArithmeticShiftRight(r), (l, r) => l >> (int) r,
                e => e.ToConstantUnsigned().Integer, c => Create(Size, c));
        }

        public override IValue Equal(IValue value)
        {
            return ToConstantUnsigned().Equal(value);
        }

        public override IValue LogicalShiftRight(IValue value)
        {
            return ToConstantUnsigned().LogicalShiftRight(value);
        }

        public override IValue Multiply(IValue value)
        {
            return ToConstantUnsigned().Multiply(value);
        }

        public override IValue Not()
        {
            return ToConstantUnsigned().Not();
        }

        public override IValue NotEqual(IValue value)
        {
            return ToConstantUnsigned().NotEqual(value);
        }

        public override IValue Or(IValue value)
        {
            return ToConstantUnsigned().Or(value);
        }

        public override IValue Read(ICollectionFactory collectionFactory, IValue offset, Bits size)
        {
            return ToConstantBitVector(collectionFactory).Read(collectionFactory, offset, size);
        }

        public override IValue Select(IValue trueValue, IValue falseValue)
        {
            return ToConstantBool().Select(trueValue, falseValue);
        }

        public override IValue ShiftLeft(IValue value)
        {
            return ToConstantUnsigned().ShiftLeft(value);
        }

        public override IValue SignedDivide(IValue value)
        {
            return Create(value, (l, r) => l.SignedDivide(r), (l, r) => l / r);
        }

        public override IValue SignedGreater(IValue value)
        {
            return Create(value, (l, r) => l.SignedGreater(r), (l, r) => l > r);
        }

        public override IValue SignedGreaterOrEqual(IValue value)
        {
            return Create(value, (l, r) => l.SignedGreaterOrEqual(r), (l, r) => l >= r);
        }

        public override IValue SignedLess(IValue value)
        {
            return Create(value, (l, r) => l.SignedLess(r), (l, r) => l < r);
        }

        public override IValue SignedLessOrEqual(IValue value)
        {
            return Create(value, (l, r) => l.SignedLessOrEqual(r), (l, r) => l <= r);
        }

        public override IValue SignedRemainder(IValue value)
        {
            return Create(value, (l, r) => l.SignedRemainder(r), (l, r) => l % r);
        }

        public override IValue SignedToFloat(Bits size)
        {
            return ConstantFloat.Create(size, _value)
                   ?? ToSymbolicBitVector().SignedToFloat(size);
        }

        public override IValue SignExtend(Bits size)
        {
            return new ConstantSigned(size, _value);
        }

        public override IValue Subtract(IValue value)
        {
            return ToConstantUnsigned().Subtract(value);
        }

        public override IValue Truncate(Bits size)
        {
            return ToConstantUnsigned().Truncate(size);
        }

        public override IValue UnsignedDivide(IValue value)
        {
            return ToConstantUnsigned().UnsignedDivide(value);
        }

        public override IValue UnsignedGreater(IValue value)
        {
            return ToConstantUnsigned().UnsignedGreater(value);
        }

        public override IValue UnsignedGreaterOrEqual(IValue value)
        {
            return ToConstantUnsigned().UnsignedGreaterOrEqual(value);
        }

        public override IValue UnsignedLess(IValue value)
        {
            return ToConstantUnsigned().UnsignedLess(value);
        }

        public override IValue UnsignedLessOrEqual(IValue value)
        {
            return ToConstantUnsigned().UnsignedLessOrEqual(value);
        }

        public override IValue UnsignedRemainder(IValue value)
        {
            return ToConstantUnsigned().UnsignedRemainder(value);
        }

        public override IValue UnsignedToFloat(Bits size)
        {
            return ToConstantUnsigned().UnsignedToFloat(size);
        }

        public override IValue Write(ICollectionFactory collectionFactory, IValue offset, IValue value)
        {
            return ToConstantBitVector(collectionFactory).Write(collectionFactory, offset, value);
        }

        public override IValue Xor(IValue value)
        {
            return ToConstantUnsigned().Xor(value);
        }

        public override IValue ZeroExtend(Bits size)
        {
            return ToConstantUnsigned().ZeroExtend(size);
        }

        public override IValue IfElse(Func<Context, BoolExpr> predicate, IValue falseValue)
        {
            return ToSymbolicBitVector().IfElse(predicate, falseValue);
        }

        public override SymbolicBitVector ToSymbolicBitVector()
        {
            return ToConstantUnsigned().ToSymbolicBitVector();
        }

        public override SymbolicBool ToSymbolicBool()
        {
            return ToConstantUnsigned().ToSymbolicBool();
        }

        public override ConstantBitVector ToConstantBitVector(ICollectionFactory collectionFactory)
        {
            return ToConstantUnsigned().ToConstantBitVector(collectionFactory);
        }

        public override ConstantUnsigned ToConstantUnsigned()
        {
            return ConstantUnsigned.Create(Size, _value);
        }

        public override ConstantSigned ToConstantSigned()
        {
            return this;
        }

        public override ConstantBool ToConstantBool()
        {
            return ToConstantUnsigned().ToConstantBool();
        }

        public override IValue ToConstantFloat()
        {
            return ConstantFloat.CreateFromBits(Size, _value)
                   ?? ToSymbolicFloat();
        }

        public static ConstantSigned Create(Bits size, BigInteger value)
        {
            return new(size, value.IsZero
                ? value
                : Normalize(size, value));
        }

        private static BigInteger Normalize(Bits size, BigInteger value)
        {
            var msb = BigInteger.One << ((int) (uint) size - 1);
            return (value & (msb - BigInteger.One)) - (value & msb);
        }

        private IValue Create(IValue other,
            Func<SymbolicBitVector, IValue, IValue> symbolic,
            Func<BigInteger, BigInteger, BigInteger> func)
        {
            return Create(other, symbolic, func, e => e.ToConstantSigned()._value, c => Create(Size, c));
        }

        private IValue Create(IValue other,
            Func<SymbolicBitVector, IValue, IValue> symbolic,
            Func<BigInteger, BigInteger, bool> func)
        {
            return Create(other, symbolic, func, e => e.ToConstantSigned()._value, c => new ConstantBool(c));
        }

        private IValue Create<TConstant>(IValue other,
            Func<SymbolicBitVector, IValue, IValue> symbolic,
            Func<BigInteger, BigInteger, TConstant> func,
            Func<IConstantValue, BigInteger> conversion,
            Func<TConstant, IValue> constructor)
        {
            return Size == other.Size
                ? other is IConstantValue c
                    ? constructor(func(_value, conversion(c)))
                    : symbolic(ToSymbolicBitVector(), other)
                : throw new InconsistentExpressionSizesException(Size, other.Size);
        }
    }
}
