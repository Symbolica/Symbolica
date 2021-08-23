using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class SymbolicFloat : IValue
    {
        private readonly Func<Context, FPExpr> _func;

        public SymbolicFloat(Bits size, Func<Context, FPExpr> func)
        {
            Size = size;
            _func = func;
        }

        public Bits Size { get; }

        public BigInteger GetInteger(Context context)
        {
            var expr = _func(context).Simplify();

            return expr.IsFPNaN
                ? Size.GetNan(context)
                : ToSymbolicBitVector().GetInteger(context);
        }

        public IValue GetValue(IPersistentSpace space, SymbolicBool[] constraints)
        {
            return ToSymbolicBitVector().GetValue(space, constraints);
        }

        public IProposition GetProposition(IPersistentSpace space, SymbolicBool[] constraints)
        {
            return ToSymbolicBool().GetProposition(space, constraints);
        }

        public IValue Add(IValue value)
        {
            return ToSymbolicBitVector().Add(value);
        }

        public IValue And(IValue value)
        {
            return ToSymbolicBitVector().And(value);
        }

        public IValue ArithmeticShiftRight(IValue value)
        {
            return ToSymbolicBitVector().ArithmeticShiftRight(value);
        }

        public IValue Equal(IValue value)
        {
            return ToSymbolicBitVector().Equal(value);
        }

        public IValue FloatAdd(IValue value)
        {
            return Create(value, (c, l, r) => c.MkFPAdd(c.MkFPRNE(), l, r));
        }

        public IValue FloatCeiling()
        {
            return new SymbolicFloat(Size, c => c.MkFPRoundToIntegral(c.MkFPRTP(), _func(c)));
        }

        public IValue FloatConvert(Bits size)
        {
            return new SymbolicFloat(size, c => c.MkFPToFP(c.MkFPRNE(), _func(c), size.GetSort(c)));
        }

        public IValue FloatDivide(IValue value)
        {
            return Create(value, (c, l, r) => c.MkFPDiv(c.MkFPRNE(), l, r));
        }

        public IValue FloatEqual(IValue value)
        {
            return Create(value, (c, l, r) => c.MkFPEq(l, r));
        }

        public IValue FloatFloor()
        {
            return new SymbolicFloat(Size, c => c.MkFPRoundToIntegral(c.MkFPRTN(), _func(c)));
        }

        public IValue FloatGreater(IValue value)
        {
            return Create(value, (c, l, r) => c.MkFPGt(l, r));
        }

        public IValue FloatGreaterOrEqual(IValue value)
        {
            return Create(value, (c, l, r) => c.MkFPGEq(l, r));
        }

        public IValue FloatLess(IValue value)
        {
            return Create(value, (c, l, r) => c.MkFPLt(l, r));
        }

        public IValue FloatLessOrEqual(IValue value)
        {
            return Create(value, (c, l, r) => c.MkFPLEq(l, r));
        }

        public IValue FloatMultiply(IValue value)
        {
            return Create(value, (c, l, r) => c.MkFPMul(c.MkFPRNE(), l, r));
        }

        public IValue FloatNegate()
        {
            return new SymbolicFloat(Size, c => c.MkFPNeg(_func(c)));
        }

        public IValue FloatNotEqual(IValue value)
        {
            return Create(value, (c, l, r) => c.MkNot(c.MkFPEq(l, r)));
        }

        public IValue FloatOrdered(IValue value)
        {
            return Create(value, (c, l, r) => c.MkNot(c.MkOr(c.MkFPIsNaN(l), c.MkFPIsNaN(r))));
        }

        public IValue FloatRemainder(IValue value)
        {
            return Create(value, (c, l, r) => c.MkFPRem(l, r));
        }

        public IValue FloatSubtract(IValue value)
        {
            return Create(value, (c, l, r) => c.MkFPSub(c.MkFPRNE(), l, r));
        }

        public IValue FloatToSigned(Bits size)
        {
            return new SymbolicBitVector(size, c => c.MkFPToBV(c.MkFPRTZ(), _func(c), (uint) size, true));
        }

        public IValue FloatToUnsigned(Bits size)
        {
            return new SymbolicBitVector(size, c => c.MkFPToBV(c.MkFPRTZ(), _func(c), (uint) size, false));
        }

        public IValue FloatUnordered(IValue value)
        {
            return Create(value, (c, l, r) => c.MkOr(c.MkFPIsNaN(l), c.MkFPIsNaN(r)));
        }

        public IValue LogicalShiftRight(IValue value)
        {
            return ToSymbolicBitVector().LogicalShiftRight(value);
        }

        public IValue Multiply(IValue value)
        {
            return ToSymbolicBitVector().Multiply(value);
        }

        public IValue Not()
        {
            return ToSymbolicBitVector().Not();
        }

        public IValue NotEqual(IValue value)
        {
            return ToSymbolicBitVector().NotEqual(value);
        }

        public IValue Or(IValue value)
        {
            return ToSymbolicBitVector().Or(value);
        }

        public IValue Read(ICollectionFactory collectionFactory, IValue offset, Bits size)
        {
            return ToSymbolicBitVector().Read(collectionFactory, offset, size);
        }

        public IValue Select(IValue trueValue, IValue falseValue)
        {
            return ToSymbolicBool().Select(trueValue, falseValue);
        }

        public IValue Select(Func<Context, BoolExpr> predicate, IValue falseValue)
        {
            return Create(falseValue, (c, t, f) => (FPExpr) c.MkITE(predicate(c), t, f));
        }

        public IValue ShiftLeft(IValue value)
        {
            return ToSymbolicBitVector().ShiftLeft(value);
        }

        public IValue SignedDivide(IValue value)
        {
            return ToSymbolicBitVector().SignedDivide(value);
        }

        public IValue SignedGreater(IValue value)
        {
            return ToSymbolicBitVector().SignedGreater(value);
        }

        public IValue SignedGreaterOrEqual(IValue value)
        {
            return ToSymbolicBitVector().SignedGreaterOrEqual(value);
        }

        public IValue SignedLess(IValue value)
        {
            return ToSymbolicBitVector().SignedLess(value);
        }

        public IValue SignedLessOrEqual(IValue value)
        {
            return ToSymbolicBitVector().SignedLessOrEqual(value);
        }

        public IValue SignedRemainder(IValue value)
        {
            return ToSymbolicBitVector().SignedRemainder(value);
        }

        public IValue SignedToFloat(Bits size)
        {
            return ToSymbolicBitVector().SignedToFloat(size);
        }

        public IValue SignExtend(Bits size)
        {
            return ToSymbolicBitVector().SignExtend(size);
        }

        public IValue Subtract(IValue value)
        {
            return ToSymbolicBitVector().Subtract(value);
        }

        public IValue Truncate(Bits size)
        {
            return ToSymbolicBitVector().Truncate(size);
        }

        public IValue UnsignedDivide(IValue value)
        {
            return ToSymbolicBitVector().UnsignedDivide(value);
        }

        public IValue UnsignedGreater(IValue value)
        {
            return ToSymbolicBitVector().UnsignedGreater(value);
        }

        public IValue UnsignedGreaterOrEqual(IValue value)
        {
            return ToSymbolicBitVector().UnsignedGreaterOrEqual(value);
        }

        public IValue UnsignedLess(IValue value)
        {
            return ToSymbolicBitVector().UnsignedLess(value);
        }

        public IValue UnsignedLessOrEqual(IValue value)
        {
            return ToSymbolicBitVector().UnsignedLessOrEqual(value);
        }

        public IValue UnsignedRemainder(IValue value)
        {
            return ToSymbolicBitVector().UnsignedRemainder(value);
        }

        public IValue UnsignedToFloat(Bits size)
        {
            return ToSymbolicBitVector().UnsignedToFloat(size);
        }

        public IValue Write(ICollectionFactory collectionFactory, IValue offset, IValue value)
        {
            return ToSymbolicBitVector().Write(collectionFactory, offset, value);
        }

        public IValue Xor(IValue value)
        {
            return ToSymbolicBitVector().Xor(value);
        }

        public IValue ZeroExtend(Bits size)
        {
            return ToSymbolicBitVector().ZeroExtend(size);
        }

        public SymbolicBitVector ToSymbolicBitVector()
        {
            return new(Size, c => c.MkFPToIEEEBV(_func(c)));
        }

        public SymbolicBool ToSymbolicBool()
        {
            return ToSymbolicBitVector().ToSymbolicBool();
        }

        public SymbolicFloat ToSymbolicFloat()
        {
            return this;
        }

        private IValue Create(IValue other, Func<Context, FPExpr, FPExpr, BoolExpr> func)
        {
            return new SymbolicBool(c => func(c, _func(c), other.ToSymbolicFloat()._func(c)));
        }

        private IValue Create(IValue other, Func<Context, FPExpr, FPExpr, FPExpr> func)
        {
            return new SymbolicFloat(Size, c => func(c, _func(c), other.ToSymbolicFloat()._func(c)));
        }
    }
}
