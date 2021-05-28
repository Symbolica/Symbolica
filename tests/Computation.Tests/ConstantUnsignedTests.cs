using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using FluentAssertions;
using Symbolica.Collection;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation
{
    public class ConstantUnsignedTests
    {
        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForAdd(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.Add(constantRight);
            var symbolic = symbolicLeft.Add(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForAnd(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.And(constantRight);
            var symbolic = symbolicLeft.And(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForArithmeticShiftRight(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.ArithmeticShiftRight(constantRight);
            var symbolic = symbolicLeft.ArithmeticShiftRight(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForEqual(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.Equal(constantRight);
            var symbolic = symbolicLeft.Equal(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForLogicalShiftRight(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.LogicalShiftRight(constantRight);
            var symbolic = symbolicLeft.LogicalShiftRight(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForMultiply(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.Multiply(constantRight);
            var symbolic = symbolicLeft.Multiply(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(UnaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForNot(
            ConstantUnsigned constantExpression,
            SymbolicBitVector symbolicExpression)
        {
            var constant = constantExpression.Not();
            var symbolic = symbolicExpression.Not();

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForNotEqual(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.NotEqual(constantRight);
            var symbolic = symbolicLeft.NotEqual(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForOr(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.Or(constantRight);
            var symbolic = symbolicLeft.Or(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(ReadTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForRead(Bits size,
            ConstantUnsigned constantBuffer, ConstantUnsigned constantOffset,
            SymbolicBitVector symbolicBuffer, SymbolicBitVector symbolicOffset)
        {
            var constant = constantBuffer.Read(new CollectionFactory(), constantOffset, size);
            var symbolic = symbolicBuffer.Read(new CollectionFactory(), symbolicOffset, size);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForShiftLeft(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.ShiftLeft(constantRight);
            var symbolic = symbolicLeft.ShiftLeft(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForSignedDivide(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            if (constantRight.Integer == 0)
                return;

            var constant = constantLeft.SignedDivide(constantRight);
            var symbolic = symbolicLeft.SignedDivide(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForSignedGreater(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.SignedGreater(constantRight);
            var symbolic = symbolicLeft.SignedGreater(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForSignedGreaterOrEqual(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.SignedGreaterOrEqual(constantRight);
            var symbolic = symbolicLeft.SignedGreaterOrEqual(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForSignedLess(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.SignedLess(constantRight);
            var symbolic = symbolicLeft.SignedLess(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForSignedLessOrEqual(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.SignedLessOrEqual(constantRight);
            var symbolic = symbolicLeft.SignedLessOrEqual(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForSignedRemainder(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            if (constantRight.Integer == 0)
                return;

            var constant = constantLeft.SignedRemainder(constantRight);
            var symbolic = symbolicLeft.SignedRemainder(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(ToFloatTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForSignedToFloat(Bits size,
            ConstantUnsigned constantExpression,
            SymbolicBitVector symbolicExpression)
        {
            var constant = constantExpression.SignedToFloat(size);
            var symbolic = symbolicExpression.SignedToFloat(size);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(ExtendTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForSignExtend(Bits size,
            ConstantUnsigned constantExpression,
            SymbolicBitVector symbolicExpression)
        {
            var constant = constantExpression.SignExtend(size);
            var symbolic = symbolicExpression.SignExtend(size);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForSubtract(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.Subtract(constantRight);
            var symbolic = symbolicLeft.Subtract(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(TruncateTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForTruncate(Bits size,
            ConstantUnsigned constantExpression,
            SymbolicBitVector symbolicExpression)
        {
            var constant = constantExpression.Truncate(size);
            var symbolic = symbolicExpression.Truncate(size);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForUnsignedDivide(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            if (constantRight.Integer == 0)
                return;

            var constant = constantLeft.UnsignedDivide(constantRight);
            var symbolic = symbolicLeft.UnsignedDivide(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForUnsignedGreater(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.UnsignedGreater(constantRight);
            var symbolic = symbolicLeft.UnsignedGreater(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForUnsignedGreaterOrEqual(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.UnsignedGreaterOrEqual(constantRight);
            var symbolic = symbolicLeft.UnsignedGreaterOrEqual(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForUnsignedLess(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.UnsignedLess(constantRight);
            var symbolic = symbolicLeft.UnsignedLess(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForUnsignedLessOrEqual(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.UnsignedLessOrEqual(constantRight);
            var symbolic = symbolicLeft.UnsignedLessOrEqual(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForUnsignedRemainder(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            if (constantRight.Integer == 0)
                return;

            var constant = constantLeft.UnsignedRemainder(constantRight);
            var symbolic = symbolicLeft.UnsignedRemainder(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(ToFloatTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForUnsignedToFloat(Bits size,
            ConstantUnsigned constantExpression,
            SymbolicBitVector symbolicExpression)
        {
            var constant = constantExpression.UnsignedToFloat(size);
            var symbolic = symbolicExpression.UnsignedToFloat(size);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(WriteTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForWrite(
            ConstantUnsigned constantBuffer, ConstantUnsigned constantOffset, ConstantUnsigned constantValue,
            SymbolicBitVector symbolicBuffer, SymbolicBitVector symbolicOffset, SymbolicBitVector symbolicValue)
        {
            var constant = constantBuffer.Write(new CollectionFactory(), constantOffset, constantValue);
            var symbolic = symbolicBuffer.Write(new CollectionFactory(), symbolicOffset, symbolicValue);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForXor(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.Xor(constantRight);
            var symbolic = symbolicLeft.Xor(symbolicRight);

            constant.Should().BeEquivalentTo(symbolic);
        }

        [Theory]
        [ClassData(typeof(ExtendTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicExpressionsForZeroExtend(Bits size,
            ConstantUnsigned constantExpression,
            SymbolicBitVector symbolicExpression)
        {
            var constant = constantExpression.ZeroExtend(size);
            var symbolic = symbolicExpression.ZeroExtend(size);

            constant.Should().BeEquivalentTo(symbolic);
        }

        private sealed class UnaryTestData : TheoryData<
            ConstantUnsigned,
            SymbolicBitVector>
        {
            public UnaryTestData()
            {
                foreach (var value in Values())
                    Add(ConstantUnsigned.Create((Bits) 4U, value),
                        new SymbolicBitVector((Bits) 4U, c => c.MkBV(value.ToString(), 4U)));
            }

            private static IEnumerable<BigInteger> Values()
            {
                return Enumerable.Range(-8, 24).Select(v => (BigInteger) v);
            }
        }

        private sealed class BinaryTestData : TheoryData<
            ConstantUnsigned, ConstantUnsigned,
            SymbolicBitVector, SymbolicBitVector>
        {
            public BinaryTestData()
            {
                foreach (var left in Values())
                foreach (var right in Values())
                    Add(ConstantUnsigned.Create((Bits) 4U, left),
                        ConstantUnsigned.Create((Bits) 4U, right),
                        new SymbolicBitVector((Bits) 4U, c => c.MkBV(left.ToString(), 4U)),
                        new SymbolicBitVector((Bits) 4U, c => c.MkBV(right.ToString(), 4U)));
            }

            private static IEnumerable<BigInteger> Values()
            {
                return Enumerable.Range(-8, 24).Select(v => (BigInteger) v);
            }
        }

        private sealed class ReadTestData : TheoryData<Bits,
            ConstantUnsigned, ConstantUnsigned,
            SymbolicBitVector, SymbolicBitVector>
        {
            public ReadTestData()
            {
                foreach (var buffer in Buffers())
                foreach (var offset in Offsets())
                foreach (var size in Sizes())
                    Add(size,
                        ConstantUnsigned.Create((Bits) 32U, buffer),
                        ConstantUnsigned.Create((Bits) 32U, offset),
                        new SymbolicBitVector((Bits) 32U, c => c.MkBV(buffer.ToString(), 32U)),
                        new SymbolicBitVector((Bits) 32U, c => c.MkBV(offset.ToString(), 32U)));
            }

            private static IEnumerable<BigInteger> Buffers()
            {
                yield return int.MinValue;
                yield return uint.MinValue;
                yield return int.MaxValue;
                yield return uint.MaxValue;
            }

            private static IEnumerable<BigInteger> Offsets()
            {
                yield return 0U;
                yield return 8U;
                yield return 16U;
            }

            private static IEnumerable<Bits> Sizes()
            {
                yield return (Bits) 8U;
                yield return (Bits) 16U;
            }
        }

        private sealed class WriteTestData : TheoryData<
            ConstantUnsigned, ConstantUnsigned, ConstantUnsigned,
            SymbolicBitVector, SymbolicBitVector, SymbolicBitVector>
        {
            public WriteTestData()
            {
                foreach (var buffer in Buffers())
                foreach (var offset in Offsets())
                foreach (var value in Values())
                    Add(ConstantUnsigned.Create((Bits) 32U, buffer),
                        ConstantUnsigned.Create((Bits) 32U, offset),
                        ConstantUnsigned.Create((Bits) 16U, value),
                        new SymbolicBitVector((Bits) 32U, c => c.MkBV(buffer.ToString(), 32U)),
                        new SymbolicBitVector((Bits) 32U, c => c.MkBV(offset.ToString(), 32U)),
                        new SymbolicBitVector((Bits) 16U, c => c.MkBV(value.ToString(), 16U)));
            }

            private static IEnumerable<BigInteger> Buffers()
            {
                yield return int.MinValue;
                yield return uint.MinValue;
                yield return int.MaxValue;
                yield return uint.MaxValue;
            }

            private static IEnumerable<BigInteger> Offsets()
            {
                yield return 0U;
                yield return 8U;
                yield return 16U;
            }

            private static IEnumerable<BigInteger> Values()
            {
                yield return short.MinValue;
                yield return ushort.MinValue;
                yield return short.MaxValue;
                yield return ushort.MaxValue;
            }
        }

        private sealed class ExtendTestData : TheoryData<Bits,
            ConstantUnsigned,
            SymbolicBitVector>
        {
            public ExtendTestData()
            {
                foreach (var size in Sizes())
                foreach (var value in Values())
                    Add(size,
                        ConstantUnsigned.Create((Bits) 4U, value),
                        new SymbolicBitVector((Bits) 4U, c => c.MkBV(value.ToString(), 4U)));
            }

            private static IEnumerable<Bits> Sizes()
            {
                return Enumerable.Range(4, 5).Select(s => (Bits) (uint) s);
            }

            private static IEnumerable<BigInteger> Values()
            {
                return Enumerable.Range(-8, 24).Select(v => (BigInteger) v);
            }
        }

        private sealed class TruncateTestData : TheoryData<Bits,
            ConstantUnsigned,
            SymbolicBitVector>
        {
            public TruncateTestData()
            {
                foreach (var size in Sizes())
                foreach (var value in Values())
                    Add(size,
                        ConstantUnsigned.Create((Bits) 4U, value),
                        new SymbolicBitVector((Bits) 4U, c => c.MkBV(value.ToString(), 4U)));
            }

            private static IEnumerable<Bits> Sizes()
            {
                return Enumerable.Range(1, 4).Select(s => (Bits) (uint) s);
            }

            private static IEnumerable<BigInteger> Values()
            {
                return Enumerable.Range(-8, 24).Select(v => (BigInteger) v);
            }
        }

        private sealed class ToFloatTestData : TheoryData<Bits,
            ConstantUnsigned,
            SymbolicBitVector>
        {
            public ToFloatTestData()
            {
                foreach (var size in Sizes())
                foreach (var value in Values())
                    Add(size,
                        ConstantUnsigned.Create((Bits) 32U, value),
                        new SymbolicBitVector((Bits) 32U, c => c.MkBV(value.ToString(), 32U)));
            }

            private static IEnumerable<Bits> Sizes()
            {
                yield return (Bits) 16U;
                yield return (Bits) 32U;
                yield return (Bits) 64U;
                yield return (Bits) 80U;
                yield return (Bits) 128U;
            }

            private static IEnumerable<BigInteger> Values()
            {
                return Enumerable.Range(-10, 10).Select(v => (BigInteger) v);
            }
        }

        private sealed class CollectionFactory : ICollectionFactory
        {
            public IPersistentStack<T> CreatePersistentStack<T>()
            {
                throw new NotImplementedException();
            }

            public IPersistentList<T> CreatePersistentList<T>()
            {
                return new PersistentList<T>(ImmutableList<T>.Empty);
            }

            public IPersistentDictionary<TKey, TValue> CreatePersistentDictionary<TKey, TValue>()
                where TKey : notnull
            {
                throw new NotImplementedException();
            }

            private sealed class PersistentList<T> : IPersistentList<T>
            {
                private readonly ImmutableList<T> _list;

                public PersistentList(ImmutableList<T> list)
                {
                    _list = list;
                }

                public IEnumerator<T> GetEnumerator()
                {
                    return _list.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }

                public int Count => _list.Count;

                public IPersistentList<T> Add(T value)
                {
                    return new PersistentList<T>(_list.Add(value));
                }

                public IPersistentList<T> AddRange(IEnumerable<T> items)
                {
                    return new PersistentList<T>(_list.AddRange(items));
                }

                public T Get(int index)
                {
                    return _list[index];
                }

                public IPersistentList<T> GetRange(int index, int count)
                {
                    return new PersistentList<T>(_list.GetRange(index, count));
                }

                public IPersistentList<T> SetItem(int index, T value)
                {
                    return new PersistentList<T>(_list.SetItem(index, value));
                }

                public IPersistentList<T> SetItems(IEnumerable<KeyValuePair<int, T>> items)
                {
                    return items.Aggregate((IPersistentList<T>) this, (l, i) => l.SetItem(i.Key, i.Value));
                }

                public IPersistentList<T> SetRange(int index, IPersistentList<T> values)
                {
                    return new PersistentList<T>(_list.RemoveRange(index, values.Count).InsertRange(index, values));
                }

                public int BinarySearch(T item)
                {
                    return _list.BinarySearch(item);
                }
            }
        }
    }
}
