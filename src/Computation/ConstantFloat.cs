using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal abstract class ConstantFloat : IConstantValue
    {
        protected ConstantFloat(Bits size)
        {
            Size = size;
        }

        public Bits Size { get; }
        public BigInteger Integer => ToConstantUnsigned().Integer;

        public BigInteger GetInteger(Context context)
        {
            return Integer;
        }

        public IValue GetValue(IPersistentSpace space, SymbolicBool[] constraints)
        {
            return this;
        }

        public IProposition GetProposition(IPersistentSpace space, SymbolicBool[] constraints)
        {
            return ToConstantBool().GetProposition(space, constraints);
        }

        public IValue Add(IValue value)
        {
            return ToConstantUnsigned().Add(value);
        }

        public IValue And(IValue value)
        {
            return ToConstantUnsigned().And(value);
        }

        public IValue ArithmeticShiftRight(IValue value)
        {
            return ToConstantSigned().ArithmeticShiftRight(value);
        }

        public IValue Equal(IValue value)
        {
            return ToConstantUnsigned().Equal(value);
        }

        public abstract IValue FloatAdd(IValue value);
        public abstract IValue FloatCeiling();
        public abstract IValue FloatConvert(Bits size);
        public abstract IValue FloatDivide(IValue value);
        public abstract IValue FloatEqual(IValue value);
        public abstract IValue FloatFloor();
        public abstract IValue FloatGreater(IValue value);
        public abstract IValue FloatGreaterOrEqual(IValue value);
        public abstract IValue FloatLess(IValue value);
        public abstract IValue FloatLessOrEqual(IValue value);
        public abstract IValue FloatMultiply(IValue value);
        public abstract IValue FloatNegate();
        public abstract IValue FloatNotEqual(IValue value);
        public abstract IValue FloatOrdered(IValue value);
        public abstract IValue FloatRemainder(IValue value);
        public abstract IValue FloatSubtract(IValue value);
        public abstract IValue FloatToSigned(Bits size);
        public abstract IValue FloatToUnsigned(Bits size);
        public abstract IValue FloatUnordered(IValue value);

        public IValue LogicalShiftRight(IValue value)
        {
            return ToConstantUnsigned().LogicalShiftRight(value);
        }

        public IValue Multiply(IValue value)
        {
            return ToConstantUnsigned().Multiply(value);
        }

        public IValue Not()
        {
            return ToConstantUnsigned().Not();
        }

        public IValue NotEqual(IValue value)
        {
            return ToConstantUnsigned().NotEqual(value);
        }

        public IValue Or(IValue value)
        {
            return ToConstantUnsigned().Or(value);
        }

        public IValue Read(ICollectionFactory collectionFactory, IValue offset, Bits size)
        {
            return ToConstantBitVector(collectionFactory).Read(collectionFactory, offset, size);
        }

        public IValue Select(IValue trueValue, IValue falseValue)
        {
            return ToConstantBool().Select(trueValue, falseValue);
        }

        public IValue Select(Func<Context, BoolExpr> predicate, IValue falseValue)
        {
            return ToSymbolicFloat().Select(predicate, falseValue);
        }

        public IValue ShiftLeft(IValue value)
        {
            return ToConstantUnsigned().ShiftLeft(value);
        }

        public IValue SignedDivide(IValue value)
        {
            return ToConstantSigned().SignedDivide(value);
        }

        public IValue SignedGreater(IValue value)
        {
            return ToConstantSigned().SignedGreater(value);
        }

        public IValue SignedGreaterOrEqual(IValue value)
        {
            return ToConstantSigned().SignedGreaterOrEqual(value);
        }

        public IValue SignedLess(IValue value)
        {
            return ToConstantSigned().SignedLess(value);
        }

        public IValue SignedLessOrEqual(IValue value)
        {
            return ToConstantSigned().SignedLessOrEqual(value);
        }

        public IValue SignedRemainder(IValue value)
        {
            return ToConstantSigned().SignedRemainder(value);
        }

        public IValue SignedToFloat(Bits size)
        {
            return ToConstantSigned().SignedToFloat(size);
        }

        public IValue SignExtend(Bits size)
        {
            return ToConstantSigned().SignExtend(size);
        }

        public IValue Subtract(IValue value)
        {
            return ToConstantUnsigned().Subtract(value);
        }

        public IValue Truncate(Bits size)
        {
            return ToConstantUnsigned().Truncate(size);
        }

        public IValue UnsignedDivide(IValue value)
        {
            return ToConstantUnsigned().UnsignedDivide(value);
        }

        public IValue UnsignedGreater(IValue value)
        {
            return ToConstantUnsigned().UnsignedGreater(value);
        }

        public IValue UnsignedGreaterOrEqual(IValue value)
        {
            return ToConstantUnsigned().UnsignedGreaterOrEqual(value);
        }

        public IValue UnsignedLess(IValue value)
        {
            return ToConstantUnsigned().UnsignedLess(value);
        }

        public IValue UnsignedLessOrEqual(IValue value)
        {
            return ToConstantUnsigned().UnsignedLessOrEqual(value);
        }

        public IValue UnsignedRemainder(IValue value)
        {
            return ToConstantUnsigned().UnsignedRemainder(value);
        }

        public IValue UnsignedToFloat(Bits size)
        {
            return ToConstantUnsigned().UnsignedToFloat(size);
        }

        public IValue Write(ICollectionFactory collectionFactory, IValue offset, IValue value)
        {
            return ToConstantBitVector(collectionFactory).Write(collectionFactory, offset, value);
        }

        public IValue Xor(IValue value)
        {
            return ToConstantUnsigned().Xor(value);
        }

        public IValue ZeroExtend(Bits size)
        {
            return ToConstantUnsigned().ZeroExtend(size);
        }

        public SymbolicBitVector ToSymbolicBitVector()
        {
            return ToConstantUnsigned().ToSymbolicBitVector();
        }

        public SymbolicBool ToSymbolicBool()
        {
            return ToConstantUnsigned().ToSymbolicBool();
        }

        public abstract SymbolicFloat ToSymbolicFloat();

        public ConstantBitVector ToConstantBitVector(ICollectionFactory collectionFactory)
        {
            return ToConstantUnsigned().ToConstantBitVector(collectionFactory);
        }

        public ConstantUnsigned ToConstantUnsigned()
        {
            return ToConstantSigned().ToConstantUnsigned();
        }

        public abstract ConstantSigned ToConstantSigned();

        public ConstantBool ToConstantBool()
        {
            return ToConstantUnsigned().ToConstantBool();
        }

        public IValue ToConstantFloat()
        {
            return this;
        }

        public static IValue Create(Bits size, string value)
        {
            return (uint) size switch
            {
                32U => new ConstantSingle(float.Parse(value)),
                64U => new ConstantDouble(double.Parse(value)),
                _ => new SymbolicFloat(size, c => size.ParseNormalFloat(c, value))
            };
        }

        public static IValue? Create(Bits size, BigInteger value)
        {
            return (uint) size switch
            {
                32U => new ConstantSingle((float) value),
                64U => new ConstantDouble((double) value),
                _ => null
            };
        }

        public static IValue? CreateFromBits(Bits size, BigInteger value)
        {
            return (uint) size switch
            {
                32U => new ConstantSingle(BitConverter.Int32BitsToSingle((int) value)),
                64U => new ConstantDouble(BitConverter.Int64BitsToDouble((long) value)),
                _ => null
            };
        }
    }
}
