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

        using var bv0 = Read.Create(collectionFactory, buffer0, offset0, size).AsBitVector(context);
        using var result0 = bv0.Simplify();

        using var bv1 = Read.Create(collectionFactory, buffer1, offset1, size).AsBitVector(context);
        using var result1 = bv1.Simplify();

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

        using var b0 = Read.Create(collectionFactory, buffer0, offset0, size).AsBool(context);
        using var result0 = b0.Simplify();

        using var b1 = Read.Create(collectionFactory, buffer1, offset1, size).AsBool(context);
        using var result1 = b1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
