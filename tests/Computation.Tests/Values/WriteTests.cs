using FluentAssertions;
using Symbolica.Collection;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class WriteTests
{
    private static readonly ICollectionFactory CollectionFactory = new CollectionFactory();
    private static readonly IContext Context = DisposableContext.Create();

    [Theory]
    [ClassData(typeof(WriteTestData))]
    private void ShouldCreateEquivalentConstants(
        IConstantValue constantBuffer, IConstantValue constantOffset, IConstantValue constantValue,
        SymbolicUnsigned symbolicBuffer, SymbolicUnsigned symbolicOffset, SymbolicUnsigned symbolicValue)
    {
        var constant = Write.Create(CollectionFactory,
            constantBuffer, constantOffset, constantValue).AsConstant(Context);
        var symbolic = Write.Create(CollectionFactory,
            symbolicBuffer, symbolicOffset, symbolicValue).AsConstant(Context);

        constant.Should().Be(symbolic);
    }

    [Theory]
    [ClassData(typeof(WriteTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IConstantValue constantBuffer, IConstantValue constantOffset, IConstantValue constantValue,
        SymbolicUnsigned symbolicBuffer, SymbolicUnsigned symbolicOffset, SymbolicUnsigned symbolicValue)
    {
        var constant = Write.Create(CollectionFactory,
            constantBuffer, constantOffset, constantValue).AsBitVector(Context).Simplify();
        var symbolic = Write.Create(CollectionFactory,
            symbolicBuffer, symbolicOffset, symbolicValue).AsBitVector(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(WriteTestData))]
    private void ShouldCreateEquivalentBooleans(
        IConstantValue constantBuffer, IConstantValue constantOffset, IConstantValue constantValue,
        SymbolicUnsigned symbolicBuffer, SymbolicUnsigned symbolicOffset, SymbolicUnsigned symbolicValue)
    {
        var constant = Write.Create(CollectionFactory,
            constantBuffer, constantOffset, constantValue).AsBool(Context).Simplify();
        var symbolic = Write.Create(CollectionFactory,
            symbolicBuffer, symbolicOffset, symbolicValue).AsBool(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }
}
