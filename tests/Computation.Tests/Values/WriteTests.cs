﻿using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class WriteTests
{
    private static readonly CollectionFactory CollectionFactory = new();
    private static readonly Context<ContextHandle> Context = new();

    // [Theory]
    // [ClassData(typeof(WriteTestData))]
    // private void ShouldCreateEquivalentBitVectors(
    //     IValue buffer0, IValue offset0, IValue value0,
    //     IValue buffer1, IValue offset1, IValue value1)
    // {
    //     var result0 = AggregateWrite.Create(CollectionFactory, MockAssertions.Create(), buffer0, offset0, value0).AsBitVector(Context).Simplify();
    //     var result1 = AggregateWrite.Create(CollectionFactory, MockAssertions.Create(), buffer1, offset1, value1).AsBitVector(Context).Simplify();

    //     result0.Should().BeEquivalentTo(result1);
    // }

    // [Theory]
    // [ClassData(typeof(WriteTestData))]
    // private void ShouldCreateEquivalentBooleans(
    //     IValue buffer0, IValue offset0, IValue value0,
    //     IValue buffer1, IValue offset1, IValue value1)
    // {
    //     var result0 = AggregateWrite.Create(CollectionFactory, MockAssertions.Create(), buffer0, offset0, value0).AsBool(Context).Simplify();
    //     var result1 = AggregateWrite.Create(CollectionFactory, MockAssertions.Create(), buffer1, offset1, value1).AsBool(Context).Simplify();

    //     result0.Should().BeEquivalentTo(result1);
    // }
}
