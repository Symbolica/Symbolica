using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Exceptions;
using Symbolica.Expression;
using Symbolica.Expression.Values;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Computation;

internal sealed class AsBitVector : ITypeMapper<BitVecExpr>
{
    private readonly ISolver _solver;

    public AsBitVector(ISolver solver)
    {
        _solver = solver;
    }

    internal BoolExpr AsBool(IExpression<BitVector> expression)
    {
        using var bitVector = expression.Map(this);
        using var zero = _solver.Context.MkBV(0U, (uint) expression.Size);
        using var isZero = _solver.Context.MkEq(bitVector, zero);
        return _solver.Context.MkNot(isZero);
    }

    public BitVecExpr Map(IIntegerExpression integer)
    {
        return integer.Map(new IntegerAsBitVector(_solver));
    }

    internal FPExpr AsFloat(IExpression<IInteger> expression)
    {
        using var bitVector = expression.Map(this);
        using var sort = expression.Type.GetSort(_solver);
        return _solver.Context.MkFPToFP(bitVector, sort);
    }

    public BitVecExpr Map(IFloatExpression @float)
    {
        return new AsFloat(_solver).AsBitVector(@float);
    }

    public BitVecExpr Map(IRealExpression real)
    {
        throw new UnsupportedSymbolicArithmeticException();
    }

    public BitVecExpr Map(Select select)
    {
        using var predicate = select.Predicate.Map(new AsBool(_solver));
        using var trueValue = select.TrueValue.Map(this);
        using var falseValue = select.FalseValue.Map(this);
        return (BitVecExpr) _solver.Context.MkITE(predicate, trueValue, falseValue);
    }

    private sealed class IntegerAsBitVector : IIntegerMapper<BitVecExpr>
    {
        private readonly ISolver _solver;

        public IntegerAsBitVector(ISolver solver)
        {
            _solver = solver;
        }

        public BitVecExpr Map(IBitVectorExpression bitVector)
        {
            return bitVector.Map(new BitVectorAsBitVector(_solver));
        }

        public BitVecExpr Map(IBoolExpression @bool)
        {
            return new AsBool(_solver).AsBitVector(@bool);
        }
    }

    private sealed class BitVectorAsBitVector : IBitVectorMapper<BitVecExpr>
    {
        private readonly ISolver _solver;

        public BitVectorAsBitVector(ISolver solver)
        {
            _solver = solver;
        }

        public BitVecExpr Map(ConstantBitVector constantBitVector)
        {
            return Map(constantBitVector.AsUnsigned());
        }

        public BitVecExpr Map(ConstantSigned constantSigned)
        {
            return Map(constantSigned.AsUnsigned());
        }

        public BitVecExpr Map(ConstantUnsigned constantUnsigned)
        {
            return _solver.Context.MkBV(
                ((BigInteger) constantUnsigned).ToString(),
                (uint) constantUnsigned.Type.Size);
        }

        public BitVecExpr Map(Add add)
        {
            return MapBitVectorOperands(add, _solver.Context.MkBVAdd);
        }

        public BitVecExpr Map(Address address)
        {
            return address.ToBitVector().Map(new AsBitVector(_solver));
        }

        public BitVecExpr Map(And and)
        {
            return MapBitVectorOperands(and, _solver.Context.MkBVAND);
        }

        public BitVecExpr Map(ArithmeticShiftRight arithmeticShiftRight)
        {
            return MapBitVectorOperands(arithmeticShiftRight, _solver.Context.MkBVASHR);
        }

        public BitVecExpr Map(FloatToSigned floatToSigned)
        {
            return MkFPToBv(floatToSigned, true);
        }

        public BitVecExpr Map(FloatToUnsigned floatToUnsigned)
        {
            return MkFPToBv(floatToUnsigned, false);
        }

        public BitVecExpr Map(LogicalShiftRight logicalShiftRight)
        {
            return MapBitVectorOperands(logicalShiftRight, _solver.Context.MkBVLSHR);
        }

        public BitVecExpr Map(Multiply multiply)
        {
            return MapBitVectorOperands(multiply, _solver.Context.MkBVMul);
        }

        public BitVecExpr Map(Not not)
        {
            return MapBitVectorOperand(not, _solver.Context.MkBVNot);
        }

        public BitVecExpr Map(Or or)
        {
            return MapBitVectorOperands(or, _solver.Context.MkBVOR);
        }

        public BitVecExpr Map(RealToSigned realToSigned)
        {
            using var value = realToSigned.Map(new AsReal(_solver));
            using var intValue = _solver.Context.MkReal2Int(value);
            return _solver.Context.MkInt2BV((uint) realToSigned.Type.Size, intValue);
        }

        public BitVecExpr Map(ShiftLeft shiftLeft)
        {
            return MapBitVectorOperands(shiftLeft, _solver.Context.MkBVSHL);
        }

        public BitVecExpr Map(SignedDivide signedDivide)
        {
            return MapBitVectorOperands(signedDivide, _solver.Context.MkBVSDiv);
        }

        public BitVecExpr Map(SignedRemainder signedRemainder)
        {
            return MapBitVectorOperands(signedRemainder, _solver.Context.MkBVSRem);
        }

        public BitVecExpr Map(SignExtend signExtend)
        {
            return MapBitVectorOperand(
                signExtend,
                v => _solver.Context.MkSignExt((uint) (signExtend.Type.Size - signExtend.Value.Size), v));
        }

        public BitVecExpr Map(Subtract subtract)
        {
            return MapBitVectorOperands(subtract, _solver.Context.MkBVSub);
        }

        public BitVecExpr Map(Expression.Symbol symbol)
        {
            _solver.Assert(symbol.Name, symbol.Assertions);

            return _solver.Context.MkBVConst(symbol.Name, (uint) symbol.Type.Size);
        }

        public BitVecExpr Map(Truncate truncate)
        {
            return MapBitVectorOperand(
                truncate,
                v => _solver.Context.MkExtract((uint) (truncate.Type.Size - Bits.One), 0U, v));
        }

        public BitVecExpr Map(UnsignedDivide unsignedDivide)
        {
            return MapBitVectorOperands(unsignedDivide, _solver.Context.MkBVUDiv);
        }

        public BitVecExpr Map(UnsignedRemainder unsignedRemainder)
        {
            return MapBitVectorOperands(unsignedRemainder, _solver.Context.MkBVURem);
        }

        public BitVecExpr Map(Write write)
        {
            return write.Flatten().Map(new AsBitVector(_solver));
        }

        public BitVecExpr Map(Xor xor)
        {
            return MapBitVectorOperands(xor, _solver.Context.MkBVXOR);
        }

        public BitVecExpr Map(ZeroExtend zeroExtend)
        {
            return MapBitVectorOperand(
                zeroExtend,
                v => _solver.Context.MkZeroExt((uint) (zeroExtend.Type.Size - zeroExtend.Value.Size), v));
        }

        private BitVecExpr MapBitVectorOperand(
            IUnaryBitVectorExpression expression,
            Func<BitVecExpr, BitVecExpr> createExpr)
        {
            return expression.Map(new AsBitVector(_solver), createExpr);
        }

        private BitVecExpr MapBitVectorOperands(
            IBinaryBitVectorExpression expression,
            Func<BitVecExpr, BitVecExpr, BitVecExpr> createExpr)
        {
            return expression.Map(new AsBitVector(_solver), createExpr);
        }

        private BitVecExpr MkFPToBv(IUnaryBitVectorExpression expression, bool signed)
        {
            return expression.Map(new AsFloat(_solver), v =>
            {
                using var rounding = _solver.Context.MkFPRTZ();
                return _solver.Context.MkFPToBV(
                    rounding,
                    v,
                    (uint) (expression as IExpression<BitVector>).Size,
                    signed);
            });
        }
    }
}
