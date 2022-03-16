using FluentAssertions;
using Symbolica.Collection;
using Symbolica.Computation.Values.TestData;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values;

public class ReadTests
{
    private static readonly ICollectionFactory CollectionFactory = new CollectionFactory();
    private static readonly IContext Context = PooledContext.Create();

    [Theory]
    [ClassData(typeof(ReadTestData))]
    private void ShouldCreateEquivalentBitVectors(Bits size,
        IValue buffer0, IValue offset0,
        IValue buffer1, IValue offset1)
    {
        var result0 = Read.Create(CollectionFactory, buffer0, offset0, size).AsBitVector(Context).Simplify();
        var result1 = Read.Create(CollectionFactory, buffer1, offset1, size).AsBitVector(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(ReadTestData))]
    private void ShouldCreateEquivalentBooleans(Bits size,
        IValue buffer0, IValue offset0,
        IValue buffer1, IValue offset1)
    {
        var result0 = Read.Create(CollectionFactory, buffer0, offset0, size).AsBool(Context).Simplify();
        var result1 = Read.Create(CollectionFactory, buffer1, offset1, size).AsBool(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
