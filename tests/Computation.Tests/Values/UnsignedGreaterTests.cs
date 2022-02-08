using FluentAssertions;
using Microsoft.Z3;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class UnsignedGreaterTests
{
    private static readonly Context Context = new();

    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentConstants(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicUnsigned symbolicLeft, SymbolicUnsigned symbolicRight)
    {
        var constant = UnsignedGreater.Create(constantLeft, constantRight).AsConstant(Context);
        var symbolic = UnsignedGreater.Create(symbolicLeft, symbolicRight).AsConstant(Context);

        constant.Should().Be(symbolic);
    }

    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicUnsigned symbolicLeft, SymbolicUnsigned symbolicRight)
    {
        var constant = UnsignedGreater.Create(constantLeft, constantRight).AsBitVector(Context).Simplify();
        var symbolic = UnsignedGreater.Create(symbolicLeft, symbolicRight).AsBitVector(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicUnsigned symbolicLeft, SymbolicUnsigned symbolicRight)
    {
        var constant = UnsignedGreater.Create(constantLeft, constantRight).AsBool(Context).Simplify();
        var symbolic = UnsignedGreater.Create(symbolicLeft, symbolicRight).AsBool(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }
}
