using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values;

public class SignExtendTests
{
    private static readonly Context<ContextHandle> Context = new();

    [Theory]
    [ClassData(typeof(ExtendTestData))]
    private void ShouldCreateEquivalentConstants(Bits size,
        IConstantValue constantValue,
        SymbolicUnsigned symbolicValue)
    {
        var constant = SignExtend.Create(size, constantValue).AsConstant(Context);
        var symbolic = SignExtend.Create(size, symbolicValue).AsConstant(Context);

        constant.Should().Be(symbolic);
    }

    [Theory]
    [ClassData(typeof(ExtendTestData))]
    private void ShouldCreateEquivalentBitVectors(Bits size,
        IConstantValue constantValue,
        SymbolicUnsigned symbolicValue)
    {
        var constant = SignExtend.Create(size, constantValue).AsBitVector(Context).Simplify();
        var symbolic = SignExtend.Create(size, symbolicValue).AsBitVector(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(ExtendTestData))]
    private void ShouldCreateEquivalentBooleans(Bits size,
        IConstantValue constantValue,
        SymbolicUnsigned symbolicValue)
    {
        var constant = SignExtend.Create(size, constantValue).AsBool(Context).Simplify();
        var symbolic = SignExtend.Create(size, symbolicValue).AsBool(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }
}
