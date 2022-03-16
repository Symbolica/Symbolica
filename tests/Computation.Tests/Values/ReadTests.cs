using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values;

public class ReadTests
{
    [Theory]
    [ClassData(typeof(ReadTestData))]
    private void ShouldCreateEquivalentBitVectors(Bits size,
        IValue buffer0, IValue offset0,
        IValue buffer1, IValue offset1)
    {
        var collectionFactory = new CollectionFactory();
        using var context = PooledContext.Create();

        var result0 = Read.Create(collectionFactory, buffer0, offset0, size).AsBitVector(context).Simplify();
        var result1 = Read.Create(collectionFactory, buffer1, offset1, size).AsBitVector(context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(ReadTestData))]
    private void ShouldCreateEquivalentBooleans(Bits size,
        IValue buffer0, IValue offset0,
        IValue buffer1, IValue offset1)
    {
        var collectionFactory = new CollectionFactory();
        using var context = PooledContext.Create();

        var result0 = Read.Create(collectionFactory, buffer0, offset0, size).AsBool(context).Simplify();
        var result1 = Read.Create(collectionFactory, buffer1, offset1, size).AsBool(context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
