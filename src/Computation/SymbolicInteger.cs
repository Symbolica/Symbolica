using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Computation.Exceptions;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    [Serializable]
    internal sealed class SymbolicInteger : ISigned, IUnsigned
    {
        public SymbolicInteger(Bits size, IFunc<Context, BitVecExpr> symbolic)
        {
            Size = size;
            Symbolic = symbolic;
        }

        public Bits Size { get; }

        public BigInteger AsConstant(IContextFactory contextFactory)
        {
            using var handle = contextFactory.Create();
            var expr = Symbolic.Run(handle.Context).Simplify();

            return expr.IsNumeral
                ? ((BitVecNum)expr).BigInteger
                : throw new IrreducibleSymbolicExpressionException();
        }

        public IValue GetValue(IPersistentSpace space, IBool[] constraints)
        {
            using var model = space.GetModel(constraints);

            return ConstantUnsigned.Create(Size, ((BitVecNum)model.Evaluate(Symbolic.Run)).BigInteger);
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
            return new SymbolicBool(
                new ContextFuncs.MkNot(
                    new ContextFuncs.MkEq(Symbolic, new ContextFuncs.MkBVOfUnsigned(0U, (uint)Size))));
        }

        public IFloat AsFloat()
        {
            return new SymbolicFloat(
                Size,
                new ContextFuncs.MkFPToFPOfBitVec(Symbolic, new ContextFuncs.GetSort(Size)));
        }

        public IValue IfElse(IBool predicate, IValue falseValue)
        {
            return Create(
                new ContextFuncs.MkITE<BitVecExpr>(predicate.Symbolic, Symbolic, falseValue.AsUnsigned().Symbolic));
        }

        public IFunc<Context, BitVecExpr> Symbolic { get; }

        public ISigned ArithmeticShiftRight(IUnsigned value)
        {
            return Create(new ContextFuncs.MkBVASHR(Symbolic, value.Symbolic));
        }

        public ISigned Divide(ISigned value)
        {
            return Create(new ContextFuncs.MkBVSDiv(Symbolic, value.Symbolic));
        }

        public IBool Greater(ISigned value)
        {
            return new SymbolicBool(new ContextFuncs.MkBVSGT(Symbolic, value.Symbolic));
        }

        public IBool GreaterOrEqual(ISigned value)
        {
            return new SymbolicBool(new ContextFuncs.MkBVSGE(Symbolic, value.Symbolic));
        }

        public IBool Less(ISigned value)
        {
            return new SymbolicBool(new ContextFuncs.MkBVSLT(Symbolic, value.Symbolic));
        }

        public IBool LessOrEqual(ISigned value)
        {
            return new SymbolicBool(new ContextFuncs.MkBVSLE(Symbolic, value.Symbolic));
        }

        public ISigned Remainder(ISigned value)
        {
            return Create(new ContextFuncs.MkBVSRem(Symbolic, value.Symbolic));
        }

        public IFloat SignedToFloat(Bits size)
        {
            return new SymbolicFloat(
                size,
                new ContextFuncs.MkFPToFPOfFPRMNumBitVec(
                    new ContextFuncs.MkFPRNE(), Symbolic, new ContextFuncs.GetSort(size), true));
        }

        public ISigned SignExtend(Bits size)
        {
            return new SymbolicInteger(size, new ContextFuncs.MkSignExt((uint)size - (uint)Size, Symbolic));
        }

        public IUnsigned Add(IUnsigned value)
        {
            return Create(new ContextFuncs.MkBVAdd(Symbolic, value.Symbolic));
        }

        public IBitwise And(IBitwise value)
        {
            return Create(new ContextFuncs.MkBVAND(Symbolic, value.AsUnsigned().Symbolic));
        }

        public IUnsigned Divide(IUnsigned value)
        {
            return Create(new ContextFuncs.MkBVUDiv(Symbolic, value.Symbolic));
        }

        public IBool Equal(IBitwise value)
        {
            return new SymbolicBool(new ContextFuncs.MkEq(Symbolic, value.AsUnsigned().Symbolic));
        }

        public IBool Greater(IUnsigned value)
        {
            return new SymbolicBool(new ContextFuncs.MkBVUGT(Symbolic, value.Symbolic));
        }

        public IBool GreaterOrEqual(IUnsigned value)
        {
            return new SymbolicBool(new ContextFuncs.MkBVUGE(Symbolic, value.Symbolic));
        }

        public IBool Less(IUnsigned value)
        {
            return new SymbolicBool(new ContextFuncs.MkBVULT(Symbolic, value.Symbolic));
        }

        public IBool LessOrEqual(IUnsigned value)
        {
            return new SymbolicBool(new ContextFuncs.MkBVULE(Symbolic, value.Symbolic));
        }

        public IUnsigned LogicalShiftRight(IUnsigned value)
        {
            return Create(new ContextFuncs.MkBVLSHR(Symbolic, value.Symbolic));
        }

        public IUnsigned Multiply(IUnsigned value)
        {
            return Create(new ContextFuncs.MkBVMul(Symbolic, value.Symbolic));
        }

        public IUnsigned Not()
        {
            return new SymbolicInteger(Size, new ContextFuncs.MkBVNot(Symbolic));
        }

        public IBool NotEqual(IBitwise value)
        {
            return new SymbolicBool(
                new ContextFuncs.MkNot(new ContextFuncs.MkEq(Symbolic, value.AsUnsigned().Symbolic)));
        }

        public IBitwise Or(IBitwise value)
        {
            return Create(new ContextFuncs.MkBVOR(Symbolic, value.AsUnsigned().Symbolic));
        }

        public IUnsigned Remainder(IUnsigned value)
        {
            return Create(new ContextFuncs.MkBVURem(Symbolic, value.Symbolic));
        }

        public IUnsigned ShiftLeft(IUnsigned value)
        {
            return Create(new ContextFuncs.MkBVSHL(Symbolic, value.Symbolic));
        }

        public IUnsigned Subtract(IUnsigned value)
        {
            return Create(new ContextFuncs.MkBVSub(Symbolic, value.Symbolic));
        }

        public IUnsigned Truncate(Bits size)
        {
            return new SymbolicInteger(size, new ContextFuncs.MkExtract((uint)size - 1U, 0U, Symbolic));
        }

        public IFloat UnsignedToFloat(Bits size)
        {
            return new SymbolicFloat(
                size,
                new ContextFuncs.MkFPToFPOfFPRMNumBitVec(
                    new ContextFuncs.MkFPRNE(),
                    Symbolic,
                    new ContextFuncs.GetSort(size),
                    false));
        }

        public IBitwise Xor(IBitwise value)
        {
            return Create(new ContextFuncs.MkBVXOR(Symbolic, value.AsUnsigned().Symbolic));
        }

        public IUnsigned ZeroExtend(Bits size)
        {
            return new SymbolicInteger(size, new ContextFuncs.MkZeroExt((uint)size - (uint)Size, Symbolic));
        }

        private SymbolicInteger Create(IFunc<Context, BitVecExpr> symbolic)
        {
            return new(Size, symbolic);
        }
    }
}
