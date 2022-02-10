using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class FloatFloorTests
{
    private static readonly IContext Context = DisposableContext.Create();

    [Theory]
    [ClassData(typeof(SingleUnaryTestData))]
    [ClassData(typeof(DoubleUnaryTestData))]
    private void ShouldCreateEquivalentConstants(
        IConstantValue constantValue,
        SymbolicFloat symbolicValue)
    {
        var constant = FloatFloor.Create(constantValue).AsConstant(Context);
        var symbolic = FloatFloor.Create(symbolicValue).AsConstant(Context);

        constant.Should().Be(symbolic);
    }

    [Theory]
    [ClassData(typeof(SingleUnaryTestData))]
    [ClassData(typeof(DoubleUnaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IConstantValue constantValue,
        SymbolicFloat symbolicValue)
    {
        var constant = FloatFloor.Create(constantValue).AsBitVector(Context).Simplify();
        var symbolic = FloatFloor.Create(symbolicValue).AsBitVector(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(SingleUnaryTestData))]
    [ClassData(typeof(DoubleUnaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IConstantValue constantValue,
        SymbolicFloat symbolicValue)
    {
        var constant = FloatFloor.Create(constantValue).AsBool(Context).Simplify();
        var symbolic = FloatFloor.Create(symbolicValue).AsBool(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(SingleUnaryTestData))]
    [ClassData(typeof(DoubleUnaryTestData))]
    private void ShouldCreateEquivalentFloats(
        IConstantValue constantValue,
        SymbolicFloat symbolicValue)
    {
        var constant = FloatFloor.Create(constantValue).AsFloat(Context).Simplify();
        var symbolic = FloatFloor.Create(symbolicValue).AsFloat(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }
}
