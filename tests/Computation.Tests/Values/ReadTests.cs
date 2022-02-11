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
        IConstantValue constantBuffer, IConstantValue constantOffset,
        SymbolicUnsigned symbolicBuffer, SymbolicUnsigned symbolicOffset)
    {
        var constant = Read.Create(CollectionFactory,
            constantBuffer, constantOffset, size).AsConstant(Context);
        var symbolic = Read.Create(CollectionFactory,
            symbolicBuffer, symbolicOffset, size).AsConstant(Context);

        constant.Should().Be(symbolic);
    }

    [Theory]
    [ClassData(typeof(ReadTestData))]
    private void ShouldCreateEquivalentBitVectors(Bits size,
        IConstantValue constantBuffer, IConstantValue constantOffset,
        SymbolicUnsigned symbolicBuffer, SymbolicUnsigned symbolicOffset)
    {
        var constant = Read.Create(CollectionFactory,
            constantBuffer, constantOffset, size).AsBitVector(Context).Simplify();
        var symbolic = Read.Create(CollectionFactory,
            symbolicBuffer, symbolicOffset, size).AsBitVector(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(ReadTestData))]
    private void ShouldCreateEquivalentBooleans(Bits size,
        IConstantValue constantBuffer, IConstantValue constantOffset,
        SymbolicUnsigned symbolicBuffer, SymbolicUnsigned symbolicOffset)
    {
        var constant = Read.Create(CollectionFactory,
            constantBuffer, constantOffset, size).AsBool(Context).Simplify();
        var symbolic = Read.Create(CollectionFactory,
            symbolicBuffer, symbolicOffset, size).AsBool(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }
}
