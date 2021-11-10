using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class SymbolicFloat : IFloat
    {
        public SymbolicFloat(Bits size, IFunc<Context, FPExpr> symbolic)
        {
            Size = size;
            Symbolic = symbolic;
        }

        public Bits Size { get; }

        public BigInteger AsConstant(IContextFactory contextFactory)
        {
            using var handle = contextFactory.Create();
            var expr = Symbolic.Run(handle.Context).Simplify();

            return expr.IsFPNaN
                ? Size.GetNan(handle.Context)
                : AsUnsigned().AsConstant(contextFactory);
        }

        public IValue GetValue(IPersistentSpace space, IBool[] constraints)
        {
            return AsUnsigned().GetValue(space, constraints);
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
            return new SymbolicInteger(Size, new ContextFuncs.MkFPToIEEEBV(Symbolic));
        }

        public ISigned AsSigned()
        {
            return AsUnsigned().AsSigned();
        }

        public IBool AsBool()
        {
            return AsUnsigned().AsBool();
        }

        public IFloat AsFloat()
        {
            return this;
        }

        public IValue IfElse(IBool predicate, IValue falseValue)
        {
            return Create(new ContextFuncs.MkITE<FPExpr>(predicate.Symbolic, Symbolic, falseValue.AsFloat().Symbolic));
        }

        public IFunc<Context, FPExpr> Symbolic { get; }

        public IFloat Add(IFloat value)
        {
            return Create(new ContextFuncs.MkFPAdd(new ContextFuncs.MkFPRNE(), Symbolic, value.Symbolic));
        }

        public IFloat Ceiling()
        {
            return new SymbolicFloat(Size, new ContextFuncs.MkFPRoundToIntegral(new ContextFuncs.MkFPRTP(), Symbolic));
        }

        public IFloat Convert(Bits size)
        {
            return new SymbolicFloat(
                size,
                new ContextFuncs.MkFPToFPOfFP(new ContextFuncs.MkFPRTN(), Symbolic, new ContextFuncs.GetSort(size)));
        }

        public IFloat Divide(IFloat value)
        {
            return Create(new ContextFuncs.MkFPDiv(new ContextFuncs.MkFPRNE(), Symbolic, value.Symbolic));
        }

        public IBool Equal(IFloat value)
        {
            return Create(new ContextFuncs.MkFPEq(Symbolic, value.Symbolic));
        }

        public IFloat Floor()
        {
            return new SymbolicFloat(Size, new ContextFuncs.MkFPRoundToIntegral(new ContextFuncs.MkFPRTN(), Symbolic));
        }

        public IBool Greater(IFloat value)
        {
            return Create(new ContextFuncs.MkFPGt(Symbolic, value.Symbolic));
        }

        public IBool GreaterOrEqual(IFloat value)
        {
            return Create(new ContextFuncs.MkFPGEq(Symbolic, value.Symbolic));
        }

        public IBool Less(IFloat value)
        {
            return Create(new ContextFuncs.MkFPLt(Symbolic, value.Symbolic));
        }

        public IBool LessOrEqual(IFloat value)
        {
            return Create(new ContextFuncs.MkFPLEq(Symbolic, value.Symbolic));
        }

        public IFloat Multiply(IFloat value)
        {
            return Create(new ContextFuncs.MkFPMul(new ContextFuncs.MkFPRNE(), Symbolic, value.Symbolic));
        }

        public IFloat Negate()
        {
            return new SymbolicFloat(Size, new ContextFuncs.MkFPNeg(Symbolic));
        }

        public IBool NotEqual(IFloat value)
        {
            return Create(new ContextFuncs.MkNot(new ContextFuncs.MkFPEq(Symbolic, value.Symbolic)));
        }

        public IBool Ordered(IFloat value)
        {
            return Create(
                new ContextFuncs.MkNot(
                    new ContextFuncs.MkOr(
                        new ContextFuncs.MkFPIsNaN(Symbolic), new ContextFuncs.MkFPIsNaN(value.Symbolic))));
        }

        public IFloat Power(IFloat value)
        {
            return Create(
                new ContextFuncs.MkPower<RealExpr>(
                    new ContextFuncs.MkFPToReal(Symbolic), new ContextFuncs.MkFPToReal(value.Symbolic)));
        }

        public IFloat Remainder(IFloat value)
        {
            return Create(new ContextFuncs.MkFPRem(Symbolic, value.Symbolic));
        }

        public IFloat Subtract(IFloat value)
        {
            return Create(new ContextFuncs.MkFPSub(new ContextFuncs.MkFPRNE(), Symbolic, value.Symbolic));
        }

        public ISigned ToSigned(Bits size)
        {
            return new SymbolicInteger(
                size,
                new ContextFuncs.MkFPToBV(new ContextFuncs.MkFPRTZ(), Symbolic, (uint)size, true));
        }

        public IUnsigned ToUnsigned(Bits size)
        {
            return new SymbolicInteger(
                size,
                new ContextFuncs.MkFPToBV(new ContextFuncs.MkFPRTZ(), Symbolic, (uint)size, false));
        }

        public IBool Unordered(IFloat value)
        {
            return Create(new ContextFuncs.MkOr(new ContextFuncs.MkFPIsNaN(Symbolic), new ContextFuncs.MkFPIsNaN(value.Symbolic)));
        }

        private SymbolicBool Create(IFunc<Context, BoolExpr> symbolic)
        {
            return new(symbolic);
        }

        private SymbolicFloat Create(IFunc<Context, FPExpr> symbolic)
        {
            return new(Size, symbolic);
        }

        private SymbolicReal Create(IFunc<Context, RealExpr> symbolic)
        {
            return new(Size, symbolic);
        }
    }
}
