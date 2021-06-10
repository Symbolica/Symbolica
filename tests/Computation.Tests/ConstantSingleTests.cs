using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FluentAssertions;
using Microsoft.Z3;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation
{
    public class ConstantSingleTests
    {
        private static readonly Context Context = new();

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatAdd(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatAdd(constantRight).GetNanNormalizedInteger(Context);
            var symbolic = symbolicLeft.FloatAdd(symbolicRight).GetNanNormalizedInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(UnaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatCeiling(
            ConstantSingle constantExpression,
            SymbolicFloat symbolicExpression)
        {
            var constant = constantExpression.FloatCeiling().GetNanNormalizedInteger(Context);
            var symbolic = symbolicExpression.FloatCeiling().GetNanNormalizedInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(FloatConvertTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatConvert(Bits size,
            ConstantSingle constantExpression,
            SymbolicFloat symbolicExpression)
        {
            var constant = constantExpression.FloatConvert(size).GetNanNormalizedInteger(Context);
            var symbolic = symbolicExpression.FloatConvert(size).GetNanNormalizedInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatDivide(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatDivide(constantRight).GetNanNormalizedInteger(Context);
            var symbolic = symbolicLeft.FloatDivide(symbolicRight).GetNanNormalizedInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatEqual(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatEqual(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.FloatEqual(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(UnaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatFloor(
            ConstantSingle constantExpression,
            SymbolicFloat symbolicExpression)
        {
            var constant = constantExpression.FloatFloor().GetNanNormalizedInteger(Context);
            var symbolic = symbolicExpression.FloatFloor().GetNanNormalizedInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatGreater(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatGreater(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.FloatGreater(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatGreaterOrEqual(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatGreaterOrEqual(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.FloatGreaterOrEqual(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatLess(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatLess(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.FloatLess(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatLessOrEqual(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatLessOrEqual(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.FloatLessOrEqual(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatMultiply(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatMultiply(constantRight).GetNanNormalizedInteger(Context);
            var symbolic = symbolicLeft.FloatMultiply(symbolicRight).GetNanNormalizedInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(UnaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatNegate(
            ConstantSingle constantExpression,
            SymbolicFloat symbolicExpression)
        {
            var constant = constantExpression.FloatNegate().GetNanNormalizedInteger(Context);
            var symbolic = symbolicExpression.FloatNegate().GetNanNormalizedInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatNotEqual(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatNotEqual(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.FloatNotEqual(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatOrdered(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatOrdered(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.FloatOrdered(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatRemainder(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatRemainder(constantRight).GetNanNormalizedInteger(Context);
            var symbolic = symbolicLeft.FloatRemainder(symbolicRight).GetNanNormalizedInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatSubtract(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatSubtract(constantRight).GetNanNormalizedInteger(Context);
            var symbolic = symbolicLeft.FloatSubtract(symbolicRight).GetNanNormalizedInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(FloatToSignedTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatToSigned(Bits size,
            ConstantSingle constantExpression,
            SymbolicFloat symbolicExpression)
        {
            var constant = constantExpression.FloatToSigned(size).GetInteger(Context);
            var symbolic = symbolicExpression.FloatToSigned(size).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(FloatToUnsignedTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatToUnsigned(Bits size,
            ConstantSingle constantExpression,
            SymbolicFloat symbolicExpression)
        {
            var constant = constantExpression.FloatToUnsigned(size).GetInteger(Context);
            var symbolic = symbolicExpression.FloatToUnsigned(size).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatUnordered(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.FloatUnordered(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.FloatUnordered(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
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

    internal static class ValueExtensions
    {
        public static BigInteger GetNanNormalizedInteger(this IValue self, Context context)
        {
            return self.FloatNotEqual(self).GetInteger(context).IsZero
                ? self.GetInteger(context)
                : new ConstantSingle(float.NaN).GetInteger(context);
        }
    }
}
