using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Computation.Exceptions;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class SymbolicBitVector : IValue
    {
        private readonly Func<Context, BitVecExpr> _func;

        public SymbolicBitVector(Bits size, Func<Context, BitVecExpr> func)
        {
            Size = size;
            _func = func;
        }

        public Bits Size { get; }

        public BigInteger GetInteger(Context context)
        {
            var expr = _func(context).Simplify();

            return expr.IsNumeral
                ? ((BitVecNum) expr).BigInteger
                : throw new IrreducibleSymbolicExpressionException();
        }

        public IValue GetValue(IPersistentSpace space, SymbolicBool[] constraints)
        {
            using var model = space.GetModel(constraints);

            return ConstantUnsigned.Create(Size, ((BitVecNum) model.Evaluate(_func)).BigInteger);
        }

        public IProposition GetProposition(IPersistentSpace space, SymbolicBool[] constraints)
        {
            return ToSymbolicBool().GetProposition(space, constraints);
        }

        public IValue Add(IValue value)
        {
            return Create(value, (c, l, r) => c.MkBVAdd(l, r));
        }

        public IValue And(IValue value)
        {
            return Create(value, (c, l, r) => c.MkBVAND(l, r));
        }

        public IValue ArithmeticShiftRight(IValue value)
        {
            return Create(value, (c, l, r) => c.MkBVASHR(l, r));
        }

        public IValue Equal(IValue value)
        {
            return Create(value, (c, l, r) => c.MkEq(l, r));
        }

        public IValue FloatAdd(IValue value)
        {
            return ToSymbolicFloat().FloatAdd(value);
        }

        public IValue FloatCeiling()
        {
            return ToSymbolicFloat().FloatCeiling();
        }

        public IValue FloatConvert(Bits size)
        {
            return ToSymbolicFloat().FloatConvert(size);
        }

        public IValue FloatDivide(IValue value)
        {
            return ToSymbolicFloat().FloatDivide(value);
        }

        public IValue FloatEqual(IValue value)
        {
            return ToSymbolicFloat().FloatEqual(value);
        }

        public IValue FloatFloor()
        {
            return ToSymbolicFloat().FloatFloor();
        }

        public IValue FloatGreater(IValue value)
        {
            return ToSymbolicFloat().FloatGreater(value);
        }

        public IValue FloatGreaterOrEqual(IValue value)
        {
            return ToSymbolicFloat().FloatGreaterOrEqual(value);
        }

        public IValue FloatLess(IValue value)
        {
            return ToSymbolicFloat().FloatLess(value);
        }

        public IValue FloatLessOrEqual(IValue value)
        {
            return ToSymbolicFloat().FloatLessOrEqual(value);
        }

        public IValue FloatMultiply(IValue value)
        {
            return ToSymbolicFloat().FloatMultiply(value);
        }

        public IValue FloatNegate()
        {
            return ToSymbolicFloat().FloatNegate();
        }

        public IValue FloatNotEqual(IValue value)
        {
            return ToSymbolicFloat().FloatNotEqual(value);
        }

        public IValue FloatOrdered(IValue value)
        {
            return ToSymbolicFloat().FloatOrdered(value);
        }

        public IValue FloatRemainder(IValue value)
        {
            return ToSymbolicFloat().FloatRemainder(value);
        }

        public IValue FloatSubtract(IValue value)
        {
            return ToSymbolicFloat().FloatSubtract(value);
        }

        public IValue FloatToSigned(Bits size)
        {
            return ToSymbolicFloat().FloatToSigned(size);
        }

        public IValue FloatToUnsigned(Bits size)
        {
            return ToSymbolicFloat().FloatToUnsigned(size);
        }

        public IValue FloatUnordered(IValue value)
        {
            return ToSymbolicFloat().FloatUnordered(value);
        }

        public IValue LogicalShiftRight(IValue value)
        {
            return Create(value, (c, l, r) => c.MkBVLSHR(l, r));
        }

        public IValue Multiply(IValue value)
        {
            return Create(value, (c, l, r) => c.MkBVMul(l, r));
        }

        public IValue Not()
        {
            return new SymbolicBitVector(Size, c => c.MkBVNot(_func(c)));
        }

        public IValue NotEqual(IValue value)
        {
            return Create(value, (c, l, r) => c.MkNot(c.MkEq(l, r)));
        }

        public IValue Or(IValue value)
        {
            return Create(value, (c, l, r) => c.MkBVOR(l, r));
        }

        public IValue Read(ICollectionFactory collectionFactory, IValue offset, Bits size)
        {
            return LogicalShiftRight(offset).Truncate(size);
        }

        public IValue Select(IValue trueValue, IValue falseValue)
        {
            return ToSymbolicBool().Select(trueValue, falseValue);
        }

        public IValue Select(Func<Context, BoolExpr> predicate, IValue falseValue)
        {
            return Create(falseValue, (c, t, f) => (BitVecExpr) c.MkITE(predicate(c), t, f));
        }

        public IValue ShiftLeft(IValue value)
        {
            return Create(value, (c, l, r) => c.MkBVSHL(l, r));
        }

        public IValue SignedDivide(IValue value)
        {
            return Create(value, (c, l, r) => c.MkBVSDiv(l, r));
        }

        public IValue SignedGreater(IValue value)
        {
            return Create(value, (c, l, r) => c.MkBVSGT(l, r));
        }

        public IValue SignedGreaterOrEqual(IValue value)
        {
            return Create(value, (c, l, r) => c.MkBVSGE(l, r));
        }

        public IValue SignedLess(IValue value)
        {
            return Create(value, (c, l, r) => c.MkBVSLT(l, r));
        }

        public IValue SignedLessOrEqual(IValue value)
        {
            return Create(value, (c, l, r) => c.MkBVSLE(l, r));
        }

        public IValue SignedRemainder(IValue value)
        {
            return Create(value, (c, l, r) => c.MkBVSRem(l, r));
        }

        public IValue SignedToFloat(Bits size)
        {
            return new SymbolicFloat(size, c => c.MkFPToFP(c.MkFPRNE(), _func(c), size.GetSort(c), true));
        }

        public IValue SignExtend(Bits size)
        {
            return new SymbolicBitVector(size, c => c.MkSignExt((uint) size - (uint) Size, _func(c)));
        }

        public IValue Subtract(IValue value)
        {
            return Create(value, (c, l, r) => c.MkBVSub(l, r));
        }

        public IValue Truncate(Bits size)
        {
            return new SymbolicBitVector(size, c => c.MkExtract((uint) size - 1U, 0U, _func(c)));
        }

        public IValue UnsignedDivide(IValue value)
        {
            return Create(value, (c, l, r) => c.MkBVUDiv(l, r));
        }

        public IValue UnsignedGreater(IValue value)
        {
            return Create(value, (c, l, r) => c.MkBVUGT(l, r));
        }

        public IValue UnsignedGreaterOrEqual(IValue value)
        {
            return Create(value, (c, l, r) => c.MkBVUGE(l, r));
        }

        public IValue UnsignedLess(IValue value)
        {
            return Create(value, (c, l, r) => c.MkBVULT(l, r));
        }

        public IValue UnsignedLessOrEqual(IValue value)
        {
            return Create(value, (c, l, r) => c.MkBVULE(l, r));
        }

        public IValue UnsignedRemainder(IValue value)
        {
            return Create(value, (c, l, r) => c.MkBVURem(l, r));
        }

        public IValue UnsignedToFloat(Bits size)
        {
            return new SymbolicFloat(size, c => c.MkFPToFP(c.MkFPRNE(), _func(c), size.GetSort(c), false));
        }

        public IValue Write(ICollectionFactory collectionFactory, IValue offset, IValue value)
        {
            return And(ConstantUnsigned.Create(value.Size, BigInteger.Zero)
                    .Not()
                    .ZeroExtend(Size)
                    .ShiftLeft(offset)
                    .Not())
                .Or(value
                    .ZeroExtend(Size)
                    .ShiftLeft(offset));
        }

        public IValue Xor(IValue value)
        {
            return Create(value, (c, l, r) => c.MkBVXOR(l, r));
        }

        public IValue ZeroExtend(Bits size)
        {
            return new SymbolicBitVector(size, c => c.MkZeroExt((uint) size - (uint) Size, _func(c)));
        }

        public SymbolicBitVector ToSymbolicBitVector()
        {
            return this;
        }

        public SymbolicBool ToSymbolicBool()
        {
            return new(c => c.MkNot(c.MkEq(_func(c), c.MkBV(0U, (uint) Size))));
        }

        public SymbolicFloat ToSymbolicFloat()
        {
            return new(Size, c => c.MkFPToFP(_func(c), Size.GetSort(c)));
        }

        private IValue Create(IValue other, Func<Context, BitVecExpr, BitVecExpr, BitVecExpr> func)
        {
            return new SymbolicBitVector(Size, c => func(c, _func(c), other.ToSymbolicBitVector()._func(c)));
        }

        private IValue Create(IValue other, Func<Context, BitVecExpr, BitVecExpr, BoolExpr> func)
        {
            return new SymbolicBool(c => func(c, _func(c), other.ToSymbolicBitVector()._func(c)));
        }
    }
}
