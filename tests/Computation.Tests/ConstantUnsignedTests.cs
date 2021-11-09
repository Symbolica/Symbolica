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
        private static readonly IContextFactory ContextFactory = new SharedContextFactory();

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForAdd(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicInteger symbolicLeft, SymbolicInteger symbolicRight)
        {
            var constant = constantLeft.Add(constantRight).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.Add(symbolicRight).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForAnd(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicInteger symbolicLeft, SymbolicInteger symbolicRight)
        {
            var constant = constantLeft.And(constantRight).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.And(symbolicRight).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForArithmeticShiftRight(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicInteger symbolicLeft, SymbolicInteger symbolicRight)
        {
            var constant = constantLeft.AsSigned().ArithmeticShiftRight(constantRight).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.ArithmeticShiftRight(symbolicRight).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForEqual(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicInteger symbolicLeft, SymbolicInteger symbolicRight)
        {
            var constant = constantLeft.Equal(constantRight).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.Equal(symbolicRight).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForLogicalShiftRight(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicInteger symbolicLeft, SymbolicInteger symbolicRight)
        {
            var constant = constantLeft.LogicalShiftRight(constantRight).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.LogicalShiftRight(symbolicRight).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForMultiply(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicInteger symbolicLeft, SymbolicInteger symbolicRight)
        {
            var constant = constantLeft.Multiply(constantRight).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.Multiply(symbolicRight).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(UnaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForNot(
            ConstantUnsigned constantExpression,
            SymbolicInteger symbolicExpression)
        {
            var constant = constantExpression.Not().AsConstant(ContextFactory);
            var symbolic = symbolicExpression.Not().AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForNotEqual(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicInteger symbolicLeft, SymbolicInteger symbolicRight)
        {
            var constant = constantLeft.NotEqual(constantRight).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.NotEqual(symbolicRight).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForOr(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicInteger symbolicLeft, SymbolicInteger symbolicRight)
        {
            var constant = constantLeft.Or(constantRight).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.Or(symbolicRight).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(ReadTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForRead(Bits size,
            ConstantUnsigned constantBuffer, ConstantUnsigned constantOffset,
            SymbolicInteger symbolicBuffer, SymbolicInteger symbolicOffset)
        {
            var collectionFactory = new CollectionFactory();

            var constant = constantBuffer.AsBitVector(collectionFactory)
                .Read(constantOffset, size).AsConstant(ContextFactory);
            var symbolic = symbolicBuffer.AsBitVector(collectionFactory)
                .Read(symbolicOffset, size).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForShiftLeft(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicInteger symbolicLeft, SymbolicInteger symbolicRight)
        {
            var constant = constantLeft.ShiftLeft(constantRight).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.ShiftLeft(symbolicRight).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSignedDivide(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicInteger symbolicLeft, SymbolicInteger symbolicRight)
        {
            if (constantRight.Constant == 0)
                return;

            var constant = constantLeft.AsSigned().Divide(constantRight.AsSigned()).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.Divide(symbolicRight.AsSigned()).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSignedGreater(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicInteger symbolicLeft, SymbolicInteger symbolicRight)
        {
            var constant = constantLeft.AsSigned().Greater(constantRight.AsSigned()).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.Greater(symbolicRight.AsSigned()).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSignedGreaterOrEqual(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicInteger symbolicLeft, SymbolicInteger symbolicRight)
        {
            var constant = constantLeft.AsSigned().GreaterOrEqual(constantRight.AsSigned()).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.GreaterOrEqual(symbolicRight.AsSigned()).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSignedLess(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicInteger symbolicLeft, SymbolicInteger symbolicRight)
        {
            var constant = constantLeft.AsSigned().Less(constantRight.AsSigned()).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.Less(symbolicRight.AsSigned()).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSignedLessOrEqual(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicInteger symbolicLeft, SymbolicInteger symbolicRight)
        {
            var constant = constantLeft.AsSigned().LessOrEqual(constantRight.AsSigned()).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.LessOrEqual(symbolicRight.AsSigned()).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSignedRemainder(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicInteger symbolicLeft, SymbolicInteger symbolicRight)
        {
            if (constantRight.Constant == 0)
                return;

            var constant = constantLeft.AsSigned().Remainder(constantRight.AsSigned()).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.Remainder(symbolicRight.AsSigned()).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(ToFloatTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSignedToFloat(Bits size,
            ConstantUnsigned constantExpression,
            SymbolicInteger symbolicExpression)
        {
            var constant = constantExpression.AsSigned().SignedToFloat(size).AsConstant(ContextFactory);
            var symbolic = symbolicExpression.SignedToFloat(size).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(ExtendTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSignExtend(Bits size,
            ConstantUnsigned constantExpression,
            SymbolicInteger symbolicExpression)
        {
            var constant = constantExpression.AsSigned().SignExtend(size).AsConstant(ContextFactory);
            var symbolic = symbolicExpression.SignExtend(size).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForSubtract(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicInteger symbolicLeft, SymbolicInteger symbolicRight)
        {
            var constant = constantLeft.Subtract(constantRight).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.Subtract(symbolicRight).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(TruncateTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForTruncate(Bits size,
            ConstantUnsigned constantExpression,
            SymbolicInteger symbolicExpression)
        {
            var constant = constantExpression.Truncate(size).AsConstant(ContextFactory);
            var symbolic = symbolicExpression.Truncate(size).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForUnsignedDivide(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicInteger symbolicLeft, SymbolicInteger symbolicRight)
        {
            if (constantRight.Constant == 0)
                return;

            var constant = constantLeft.Divide(constantRight).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.Divide(symbolicRight.AsUnsigned()).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForUnsignedGreater(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicInteger symbolicLeft, SymbolicInteger symbolicRight)
        {
            var constant = constantLeft.Greater(constantRight).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.Greater(symbolicRight.AsUnsigned()).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForUnsignedGreaterOrEqual(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicInteger symbolicLeft, SymbolicInteger symbolicRight)
        {
            var constant = constantLeft.GreaterOrEqual(constantRight).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.GreaterOrEqual(symbolicRight.AsUnsigned()).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForUnsignedLess(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicInteger symbolicLeft, SymbolicInteger symbolicRight)
        {
            var constant = constantLeft.Less(constantRight).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.Less(symbolicRight.AsUnsigned()).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForUnsignedLessOrEqual(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicInteger symbolicLeft, SymbolicInteger symbolicRight)
        {
            var constant = constantLeft.LessOrEqual(constantRight).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.LessOrEqual(symbolicRight.AsUnsigned()).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForUnsignedRemainder(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicInteger symbolicLeft, SymbolicInteger symbolicRight)
        {
            if (constantRight.Constant == 0)
                return;

            var constant = constantLeft.Remainder(constantRight).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.Remainder(symbolicRight.AsUnsigned()).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(ToFloatTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForUnsignedToFloat(Bits size,
            ConstantUnsigned constantExpression,
            SymbolicInteger symbolicExpression)
        {
            var constant = constantExpression.UnsignedToFloat(size).AsConstant(ContextFactory);
            var symbolic = symbolicExpression.UnsignedToFloat(size).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(WriteTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForWrite(
            ConstantUnsigned constantBuffer, ConstantUnsigned constantOffset, ConstantUnsigned constantValue,
            SymbolicInteger symbolicBuffer, SymbolicInteger symbolicOffset, SymbolicInteger symbolicValue)
        {
            var collectionFactory = new CollectionFactory();

            var constant = constantBuffer.AsBitVector(collectionFactory)
                .Write(constantOffset, constantValue.AsBitVector(collectionFactory)).AsConstant(ContextFactory);
            var symbolic = symbolicBuffer.AsBitVector(collectionFactory)
                .Write(symbolicOffset, symbolicValue.AsBitVector(collectionFactory)).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(BinaryTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForXor(
            ConstantUnsigned constantLeft, ConstantUnsigned constantRight,
            SymbolicInteger symbolicLeft, SymbolicInteger symbolicRight)
        {
            var constant = constantLeft.Xor(constantRight).AsConstant(ContextFactory);
            var symbolic = symbolicLeft.Xor(symbolicRight).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        [Theory]
        [ClassData(typeof(ExtendTestData))]
        private void ShouldCreateEquivalentConstantAndSymbolicValuesForZeroExtend(Bits size,
            ConstantUnsigned constantExpression,
            SymbolicInteger symbolicExpression)
        {
            var constant = constantExpression.ZeroExtend(size).AsConstant(ContextFactory);
            var symbolic = symbolicExpression.ZeroExtend(size).AsConstant(ContextFactory);

            constant.Should().Be(symbolic);
        }

        private sealed class UnaryTestData : TheoryData<
            ConstantUnsigned,
            SymbolicInteger>
        {
            public UnaryTestData()
            {
                foreach (var value in Values())
                    Add(ConstantUnsigned.Create((Bits) 4U, value),
                        new SymbolicInteger((Bits) 4U, c => c.MkBV(value.ToString(), 4U)));
            }

            private static IEnumerable<BigInteger> Values()
            {
                return Enumerable.Range(-8, 24).Select(v => (BigInteger) v);
            }
        }

        private sealed class BinaryTestData : TheoryData<
            ConstantUnsigned, ConstantUnsigned,
            SymbolicInteger, SymbolicInteger>
        {
            public BinaryTestData()
            {
                foreach (var left in Values())
                foreach (var right in Values())
                    Add(ConstantUnsigned.Create((Bits) 4U, left),
                        ConstantUnsigned.Create((Bits) 4U, right),
                        new SymbolicInteger((Bits) 4U, c => c.MkBV(left.ToString(), 4U)),
                        new SymbolicInteger((Bits) 4U, c => c.MkBV(right.ToString(), 4U)));
            }

            private static IEnumerable<BigInteger> Values()
            {
                return Enumerable.Range(-8, 24).Select(v => (BigInteger) v);
            }
        }

        private sealed class ReadTestData : TheoryData<Bits,
            ConstantUnsigned, ConstantUnsigned,
            SymbolicInteger, SymbolicInteger>
        {
            public ReadTestData()
            {
                foreach (var buffer in Buffers())
                foreach (var offset in Offsets())
                foreach (var size in Sizes())
                    Add(size,
                        ConstantUnsigned.Create((Bits) 32U, buffer),
                        ConstantUnsigned.Create((Bits) 32U, offset),
                        new SymbolicInteger((Bits) 32U, c => c.MkBV(buffer.ToString(), 32U)),
                        new SymbolicInteger((Bits) 32U, c => c.MkBV(offset.ToString(), 32U)));
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
            SymbolicInteger, SymbolicInteger, SymbolicInteger>
        {
            public WriteTestData()
            {
                foreach (var buffer in Buffers())
                foreach (var offset in Offsets())
                foreach (var value in Values())
                    Add(ConstantUnsigned.Create((Bits) 32U, buffer),
                        ConstantUnsigned.Create((Bits) 32U, offset),
                        ConstantUnsigned.Create((Bits) 16U, value),
                        new SymbolicInteger((Bits) 32U, c => c.MkBV(buffer.ToString(), 32U)),
                        new SymbolicInteger((Bits) 32U, c => c.MkBV(offset.ToString(), 32U)),
                        new SymbolicInteger((Bits) 16U, c => c.MkBV(value.ToString(), 16U)));
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
            SymbolicInteger>
        {
            public ExtendTestData()
            {
                foreach (var size in Sizes())
                foreach (var value in Values())
                    Add(size,
                        ConstantUnsigned.Create((Bits) 4U, value),
                        new SymbolicInteger((Bits) 4U, c => c.MkBV(value.ToString(), 4U)));
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
            SymbolicInteger>
        {
            public TruncateTestData()
            {
                foreach (var size in Sizes())
                foreach (var value in Values())
                    Add(size,
                        ConstantUnsigned.Create((Bits) 4U, value),
                        new SymbolicInteger((Bits) 4U, c => c.MkBV(value.ToString(), 4U)));
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
            SymbolicInteger>
        {
            public ToFloatTestData()
            {
                foreach (var size in Sizes())
                foreach (var value in Values())
                    Add(size,
                        ConstantUnsigned.Create((Bits) 32U, value),
                        new SymbolicInteger((Bits) 32U, c => c.MkBV(value.ToString(), 32U)));
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
