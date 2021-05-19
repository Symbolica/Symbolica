using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal interface IValue
    {
        Bits Size { get; }
        BigInteger Integer { get; }

        IValue GetValue(IPersistentSpace space, SymbolicBool[] constraints);
        IProposition GetProposition(IPersistentSpace space, SymbolicBool[] constraints);
        IValue Add(IValue value);
        IValue And(IValue value);
        IValue ArithmeticShiftRight(IValue value);
        IValue Equal(IValue value);
        IValue FloatAdd(IValue value);
        IValue FloatCeiling();
        IValue FloatConvert(Bits size);
        IValue FloatDivide(IValue value);
        IValue FloatEqual(IValue value);
        IValue FloatFloor();
        IValue FloatGreater(IValue value);
        IValue FloatGreaterOrEqual(IValue value);
        IValue FloatLess(IValue value);
        IValue FloatLessOrEqual(IValue value);
        IValue FloatMultiply(IValue value);
        IValue FloatNegate();
        IValue FloatNotEqual(IValue value);
        IValue FloatOrdered(IValue value);
        IValue FloatRemainder(IValue value);
        IValue FloatSubtract(IValue value);
        IValue FloatToSigned(Bits size);
        IValue FloatToUnsigned(Bits size);
        IValue FloatUnordered(IValue value);
        IValue LogicalShiftRight(IValue value);
        IValue Multiply(IValue value);
        IValue Not();
        IValue NotEqual(IValue value);
        IValue Or(IValue value);
        IValue Read(ICollectionFactory collectionFactory, IValue offset, Bits size);
        IValue Select(IValue trueValue, IValue falseValue);
        IValue Select(Func<Context, BoolExpr> predicate, IValue falseValue);
        IValue ShiftLeft(IValue value);
        IValue SignedDivide(IValue value);
        IValue SignedGreater(IValue value);
        IValue SignedGreaterOrEqual(IValue value);
        IValue SignedLess(IValue value);
        IValue SignedLessOrEqual(IValue value);
        IValue SignedRemainder(IValue value);
        IValue SignedToFloat(Bits size);
        IValue SignExtend(Bits size);
        IValue Subtract(IValue value);
        IValue Truncate(Bits size);
        IValue UnsignedDivide(IValue value);
        IValue UnsignedGreater(IValue value);
        IValue UnsignedGreaterOrEqual(IValue value);
        IValue UnsignedLess(IValue value);
        IValue UnsignedLessOrEqual(IValue value);
        IValue UnsignedRemainder(IValue value);
        IValue UnsignedToFloat(Bits size);
        IValue Write(ICollectionFactory collectionFactory, IValue offset, IValue value);
        IValue Xor(IValue value);
        IValue ZeroExtend(Bits size);
        SymbolicBitVector ToSymbolicBitVector();
        SymbolicBool ToSymbolicBool();
        SymbolicFloat ToSymbolicFloat();
    }
}