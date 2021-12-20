using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Symbolica.Collection;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation
{
    public class ExpressionTests
    {
        private static readonly IContextFactory ContextFactory = new SharedContextFactory();
        private static readonly ICollectionFactory CollectionFactory = new CollectionFactory();

        private static IExpression Create(IValue value)
        {
            return Expression.Create(ContextFactory, CollectionFactory,
                value, null);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForAdd(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.Add(constantRight).Constant;
            var symbolic = symbolicLeft.Add(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForAnd(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.And(constantRight).Constant;
            var symbolic = symbolicLeft.And(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForArithmeticShiftRight(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.ArithmeticShiftRight(constantRight).Constant;
            var symbolic = symbolicLeft.ArithmeticShiftRight(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForEqual(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.Equal(constantRight).Constant;
            var symbolic = symbolicLeft.Equal(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(SingleBinaryTestData))]
        [ClassData(typeof(DoubleBinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatAdd(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.FloatAdd(constantRight).GetNanNormalizedConstant(ContextFactory);
            var symbolic = symbolicLeft.FloatAdd(symbolicRight).GetNanNormalizedConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(SingleUnaryTestData))]
        [ClassData(typeof(DoubleUnaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatCeiling(
            IExpression constantExpression,
            IExpression symbolicExpression)
        {
            var constant = constantExpression.FloatCeiling().GetNanNormalizedConstant(ContextFactory);
            var symbolic = symbolicExpression.FloatCeiling().GetNanNormalizedConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(SingleConvertTestData))]
        [ClassData(typeof(DoubleConvertTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatConvert(Bits size,
            IExpression constantExpression,
            IExpression symbolicExpression)
        {
            var constant = constantExpression.FloatConvert(size).GetNanNormalizedConstant(ContextFactory);
            var symbolic = symbolicExpression.FloatConvert(size).GetNanNormalizedConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(SingleBinaryTestData))]
        [ClassData(typeof(DoubleBinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatDivide(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.FloatDivide(constantRight).GetNanNormalizedConstant(ContextFactory);
            var symbolic = symbolicLeft.FloatDivide(symbolicRight).GetNanNormalizedConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(SingleBinaryTestData))]
        [ClassData(typeof(DoubleBinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatEqual(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.FloatEqual(constantRight).Constant;
            var symbolic = symbolicLeft.FloatEqual(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(SingleUnaryTestData))]
        [ClassData(typeof(DoubleUnaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatFloor(
            IExpression constantExpression,
            IExpression symbolicExpression)
        {
            var constant = constantExpression.FloatFloor().GetNanNormalizedConstant(ContextFactory);
            var symbolic = symbolicExpression.FloatFloor().GetNanNormalizedConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(SingleBinaryTestData))]
        [ClassData(typeof(DoubleBinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatGreater(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.FloatGreater(constantRight).Constant;
            var symbolic = symbolicLeft.FloatGreater(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(SingleBinaryTestData))]
        [ClassData(typeof(DoubleBinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatGreaterOrEqual(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.FloatGreaterOrEqual(constantRight).Constant;
            var symbolic = symbolicLeft.FloatGreaterOrEqual(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(SingleBinaryTestData))]
        [ClassData(typeof(DoubleBinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatLess(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.FloatLess(constantRight).Constant;
            var symbolic = symbolicLeft.FloatLess(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(SingleBinaryTestData))]
        [ClassData(typeof(DoubleBinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatLessOrEqual(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.FloatLessOrEqual(constantRight).Constant;
            var symbolic = symbolicLeft.FloatLessOrEqual(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(SingleBinaryTestData))]
        [ClassData(typeof(DoubleBinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatMultiply(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.FloatMultiply(constantRight).GetNanNormalizedConstant(ContextFactory);
            var symbolic = symbolicLeft.FloatMultiply(symbolicRight).GetNanNormalizedConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(SingleUnaryTestData))]
        [ClassData(typeof(DoubleUnaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatNegate(
            IExpression constantExpression,
            IExpression symbolicExpression)
        {
            var constant = constantExpression.FloatNegate().GetNanNormalizedConstant(ContextFactory);
            var symbolic = symbolicExpression.FloatNegate().GetNanNormalizedConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(SingleBinaryTestData))]
        [ClassData(typeof(DoubleBinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatNotEqual(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.FloatNotEqual(constantRight).Constant;
            var symbolic = symbolicLeft.FloatNotEqual(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(SingleBinaryTestData))]
        [ClassData(typeof(DoubleBinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatOrdered(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.FloatOrdered(constantRight).Constant;
            var symbolic = symbolicLeft.FloatOrdered(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(SinglePowerTestData))]
        [ClassData(typeof(DoublePowerTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatPower(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.FloatPower(constantRight).FloatToSigned(constantLeft.Size).Constant;
            var symbolic = symbolicLeft.FloatPower(symbolicRight).FloatToSigned(symbolicLeft.Size).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(SingleBinaryTestData))]
        [ClassData(typeof(DoubleBinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatRemainder(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.FloatRemainder(constantRight).GetNanNormalizedConstant(ContextFactory);
            var symbolic = symbolicLeft.FloatRemainder(symbolicRight).GetNanNormalizedConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(SingleBinaryTestData))]
        [ClassData(typeof(DoubleBinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatSubtract(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.FloatSubtract(constantRight).GetNanNormalizedConstant(ContextFactory);
            var symbolic = symbolicLeft.FloatSubtract(symbolicRight).GetNanNormalizedConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(SingleToSignedTestData))]
        [ClassData(typeof(DoubleToSignedTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatToSigned(Bits size,
            IExpression constantExpression,
            IExpression symbolicExpression)
        {
            var constant = constantExpression.FloatToSigned(size).Constant;
            var symbolic = symbolicExpression.FloatToSigned(size).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(SingleToUnsignedTestData))]
        [ClassData(typeof(DoubleToUnsignedTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatToUnsigned(Bits size,
            IExpression constantExpression,
            IExpression symbolicExpression)
        {
            var constant = constantExpression.FloatToUnsigned(size).Constant;
            var symbolic = symbolicExpression.FloatToUnsigned(size).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(SingleBinaryTestData))]
        [ClassData(typeof(DoubleBinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatUnordered(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.FloatUnordered(constantRight).Constant;
            var symbolic = symbolicLeft.FloatUnordered(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForLogicalShiftRight(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.LogicalShiftRight(constantRight).Constant;
            var symbolic = symbolicLeft.LogicalShiftRight(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForMultiply(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.Multiply(constantRight).Constant;
            var symbolic = symbolicLeft.Multiply(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForNotEqual(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.NotEqual(constantRight).Constant;
            var symbolic = symbolicLeft.NotEqual(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForOr(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.Or(constantRight).Constant;
            var symbolic = symbolicLeft.Or(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(ReadTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForRead(Bits size,
            IExpression constantBuffer, IExpression constantOffset,
            IExpression symbolicBuffer, IExpression symbolicOffset)
        {
            var constant = constantBuffer.Read(constantOffset, size).Constant;
            var symbolic = symbolicBuffer.Read(symbolicOffset, size).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForShiftLeft(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.ShiftLeft(constantRight).Constant;
            var symbolic = symbolicLeft.ShiftLeft(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSignedDivide(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            if (constantRight.Constant == 0)
                return;

            var constant = constantLeft.SignedDivide(constantRight).Constant;
            var symbolic = symbolicLeft.SignedDivide(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSignedGreater(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.SignedGreater(constantRight).Constant;
            var symbolic = symbolicLeft.SignedGreater(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSignedGreaterOrEqual(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.SignedGreaterOrEqual(constantRight).Constant;
            var symbolic = symbolicLeft.SignedGreaterOrEqual(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSignedLess(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.SignedLess(constantRight).Constant;
            var symbolic = symbolicLeft.SignedLess(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSignedLessOrEqual(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.SignedLessOrEqual(constantRight).Constant;
            var symbolic = symbolicLeft.SignedLessOrEqual(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSignedRemainder(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            if (constantRight.Constant == 0)
                return;

            var constant = constantLeft.SignedRemainder(constantRight).Constant;
            var symbolic = symbolicLeft.SignedRemainder(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(ToFloatTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSignedToFloat(Bits size,
            IExpression constantExpression,
            IExpression symbolicExpression)
        {
            var constant = constantExpression.SignedToFloat(size).Constant;
            var symbolic = symbolicExpression.SignedToFloat(size).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(ExtendTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSignExtend(Bits size,
            IExpression constantExpression,
            IExpression symbolicExpression)
        {
            var constant = constantExpression.SignExtend(size).Constant;
            var symbolic = symbolicExpression.SignExtend(size).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSubtract(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.Subtract(constantRight).Constant;
            var symbolic = symbolicLeft.Subtract(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(TruncateTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForTruncate(Bits size,
            IExpression constantExpression,
            IExpression symbolicExpression)
        {
            var constant = constantExpression.Truncate(size).Constant;
            var symbolic = symbolicExpression.Truncate(size).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForUnsignedDivide(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            if (constantRight.Constant == 0)
                return;

            var constant = constantLeft.UnsignedDivide(constantRight).Constant;
            var symbolic = symbolicLeft.UnsignedDivide(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForUnsignedGreater(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.UnsignedGreater(constantRight).Constant;
            var symbolic = symbolicLeft.UnsignedGreater(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForUnsignedGreaterOrEqual(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.UnsignedGreaterOrEqual(constantRight).Constant;
            var symbolic = symbolicLeft.UnsignedGreaterOrEqual(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForUnsignedLess(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.UnsignedLess(constantRight).Constant;
            var symbolic = symbolicLeft.UnsignedLess(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForUnsignedLessOrEqual(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.UnsignedLessOrEqual(constantRight).Constant;
            var symbolic = symbolicLeft.UnsignedLessOrEqual(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForUnsignedRemainder(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            if (constantRight.Constant == 0)
                return;

            var constant = constantLeft.UnsignedRemainder(constantRight).Constant;
            var symbolic = symbolicLeft.UnsignedRemainder(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(ToFloatTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForUnsignedToFloat(Bits size,
            IExpression constantExpression,
            IExpression symbolicExpression)
        {
            var constant = constantExpression.UnsignedToFloat(size).Constant;
            var symbolic = symbolicExpression.UnsignedToFloat(size).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(WriteTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForWrite(
            IExpression constantBuffer, IExpression constantOffset, IExpression constantValue,
            IExpression symbolicBuffer, IExpression symbolicOffset, IExpression symbolicValue)
        {
            var constant = constantBuffer.Write(constantOffset, constantValue).Constant;
            var symbolic = symbolicBuffer.Write(symbolicOffset, symbolicValue).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForXor(
            IExpression constantLeft, IExpression constantRight,
            IExpression symbolicLeft, IExpression symbolicRight)
        {
            var constant = constantLeft.Xor(constantRight).Constant;
            var symbolic = symbolicLeft.Xor(symbolicRight).Constant;

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(ExtendTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForZeroExtend(Bits size,
            IExpression constantExpression,
            IExpression symbolicExpression)
        {
            var constant = constantExpression.ZeroExtend(size).Constant;
            var symbolic = symbolicExpression.ZeroExtend(size).Constant;

            constant.Should().Be(symbolic);
        }

        private sealed class BinaryTestData : TheoryData<
            IExpression, IExpression,
            IExpression, IExpression>
        {
            public BinaryTestData()
            {
                foreach (var left in Values())
                foreach (var right in Values())
                    Add(Create(left),
                        Create(right),
                        Create(new SymbolicUnsigned(left)),
                        Create(new SymbolicUnsigned(right)));
            }

            private static IEnumerable<ConstantUnsigned> Values()
            {
                return Enumerable.Range(-8, 24).Select(v => ConstantUnsigned.Create((Bits) 4U, v));
            }
        }

        private sealed class ReadTestData : TheoryData<Bits,
            IExpression, IExpression,
            IExpression, IExpression>
        {
            public ReadTestData()
            {
                foreach (var buffer in Buffers())
                foreach (var offset in Offsets())
                foreach (var size in Sizes())
                    Add(size,
                        Create(buffer),
                        Create(offset),
                        Create(new SymbolicUnsigned(buffer)),
                        Create(new SymbolicUnsigned(offset)));
            }

            private static IEnumerable<ConstantUnsigned> Buffers()
            {
                yield return ConstantUnsigned.Create((Bits) 32U, int.MinValue);
                yield return ConstantUnsigned.Create((Bits) 32U, uint.MinValue);
                yield return ConstantUnsigned.Create((Bits) 32U, int.MaxValue);
                yield return ConstantUnsigned.Create((Bits) 32U, uint.MaxValue);
            }

            private static IEnumerable<ConstantUnsigned> Offsets()
            {
                yield return ConstantUnsigned.Create((Bits) 32U, 0U);
                yield return ConstantUnsigned.Create((Bits) 32U, 8U);
                yield return ConstantUnsigned.Create((Bits) 32U, 16U);
            }

            private static IEnumerable<Bits> Sizes()
            {
                yield return (Bits) 8U;
                yield return (Bits) 16U;
            }
        }

        private sealed class WriteTestData : TheoryData<
            IExpression, IExpression, IExpression,
            IExpression, IExpression, IExpression>
        {
            public WriteTestData()
            {
                foreach (var buffer in Buffers())
                foreach (var offset in Offsets())
                foreach (var value in Values())
                    Add(Create(buffer),
                        Create(offset),
                        Create(value),
                        Create(new SymbolicUnsigned(buffer)),
                        Create(new SymbolicUnsigned(offset)),
                        Create(new SymbolicUnsigned(value)));
            }

            private static IEnumerable<ConstantUnsigned> Buffers()
            {
                yield return ConstantUnsigned.Create((Bits) 32U, int.MinValue);
                yield return ConstantUnsigned.Create((Bits) 32U, uint.MinValue);
                yield return ConstantUnsigned.Create((Bits) 32U, int.MaxValue);
                yield return ConstantUnsigned.Create((Bits) 32U, uint.MaxValue);
            }

            private static IEnumerable<ConstantUnsigned> Offsets()
            {
                yield return ConstantUnsigned.Create((Bits) 32U, 0U);
                yield return ConstantUnsigned.Create((Bits) 32U, 8U);
                yield return ConstantUnsigned.Create((Bits) 32U, 16U);
            }

            private static IEnumerable<ConstantUnsigned> Values()
            {
                yield return ConstantUnsigned.Create((Bits) 16U, short.MinValue);
                yield return ConstantUnsigned.Create((Bits) 16U, ushort.MinValue);
                yield return ConstantUnsigned.Create((Bits) 16U, short.MaxValue);
                yield return ConstantUnsigned.Create((Bits) 16U, ushort.MaxValue);
            }
        }

        private sealed class ExtendTestData : TheoryData<Bits,
            IExpression,
            IExpression>
        {
            public ExtendTestData()
            {
                foreach (var size in Sizes())
                foreach (var value in Values())
                    Add(size,
                        Create(value),
                        Create(new SymbolicUnsigned(value)));
            }

            private static IEnumerable<Bits> Sizes()
            {
                return Enumerable.Range(4, 5).Select(s => (Bits) (uint) s);
            }

            private static IEnumerable<ConstantUnsigned> Values()
            {
                return Enumerable.Range(-8, 24).Select(v => ConstantUnsigned.Create((Bits) 4U, v));
            }
        }

        private sealed class TruncateTestData : TheoryData<Bits,
            IExpression,
            IExpression>
        {
            public TruncateTestData()
            {
                foreach (var size in Sizes())
                foreach (var value in Values())
                    Add(size,
                        Create(value),
                        Create(new SymbolicUnsigned(value)));
            }

            private static IEnumerable<Bits> Sizes()
            {
                return Enumerable.Range(1, 4).Select(s => (Bits) (uint) s);
            }

            private static IEnumerable<ConstantUnsigned> Values()
            {
                return Enumerable.Range(-8, 24).Select(v => ConstantUnsigned.Create((Bits) 4U, v));
            }
        }

        private sealed class ToFloatTestData : TheoryData<Bits,
            IExpression,
            IExpression>
        {
            public ToFloatTestData()
            {
                foreach (var size in Sizes())
                foreach (var value in Values())
                    Add(size,
                        Create(value),
                        Create(new SymbolicUnsigned(value)));
            }

            private static IEnumerable<Bits> Sizes()
            {
                yield return (Bits) 16U;
                yield return (Bits) 32U;
                yield return (Bits) 64U;
                yield return (Bits) 80U;
                yield return (Bits) 128U;
            }

            private static IEnumerable<ConstantUnsigned> Values()
            {
                return Enumerable.Range(-10, 10).Select(v => ConstantUnsigned.Create((Bits) 32U, v));
            }
        }

        private sealed class SingleUnaryTestData : TheoryData<
            IExpression,
            IExpression>
        {
            public SingleUnaryTestData()
            {
                foreach (var value in Values())
                    Add(Create(new ConstantSingle(value)),
                        Create(new SymbolicSingle(value)));
            }

            private static IEnumerable<float> Values()
            {
                yield return 0f;
                yield return float.Epsilon;
                yield return float.NaN;
                yield return float.NegativeInfinity;
                yield return float.MinValue;
                yield return float.MaxValue;
                yield return float.PositiveInfinity;

                foreach (var i in Enumerable.Range(-10, 10))
                {
                    yield return i / 3f;
                    yield return 3f / i;
                }

                foreach (var i in Enumerable.Range(1, 10))
                {
                    yield return i / 3f;
                    yield return 3f / i;
                }
            }
        }

        private sealed class SingleBinaryTestData : TheoryData<
            IExpression, IExpression,
            IExpression, IExpression>
        {
            public SingleBinaryTestData()
            {
                foreach (var left in Values())
                foreach (var right in Values())
                    Add(Create(new ConstantSingle(left)),
                        Create(new ConstantSingle(right)),
                        Create(new SymbolicSingle(left)),
                        Create(new SymbolicSingle(right)));
            }

            private static IEnumerable<float> Values()
            {
                yield return 0f;
                yield return float.Epsilon;
                yield return float.NaN;
                yield return float.NegativeInfinity;
                yield return float.MinValue;
                yield return float.MaxValue;
                yield return float.PositiveInfinity;

                foreach (var i in Enumerable.Range(-10, 10))
                {
                    yield return i / 3f;
                    yield return 3f / i;
                }

                foreach (var i in Enumerable.Range(1, 10))
                {
                    yield return i / 3f;
                    yield return 3f / i;
                }
            }
        }

        private sealed class SinglePowerTestData : TheoryData<
            IExpression, IExpression,
            IExpression, IExpression>
        {
            public SinglePowerTestData()
            {
                foreach (var left in Values())
                foreach (var right in Values())
                    Add(Create(new ConstantSingle(left)),
                        Create(new ConstantSingle(right)),
                        Create(new SymbolicSingle(left)),
                        Create(new SymbolicSingle(right)));
            }

            private static IEnumerable<float> Values()
            {
                return Enumerable.Range(1, 5).Select(i => (float) i);
            }
        }

        private sealed class SingleConvertTestData : TheoryData<Bits,
            IExpression,
            IExpression>
        {
            public SingleConvertTestData()
            {
                foreach (var size in Sizes())
                foreach (var value in Values())
                    Add(size,
                        Create(new ConstantSingle(value)),
                        Create(new SymbolicSingle(value)));
            }

            private static IEnumerable<Bits> Sizes()
            {
                yield return (Bits) 16U;
                yield return (Bits) 32U;
                yield return (Bits) 64U;
                yield return (Bits) 80U;
                yield return (Bits) 128U;
            }

            private static IEnumerable<float> Values()
            {
                yield return 0f;
                yield return float.Epsilon;
                yield return float.NaN;
                yield return float.NegativeInfinity;
                yield return float.MinValue;
                yield return float.MaxValue;
                yield return float.PositiveInfinity;

                foreach (var i in Enumerable.Range(-10, 10))
                {
                    yield return i / 3f;
                    yield return 3f / i;
                }

                foreach (var i in Enumerable.Range(1, 10))
                {
                    yield return i / 3f;
                    yield return 3f / i;
                }
            }
        }

        private sealed class SingleToSignedTestData : TheoryData<Bits,
            IExpression,
            IExpression>
        {
            public SingleToSignedTestData()
            {
                foreach (var size in Sizes())
                foreach (var value in Values())
                    Add(size,
                        Create(new ConstantSingle(value)),
                        Create(new SymbolicSingle(value)));
            }

            private static IEnumerable<Bits> Sizes()
            {
                yield return (Bits) 16U;
                yield return (Bits) 32U;
                yield return (Bits) 64U;
                yield return (Bits) 80U;
                yield return (Bits) 128U;
            }

            private static IEnumerable<float> Values()
            {
                yield return 0f;
                yield return float.Epsilon;

                foreach (var i in Enumerable.Range(-10, 10))
                {
                    yield return i / 3f;
                    yield return 3f / i;
                }

                foreach (var i in Enumerable.Range(1, 10))
                {
                    yield return i / 3f;
                    yield return 3f / i;
                }
            }
        }

        private sealed class SingleToUnsignedTestData : TheoryData<Bits,
            IExpression,
            IExpression>
        {
            public SingleToUnsignedTestData()
            {
                foreach (var size in Sizes())
                foreach (var value in Values())
                    Add(size,
                        Create(new ConstantSingle(value)),
                        Create(new SymbolicSingle(value)));
            }

            private static IEnumerable<Bits> Sizes()
            {
                yield return (Bits) 16U;
                yield return (Bits) 32U;
                yield return (Bits) 64U;
                yield return (Bits) 80U;
                yield return (Bits) 128U;
            }

            private static IEnumerable<float> Values()
            {
                yield return 0f;
                yield return float.Epsilon;

                foreach (var i in Enumerable.Range(1, 10))
                {
                    yield return i / 3f;
                    yield return 3f / i;
                }
            }
        }

        private sealed class DoubleUnaryTestData : TheoryData<
            IExpression,
            IExpression>
        {
            public DoubleUnaryTestData()
            {
                foreach (var value in Values())
                    Add(Create(new ConstantDouble(value)),
                        Create(new SymbolicDouble(value)));
            }

            private static IEnumerable<double> Values()
            {
                yield return 0d;
                yield return double.Epsilon;
                yield return double.NaN;
                yield return double.NegativeInfinity;
                yield return double.MinValue;
                yield return double.MaxValue;
                yield return double.PositiveInfinity;

                foreach (var i in Enumerable.Range(-10, 10))
                {
                    yield return i / 3d;
                    yield return 3d / i;
                }

                foreach (var i in Enumerable.Range(1, 10))
                {
                    yield return i / 3d;
                    yield return 3d / i;
                }
            }
        }

        private sealed class DoubleBinaryTestData : TheoryData<
            IExpression, IExpression,
            IExpression, IExpression>
        {
            public DoubleBinaryTestData()
            {
                foreach (var left in Values())
                foreach (var right in Values())
                    Add(Create(new ConstantDouble(left)),
                        Create(new ConstantDouble(right)),
                        Create(new SymbolicDouble(left)),
                        Create(new SymbolicDouble(right)));
            }

            private static IEnumerable<double> Values()
            {
                yield return 0d;
                yield return double.Epsilon;
                yield return double.NaN;
                yield return double.NegativeInfinity;
                yield return double.MinValue;
                yield return double.MaxValue;
                yield return double.PositiveInfinity;

                foreach (var i in Enumerable.Range(-10, 10))
                {
                    yield return i / 3d;
                    yield return 3d / i;
                }

                foreach (var i in Enumerable.Range(1, 10))
                {
                    yield return i / 3d;
                    yield return 3d / i;
                }
            }
        }

        private sealed class DoublePowerTestData : TheoryData<
            IExpression, IExpression,
            IExpression, IExpression>
        {
            public DoublePowerTestData()
            {
                foreach (var left in Values())
                foreach (var right in Values())
                    Add(Create(new ConstantDouble(left)),
                        Create(new ConstantDouble(right)),
                        Create(new SymbolicDouble(left)),
                        Create(new SymbolicDouble(right)));
            }

            private static IEnumerable<double> Values()
            {
                return Enumerable.Range(1, 10).Select(i => (double) i);
            }
        }

        private sealed class DoubleConvertTestData : TheoryData<Bits,
            IExpression,
            IExpression>
        {
            public DoubleConvertTestData()
            {
                foreach (var size in Sizes())
                foreach (var value in Values())
                    Add(size,
                        Create(new ConstantDouble(value)),
                        Create(new SymbolicDouble(value)));
            }

            private static IEnumerable<Bits> Sizes()
            {
                yield return (Bits) 16U;
                yield return (Bits) 32U;
                yield return (Bits) 64U;
                yield return (Bits) 80U;
                yield return (Bits) 128U;
            }

            private static IEnumerable<double> Values()
            {
                yield return 0d;
                yield return double.Epsilon;
                yield return double.NaN;
                yield return double.NegativeInfinity;
                yield return double.MinValue;
                yield return double.MaxValue;
                yield return double.PositiveInfinity;

                foreach (var i in Enumerable.Range(-10, 10))
                {
                    yield return i / 3d;
                    yield return 3d / i;
                }

                foreach (var i in Enumerable.Range(1, 10))
                {
                    yield return i / 3d;
                    yield return 3d / i;
                }
            }
        }

        private sealed class DoubleToSignedTestData : TheoryData<Bits,
            IExpression,
            IExpression>
        {
            public DoubleToSignedTestData()
            {
                foreach (var size in Sizes())
                foreach (var value in Values())
                    Add(size,
                        Create(new ConstantDouble(value)),
                        Create(new SymbolicDouble(value)));
            }

            private static IEnumerable<Bits> Sizes()
            {
                yield return (Bits) 16U;
                yield return (Bits) 32U;
                yield return (Bits) 64U;
                yield return (Bits) 80U;
                yield return (Bits) 128U;
            }

            private static IEnumerable<double> Values()
            {
                yield return 0d;
                yield return double.Epsilon;

                foreach (var i in Enumerable.Range(-10, 10))
                {
                    yield return i / 3d;
                    yield return 3d / i;
                }

                foreach (var i in Enumerable.Range(1, 10))
                {
                    yield return i / 3d;
                    yield return 3d / i;
                }
            }
        }

        private sealed class DoubleToUnsignedTestData : TheoryData<Bits,
            IExpression,
            IExpression>
        {
            public DoubleToUnsignedTestData()
            {
                foreach (var size in Sizes())
                foreach (var value in Values())
                    Add(size,
                        Create(new ConstantDouble(value)),
                        Create(new SymbolicDouble(value)));
            }

            private static IEnumerable<Bits> Sizes()
            {
                yield return (Bits) 16U;
                yield return (Bits) 32U;
                yield return (Bits) 64U;
                yield return (Bits) 80U;
                yield return (Bits) 128U;
            }

            private static IEnumerable<double> Values()
            {
                yield return 0d;
                yield return double.Epsilon;

                foreach (var i in Enumerable.Range(1, 10))
                {
                    yield return i / 3d;
                    yield return 3d / i;
                }
            }
        }
    }
}
