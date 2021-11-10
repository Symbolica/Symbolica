using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Computation.Exceptions;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class ConstantSigned : ISigned, IConstantInteger
    {
        private ConstantSigned(Bits size, BigInteger constant)
        {
            Size = size;
            Constant = constant;
        }

        public BigInteger Constant { get; }
        public Bits Size { get; }

        public BigInteger AsConstant(IContextFactory contextFactory)
        {
            return AsUnsigned().AsConstant(contextFactory);
        }

        public IValue GetValue(IPersistentSpace space, IBool[] constraints)
        {
            return this;
        }

        public IBitwise AsBitwise()
        {
            return AsUnsigned();
        }

        public IBitVector AsBitVector(ICollectionFactory collectionFactory)
        {
            return AsUnsigned().AsBitVector(collectionFactory);
        }

        public IUnsigned AsUnsigned()
        {
            return ConstantUnsigned.Create(Size, Constant);
        }

        public ISigned AsSigned()
        {
            return this;
        }

        public IBool AsBool()
        {
            return AsUnsigned().AsBool();
        }

        public IFloat AsFloat()
        {
            return (uint)Size switch
            {
                32U => new ConstantSingle(BitConverter.Int32BitsToSingle((int)Constant)),
                64U => new ConstantDouble(BitConverter.Int64BitsToDouble((long)Constant)),
                _ => new SymbolicInteger(Size, Symbolic).AsFloat()
            };
        }

        public IValue IfElse(IBool predicate, IValue falseValue)
        {
            return new SymbolicInteger(Size, Symbolic).IfElse(predicate, falseValue);
        }

        public IFunc<Context, BitVecExpr> Symbolic => AsUnsigned().Symbolic;

        public ISigned ArithmeticShiftRight(IUnsigned value)
        {
            return Create(value, (l, r) => l.ArithmeticShiftRight(r), (l, r) => l >> (int)r);
        }

        public ISigned Divide(ISigned value)
        {
            return Create(value, (l, r) => l.Divide(r), (l, r) => l / r);
        }

        public IBool Greater(ISigned value)
        {
            return Create(value, (l, r) => l.Greater(r), (l, r) => l > r);
        }

        public IBool GreaterOrEqual(ISigned value)
        {
            return Create(value, (l, r) => l.GreaterOrEqual(r), (l, r) => l >= r);
        }

        public IBool Less(ISigned value)
        {
            return Create(value, (l, r) => l.Less(r), (l, r) => l < r);
        }

        public IBool LessOrEqual(ISigned value)
        {
            return Create(value, (l, r) => l.LessOrEqual(r), (l, r) => l <= r);
        }

        public ISigned Remainder(ISigned value)
        {
            return Create(value, (l, r) => l.Remainder(r), (l, r) => l % r);
        }

        public IFloat SignedToFloat(Bits size)
        {
            return (uint)size switch
            {
                32U => new ConstantSingle((float)Constant),
                64U => new ConstantDouble((double)Constant),
                _ => new SymbolicInteger(Size, Symbolic).SignedToFloat(size)
            };
        }

        public ISigned SignExtend(Bits size)
        {
            return new ConstantSigned(size, Constant);
        }

        public static ConstantSigned Create(Bits size, BigInteger value)
        {
            return new(size, value.IsZero
                ? value
                : Normalize(size, value));
        }

        private static BigInteger Normalize(Bits size, BigInteger value)
        {
            var msb = BigInteger.One << ((int)(uint)size - 1);
            return (value & (msb - BigInteger.One)) - (value & msb);
        }

        private ISigned Create(IUnsigned other,
            Func<SymbolicInteger, IUnsigned, ISigned> symbolic,
            Func<BigInteger, BigInteger, BigInteger> constant)
        {
            return Create(other, symbolic, (l, r) => Create(Size, constant(l, r)));
        }

        private ISigned Create(ISigned other,
            Func<SymbolicInteger, ISigned, ISigned> symbolic,
            Func<BigInteger, BigInteger, BigInteger> constant)
        {
            return Create(other, symbolic, (l, r) => Create(Size, constant(l, r)));
        }

        private IBool Create(ISigned other,
            Func<SymbolicInteger, ISigned, IBool> symbolic,
            Func<BigInteger, BigInteger, bool> constant)
        {
            return Create(other, symbolic, (l, r) => new ConstantBool(constant(l, r)));
        }

        private TValue Create<TOther, TValue>(TOther other,
            Func<SymbolicInteger, TOther, TValue> symbolic,
            Func<BigInteger, BigInteger, TValue> constant)
            where TOther : IValue
        {
            return Size == other.Size
                ? other is IConstantInteger c
                    ? constant(Constant, c.Constant)
                    : symbolic(new SymbolicInteger(Size, Symbolic), other)
                : throw new InconsistentExpressionSizesException(Size, other.Size);
        }
    }
}
