using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using FluentAssertions;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation
{
    public class ConstantUnsignedTests
    {
        private static readonly Context Context = new();

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForAdd(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.Add(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.Add(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForAnd(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.And(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.And(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForArithmeticShiftRight(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.ArithmeticShiftRight(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.ArithmeticShiftRight(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForEqual(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.Equal(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.Equal(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForLogicalShiftRight(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.LogicalShiftRight(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.LogicalShiftRight(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForMultiply(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.Multiply(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.Multiply(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(UnaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForNot(
            ConstantUnsigned constantExpression,
            SymbolicBitVector symbolicExpression)
        {
            var constant = constantExpression.Not().GetInteger(Context);
            var symbolic = symbolicExpression.Not().GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForNotEqual(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.NotEqual(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.NotEqual(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForOr(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.Or(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.Or(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(ReadTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForRead(Bits size,
            ConstantUnsigned constantBuffer, ConstantUnsigned constantOffset,
            SymbolicBitVector symbolicBuffer, SymbolicBitVector symbolicOffset)
        {
            var collectionFactory = new CollectionFactory();

            var constant = constantBuffer.Read(collectionFactory, constantOffset, size).GetInteger(Context);
            var symbolic = symbolicBuffer.Read(collectionFactory, symbolicOffset, size).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForShiftLeft(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.ShiftLeft(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.ShiftLeft(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSignedDivide(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            if (constantRight.Integer == 0)
                return;

            var constant = constantLeft.SignedDivide(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.SignedDivide(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSignedGreater(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.SignedGreater(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.SignedGreater(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSignedGreaterOrEqual(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.SignedGreaterOrEqual(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.SignedGreaterOrEqual(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSignedLess(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.SignedLess(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.SignedLess(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSignedLessOrEqual(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.SignedLessOrEqual(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.SignedLessOrEqual(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSignedRemainder(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            if (constantRight.Integer == 0)
                return;

            var constant = constantLeft.SignedRemainder(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.SignedRemainder(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(ToFloatTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSignedToFloat(Bits size,
            ConstantUnsigned constantExpression,
            SymbolicBitVector symbolicExpression)
        {
            var constant = constantExpression.SignedToFloat(size).GetInteger(Context);
            var symbolic = symbolicExpression.SignedToFloat(size).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(ExtendTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSignExtend(Bits size,
            ConstantUnsigned constantExpression,
            SymbolicBitVector symbolicExpression)
        {
            var constant = constantExpression.SignExtend(size).GetInteger(Context);
            var symbolic = symbolicExpression.SignExtend(size).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSubtract(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.Subtract(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.Subtract(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(TruncateTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForTruncate(Bits size,
            ConstantUnsigned constantExpression,
            SymbolicBitVector symbolicExpression)
        {
            var constant = constantExpression.Truncate(size).GetInteger(Context);
            var symbolic = symbolicExpression.Truncate(size).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForUnsignedDivide(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            if (constantRight.Integer == 0)
                return;

            var constant = constantLeft.UnsignedDivide(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.UnsignedDivide(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForUnsignedGreater(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.UnsignedGreater(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.UnsignedGreater(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForUnsignedGreaterOrEqual(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.UnsignedGreaterOrEqual(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.UnsignedGreaterOrEqual(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForUnsignedLess(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.UnsignedLess(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.UnsignedLess(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForUnsignedLessOrEqual(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.UnsignedLessOrEqual(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.UnsignedLessOrEqual(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForUnsignedRemainder(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            if (constantRight.Integer == 0)
                return;

            var constant = constantLeft.UnsignedRemainder(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.UnsignedRemainder(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(ToFloatTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForUnsignedToFloat(Bits size,
            ConstantUnsigned constantExpression,
            SymbolicBitVector symbolicExpression)
        {
            var constant = constantExpression.UnsignedToFloat(size).GetInteger(Context);
            var symbolic = symbolicExpression.UnsignedToFloat(size).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(WriteTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForWrite(
            ConstantUnsigned constantBuffer, ConstantUnsigned constantOffset, ConstantUnsigned constantValue,
            SymbolicBitVector symbolicBuffer, SymbolicBitVector symbolicOffset, SymbolicBitVector symbolicValue)
        {
            var collectionFactory = new CollectionFactory();

            var constant = constantBuffer.Write(collectionFactory, constantOffset, constantValue).GetInteger(Context);
            var symbolic = symbolicBuffer.Write(collectionFactory, symbolicOffset, symbolicValue).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForXor(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicBitVector symbolicLeft, SymbolicBitVector symbolicRight)
        {
            var constant = constantLeft.Xor(constantRight).GetInteger(Context);
            var symbolic = symbolicLeft.Xor(symbolicRight).GetInteger(Context);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(ExtendTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForZeroExtend(Bits size,
            ConstantUnsigned constantExpression,
            SymbolicBitVector symbolicExpression)
        {
            var constant = constantExpression.ZeroExtend(size).GetInteger(Context);
            var symbolic = symbolicExpression.ZeroExtend(size).GetInteger(Context);

            constant.Should().Be(symbolic);
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
