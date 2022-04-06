using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Exceptions;
using Symbolica.Expression;
using Symbolica.Expression.Values;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Computation;

internal sealed class AsFloat : ITypeMapper<FPExpr>
{
    private readonly ISolver _solver;

    public AsFloat(ISolver solver)
    {
        _solver = solver;
    }

    internal BoolExpr AsBool(IExpression<Float> expression)
    {
        using var flt = expression.Map(this);
        using var isZero = _solver.Context.MkFPIsZero(flt);
        return _solver.Context.MkNot(isZero);
    }

    public BitVecExpr AsBitVector(IExpression<Float> expression)
    {
        using var flt = expression.Map(this);
        using var simplified = flt.Simplify();

        return simplified.IsFPNaN
            ? CreateNan(expression.Type)
            : _solver.Context.MkFPToIEEEBV(flt);
    }

    private BitVecExpr CreateNan(Float @float)
    {
        using var sort = @float.GetSort(_solver);
        var nan = ((BigInteger.One << ((int) sort.EBits + 2)) - BigInteger.One) << ((int) sort.SBits - 2);

        return _solver.Context.MkBV(nan.ToString(), (uint) @float.Size);
    }

    public FPExpr Map(IIntegerExpression integer)
    {
        return new AsBitVector(_solver).AsFloat(integer);
    }

    public FPExpr Map(IFloatExpression @float)
    {
        return @float.Map(new FloatAsFloat(_solver));
    }

    public FPExpr Map(IRealExpression real)
    {
        throw new UnsupportedSymbolicArithmeticException();
    }

    public FPExpr Map(Select select)
    {
        using var predicate = select.Predicate.Map(new AsBool(_solver));
        using var trueValue = select.TrueValue.Map(this);
        using var falseValue = select.FalseValue.Map(this);
        return (FPExpr) _solver.Context.MkITE(predicate, trueValue, falseValue);
    }

    private sealed class FloatAsFloat : IFloatMapper<FPExpr>
    {
        private readonly ISolver _solver;

        public FloatAsFloat(ISolver solver)
        {
            _solver = solver;
        }

        public FPExpr Map(ConstantDouble constantDouble)
        {
            using var sort = constantDouble.Type.GetSort(_solver);
            return _solver.Context.MkFP(constantDouble, sort);
        }

        public FPExpr Map(ConstantSingle constantSingle)
        {
            using var sort = constantSingle.Type.GetSort(_solver);
            return _solver.Context.MkFP(constantSingle, sort);
        }

        public FPExpr Map(FloatAdd floatAdd)
        {
            return MapFloatOperands(floatAdd, _solver.Context.MkFPRNE, _solver.Context.MkFPAdd);
        }

        public FPExpr Map(FloatCeiling floatCeiling)
        {
            return MapFloatOperand(
                floatCeiling,
                _solver.Context.MkFPRTP,
                _solver.Context.MkFPRoundToIntegral);
        }

        public FPExpr Map(FloatConvert floatConvert)
        {
            return MapFloatOperand(floatConvert, _solver.Context.MkFPRNE, _solver.Context.MkFPToFP);
        }

        public FPExpr Map(FloatDivide floatDivide)
        {
            return MapFloatOperands(floatDivide, _solver.Context.MkFPRNE, _solver.Context.MkFPDiv);
        }

        public FPExpr Map(FloatFloor floatFloor)
        {
            return MapFloatOperand(
                floatFloor,
                _solver.Context.MkFPRTN,
                _solver.Context.MkFPRoundToIntegral);
        }

        public FPExpr Map(FloatMultiply floatMultiply)
        {
            return MapFloatOperands(floatMultiply, _solver.Context.MkFPRNE, _solver.Context.MkFPMul);
        }

        public FPExpr Map(FloatNegate floatNegate)
        {
            return MapFloatOperand(floatNegate, _solver.Context.MkFPNeg);
        }

        public FPExpr Map(FloatRemainder floatRemainder)
        {
            return MapFloatOperands(floatRemainder, _solver.Context.MkFPRem);
        }

        public FPExpr Map(FloatSubtract floatSubtract)
        {
            return MapFloatOperands(floatSubtract, _solver.Context.MkFPRNE, _solver.Context.MkFPSub);
        }

        public FPExpr Map(NormalFloat normalFloat)
        {
            using var sort = normalFloat.Type.GetSort(_solver);
            var (sign, significand, exponent) = ParseDecimal(normalFloat.Value);

            if (significand.IsZero)
                return _solver.Context.MkFPZero(sort, sign);

            var (numerator, denominator) = exponent < 0
                ? (significand, BigInteger.Pow(10, -exponent))
                : (significand * BigInteger.Pow(10, exponent), BigInteger.One);

            var precision = (int) sort.SBits;
            var scale = precision - 1L;

            var lower = BigInteger.One << (precision - 1);
            while (numerator / denominator < lower)
            {
                numerator <<= 1;
                --scale;
            }

            var upper = BigInteger.One << precision;
            while (upper <= numerator / denominator)
            {
                denominator <<= 1;
                ++scale;
            }

            var quotient = numerator / denominator;
            var remainder = numerator % denominator;
            var midpoint = denominator >> 1;

            var (rounded, shift) = remainder > midpoint || remainder == midpoint && !quotient.IsEven
                ? quotient == upper - BigInteger.One
                    ? (lower, scale + 1L)
                    : (quotient + BigInteger.One, scale)
                : (quotient, scale);

            return _solver.Context.MkFP(sign, shift, (ulong) (rounded % lower), sort);
        }

        public FPExpr Map(SignedToFloat signedToFloat)
        {
            return MkFPToFP(signedToFloat, true);
        }

        public FPExpr Map(UnsignedToFloat unsignedToFloat)
        {
            return MkFPToFP(unsignedToFloat, false);
        }

        private FPExpr MkFPToFP(IUnaryFloatExpression expression, bool signed)
        {
            using var rounding = _solver.Context.MkFPRNE();
            using var value = expression.Value.Map(new AsBitVector(_solver));
            using var sort = expression.Type.GetSort(_solver);
            return _solver.Context.MkFPToFP(rounding, value, sort, signed);
        }

        private FPExpr MapFloatOperand(
            IUnaryFloatExpression expression,
            Func<FPExpr, FPExpr> createExpr)
        {
            return expression.Map(new AsFloat(_solver), createExpr);
        }

        private FPExpr MapFloatOperand(
            IUnaryFloatExpression expression,
            Func<FPRMExpr> mkRounding,
            Func<FPRMExpr, FPExpr, FPExpr> createExpr)
        {
            return MapFloatOperand(
                expression,
                v =>
                {
                    using var rounding = mkRounding();
                    return createExpr(rounding, v);
                });
        }

        private FPExpr MapFloatOperand(
            IUnaryFloatExpression expression,
            Func<FPRMExpr> mkRounding,
            Func<FPRMExpr, FPExpr, FPSort, FPExpr> createExpr)
        {
            return MapFloatOperand(
                expression,
                mkRounding,
                (rounding, v) =>
                {
                    using var sort = expression.Type.GetSort(_solver);
                    return createExpr(rounding, v, sort);
                });
        }

        private FPExpr MapFloatOperands(
            IBinaryFloatExpression expression,
            Func<FPExpr, FPExpr, FPExpr> createExpr)
        {
            return expression.Map(new AsFloat(_solver), createExpr);
        }

        private FPExpr MapFloatOperands(
            IBinaryFloatExpression expression,
            Func<FPRMExpr> mkRounding,
            Func<FPRMExpr, FPExpr, FPExpr, FPExpr> createExpr)
        {
            return MapFloatOperands(
                expression,
                (l, r) =>
                {
                    using var rounding = mkRounding();
                    return createExpr(rounding, l, r);
                });
        }

        private static (BigInteger, int) ParseStandardNonNegativeDecimal(string value, int exponent)
        {
            var index = value.IndexOf('.');

            return index == -1
                ? (BigInteger.Parse(value), exponent)
                : (BigInteger.Parse(value[..index] + value[(index + 1)..]), exponent - (value.Length - 1 - index));
        }

        private static (BigInteger, int) ParseNonNegativeDecimal(string value)
        {
            var index = value.IndexOfAny(new[] { 'e', 'E' });

            return index == -1
                ? ParseStandardNonNegativeDecimal(value, 0)
                : ParseStandardNonNegativeDecimal(value[..index], int.Parse(value[(index + 1)..]));
        }

        private static (bool, BigInteger, int) ParseDecimal(string value)
        {
            var sign = value[0] == '-';

            var (significand, exponent) = sign
                ? ParseNonNegativeDecimal(value[1..])
                : ParseNonNegativeDecimal(value);

            return (sign, significand, exponent);
        }
    }
}
