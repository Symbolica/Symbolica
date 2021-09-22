using System.Collections.Generic;
using System.Linq;
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
            var constant = constantLeft.Add(constantRight).AsSingleNanNormalizedConstant(Context);
            var symbolic = symbolicLeft.Add(symbolicRight).AsSingleNanNormalizedConstant(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(UnaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatCeiling(
            ConstantSingle constantExpression,
            SymbolicFloat symbolicExpression)
        {
            var constant = constantExpression.Ceiling().AsSingleNanNormalizedConstant(Context);
            var symbolic = symbolicExpression.Ceiling().AsSingleNanNormalizedConstant(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(FloatConvertTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatConvert(Bits size,
            ConstantSingle constantExpression,
            SymbolicFloat symbolicExpression)
        {
            var constant = constantExpression.Convert(size).AsSingleNanNormalizedConstant(Context);
            var symbolic = symbolicExpression.Convert(size).AsSingleNanNormalizedConstant(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatDivide(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.Divide(constantRight).AsSingleNanNormalizedConstant(Context);
            var symbolic = symbolicLeft.Divide(symbolicRight).AsSingleNanNormalizedConstant(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatEqual(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.Equal(constantRight).AsConstant(Context);
            var symbolic = symbolicLeft.Equal(symbolicRight).AsConstant(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(UnaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatFloor(
            ConstantSingle constantExpression,
            SymbolicFloat symbolicExpression)
        {
            var constant = constantExpression.Floor().AsSingleNanNormalizedConstant(Context);
            var symbolic = symbolicExpression.Floor().AsSingleNanNormalizedConstant(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatGreater(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.Greater(constantRight).AsConstant(Context);
            var symbolic = symbolicLeft.Greater(symbolicRight).AsConstant(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatGreaterOrEqual(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.GreaterOrEqual(constantRight).AsConstant(Context);
            var symbolic = symbolicLeft.GreaterOrEqual(symbolicRight).AsConstant(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatLess(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.Less(constantRight).AsConstant(Context);
            var symbolic = symbolicLeft.Less(symbolicRight).AsConstant(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatLessOrEqual(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.LessOrEqual(constantRight).AsConstant(Context);
            var symbolic = symbolicLeft.LessOrEqual(symbolicRight).AsConstant(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatMultiply(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.Multiply(constantRight).AsSingleNanNormalizedConstant(Context);
            var symbolic = symbolicLeft.Multiply(symbolicRight).AsSingleNanNormalizedConstant(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(UnaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatNegate(
            ConstantSingle constantExpression,
            SymbolicFloat symbolicExpression)
        {
            var constant = constantExpression.Negate().AsSingleNanNormalizedConstant(Context);
            var symbolic = symbolicExpression.Negate().AsSingleNanNormalizedConstant(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatNotEqual(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.NotEqual(constantRight).AsConstant(Context);
            var symbolic = symbolicLeft.NotEqual(symbolicRight).AsConstant(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatOrdered(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.Ordered(constantRight).AsConstant(Context);
            var symbolic = symbolicLeft.Ordered(symbolicRight).AsConstant(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatRemainder(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.Remainder(constantRight).AsSingleNanNormalizedConstant(Context);
            var symbolic = symbolicLeft.Remainder(symbolicRight).AsSingleNanNormalizedConstant(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatSubtract(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.Subtract(constantRight).AsSingleNanNormalizedConstant(Context);
            var symbolic = symbolicLeft.Subtract(symbolicRight).AsSingleNanNormalizedConstant(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(FloatToSignedTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatToSigned(Bits size,
            ConstantSingle constantExpression,
            SymbolicFloat symbolicExpression)
        {
            var constant = constantExpression.ToSigned(size).AsConstant(Context);
            var symbolic = symbolicExpression.ToSigned(size).AsConstant(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(FloatToUnsignedTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatToUnsigned(Bits size,
            ConstantSingle constantExpression,
            SymbolicFloat symbolicExpression)
        {
            var constant = constantExpression.ToUnsigned(size).AsConstant(Context);
            var symbolic = symbolicExpression.ToUnsigned(size).AsConstant(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForFloatUnordered(
            ConstantSingle constantLeft, ConstantSingle constantRight,
            SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
        {
            var constant = constantLeft.Unordered(constantRight).AsConstant(Context);
            var symbolic = symbolicLeft.Unordered(symbolicRight).AsConstant(Context);

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
}
