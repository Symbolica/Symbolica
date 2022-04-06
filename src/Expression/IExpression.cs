using System;
using Symbolica.Collection;
using Symbolica.Expression.Values;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression;

public interface IType
{
    Bits Size { get; }
}

public interface IInteger : IType { }

public record Bool : IInteger
{
    public Bits Size => Bits.One;

    public static Bool Type => new();
}

public record BitVector : IInteger
{
    public BitVector(Bits size)
    {
        Size = size;
    }

    public Bits Size { get; }
}

public record Real : IType
{
    public Real(Bits size)
    {
        Size = size;
    }

    public Bits Size { get; }
}

public interface IExpression<out T> : IEquatable<IExpression<IType>>
    where T : IType
{
    Bits Size => Type.Size;

    T Type { get; }

    U Map<U>(IArityMapper<U> mapper);

    U Map<U>(ITypeMapper<U> mapper);
}

public interface IIntegerExpression : IExpression<IInteger>
{
    U Map<U>(IIntegerMapper<U> mapper);
}

public interface IBoolExpression : IExpression<Bool>, IIntegerExpression
{
    U Map<U>(IBoolMapper<U> mapper);
}

public interface IBitVectorExpression : IExpression<BitVector>, IIntegerExpression
{
    U Map<U>(IBitVectorMapper<U> mapper);
}

public interface IFloatExpression : IExpression<Float>
{
    U Map<U>(IFloatMapper<U> mapper);
}

public interface IRealExpression : IExpression<Real>
{
    U Map<U>(IRealMapper<U> mapper);
}

public interface IConstantValue<out T> : IExpression<T>
    where T : IType
{
    ConstantBitVector AsBitVector(ICollectionFactory collectionFactory);
    ConstantUnsigned AsUnsigned();
    ConstantSigned AsSigned();
    ConstantBool AsBool();
    ConstantSingle AsSingle();
    ConstantDouble AsDouble();

    U Map<U>(IConstantMapper<U> mapper);
}

public interface IConstantBitVector : IConstantValue<BitVector>, IBitVectorExpression { }

public interface IConstantBool : IConstantValue<Bool>, IBoolExpression { }

public interface IConstantFloat : IConstantValue<Float>, IFloatExpression { }

public interface IUnary<T, V> : IExpression<T>
    where T : IType
    where V : IType
{
    IExpression<V> Value { get; }

    U Map<U>(IUnaryMapper<U> mapper);
}

public interface IUnary<T> : IUnary<T, IType>
    where T : IType
{ }

public interface IUnaryBitVectorExpression : IBitVectorExpression, IUnary<BitVector> { }

public interface IUnaryBitVectorExpression<V> : IBitVectorExpression, IUnary<BitVector, V>
    where V : IType
{ }

public interface IUnaryBoolExpression : IUnaryBoolExpression<IType> { }

public interface IUnaryBoolExpression<V> : IBoolExpression, IUnary<Bool, V>
    where V : IType
{ }

public interface IUnaryFloatExpression : IFloatExpression, IUnary<Float> { }

public interface IUnaryFloatExpression<V> : IFloatExpression, IUnary<Float, V>
    where V : IType
{ }

public interface IUnaryRealExpression<V> : IRealExpression, IUnary<Real, V>
    where V : IType
{ }

public interface IBinary<T, VL, VR> : IExpression<T>
    where T : IType
    where VL : IType
    where VR : IType
{
    IExpression<VL> Left { get; }
    IExpression<VR> Right { get; }

    U Map<U>(IBinaryMapper<U> mapper);
}

public interface IBinaryExpression<T, V> : IBinary<T, V, V>
    where T : IType
    where V : IType
{ }

public interface IBinaryExpression<T> : IBinary<T, IType, IType>
    where T : IType
{ }

public interface IBinaryBitVectorExpression : IBitVectorExpression, IBinaryExpression<BitVector> { }

public interface IBinaryBoolExpression : IBoolExpression, IBinaryExpression<Bool> { }

public interface IBinaryFloatExpression : IFloatExpression, IBinaryExpression<Float> { }

public interface IBinaryRealExpression : IRealExpression, IBinaryExpression<Real> { }

public interface IArityMapper<out U>
{
    U Map<T>(IConstantValue<T> constant)
        where T : IType;
    U Map<T, V>(IUnary<T, V> unary)
         where T : IType
         where V : IType;
    U Map<T, VL, VR>(IBinary<T, VL, VR> binary)
        where T : IType
        where VL : IType
        where VR : IType;
    U Map(NormalFloat normalFloat);
    U Map(Select select);
    U Map(Symbol symbol);
    U Map(Write write);
}

public interface IConstantMapper<out T>
{
    T Map(ConstantBitVector constantBitVector);
    T Map(ConstantBool constantBool);
    T Map(ConstantDouble constantDouble);
    T Map(ConstantSigned constantSigned);
    T Map(ConstantSingle constantSingle);
    T Map(ConstantUnsigned constantUnsigned);
}

public interface IUnaryMapper<out T>
{
    T Map(FloatCeiling floatCeiling);
    T Map(FloatConvert floatConvert);
    T Map(FloatFloor floatFloor);
    T Map(FloatNegate floatNegate);
    T Map(FloatToSigned floatToSigned);
    T Map(FloatToUnsigned floatToUnsigned);
    T Map(Logical logical);
    T Map(LogicalNot logicalNot);
    T Map(Not not);
    T Map(RealConvert realConvert);
    T Map(RealToSigned realToSigned);
    T Map(SignedToFloat signedToFloat);
    T Map(SignExtend signExtend);
    T Map(Truncate truncate);
    T Map(UnsignedToFloat unsignedToFloat);
    T Map(ZeroExtend zeroExtend);
}

public interface IBinaryMapper<out T>
{
    T Map(Add add);
    T Map(And and);
    T Map(ArithmeticShiftRight arithmeticShiftRight);
    T Map(Equal equal);
    T Map(FloatAdd floatAdd);
    T Map(FloatDivide floatDivide);
    T Map(FloatEqual floatEqual);
    T Map(FloatGreater floatGreater);
    T Map(FloatGreaterOrEqual floatGreaterOrEqual);
    T Map(FloatLess floatLess);
    T Map(FloatLessOrEqual floatLessOrEqual);
    T Map(FloatMultiply floatMultiply);
    T Map(FloatPower floatPower);
    T Map(FloatRemainder floatRemainder);
    T Map(FloatSubtract floatSubtract);
    T Map(FloatUnordered floatUnordered);
    T Map(LogicalAnd logicalAnd);
    T Map(LogicalOr logicalOr);
    T Map(LogicalShiftRight logicalShiftRight);
    T Map(LogicalXor logicalXor);
    T Map(Multiply multiply);
    T Map(Or or);
    T Map(ShiftLeft shiftLeft);
    T Map(SignedDivide signedDivide);
    T Map(SignedGreater signedGreater);
    T Map(SignedGreaterOrEqual signedGreaterOrEqual);
    T Map(SignedLess signedLess);
    T Map(SignedLessOrEqual signedLessOrEqual);
    T Map(SignedRemainder signedRemainder);
    T Map(Subtract subtract);
    T Map(UnsignedDivide unsignedDivide);
    T Map(UnsignedGreater unsignedGreater);
    T Map(UnsignedGreaterOrEqual unsignedGreaterOrEqual);
    T Map(UnsignedLess unsignedLess);
    T Map(UnsignedLessOrEqual unsignedLessOrEqual);
    T Map(UnsignedRemainder unsignedRemainder);
    T Map(Xor xor);
}

public interface ITypeMapper<out U>
{
    U Map(IIntegerExpression integer);
    U Map(IFloatExpression @float);
    U Map(IRealExpression real);
    U Map(Select select);
}

public interface IIntegerMapper<out U>
{
    U Map(IBitVectorExpression bitVector);
    U Map(IBoolExpression @bool);
}

public interface IBitVectorMapper<out U>
{
    U Map(ConstantBitVector constantBitVector);
    U Map(ConstantSigned constantSigned);
    U Map(ConstantUnsigned constantUnsigned);
    U Map(Add add);
    U Map(And and);
    U Map(ArithmeticShiftRight arithmeticShiftRight);
    U Map(FloatToSigned floatToSigned);
    U Map(FloatToUnsigned floatToUnsigned);
    U Map(LogicalShiftRight logicalShiftRight);
    U Map(Multiply multiply);
    U Map(Not not);
    U Map(Or or);
    U Map(RealToSigned realToSigned);
    U Map(ShiftLeft shiftLeft);
    U Map(SignedDivide signedDivide);
    U Map(SignedRemainder signedRemainder);
    U Map(SignExtend signExtend);
    U Map(Subtract subtract);
    U Map(Symbol symbol);
    U Map(Truncate truncate);
    U Map(UnsignedDivide unsignedDivide);
    U Map(UnsignedRemainder unsignedRemainder);
    U Map(Write write);
    U Map(Xor xor);
    U Map(ZeroExtend zeroExtend);
}

public interface IBoolMapper<out U>
{
    U Map(ConstantBool constantBool);
    U Map(Equal equal);
    U Map(FloatEqual floatEqual);
    U Map(FloatGreater floatGreater);
    U Map(FloatGreaterOrEqual floatGreaterOrEqual);
    U Map(FloatLess floatLess);
    U Map(FloatLessOrEqual floatLessOrEqual);
    U Map(FloatUnordered floatUnordered);
    U Map(Logical logical);
    U Map(LogicalAnd logicalAnd);
    U Map(LogicalNot logicalNot);
    U Map(LogicalOr logicalOr);
    U Map(LogicalXor logicalXor);
    U Map(SignedGreater signedGreater);
    U Map(SignedGreaterOrEqual signedGreaterOrEqual);
    U Map(SignedLess signedLess);
    U Map(SignedLessOrEqual signedLessOrEqual);
    U Map(UnsignedGreater unsignedGreater);
    U Map(UnsignedGreaterOrEqual unsignedGreaterOrEqual);
    U Map(UnsignedLess unsignedLess);
    U Map(UnsignedLessOrEqual unsignedLessOrEqual);
}

public interface IFloatMapper<out U>
{
    U Map(ConstantDouble constantDouble);
    U Map(ConstantSingle constantSingle);
    U Map(FloatAdd floatAdd);
    U Map(FloatCeiling floatCeiling);
    U Map(FloatConvert floatConvert);
    U Map(FloatDivide floatDivide);
    U Map(FloatFloor floatFloor);
    U Map(FloatMultiply floatMultiply);
    U Map(FloatNegate floatNegate);
    U Map(FloatRemainder floatRemainder);
    U Map(FloatSubtract floatSubtract);
    U Map(NormalFloat normalFloat);
    U Map(SignedToFloat signedToFloat);
    U Map(UnsignedToFloat unsignedToFloat);
}

public interface IRealMapper<out U>
{
    U Map(FloatPower floatPower);
    U Map(RealConvert realConvert);
}
