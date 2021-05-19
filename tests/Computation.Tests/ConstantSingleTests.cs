using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FluentAssertions;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation
{
    public class ConstantSingleTests
    {
        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForFloatAdd(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatAdd(constantRight);
            var symbolic = symbolicLeft.FloatAdd(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic, o => o
                .Using<BigInteger>(c => c.Subject.NormalizeNaN().Should().Be(c.Expectation.NormalizeNaN()))
                .WhenTypeIs<BigInteger>());
        }

        [Theory]
        [ClassData(typeof(UnaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForFloatCeiling(
            ConstantSingle constantExpression,
            SymbolicFloat symbolicExpression)
        {
            var constant = constantExpression.FloatCeiling();
            var symbolic = symbolicExpression.FloatCeiling();

            constant.Should().BeEquivalentTo(symbolic, o => o
                .Using<BigInteger>(c => c.Subject.NormalizeNaN().Should().Be(c.Expectation.NormalizeNaN()))
                .WhenTypeIs<BigInteger>());
        }

        [Theory]
        [ClassData(typeof(FloatConvertTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForFloatConvert(Bits size,
            ConstantSingle constantExpression,
            SymbolicFloat symbolicExpression)
        {
            var constant = constantExpression.FloatConvert(size);
            var symbolic = symbolicExpression.FloatConvert(size);

            constant.Should().BeEquivalentTo(symbolic, o => o
                .Using<BigInteger>(c => c.Subject.NormalizeNaN().Should().Be(c.Expectation.NormalizeNaN()))
                .WhenTypeIs<BigInteger>());
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForFloatDivide(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatDivide(constantRight);
            var symbolic = symbolicLeft.FloatDivide(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic, o => o
                .Using<BigInteger>(c => c.Subject.NormalizeNaN().Should().Be(c.Expectation.NormalizeNaN()))
                .WhenTypeIs<BigInteger>());
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForFloatEqual(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatEqual(constantRight);
            var symbolic = symbolicLeft.FloatEqual(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(UnaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForFloatFloor(
            ConstantSingle constantExpression,
            SymbolicFloat symbolicExpression)
        {
            var constant = constantExpression.FloatFloor();
            var symbolic = symbolicExpression.FloatFloor();

            constant.Should().BeEquivalentTo(symbolic, o => o
                .Using<BigInteger>(c => c.Subject.NormalizeNaN().Should().Be(c.Expectation.NormalizeNaN()))
                .WhenTypeIs<BigInteger>());
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForFloatGreater(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatGreater(constantRight);
            var symbolic = symbolicLeft.FloatGreater(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForFloatGreaterOrEqual(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatGreaterOrEqual(constantRight);
            var symbolic = symbolicLeft.FloatGreaterOrEqual(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForFloatLess(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatLess(constantRight);
            var symbolic = symbolicLeft.FloatLess(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForFloatLessOrEqual(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatLessOrEqual(constantRight);
            var symbolic = symbolicLeft.FloatLessOrEqual(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForFloatMultiply(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatMultiply(constantRight);
            var symbolic = symbolicLeft.FloatMultiply(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic, o => o
                .Using<BigInteger>(c => c.Subject.NormalizeNaN().Should().Be(c.Expectation.NormalizeNaN()))
                .WhenTypeIs<BigInteger>());
        }

        [Theory]
        [ClassData(typeof(UnaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForFloatNegate(
            ConstantSingle constantExpression,
            SymbolicFloat symbolicExpression)
        {
            var constant = constantExpression.FloatNegate();
            var symbolic = symbolicExpression.FloatNegate();

            constant.Should().BeEquivalentTo(symbolic, o => o
                .Using<BigInteger>(c => c.Subject.NormalizeNaN().Should().Be(c.Expectation.NormalizeNaN()))
                .WhenTypeIs<BigInteger>());
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForFloatNotEqual(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatNotEqual(constantRight);
            var symbolic = symbolicLeft.FloatNotEqual(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForFloatOrdered(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatOrdered(constantRight);
            var symbolic = symbolicLeft.FloatOrdered(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForFloatRemainder(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatRemainder(constantRight);
            var symbolic = symbolicLeft.FloatRemainder(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic, o => o
                .Using<BigInteger>(c => c.Subject.NormalizeNaN().Should().Be(c.Expectation.NormalizeNaN()))
                .WhenTypeIs<BigInteger>());
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForFloatSubtract(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatSubtract(constantRight);
            var symbolic = symbolicLeft.FloatSubtract(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic, o => o
                .Using<BigInteger>(c => c.Subject.NormalizeNaN().Should().Be(c.Expectation.NormalizeNaN()))
                .WhenTypeIs<BigInteger>());
        }

        [Theory]
        [ClassData(typeof(FloatToSignedTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForFloatToSigned(Bits size,
            ConstantSingle constantExpression,
            SymbolicFloat symbolicExpression)
        {
            var constant = constantExpression.FloatToSigned(size);
            var symbolic = symbolicExpression.FloatToSigned(size);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(FloatToUnsignedTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForFloatToUnsigned(Bits size,
            ConstantSingle constantExpression,
            SymbolicFloat symbolicExpression)
        {
            var constant = constantExpression.FloatToUnsigned(size);
            var symbolic = symbolicExpression.FloatToUnsigned(size);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForFloatUnordered(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatUnordered(constantRight);
            var symbolic = symbolicLeft.FloatUnordered(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        private sealed class UnaryTestData : TheoryData<
            ConstantSingle,
            SymbolicFloat>
        {
            public UnaryTestData()
            {
                foreach (var value in Values())
                    Add(new ConstantSingle(value),
                        new SymbolicFloat((Bits) 32U, c => c.MkFP(value, c.MkFPSort32())));
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

        private sealed class BinaryTestData : TheoryData<
            ConstantSingle, ConstantSingle,
            SymbolicFloat, SymbolicFloat>
        {
            public BinaryTestData()
            {
                foreach (var left in Values())
                foreach (var right in Values())
                    Add(new ConstantSingle(left),
                        new ConstantSingle(right),
                        new SymbolicFloat((Bits) 32U, c => c.MkFP(left, c.MkFPSort32())),
                        new SymbolicFloat((Bits) 32U, c => c.MkFP(right, c.MkFPSort32())));
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

        private sealed class FloatConvertTestData : TheoryData<Bits,
            ConstantSingle,
            SymbolicFloat>
        {
            public FloatConvertTestData()
            {
                foreach (var size in Sizes())
                foreach (var value in Values())
                    Add(size,
                        new ConstantSingle(value),
                        new SymbolicFloat((Bits) 32U, c => c.MkFP(value, c.MkFPSort32())));
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

        private sealed class FloatToSignedTestData : TheoryData<Bits,
            ConstantSingle,
            SymbolicFloat>
        {
            public FloatToSignedTestData()
            {
                foreach (var size in Sizes())
                foreach (var value in Values())
                    Add(size,
                        new ConstantSingle(value),
                        new SymbolicFloat((Bits) 32U, c => c.MkFP(value, c.MkFPSort32())));
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

        private sealed class FloatToUnsignedTestData : TheoryData<Bits,
            ConstantSingle,
            SymbolicFloat>
        {
            public FloatToUnsignedTestData()
            {
                foreach (var size in Sizes())
                foreach (var value in Values())
                    Add(size,
                        new ConstantSingle(value),
                        new SymbolicFloat((Bits) 32U, c => c.MkFP(value, c.MkFPSort32())));
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
    }

    internal static class BigIntegerExtensions
    {
        public static BigInteger NormalizeNaN(this BigInteger self)
        {
            var expression = ConstantUnsigned.Create((Bits) 32U, self);
            var isNan = !expression.FloatNotEqual(expression).Integer.IsZero;

            return isNan
                ? new ConstantSingle(float.NaN).Integer
                : self;
        }
    }
}