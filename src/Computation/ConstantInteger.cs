using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal abstract class ConstantInteger : IConstantValue
    {
        protected ConstantInteger(Bits size)
        {
            Size = size;
        }

        public Bits Size { get; }

        public abstract BigInteger GetInteger(Context context);

        public IValue GetValue(IPersistentSpace space, SymbolicBool[] constraints)
        {
            return this;
        }

        public abstract IProposition GetProposition(IPersistentSpace space, SymbolicBool[] constraints);
        public abstract IValue Add(IValue value);
        public abstract IValue And(IValue value);
        public abstract IValue ArithmeticShiftRight(IValue value);
        public abstract IValue Equal(IValue value);

        public IValue FloatAdd(IValue value)
        {
            return ToConstantFloat().FloatAdd(value);
        }

        public IValue FloatCeiling()
        {
            return ToConstantFloat().FloatCeiling();
        }

        public IValue FloatConvert(Bits size)
        {
            return ToConstantFloat().FloatConvert(size);
        }

        public IValue FloatDivide(IValue value)
        {
            return ToConstantFloat().FloatDivide(value);
        }

        public IValue FloatEqual(IValue value)
        {
            return ToConstantFloat().FloatEqual(value);
        }

        public IValue FloatFloor()
        {
            return ToConstantFloat().FloatFloor();
        }

        public IValue FloatGreater(IValue value)
        {
            return ToConstantFloat().FloatGreater(value);
        }

        public IValue FloatGreaterOrEqual(IValue value)
        {
            return ToConstantFloat().FloatGreaterOrEqual(value);
        }

        public IValue FloatLess(IValue value)
        {
            return ToConstantFloat().FloatLess(value);
        }

        public IValue FloatLessOrEqual(IValue value)
        {
            return ToConstantFloat().FloatLessOrEqual(value);
        }

        public IValue FloatMultiply(IValue value)
        {
            return ToConstantFloat().FloatMultiply(value);
        }

        public IValue FloatNegate()
        {
            return ToConstantFloat().FloatNegate();
        }

        public IValue FloatNotEqual(IValue value)
        {
            return ToConstantFloat().FloatNotEqual(value);
        }

        public IValue FloatOrdered(IValue value)
        {
            return ToConstantFloat().FloatOrdered(value);
        }

        public IValue FloatRemainder(IValue value)
        {
            return ToConstantFloat().FloatRemainder(value);
        }

        public IValue FloatSubtract(IValue value)
        {
            return ToConstantFloat().FloatSubtract(value);
        }

        public IValue FloatToSigned(Bits size)
        {
            return ToConstantFloat().FloatToSigned(size);
        }

        public IValue FloatToUnsigned(Bits size)
        {
            return ToConstantFloat().FloatToUnsigned(size);
        }

        public IValue FloatUnordered(IValue value)
        {
            return ToConstantFloat().FloatUnordered(value);
        }

        public abstract IValue LogicalShiftRight(IValue value);
        public abstract IValue Multiply(IValue value);
        public abstract IValue Not();
        public abstract IValue NotEqual(IValue value);
        public abstract IValue Or(IValue value);
        public abstract IValue Read(ICollectionFactory collectionFactory, IValue offset, Bits size);
        public abstract IValue Select(IValue trueValue, IValue falseValue);
        public abstract IValue Select(Func<Context, BoolExpr> predicate, IValue falseValue);
        public abstract IValue ShiftLeft(IValue value);
        public abstract IValue SignedDivide(IValue value);
        public abstract IValue SignedGreater(IValue value);
        public abstract IValue SignedGreaterOrEqual(IValue value);
        public abstract IValue SignedLess(IValue value);
        public abstract IValue SignedLessOrEqual(IValue value);
        public abstract IValue SignedRemainder(IValue value);
        public abstract IValue SignedToFloat(Bits size);
        public abstract IValue SignExtend(Bits size);
        public abstract IValue Subtract(IValue value);
        public abstract IValue Truncate(Bits size);
        public abstract IValue UnsignedDivide(IValue value);
        public abstract IValue UnsignedGreater(IValue value);
        public abstract IValue UnsignedGreaterOrEqual(IValue value);
        public abstract IValue UnsignedLess(IValue value);
        public abstract IValue UnsignedLessOrEqual(IValue value);
        public abstract IValue UnsignedRemainder(IValue value);
        public abstract IValue UnsignedToFloat(Bits size);
        public abstract IValue Write(ICollectionFactory collectionFactory, IValue offset, IValue value);
        public abstract IValue Xor(IValue value);
        public abstract IValue ZeroExtend(Bits size);

        public abstract SymbolicBitVector ToSymbolicBitVector();
        public abstract SymbolicBool ToSymbolicBool();

        public SymbolicFloat ToSymbolicFloat()
        {
            return ToSymbolicBitVector().ToSymbolicFloat();
        }

        public abstract ConstantBitVector ToConstantBitVector(ICollectionFactory collectionFactory);
        public abstract ConstantUnsigned ToConstantUnsigned();
        public abstract ConstantSigned ToConstantSigned();
        public abstract ConstantBool ToConstantBool();
        public abstract IValue ToConstantFloat();
    }
}
