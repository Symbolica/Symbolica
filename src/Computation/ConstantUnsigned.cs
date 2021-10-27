using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Computation.Exceptions;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class ConstantUnsigned : IUnsigned, IConstantInteger
    {
        private ConstantUnsigned(Bits size, BigInteger constant)
        {
            Size = size;
            Constant = constant;
        }

        public BigInteger Constant { get; }
        public Bits Size { get; }

        public BigInteger AsConstant(Context context)
        {
            return Constant;
        }

        public IValue GetValue(IPersistentSpace space, IBool[] constraints)
        {
            return this;
        }

        public IBitwise AsBitwise()
        {
            return this;
        }

        public IBitVector AsBitVector(ICollectionFactory collectionFactory)
        {
            return ConstantBitVector.Create(collectionFactory, Size, Constant);
        }

        public IUnsigned AsUnsigned()
        {
            return this;
        }

        public ISigned AsSigned()
        {
            return ConstantSigned.Create(Size, Constant);
        }

        public IBool AsBool()
        {
            return new ConstantBool(!Constant.IsZero);
        }

        public IFloat AsFloat()
        {
            return AsSigned().AsFloat();
        }

        public IValue IfElse(IBool predicate, IValue falseValue)
        {
            return new SymbolicInteger(Size, Symbolic).IfElse(predicate, falseValue);
        }

        public Func<Context, BitVecExpr> Symbolic => c => c.MkBV(Constant.ToString(), (uint) Size);

        public IUnsigned Add(IUnsigned value)
        {
            return Create(value, (l, r) => l.Add(r), (l, r) => l + r);
        }

        public IBitwise And(IBitwise value)
        {
            return Constant.IsZero
                ? this
                : Create(value.AsUnsigned(), (l, r) => l.And(r), (l, r) => l & r);
        }

        public IUnsigned Divide(IUnsigned value)
        {
            return Create(value, (l, r) => l.Divide(r), (l, r) => l / r);
        }

        public IBool Equal(IBitwise value)
        {
            return Create(value.AsUnsigned(), (l, r) => l.Equal(r), (l, r) => l == r);
        }

        public IBool Greater(IUnsigned value)
        {
            return Create(value, (l, r) => l.Greater(r), (l, r) => l > r);
        }

        public IBool GreaterOrEqual(IUnsigned value)
        {
            return Create(value, (l, r) => l.GreaterOrEqual(r), (l, r) => l >= r);
        }

        public IBool Less(IUnsigned value)
        {
            return Create(value, (l, r) => l.Less(r), (l, r) => l < r);
        }

        public IBool LessOrEqual(IUnsigned value)
        {
            return Create(value, (l, r) => l.LessOrEqual(r), (l, r) => l <= r);
        }

        public IUnsigned LogicalShiftRight(IUnsigned value)
        {
            return Create(value, (l, r) => l.LogicalShiftRight(r), (l, r) => l >> (int) r);
        }

        public IUnsigned Multiply(IUnsigned value)
        {
            return Create(value, (l, r) => l.Multiply(r), (l, r) => l * r);
        }

        public IUnsigned Not()
        {
            return Create(Size, ~Constant);
        }

        public IBool NotEqual(IBitwise value)
        {
            return Create(value.AsUnsigned(), (l, r) => l.NotEqual(r), (l, r) => l != r);
        }

        public IBitwise Or(IBitwise value)
        {
            return Constant.IsZero
                ? value
                : Create(value.AsUnsigned(), (l, r) => l.Or(r), (l, r) => l | r);
        }

        public IUnsigned Remainder(IUnsigned value)
        {
            return Create(value, (l, r) => l.Remainder(r), (l, r) => l % r);
        }

        public IUnsigned ShiftLeft(IUnsigned value)
        {
            return Create(value, (l, r) => l.ShiftLeft(r), (l, r) => l << (int) r);
        }

        public IUnsigned Subtract(IUnsigned value)
        {
            return Create(value, (l, r) => l.Subtract(r), (l, r) => l - r);
        }

        public IUnsigned Truncate(Bits size)
        {
            return Create(size, Constant);
        }

        public IFloat UnsignedToFloat(Bits size)
        {
            return (uint) size switch
            {
                32U => new ConstantSingle((float) Constant),
                64U => new ConstantDouble((double) Constant),
                _ => new SymbolicInteger(Size, Symbolic).UnsignedToFloat(size)
            };
        }

        public IBitwise Xor(IBitwise value)
        {
            return Create(value.AsUnsigned(), (l, r) => l.Xor(r), (l, r) => l ^ r);
        }

        public IUnsigned ZeroExtend(Bits size)
        {
            return new ConstantUnsigned(size, Constant);
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

        private IUnsigned Create(IUnsigned other,
            Func<SymbolicInteger, IUnsigned, IBitwise> symbolic,
            Func<BigInteger, BigInteger, BigInteger> constant)
        {
            return Create(other, (l, r) => symbolic(l, r).AsUnsigned(), (l, r) => Create(Size, constant(l, r)));
        }

        private IBool Create(IUnsigned other,
            Func<SymbolicInteger, IUnsigned, IBool> symbolic,
            Func<BigInteger, BigInteger, bool> constant)
        {
            return Create(other, symbolic, (l, r) => new ConstantBool(constant(l, r)));
        }

        private TValue Create<TValue>(IUnsigned other,
            Func<SymbolicInteger, IUnsigned, TValue> symbolic,
            Func<BigInteger, BigInteger, TValue> constant)
        {
            return Size == other.Size
                ? other is IConstantInteger c
                    ? constant(Constant, c.Constant)
                    : symbolic(new SymbolicInteger(Size, Symbolic), other)
                : throw new InconsistentExpressionSizesException(Size, other.Size);
        }
    }
}
