using FluentAssertions;
using Microsoft.Z3;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class FloatNotEqualTests
{
    private static readonly Context Context = new();

    [Theory]
    [ClassData(typeof(SingleBinaryTestData))]
    [ClassData(typeof(DoubleBinaryTestData))]
    private void ShouldCreateEquivalentConstants(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
    {
        var constant = FloatNotEqual.Create(constantLeft, constantRight).AsConstant(Context);
        var symbolic = FloatNotEqual.Create(symbolicLeft, symbolicRight).AsConstant(Context);

        constant.Should().Be(symbolic);
    }

    [Theory]
    [ClassData(typeof(SingleBinaryTestData))]
    [ClassData(typeof(DoubleBinaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
    {
        var constant = FloatNotEqual.Create(constantLeft, constantRight).AsBitVector(Context).Simplify();
        var symbolic = FloatNotEqual.Create(symbolicLeft, symbolicRight).AsBitVector(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(SingleBinaryTestData))]
    [ClassData(typeof(DoubleBinaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
    {
        var constant = FloatNotEqual.Create(constantLeft, constantRight).AsBool(Context).Simplify();
        var symbolic = FloatNotEqual.Create(symbolicLeft, symbolicRight).AsBool(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }
}
