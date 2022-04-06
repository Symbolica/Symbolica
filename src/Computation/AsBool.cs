using System;
using Microsoft.Z3;
using Symbolica.Computation.Exceptions;
using Symbolica.Expression;
using Symbolica.Expression.Values;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Computation;

internal sealed class AsBool : ITypeMapper<BoolExpr>
{
    private readonly ISolver _solver;

    public AsBool(ISolver solver)
    {
        _solver = solver;
    }

    internal BitVecExpr AsBitVector(IExpression<Bool> expression)
    {
        using var predicate = expression.Map(this);
        using var @true = _solver.Context.MkBV(new[] { true });
        using var @false = _solver.Context.MkBV(new[] { false });
        return (BitVecExpr) _solver.Context.MkITE(predicate, @true, @false);
    }

    public BoolExpr Map(Select select)
    {
        using var predicate = select.Predicate.Map(this);
        using var trueValue = select.TrueValue.Map(this);
        using var falseValue = select.FalseValue.Map(this);
        return (BoolExpr) _solver.Context.MkITE(predicate, trueValue, falseValue);
    }

    public BoolExpr Map(IIntegerExpression integer)
    {
        return integer.Map(new IntegerAsBool(_solver));
    }

    public BoolExpr Map(IBoolExpression @bool)
    {
        return @bool.Map(new BoolAsBool(_solver));
    }

    public BoolExpr Map(IFloatExpression @float)
    {
        return new AsFloat(_solver).AsBool(@float);
    }

    public BoolExpr Map(IRealExpression real)
    {
        throw new UnsupportedSymbolicArithmeticException();
    }

    private sealed class IntegerAsBool : IIntegerMapper<BoolExpr>
    {
        private readonly ISolver _solver;

        public IntegerAsBool(ISolver solver)
        {
            _solver = solver;
        }

        public BoolExpr Map(IBitVectorExpression bitVector)
        {
            return new AsBitVector(_solver).AsBool(bitVector);
        }

        public BoolExpr Map(IBoolExpression @bool)
        {
            return @bool.Map(new BoolAsBool(_solver));
        }
    }

    private sealed class BoolAsBool : IBoolMapper<BoolExpr>
    {
        private readonly ISolver _solver;

        public BoolAsBool(ISolver solver)
        {
            _solver = solver;
        }

        public BoolExpr Map(ConstantBool constantBool)
        {
            return _solver.Context.MkBool(constantBool);
        }

        public BoolExpr Map(Equal equal)
        {
            // TODO: Can we now make this type safe by that the left and right are of the correct type?
            return equal.Left is Bool || equal.Right is Bool
                ? MapBoolOperands(equal, _solver.Context.MkEq)
                : MapBitVectorOperands(equal, _solver.Context.MkEq);
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

        public BoolExpr Map(FloatUnordered floatUnordered)
        {
            return MapFloatOperands(floatUnordered, (left, right) =>
            {
                using var leftIsNaN = _solver.Context.MkFPIsNaN(left);
                using var rightIsNaN = _solver.Context.MkFPIsNaN(right);
                return _solver.Context.MkOr(leftIsNaN, rightIsNaN);
            });
        }

        public BoolExpr Map(Logical logical)
        {
            return logical.Value.Map(new AsBool(_solver));
        }

        public BoolExpr Map(LogicalAnd logicalAnd)
        {
            return MapBoolOperands(logicalAnd, (l, r) => _solver.Context.MkAnd(l, r));
        }

        public BoolExpr Map(LogicalNot logicalNot)
        {
            using var value = logicalNot.Value.Map(new AsBool(_solver));
            return _solver.Context.MkNot(value);
        }

        public BoolExpr Map(LogicalOr logicalOr)
        {
            return MapBoolOperands(logicalOr, (l, r) => _solver.Context.MkOr(l, r));
        }

        public BoolExpr Map(LogicalXor logicalXor)
        {
            return MapBoolOperands(logicalXor, (l, r) => _solver.Context.MkXor(l, r));
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

        private BoolExpr MapBoolOperands(
            IBinaryBoolExpression expression,
            Func<BoolExpr, BoolExpr, BoolExpr> createExpr)
        {
            return expression.Map(new AsBool(_solver), createExpr);
        }

        private BoolExpr MapBitVectorOperands(
            IBinaryBoolExpression expression,
            Func<BitVecExpr, BitVecExpr, BoolExpr> createExpr)
        {
            return expression.Map(new AsBitVector(_solver), createExpr);
        }

        private BoolExpr MapFloatOperands(
            IBinaryBoolExpression expression,
            Func<FPExpr, FPExpr, BoolExpr> createExpr)
        {
            return expression.Map(new AsFloat(_solver), createExpr);
        }
    }
}
