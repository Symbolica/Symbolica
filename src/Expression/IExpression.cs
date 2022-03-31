using System;
using Symbolica.Collection;
using Symbolica.Expression.Values;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression;

public interface IExpression : IEquatable<IExpression>
{
    Bits Size { get; }

    T Map<T>(IExprMapper<T> mapper);
}

public interface IConstantValue : IExpression
{
    ConstantBitVector AsBitVector(ICollectionFactory collectionFactory);
    ConstantUnsigned AsUnsigned();
    ConstantSigned AsSigned();
    ConstantBool AsBool();
    ConstantSingle AsSingle();
    ConstantDouble AsDouble();

    T Map<T>(IConstantMapper<T> mapper);
}

public interface IRealValue : IExpression
{
}

public interface IUnaryExpr<T> : IExpression
    where T : IExpression
{
    Bits IExpression.Size => Value.Size;
    T Value { get; }

    U Map<U>(IUnaryExprMapper<U> mapper);
}

public interface IUnaryExpr : IUnaryExpr<IExpression>
{
}

public interface IBinaryExpr : IExpression
{
    IExpression Left { get; }
    IExpression Right { get; }
    Bits IExpression.Size => Left.Size;

    T Map<T>(IBinaryExprMapper<T> mapper);
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

public interface IUnaryExprMapper<out T>
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

public interface IBinaryExprMapper<out T>
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

public interface IExprMapper<out T>
{
    T Map(IConstantValue constant);
    T Map<U>(IUnaryExpr<U> unary)
         where U : IExpression;
    T Map(IBinaryExpr binary);
    T Map(Address address);
    T Map(Select select);
    T Map(Symbol symbol);
}
