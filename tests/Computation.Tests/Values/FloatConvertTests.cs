using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values;

public class FloatConvertTests
{
    private static readonly IContext Context = DisposableContext.Create();

    [Theory]
    [ClassData(typeof(SingleConvertTestData))]
    [ClassData(typeof(DoubleConvertTestData))]
    private void ShouldCreateEquivalentConstants(Bits size,
        IConstantValue constantValue,
        SymbolicFloat symbolicValue)
    {
        var constant = FloatConvert.Create(size, constantValue).AsConstant(Context);
        var symbolic = FloatConvert.Create(size, symbolicValue).AsConstant(Context);

        constant.Should().Be(symbolic);
    }

    [Theory]
    [ClassData(typeof(SingleConvertTestData))]
    [ClassData(typeof(DoubleConvertTestData))]
    private void ShouldCreateEquivalentBitVectors(Bits size,
        IConstantValue constantValue,
        SymbolicFloat symbolicValue)
    {
        var constant = FloatConvert.Create(size, constantValue).AsBitVector(Context).Simplify();
        var symbolic = FloatConvert.Create(size, symbolicValue).AsBitVector(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(SingleConvertTestData))]
    [ClassData(typeof(DoubleConvertTestData))]
    private void ShouldCreateEquivalentBooleans(Bits size,
        IConstantValue constantValue,
        SymbolicFloat symbolicValue)
    {
        var constant = FloatConvert.Create(size, constantValue).AsBool(Context).Simplify();
        var symbolic = FloatConvert.Create(size, symbolicValue).AsBool(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(SingleConvertTestData))]
    [ClassData(typeof(DoubleConvertTestData))]
    private void ShouldCreateEquivalentFloats(Bits size,
        IConstantValue constantValue,
        SymbolicFloat symbolicValue)
    {
        var constant = FloatConvert.Create(size, constantValue).AsFloat(Context).Simplify();
        var symbolic = FloatConvert.Create(size, symbolicValue).AsFloat(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }
}
