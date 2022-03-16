using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class WriteTests
{
    [Theory]
    [ClassData(typeof(WriteTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IValue buffer0, IValue offset0, IValue value0,
        IValue buffer1, IValue offset1, IValue value1)
    {
        var collectionFactory = new CollectionFactory();
        using var context = PooledContext.Create();

        var result0 = Write.Create(collectionFactory, buffer0, offset0, value0).AsBitVector(context).Simplify();
        var result1 = Write.Create(collectionFactory, buffer1, offset1, value1).AsBitVector(context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(WriteTestData))]
    private void ShouldCreateEquivalentBooleans(
        IValue buffer0, IValue offset0, IValue value0,
        IValue buffer1, IValue offset1, IValue value1)
    {
        var collectionFactory = new CollectionFactory();
        using var context = PooledContext.Create();

        var result0 = Write.Create(collectionFactory, buffer0, offset0, value0).AsBool(context).Simplify();
        var result1 = Write.Create(collectionFactory, buffer1, offset1, value1).AsBool(context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
