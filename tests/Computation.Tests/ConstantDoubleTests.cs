﻿using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation
{
    public class ConstantDoubleTests
    {
        private static readonly IContextFactory ContextFactory = new SharedContextFactory();

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatAdd(
            ConstantDouble constantLeft, ConstantDouble constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.Add(constantRight).AsDoubleNanNormalizedConstant(ContextFactory);
            var symbolic = symbolicLeft.Add(symbolicRight).AsDoubleNanNormalizedConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(UnaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatCeiling(
            ConstantDouble constantExpression,
            SymbolicFloat symbolicExpression)
        {
            var constant = constantExpression.Ceiling().AsDoubleNanNormalizedConstant(ContextFactory);
            var symbolic = symbolicExpression.Ceiling().AsDoubleNanNormalizedConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(FloatConvertTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatConvert(Bits size,
            ConstantDouble constantExpression,
            SymbolicFloat symbolicExpression)
        {
            var constant = constantExpression.Convert(size).AsDoubleNanNormalizedConstant(ContextFactory);
            var symbolic = symbolicExpression.Convert(size).AsDoubleNanNormalizedConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatDivide(
            ConstantDouble constantLeft, ConstantDouble constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.Divide(constantRight).AsDoubleNanNormalizedConstant(ContextFactory);
            var symbolic = symbolicLeft.Divide(symbolicRight).AsDoubleNanNormalizedConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatEqual(
            ConstantDouble constantLeft, ConstantDouble constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.Equal(constantRight).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.Equal(symbolicRight).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(UnaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatFloor(
            ConstantDouble constantExpression,
            SymbolicFloat symbolicExpression)
        {
            var constant = constantExpression.Floor().AsDoubleNanNormalizedConstant(ContextFactory);
            var symbolic = symbolicExpression.Floor().AsDoubleNanNormalizedConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatGreater(
            ConstantDouble constantLeft, ConstantDouble constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.Greater(constantRight).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.Greater(symbolicRight).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatGreaterOrEqual(
            ConstantDouble constantLeft, ConstantDouble constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.GreaterOrEqual(constantRight).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.GreaterOrEqual(symbolicRight).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatLess(
            ConstantDouble constantLeft, ConstantDouble constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.Less(constantRight).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.Less(symbolicRight).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatLessOrEqual(
            ConstantDouble constantLeft, ConstantDouble constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.LessOrEqual(constantRight).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.LessOrEqual(symbolicRight).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatMultiply(
            ConstantDouble constantLeft, ConstantDouble constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.Multiply(constantRight).AsDoubleNanNormalizedConstant(ContextFactory);
            var symbolic = symbolicLeft.Multiply(symbolicRight).AsDoubleNanNormalizedConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(UnaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatNegate(
            ConstantDouble constantExpression,
            SymbolicFloat symbolicExpression)
        {
            var constant = constantExpression.Negate().AsDoubleNanNormalizedConstant(ContextFactory);
            var symbolic = symbolicExpression.Negate().AsDoubleNanNormalizedConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatNotEqual(
            ConstantDouble constantLeft, ConstantDouble constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.NotEqual(constantRight).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.NotEqual(symbolicRight).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatOrdered(
            ConstantDouble constantLeft, ConstantDouble constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.Ordered(constantRight).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.Ordered(symbolicRight).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(PowerTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatPower(
            ConstantDouble constantLeft, ConstantDouble constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.Power(constantRight).ToSigned(constantLeft.Size).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.Power(symbolicRight).ToSigned(symbolicLeft.Size).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatRemainder(
            ConstantDouble constantLeft, ConstantDouble constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.Remainder(constantRight).AsDoubleNanNormalizedConstant(ContextFactory);
            var symbolic = symbolicLeft.Remainder(symbolicRight).AsDoubleNanNormalizedConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatSubtract(
            ConstantDouble constantLeft, ConstantDouble constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.Subtract(constantRight).AsDoubleNanNormalizedConstant(ContextFactory);
            var symbolic = symbolicLeft.Subtract(symbolicRight).AsDoubleNanNormalizedConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(FloatToSignedTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatToSigned(Bits size,
            ConstantDouble constantExpression,
            SymbolicFloat symbolicExpression)
        {
            var constant = constantExpression.ToSigned(size).AsConstant(ContextFactory);
            var symbolic = symbolicExpression.ToSigned(size).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(FloatToUnsignedTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatToUnsigned(Bits size,
            ConstantDouble constantExpression,
            SymbolicFloat symbolicExpression)
        {
            var constant = constantExpression.ToUnsigned(size).AsConstant(ContextFactory);
            var symbolic = symbolicExpression.ToUnsigned(size).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatUnordered(
            ConstantDouble constantLeft, ConstantDouble constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.Unordered(constantRight).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.Unordered(symbolicRight).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        private sealed class UnaryTestData : TheoryData<
            ConstantDouble,
            SymbolicFloat>
        {
            public UnaryTestData()
            {
                foreach (var value in Values())
                    Add(new ConstantDouble(value),
                        new SymbolicFloat((Bits) 64U, c => c.MkFP(value, c.MkFPSort64())));
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

        private sealed class BinaryTestData : TheoryData<
            ConstantDouble, ConstantDouble,
            SymbolicFloat, SymbolicFloat>
        {
            public BinaryTestData()
            {
                foreach (var left in Values())
                foreach (var right in Values())
                    Add(new ConstantDouble(left),
                        new ConstantDouble(right),
                        new SymbolicFloat((Bits) 64U, c => c.MkFP(left, c.MkFPSort64())),
                        new SymbolicFloat((Bits) 64U, c => c.MkFP(right, c.MkFPSort64())));
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

        private sealed class PowerTestData : TheoryData<
            ConstantDouble, ConstantDouble,
            SymbolicFloat, SymbolicFloat>
        {
            public PowerTestData()
            {
                foreach (var left in Values())
                foreach (var right in Values())
                    Add(new ConstantDouble(left),
                        new ConstantDouble(right),
                        new SymbolicFloat((Bits) 64U, c => c.MkFP(left, c.MkFPSort64())),
                        new SymbolicFloat((Bits) 64U, c => c.MkFP(right, c.MkFPSort64())));
            }

            private static IEnumerable<double> Values()
            {
                return Enumerable.Range(1, 10).Select(i => (double) i);
            }
        }

        private sealed class FloatConvertTestData : TheoryData<Bits,
            ConstantDouble,
            SymbolicFloat>
        {
            public FloatConvertTestData()
            {
                foreach (var size in Sizes())
                foreach (var value in Values())
                    Add(size,
                        new ConstantDouble(value),
                        new SymbolicFloat((Bits) 64U, c => c.MkFP(value, c.MkFPSort64())));
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

        private sealed class FloatToSignedTestData : TheoryData<Bits,
            ConstantDouble,
            SymbolicFloat>
        {
            public FloatToSignedTestData()
            {
                foreach (var size in Sizes())
                foreach (var value in Values())
                    Add(size,
                        new ConstantDouble(value),
                        new SymbolicFloat((Bits) 64U, c => c.MkFP(value, c.MkFPSort64())));
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

        private sealed class FloatToUnsignedTestData : TheoryData<Bits,
            ConstantDouble,
            SymbolicFloat>
        {
            public FloatToUnsignedTestData()
            {
                foreach (var size in Sizes())
                foreach (var value in Values())
                    Add(size,
                        new ConstantDouble(value),
                        new SymbolicFloat((Bits) 64U, c => c.MkFP(value, c.MkFPSort64())));
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