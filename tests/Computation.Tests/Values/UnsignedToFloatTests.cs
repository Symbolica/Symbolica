using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values;

public class UnsignedToFloatTests
{
    private static readonly IContext Context = ContextFactory.Create();

    [Theory]
    [ClassData(typeof(ToFloatTestData))]
    private void ShouldCreateEquivalentConstants(Bits size,
        IValue value0,
        IValue value1)
    {
        var result0 = UnsignedToFloat.Create(size, value0).AsConstant(Context);
        var result1 = UnsignedToFloat.Create(size, value1).AsConstant(Context);

        result0.Should().Be(result1);
    }

    [Theory]
    [ClassData(typeof(ToFloatTestData))]
    private void ShouldCreateEquivalentBitVectors(Bits size,
        IValue value0,
        IValue value1)
    {
        var result0 = UnsignedToFloat.Create(size, value0).AsBitVector(Context).Simplify();
        var result1 = UnsignedToFloat.Create(size, value1).AsBitVector(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(ToFloatTestData))]
    private void ShouldCreateEquivalentBooleans(Bits size,
        IValue value0,
        IValue value1)
    {
        var result0 = UnsignedToFloat.Create(size, value0).AsBool(Context).Simplify();
        var result1 = UnsignedToFloat.Create(size, value1).AsBool(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(ToFloatTestData))]
    private void ShouldCreateEquivalentFloats(Bits size,
        IValue value0,
        IValue value1)
    {
        var result0 = UnsignedToFloat.Create(size, value0).AsFloat(Context).Simplify();
        var result1 = UnsignedToFloat.Create(size, value1).AsFloat(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
