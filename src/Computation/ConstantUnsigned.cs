using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class ConstantUnsigned : ConstantInteger
    {
        private ConstantUnsigned(Bits size, BigInteger value)
            : base(size)
        {
            Integer = value;
        }

        public override BigInteger Integer { get; }

        public override IProposition GetProposition(IPersistentSpace space, SymbolicBool[] constraints)
        {
            return ToConstantBool().GetProposition(space, constraints);
        }

        public override IValue Add(IValue value)
        {
            return Create(value, (l, r) => l.Add(r), (l, r) => l + r);
        }

        public override IValue And(IValue value)
        {
            return Create(value, (l, r) => l.And(r), (l, r) => l & r);
        }

        public override IValue ArithmeticShiftRight(IValue value)
        {
            return ToConstantSigned().ArithmeticShiftRight(value);
        }

        public override IValue Equal(IValue value)
        {
            return Create(value, (l, r) => l.Equal(r), (l, r) => l == r);
        }

        public override IValue LogicalShiftRight(IValue value)
        {
            return Create(value, (l, r) => l.LogicalShiftRight(r), (l, r) => l >> (int) r);
        }

        public override IValue Multiply(IValue value)
        {
            return Create(value, (l, r) => l.Multiply(r), (l, r) => l * r);
        }

        public override IValue Not()
        {
            return Create(Size, ~Integer);
        }

        public override IValue NotEqual(IValue value)
        {
            return Create(value, (l, r) => l.NotEqual(r), (l, r) => l != r);
        }

        public override IValue Or(IValue value)
        {
            return Create(value, (l, r) => l.Or(r), (l, r) => l | r);
        }

        public override IValue Read(ICollectionFactory collectionFactory, IValue offset, Bits size)
        {
            return ToConstantBitVector(collectionFactory).Read(collectionFactory, offset, size);
        }

        public override IValue Select(IValue trueValue, IValue falseValue)
        {
            return ToConstantBool().Select(trueValue, falseValue);
        }

        public override IValue Select(Func<Context, BoolExpr> predicate, IValue falseValue)
        {
            return ToSymbolicBitVector().Select(predicate, falseValue);
        }

        public override IValue ShiftLeft(IValue value)
        {
            return Create(value, (l, r) => l.ShiftLeft(r), (l, r) => l << (int) r);
        }

        public override IValue SignedDivide(IValue value)
        {
            return ToConstantSigned().SignedDivide(value);
        }

        public override IValue SignedGreater(IValue value)
        {
            return ToConstantSigned().SignedGreater(value);
        }

        public override IValue SignedGreaterOrEqual(IValue value)
        {
            return ToConstantSigned().SignedGreaterOrEqual(value);
        }

        public override IValue SignedLess(IValue value)
        {
            return ToConstantSigned().SignedLess(value);
        }

        public override IValue SignedLessOrEqual(IValue value)
        {
            return ToConstantSigned().SignedLessOrEqual(value);
        }

        public override IValue SignedRemainder(IValue value)
        {
            return ToConstantSigned().SignedRemainder(value);
        }

        public override IValue SignedToFloat(Bits size)
        {
            return ToConstantSigned().SignedToFloat(size);
        }

        public override IValue SignExtend(Bits size)
        {
            return ToConstantSigned().SignExtend(size);
        }

        public override IValue Subtract(IValue value)
        {
            return Create(value, (l, r) => l.Subtract(r), (l, r) => l - r);
        }

        public override IValue Truncate(Bits size)
        {
            return Create(size, Integer);
        }

        public override IValue UnsignedDivide(IValue value)
        {
            return Create(value, (l, r) => l.UnsignedDivide(r), (l, r) => l / r);
        }

        public override IValue UnsignedGreater(IValue value)
        {
            return Create(value, (l, r) => l.UnsignedGreater(r), (l, r) => l > r);
        }

        public override IValue UnsignedGreaterOrEqual(IValue value)
        {
            return Create(value, (l, r) => l.UnsignedGreaterOrEqual(r), (l, r) => l >= r);
        }

        public override IValue UnsignedLess(IValue value)
        {
            return Create(value, (l, r) => l.UnsignedLess(r), (l, r) => l < r);
        }

        public override IValue UnsignedLessOrEqual(IValue value)
        {
            return Create(value, (l, r) => l.UnsignedLessOrEqual(r), (l, r) => l <= r);
        }

        public override IValue UnsignedRemainder(IValue value)
        {
            return Create(value, (l, r) => l.UnsignedRemainder(r), (l, r) => l % r);
        }

        public override IValue UnsignedToFloat(Bits size)
        {
            return ConstantFloat.Create(size, Integer)
                   ?? ToSymbolicBitVector().UnsignedToFloat(size);
        }

        public override IValue Write(ICollectionFactory collectionFactory, IValue offset, IValue value)
        {
            return ToConstantBitVector(collectionFactory).Write(collectionFactory, offset, value);
        }

        public override IValue Xor(IValue value)
        {
            return Create(value, (l, r) => l.Xor(r), (l, r) => l ^ r);
        }

        public override IValue ZeroExtend(Bits size)
        {
            return new ConstantUnsigned(size, Integer);
        }

        public override SymbolicBitVector ToSymbolicBitVector()
        {
            return new(Size, c => c.MkBV(Integer.ToString(), (uint) Size));
        }

        public override SymbolicBool ToSymbolicBool()
        {
            return new(c => c.MkBool(!Integer.IsZero));
        }

        public override ConstantBitVector ToConstantBitVector(ICollectionFactory collectionFactory)
        {
            return ConstantBitVector.Create(collectionFactory, Size, Integer);
        }

        public override ConstantUnsigned ToConstantUnsigned()
        {
            return this;
        }

        public override ConstantSigned ToConstantSigned()
        {
            return ConstantSigned.Create(Size, Integer);
        }

        public override ConstantBool ToConstantBool()
        {
            return new(!Integer.IsZero);
        }

        public override IValue ToConstantFloat()
        {
            return ToConstantSigned().ToConstantFloat();
        }

        public static ConstantUnsigned Create(Bits size, BigInteger value)
        {
            return new(size, value.IsZero || value.Sign > 0 && value.GetBitLength() <= (uint) size
                ? value
                : Normalize(size, value));
        }

        private static BigInteger Normalize(Bits size, BigInteger value)
        {
            return value & ((BigInteger.One << (int) (uint) size) - BigInteger.One);
        }

        private IValue Create(IValue other,
            Func<SymbolicBitVector, IValue, IValue> symbolic,
            Func<BigInteger, BigInteger, BigInteger> func)
        {
            return Create(other, symbolic, func, c => Create(Size, c));
        }

        private IValue Create(IValue other,
            Func<SymbolicBitVector, IValue, IValue> symbolic,
            Func<BigInteger, BigInteger, bool> func)
        {
            return Create(other, symbolic, func, c => new ConstantBool(c));
        }

        private IValue Create<TConstant>(IValue other,
            Func<SymbolicBitVector, IValue, IValue> symbolic,
            Func<BigInteger, BigInteger, TConstant> func,
            Func<TConstant, IValue> constructor)
        {
            return Size == other.Size
                ? other is IConstantValue c
                    ? constructor(func(Integer, c.ToConstantUnsigned().Integer))
                    : symbolic(ToSymbolicBitVector(), other)
                : throw new Exception("Expression sizes are different.");
        }
    }
}