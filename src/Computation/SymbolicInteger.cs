using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Computation.Exceptions;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class SymbolicInteger : ISigned, IUnsigned
    {
        public SymbolicInteger(Bits size, Func<Context, BitVecExpr> symbolic)
        {
            Size = size;
            Symbolic = symbolic;
        }

        public Bits Size { get; }

        public BigInteger AsConstant(IContextFactory contextFactory)
        {
            using var handle = contextFactory.Create();
            var expr = Symbolic(handle.Context).Simplify();

            return expr.IsNumeral
                ? ((BitVecNum) expr).BigInteger
                : throw new IrreducibleSymbolicExpressionException();
        }

        public IValue GetValue(IPersistentSpace space, IBool[] constraints)
        {
            using var model = space.GetModel(constraints);

            return ConstantUnsigned.Create(Size, ((BitVecNum) model.Evaluate(Symbolic)).BigInteger);
        }

        public IBitwise AsBitwise()
        {
            return this;
        }

        public IBitVector AsBitVector(ICollectionFactory collectionFactory)
        {
            return SymbolicBitVector.Create(this);
        }

        public IUnsigned AsUnsigned()
        {
            return this;
        }

        public ISigned AsSigned()
        {
            return this;
        }

        public IBool AsBool()
        {
            return new SymbolicBool(c => c.MkNot(c.MkEq(Symbolic(c), c.MkBV(0U, (uint) Size))));
        }

        public IFloat AsFloat()
        {
            return new SymbolicFloat(Size, c => c.MkFPToFP(Symbolic(c), Size.GetSort(c)));
        }

        public IValue IfElse(IBool predicate, IValue falseValue)
        {
            return Create(falseValue.AsUnsigned(), (c, t, f) => (BitVecExpr) c.MkITE(predicate.Symbolic(c), t, f));
        }

        public Func<Context, BitVecExpr> Symbolic { get; }

        public ISigned ArithmeticShiftRight(IUnsigned value)
        {
            return Create(value, (c, l, r) => c.MkBVASHR(l, r));
        }

        public ISigned Divide(ISigned value)
        {
            return Create(value, (c, l, r) => c.MkBVSDiv(l, r));
        }

        public IBool Greater(ISigned value)
        {
            return Create(value, (c, l, r) => c.MkBVSGT(l, r));
        }

        public IBool GreaterOrEqual(ISigned value)
        {
            return Create(value, (c, l, r) => c.MkBVSGE(l, r));
        }

        public IBool Less(ISigned value)
        {
            return Create(value, (c, l, r) => c.MkBVSLT(l, r));
        }

        public IBool LessOrEqual(ISigned value)
        {
            return Create(value, (c, l, r) => c.MkBVSLE(l, r));
        }

        public ISigned Remainder(ISigned value)
        {
            return Create(value, (c, l, r) => c.MkBVSRem(l, r));
        }

        public IFloat SignedToFloat(Bits size)
        {
            return new SymbolicFloat(size, c => c.MkFPToFP(c.MkFPRNE(), Symbolic(c), size.GetSort(c), true));
        }

        public ISigned SignExtend(Bits size)
        {
            return new SymbolicInteger(size, c => c.MkSignExt((uint) size - (uint) Size, Symbolic(c)));
        }

        public IUnsigned Add(IUnsigned value)
        {
            return Create(value, (c, l, r) => c.MkBVAdd(l, r));
        }

        public IBitwise And(IBitwise value)
        {
            return Create(value.AsUnsigned(), (c, l, r) => c.MkBVAND(l, r));
        }

        public IUnsigned Divide(IUnsigned value)
        {
            return Create(value, (c, l, r) => c.MkBVUDiv(l, r));
        }

        public IBool Equal(IBitwise value)
        {
            return Create(value.AsUnsigned(), (c, l, r) => c.MkEq(l, r));
        }

        public IBool Greater(IUnsigned value)
        {
            return Create(value, (c, l, r) => c.MkBVUGT(l, r));
        }

        public IBool GreaterOrEqual(IUnsigned value)
        {
            return Create(value, (c, l, r) => c.MkBVUGE(l, r));
        }

        public IBool Less(IUnsigned value)
        {
            return Create(value, (c, l, r) => c.MkBVULT(l, r));
        }

        public IBool LessOrEqual(IUnsigned value)
        {
            return Create(value, (c, l, r) => c.MkBVULE(l, r));
        }

        public IUnsigned LogicalShiftRight(IUnsigned value)
        {
            return Create(value, (c, l, r) => c.MkBVLSHR(l, r));
        }

        public IUnsigned Multiply(IUnsigned value)
        {
            return Create(value, (c, l, r) => c.MkBVMul(l, r));
        }

        public IUnsigned Not()
        {
            return new SymbolicInteger(Size, c => c.MkBVNot(Symbolic(c)));
        }

        public IBool NotEqual(IBitwise value)
        {
            return Create(value.AsUnsigned(), (c, l, r) => c.MkNot(c.MkEq(l, r)));
        }

        public IBitwise Or(IBitwise value)
        {
            return Create(value.AsUnsigned(), (c, l, r) => c.MkBVOR(l, r));
        }

        public IUnsigned Remainder(IUnsigned value)
        {
            return Create(value, (c, l, r) => c.MkBVURem(l, r));
        }

        public IUnsigned ShiftLeft(IUnsigned value)
        {
            return Create(value, (c, l, r) => c.MkBVSHL(l, r));
        }

        public IUnsigned Subtract(IUnsigned value)
        {
            return Create(value, (c, l, r) => c.MkBVSub(l, r));
        }

        public IUnsigned Truncate(Bits size)
        {
            return new SymbolicInteger(size, c => c.MkExtract((uint) size - 1U, 0U, Symbolic(c)));
        }

        public IFloat UnsignedToFloat(Bits size)
        {
            return new SymbolicFloat(size, c => c.MkFPToFP(c.MkFPRNE(), Symbolic(c), size.GetSort(c), false));
        }

        public IBitwise Xor(IBitwise value)
        {
            return Create(value.AsUnsigned(), (c, l, r) => c.MkBVXOR(l, r));
        }

        public IUnsigned ZeroExtend(Bits size)
        {
            return new SymbolicInteger(size, c => c.MkZeroExt((uint) size - (uint) Size, Symbolic(c)));
        }

        private SymbolicInteger Create(IUnsigned other, Func<Context, BitVecExpr, BitVecExpr, BitVecExpr> symbolic)
        {
            return new(Size, c => symbolic(c, Symbolic(c), other.Symbolic(c)));
        }

        private SymbolicInteger Create(ISigned other, Func<Context, BitVecExpr, BitVecExpr, BitVecExpr> symbolic)
        {
            return new(Size, c => symbolic(c, Symbolic(c), other.Symbolic(c)));
        }

        private SymbolicBool Create(IUnsigned other, Func<Context, BitVecExpr, BitVecExpr, BoolExpr> symbolic)
        {
            return new(c => symbolic(c, Symbolic(c), other.Symbolic(c)));
        }

        private SymbolicBool Create(ISigned other, Func<Context, BitVecExpr, BitVecExpr, BoolExpr> symbolic)
        {
            return new(c => symbolic(c, Symbolic(c), other.Symbolic(c)));
        }
    }
}
