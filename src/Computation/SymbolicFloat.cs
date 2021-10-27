using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class SymbolicFloat : IFloat
    {
        public SymbolicFloat(Bits size, Func<Context, FPExpr> symbolic)
        {
            Size = size;
            Symbolic = symbolic;
        }

        public Bits Size { get; }

        public BigInteger AsConstant(Context context)
        {
            var expr = Symbolic(context).Simplify();

            return expr.IsFPNaN
                ? Size.GetNan(context)
                : AsUnsigned().AsConstant(context);
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
            return new SymbolicInteger(Size, c => c.MkFPToIEEEBV(Symbolic(c)));
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
            return Create(falseValue.AsFloat(), (c, t, f) => (FPExpr) c.MkITE(predicate.Symbolic(c), t, f));
        }

        public Func<Context, FPExpr> Symbolic { get; }

        public IFloat Add(IFloat value)
        {
            return Create(value, (c, l, r) => c.MkFPAdd(c.MkFPRNE(), l, r));
        }

        public IFloat Ceiling()
        {
            return new SymbolicFloat(Size, c => c.MkFPRoundToIntegral(c.MkFPRTP(), Symbolic(c)));
        }

        public IFloat Convert(Bits size)
        {
            return new SymbolicFloat(size, c => c.MkFPToFP(c.MkFPRNE(), Symbolic(c), size.GetSort(c)));
        }

        public IFloat Divide(IFloat value)
        {
            return Create(value, (c, l, r) => c.MkFPDiv(c.MkFPRNE(), l, r));
        }

        public IBool Equal(IFloat value)
        {
            return Create(value, (c, l, r) => c.MkFPEq(l, r));
        }

        public IFloat Floor()
        {
            return new SymbolicFloat(Size, c => c.MkFPRoundToIntegral(c.MkFPRTN(), Symbolic(c)));
        }

        public IBool Greater(IFloat value)
        {
            return Create(value, (c, l, r) => c.MkFPGt(l, r));
        }

        public IBool GreaterOrEqual(IFloat value)
        {
            return Create(value, (c, l, r) => c.MkFPGEq(l, r));
        }

        public IBool Less(IFloat value)
        {
            return Create(value, (c, l, r) => c.MkFPLt(l, r));
        }

        public IBool LessOrEqual(IFloat value)
        {
            return Create(value, (c, l, r) => c.MkFPLEq(l, r));
        }

        public IFloat Multiply(IFloat value)
        {
            return Create(value, (c, l, r) => c.MkFPMul(c.MkFPRNE(), l, r));
        }

        public IFloat Negate()
        {
            return new SymbolicFloat(Size, c => c.MkFPNeg(Symbolic(c)));
        }

        public IBool NotEqual(IFloat value)
        {
            return Create(value, (c, l, r) => c.MkNot(c.MkFPEq(l, r)));
        }

        public IBool Ordered(IFloat value)
        {
            return Create(value, (c, l, r) => c.MkNot(c.MkOr(c.MkFPIsNaN(l), c.MkFPIsNaN(r))));
        }

        public IFloat Power(IFloat value)
        {
            return Create(value, (c, l, r) => (RealExpr) c.MkPower(c.MkFPToReal(l), c.MkFPToReal(r)));
        }

        public IFloat Remainder(IFloat value)
        {
            return Create(value, (c, l, r) => c.MkFPRem(l, r));
        }

        public IFloat Subtract(IFloat value)
        {
            return Create(value, (c, l, r) => c.MkFPSub(c.MkFPRNE(), l, r));
        }

        public ISigned ToSigned(Bits size)
        {
            return new SymbolicInteger(size, c => c.MkFPToBV(c.MkFPRTZ(), Symbolic(c), (uint) size, true));
        }

        public IUnsigned ToUnsigned(Bits size)
        {
            return new SymbolicInteger(size, c => c.MkFPToBV(c.MkFPRTZ(), Symbolic(c), (uint) size, false));
        }

        public IBool Unordered(IFloat value)
        {
            return Create(value, (c, l, r) => c.MkOr(c.MkFPIsNaN(l), c.MkFPIsNaN(r)));
        }

        private SymbolicBool Create(IFloat other, Func<Context, FPExpr, FPExpr, BoolExpr> symbolic)
        {
            return new(c => symbolic(c, Symbolic(c), other.Symbolic(c)));
        }

        private SymbolicFloat Create(IFloat other, Func<Context, FPExpr, FPExpr, FPExpr> symbolic)
        {
            return new(Size, c => symbolic(c, Symbolic(c), other.Symbolic(c)));
        }

        private SymbolicReal Create(IFloat other, Func<Context, FPExpr, FPExpr, RealExpr> symbolic)
        {
            return new(Size, c => symbolic(c, Symbolic(c), other.Symbolic(c)));
        }
    }
}
