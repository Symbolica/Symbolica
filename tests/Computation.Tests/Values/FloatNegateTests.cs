using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class FloatNegateTests
{
    private static readonly DisposableContext<ContextHandle> Context = new();

    [Theory]
    [ClassData(typeof(SingleUnaryTestData))]
    [ClassData(typeof(DoubleUnaryTestData))]
    private void ShouldCreateEquivalentConstants(
        IConstantValue constantValue,
        SymbolicFloat symbolicValue)
    {
        var constant = FloatNegate.Create(constantValue).AsConstant(Context);
        var symbolic = FloatNegate.Create(symbolicValue).AsConstant(Context);

        constant.Should().Be(symbolic);
    }

    [Theory]
    [ClassData(typeof(SingleUnaryTestData))]
    [ClassData(typeof(DoubleUnaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IConstantValue constantValue,
        SymbolicFloat symbolicValue)
    {
        var constant = FloatNegate.Create(constantValue).AsBitVector(Context).Simplify();
        var symbolic = FloatNegate.Create(symbolicValue).AsBitVector(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(SingleUnaryTestData))]
    [ClassData(typeof(DoubleUnaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IConstantValue constantValue,
        SymbolicFloat symbolicValue)
    {
        var constant = FloatNegate.Create(constantValue).AsBool(Context).Simplify();
        var symbolic = FloatNegate.Create(symbolicValue).AsBool(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(SingleUnaryTestData))]
    [ClassData(typeof(DoubleUnaryTestData))]
    private void ShouldCreateEquivalentFloats(
        IConstantValue constantValue,
        SymbolicFloat symbolicValue)
    {
        var constant = FloatNegate.Create(constantValue).AsFloat(Context).Simplify();
        var symbolic = FloatNegate.Create(symbolicValue).AsFloat(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }
}
