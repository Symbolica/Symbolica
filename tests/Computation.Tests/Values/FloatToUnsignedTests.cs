using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values;

public class FloatToUnsignedTests
{
    private static readonly IContext Context = PooledContext.Create();

    [Theory]
    [ClassData(typeof(SingleToUnsignedTestData))]
    [ClassData(typeof(DoubleToUnsignedTestData))]
    private void ShouldCreateEquivalentBitVectors(Bits size,
        IValue value0,
        IValue value1)
    {
        var result0 = FloatToUnsigned.Create(size, value0).AsBitVector(Context).Simplify();
        var result1 = FloatToUnsigned.Create(size, value1).AsBitVector(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(SingleToUnsignedTestData))]
    [ClassData(typeof(DoubleToUnsignedTestData))]
    private void ShouldCreateEquivalentBooleans(Bits size,
        IValue value0,
        IValue value1)
    {
        var result0 = FloatToUnsigned.Create(size, value0).AsBool(Context).Simplify();
        var result1 = FloatToUnsigned.Create(size, value1).AsBool(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(SingleToUnsignedTestData))]
    [ClassData(typeof(DoubleToUnsignedTestData))]
    private void ShouldCreateEquivalentFloats(Bits size,
        IValue value0,
        IValue value1)
    {
        var result0 = FloatToUnsigned.Create(size, value0).AsFloat(Context).Simplify();
        var result1 = FloatToUnsigned.Create(size, value1).AsFloat(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
