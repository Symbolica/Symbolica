using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values;

public class ReadTests
{
    private static readonly CollectionFactory CollectionFactory = new();
    private static readonly Context<ContextHandle> Context = new();

    [Theory]
    [ClassData(typeof(ReadTestData))]
    private void ShouldCreateEquivalentConstants(Bits size,
        IValue buffer0, IValue offset0,
        IValue buffer1, IValue offset1)
    {
        var result0 = Read.Create(CollectionFactory, MockAssertions.Create(), buffer0, offset0, size).AsConstant(Context);
        var result1 = Read.Create(CollectionFactory, MockAssertions.Create(), buffer1, offset1, size).AsConstant(Context);

        result0.Should().Be(result1);
    }

    [Theory]
    [ClassData(typeof(ReadTestData))]
    private void ShouldCreateEquivalentBitVectors(Bits size,
        IValue buffer0, IValue offset0,
        IValue buffer1, IValue offset1)
    {
        var result0 = Read.Create(CollectionFactory, MockAssertions.Create(), buffer0, offset0, size).AsBitVector(Context).Simplify();
        var result1 = Read.Create(CollectionFactory, MockAssertions.Create(), buffer1, offset1, size).AsBitVector(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(ReadTestData))]
    private void ShouldCreateEquivalentBooleans(Bits size,
        IValue buffer0, IValue offset0,
        IValue buffer1, IValue offset1)
    {
        var result0 = Read.Create(CollectionFactory, MockAssertions.Create(), buffer0, offset0, size).AsBool(Context).Simplify();
        var result1 = Read.Create(CollectionFactory, MockAssertions.Create(), buffer1, offset1, size).AsBool(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
