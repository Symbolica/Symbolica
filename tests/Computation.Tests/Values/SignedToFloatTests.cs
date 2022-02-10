using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values;

public class SignedToFloatTests
{
    private static readonly DisposableContext Context = new();

    [Theory]
    [ClassData(typeof(ToFloatTestData))]
    private void ShouldCreateEquivalentConstants(Bits size,
        IConstantValue constantValue,
        SymbolicUnsigned symbolicValue)
    {
        var constant = SignedToFloat.Create(size, constantValue).AsConstant(Context);
        var symbolic = SignedToFloat.Create(size, symbolicValue).AsConstant(Context);

        constant.Should().Be(symbolic);
    }

    [Theory]
    [ClassData(typeof(ToFloatTestData))]
    private void ShouldCreateEquivalentBitVectors(Bits size,
        IConstantValue constantValue,
        SymbolicUnsigned symbolicValue)
    {
        var constant = SignedToFloat.Create(size, constantValue).AsBitVector(Context).Simplify();
        var symbolic = SignedToFloat.Create(size, symbolicValue).AsBitVector(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(ToFloatTestData))]
    private void ShouldCreateEquivalentBooleans(Bits size,
        IConstantValue constantValue,
        SymbolicUnsigned symbolicValue)
    {
        var constant = SignedToFloat.Create(size, constantValue).AsBool(Context).Simplify();
        var symbolic = SignedToFloat.Create(size, symbolicValue).AsBool(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(ToFloatTestData))]
    private void ShouldCreateEquivalentFloats(Bits size,
        IConstantValue constantValue,
        SymbolicUnsigned symbolicValue)
    {
        var constant = SignedToFloat.Create(size, constantValue).AsFloat(Context).Simplify();
        var symbolic = SignedToFloat.Create(size, symbolicValue).AsFloat(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }
}
