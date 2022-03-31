using System;
using Microsoft.Z3;
using Symbolica.Expression;
using Symbolica.Expression.Values;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Computation;

internal sealed class AsBool : IExprMapper<BoolExpr>
{
    private readonly ISolver _solver;

    public AsBool(ISolver solver)
    {
        _solver = solver;
    }

    public BoolExpr Map(IConstantValue constant)
    {
        return constant.Map(new ConstantAsBool(_solver));
    }

    public BoolExpr Map<U>(IUnaryExpr<U> unary) where U : IExpression
    {
        return unary.Map(new UnaryAsBool(_solver));
    }

    public BoolExpr Map(IBinaryExpr binary)
    {
        return binary.Map(new BinaryAsBool(_solver));
    }

    public BoolExpr Map(Address address)
    {
        return address.Aggregate().Map(this);
    }

    public BoolExpr Map(Select select)
    {
        using var predicate = select.Predicate.Map(this);
        using var trueValue = select.TrueValue.Map(this);
        using var falseValue = select.FalseValue.Map(this);
        return (BoolExpr) _solver.Context.MkITE(predicate, trueValue, falseValue);
    }

    public BoolExpr Map(Expression.Symbol symbol)
    {
        return new AsBitVector(_solver).AsBool(symbol);
    }

    private sealed class ConstantAsBool : IConstantMapper<BoolExpr>
    {
        private readonly ISolver _solver;

        public ConstantAsBool(ISolver solver)
        {
            _solver = solver;
        }

        public BoolExpr Map(ConstantBitVector constantBitVector)
        {
            return new AsBitVector(_solver).AsBool(constantBitVector);
        }

        public BoolExpr Map(ConstantBool constantBool)
        {
            return _solver.Context.MkBool(constantBool);
        }

        public BoolExpr Map(ConstantDouble constantDouble)
        {
            return new AsFloat(_solver).AsBool(constantDouble);
        }

        public BoolExpr Map(ConstantSigned constantSigned)
        {
            return new AsBitVector(_solver).AsBool(constantSigned);
        }

        public BoolExpr Map(ConstantSingle constantSingle)
        {
            return new AsFloat(_solver).AsBool(constantSingle);
        }

        public BoolExpr Map(ConstantUnsigned constantUnsigned)
        {
            return new AsBitVector(_solver).AsBool(constantUnsigned);
        }
    }

    private sealed class UnaryAsBool : IUnaryExprMapper<BoolExpr>
    {
        private readonly ISolver _solver;

        public UnaryAsBool(ISolver solver)
        {
            _solver = solver;
        }

        public BoolExpr Map(FloatCeiling floatCeiling)
        {
            return new AsFloat(_solver).AsBool(floatCeiling);
        }

        public BoolExpr Map(FloatConvert floatConvert)
        {
            return new AsFloat(_solver).AsBool(floatConvert);
        }

        public BoolExpr Map(FloatFloor floatFloor)
        {
            return new AsFloat(_solver).AsBool(floatFloor);
        }

        public BoolExpr Map(FloatNegate floatNegate)
        {
            return new AsFloat(_solver).AsBool(floatNegate);
        }

        public BoolExpr Map(FloatToSigned floatToSigned)
        {
            return new AsBitVector(_solver).AsBool(floatToSigned);
        }

        public BoolExpr Map(FloatToUnsigned floatToUnsigned)
        {
            return new AsBitVector(_solver).AsBool(floatToUnsigned);
        }

        public BoolExpr Map(Logical logical)
        {
            return logical.Value.Map(new AsBool(_solver));
        }

        public BoolExpr Map(LogicalNot logicalNot)
        {
            using var value = logicalNot.Value.Map(new AsBool(_solver));
            return _solver.Context.MkNot(value);
        }

        public BoolExpr Map(Not not)
        {
            return new AsBitVector(_solver).AsBool(not);
        }

        public BoolExpr Map(RealConvert realConvert)
        {
            return new AsFloat(_solver).AsBool(realConvert);
        }

        public BoolExpr Map(RealToSigned realToSigned)
        {
            return new AsBitVector(_solver).AsBool(realToSigned);
        }

        public BoolExpr Map(SignedToFloat signedToFloat)
        {
            return new AsFloat(_solver).AsBool(signedToFloat);
        }

        public BoolExpr Map(SignExtend signExtend)
        {
            return new AsBitVector(_solver).AsBool(signExtend);
        }

        public BoolExpr Map(Truncate truncate)
        {
            return new AsBitVector(_solver).AsBool(truncate);
        }

        public BoolExpr Map(UnsignedToFloat unsignedToFloat)
        {
            return new AsFloat(_solver).AsBool(unsignedToFloat);
        }

        public BoolExpr Map(ZeroExtend zeroExtend)
        {
            return new AsBitVector(_solver).AsBool(zeroExtend);
        }
    }

    private sealed class BinaryAsBool : IBinaryExprMapper<BoolExpr>
    {
        private readonly ISolver _solver;

        public BinaryAsBool(ISolver solver)
        {
            _solver = solver;
        }

        public BoolExpr Map(Add add)
        {
            return new AsBitVector(_solver).AsBool(add);
        }

        public BoolExpr Map(And and)
        {
            return new AsBitVector(_solver).AsBool(and);
        }

        public BoolExpr Map(ArithmeticShiftRight arithmeticShiftRight)
        {
            return new AsBitVector(_solver).AsBool(arithmeticShiftRight);
        }

        public BoolExpr Map(Equal equal)
        {
            return equal.Left is Expression.Bool || equal.Right is Expression.Bool
                ? MapBoolOperands(equal, _solver.Context.MkEq)
                : MapBitVectorOperands(equal, _solver.Context.MkEq);
        }

        public BoolExpr Map(FloatAdd floatAdd)
        {
            return new AsFloat(_solver).AsBool(floatAdd);
        }

        public BoolExpr Map(FloatDivide floatDivide)
        {
            return new AsFloat(_solver).AsBool(floatDivide);
        }

        public BoolExpr Map(FloatEqual floatEqual)
        {
            return MapFloatOperands(floatEqual, _solver.Context.MkFPEq);
        }

        public BoolExpr Map(FloatGreater floatGreater)
        {
            return MapFloatOperands(floatGreater, _solver.Context.MkFPGt);
        }

        public BoolExpr Map(FloatGreaterOrEqual floatGreaterOrEqual)
        {
            return MapFloatOperands(floatGreaterOrEqual, _solver.Context.MkFPGEq);
        }

        public BoolExpr Map(FloatLess floatLess)
        {
            return MapFloatOperands(floatLess, _solver.Context.MkFPLt);
        }

        public BoolExpr Map(FloatLessOrEqual floatLessOrEqual)
        {
            return MapFloatOperands(floatLessOrEqual, _solver.Context.MkFPLEq);
        }

        public BoolExpr Map(FloatMultiply floatMultiply)
        {
            return new AsFloat(_solver).AsBool(floatMultiply);
        }

        public BoolExpr Map(FloatPower floatPower)
        {
            return new AsFloat(_solver).AsBool(floatPower);
        }

        public BoolExpr Map(FloatRemainder floatRemainder)
        {
            return new AsFloat(_solver).AsBool(floatRemainder);
        }

        public BoolExpr Map(FloatSubtract floatSubtract)
        {
            return new AsFloat(_solver).AsBool(floatSubtract);
        }

        public BoolExpr Map(FloatUnordered floatUnordered)
        {
            return MapFloatOperands(floatUnordered, (left, right) =>
            {
                using var leftIsNaN = _solver.Context.MkFPIsNaN(left);
                using var rightIsNaN = _solver.Context.MkFPIsNaN(right);
                return _solver.Context.MkOr(leftIsNaN, rightIsNaN);
            });
        }

        public BoolExpr Map(LogicalAnd logicalAnd)
        {
            return MapBoolOperands(logicalAnd, (l, r) => _solver.Context.MkAnd(l, r));
        }

        public BoolExpr Map(LogicalOr logicalOr)
        {
            return MapBoolOperands(logicalOr, (l, r) => _solver.Context.MkOr(l, r));
        }

        public BoolExpr Map(LogicalShiftRight logicalShiftRight)
        {
            return new AsBitVector(_solver).AsBool(logicalShiftRight);
        }

        public BoolExpr Map(LogicalXor logicalXor)
        {
            return MapBoolOperands(logicalXor, (l, r) => _solver.Context.MkXor(l, r));
        }

        public BoolExpr Map(Multiply multiply)
        {
            return new AsBitVector(_solver).AsBool(multiply);
        }

        public BoolExpr Map(Or or)
        {
            return new AsBitVector(_solver).AsBool(or);
        }

        public BoolExpr Map(ShiftLeft shiftLeft)
        {
            return new AsBitVector(_solver).AsBool(shiftLeft);
        }

        public BoolExpr Map(SignedDivide signedDivide)
        {
            return new AsBitVector(_solver).AsBool(signedDivide);
        }

        public BoolExpr Map(SignedGreater signedGreater)
        {
            return MapBitVectorOperands(signedGreater, _solver.Context.MkBVSGT);
        }

        public BoolExpr Map(SignedGreaterOrEqual signedGreaterOrEqual)
        {
            return MapBitVectorOperands(signedGreaterOrEqual, _solver.Context.MkBVSGE);
        }

        public BoolExpr Map(SignedLess signedLess)
        {
            return MapBitVectorOperands(signedLess, _solver.Context.MkBVSLT);
        }

        public BoolExpr Map(SignedLessOrEqual signedLessOrEqual)
        {
            return MapBitVectorOperands(signedLessOrEqual, _solver.Context.MkBVSLE);
        }

        public BoolExpr Map(SignedRemainder signedRemainder)
        {
            return new AsBitVector(_solver).AsBool(signedRemainder);
        }

        public BoolExpr Map(Subtract subtract)
        {
            return new AsBitVector(_solver).AsBool(subtract);
        }

        public BoolExpr Map(UnsignedDivide unsignedDivide)
        {
            return new AsBitVector(_solver).AsBool(unsignedDivide);
        }

        public BoolExpr Map(UnsignedGreater unsignedGreater)
        {
            return MapBitVectorOperands(unsignedGreater, _solver.Context.MkBVUGT);
        }

        public BoolExpr Map(UnsignedGreaterOrEqual unsignedGreaterOrEqual)
        {
            return MapBitVectorOperands(unsignedGreaterOrEqual, _solver.Context.MkBVUGE);
        }

        public BoolExpr Map(UnsignedLess unsignedLess)
        {
            return MapBitVectorOperands(unsignedLess, _solver.Context.MkBVULT);
        }

        public BoolExpr Map(UnsignedLessOrEqual unsignedLessOrEqual)
        {
            return MapBitVectorOperands(unsignedLessOrEqual, _solver.Context.MkBVULE);
        }

        public BoolExpr Map(UnsignedRemainder unsignedRemainder)
        {
            return new AsBitVector(_solver).AsBool(unsignedRemainder);
        }

        public BoolExpr Map(Xor xor)
        {
            return new AsBitVector(_solver).AsBool(xor);
        }

        private BoolExpr MapBoolOperands(
            IBinaryExpr expression,
            Func<BoolExpr, BoolExpr, BoolExpr> createExpr)
        {
            return MapBinaryOperands(expression, new AsBool(_solver), createExpr);
        }

        private BoolExpr MapBitVectorOperands(
            IBinaryExpr expression,
            Func<BitVecExpr, BitVecExpr, BoolExpr> createExpr)
        {
            return MapBinaryOperands(expression, new AsBitVector(_solver), createExpr);
        }

        private BoolExpr MapFloatOperands(
            IBinaryExpr expression,
            Func<FPExpr, FPExpr, BoolExpr> createExpr)
        {
            return MapBinaryOperands(expression, new AsFloat(_solver), createExpr);
        }

        private static BoolExpr MapBinaryOperands<T>(
            IBinaryExpr expression,
            IExprMapper<T> operandMapper,
            Func<T, T, BoolExpr> createExpr)
            where T : Expr
        {
            using var left = expression.Left.Map(operandMapper);
            using var right = expression.Right.Map(operandMapper);
            return createExpr(left, right);
        }
    }
}
