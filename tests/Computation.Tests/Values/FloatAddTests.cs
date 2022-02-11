using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class FloatAddTests
{
    private static readonly DisposableContext<ContextHandle> Context = new();

    [Theory]
    [ClassData(typeof(SingleBinaryTestData))]
    [ClassData(typeof(DoubleBinaryTestData))]
    private void ShouldCreateEquivalentConstants(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
    {
        var constant = FloatAdd.Create(constantLeft, constantRight).AsConstant(Context);
        var symbolic = FloatAdd.Create(symbolicLeft, symbolicRight).AsConstant(Context);

        constant.Should().Be(symbolic);
    }

    [Theory]
    [ClassData(typeof(SingleBinaryTestData))]
    [ClassData(typeof(DoubleBinaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
    {
        var constant = FloatAdd.Create(constantLeft, constantRight).AsBitVector(Context).Simplify();
        var symbolic = FloatAdd.Create(symbolicLeft, symbolicRight).AsBitVector(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(SingleBinaryTestData))]
    [ClassData(typeof(DoubleBinaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
    {
        var constant = FloatAdd.Create(constantLeft, constantRight).AsBool(Context).Simplify();
        var symbolic = FloatAdd.Create(symbolicLeft, symbolicRight).AsBool(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(SingleBinaryTestData))]
    [ClassData(typeof(DoubleBinaryTestData))]
    private void ShouldCreateEquivalentFloats(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
    {
        var constant = FloatAdd.Create(constantLeft, constantRight).AsFloat(Context).Simplify();
        var symbolic = FloatAdd.Create(symbolicLeft, symbolicRight).AsFloat(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }
}
