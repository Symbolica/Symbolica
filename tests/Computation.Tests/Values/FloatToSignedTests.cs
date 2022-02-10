using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values;

public class FloatToSignedTests
{
    private static readonly IContext Context = DisposableContext.Create();

    [Theory]
    [ClassData(typeof(SingleToSignedTestData))]
    [ClassData(typeof(DoubleToSignedTestData))]
    private void ShouldCreateEquivalentConstants(Bits size,
        IConstantValue constantValue,
        SymbolicFloat symbolicValue)
    {
        var constant = FloatToSigned.Create(size, constantValue).AsConstant(Context);
        var symbolic = FloatToSigned.Create(size, symbolicValue).AsConstant(Context);

        constant.Should().Be(symbolic);
    }

    [Theory]
    [ClassData(typeof(SingleToSignedTestData))]
    [ClassData(typeof(DoubleToSignedTestData))]
    private void ShouldCreateEquivalentBitVectors(Bits size,
        IConstantValue constantValue,
        SymbolicFloat symbolicValue)
    {
        var constant = FloatToSigned.Create(size, constantValue).AsBitVector(Context).Simplify();
        var symbolic = FloatToSigned.Create(size, symbolicValue).AsBitVector(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(SingleToSignedTestData))]
    [ClassData(typeof(DoubleToSignedTestData))]
    private void ShouldCreateEquivalentBooleans(Bits size,
        IConstantValue constantValue,
        SymbolicFloat symbolicValue)
    {
        var constant = FloatToSigned.Create(size, constantValue).AsBool(Context).Simplify();
        var symbolic = FloatToSigned.Create(size, symbolicValue).AsBool(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(SingleToSignedTestData))]
    [ClassData(typeof(DoubleToSignedTestData))]
    private void ShouldCreateEquivalentFloats(Bits size,
        IConstantValue constantValue,
        SymbolicFloat symbolicValue)
    {
        var constant = FloatToSigned.Create(size, constantValue).AsFloat(Context).Simplify();
        var symbolic = FloatToSigned.Create(size, symbolicValue).AsFloat(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }
}
