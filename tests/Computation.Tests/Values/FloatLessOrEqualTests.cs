using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class FloatLessOrEqualTests
{
    private static readonly IContext Context = PooledContext.Create();

    [Theory]
    [ClassData(typeof(SingleBinaryTestData))]
    [ClassData(typeof(DoubleBinaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        var result0 = FloatLessOrEqual.Create(left0, right0).AsBitVector(Context).Simplify();
        var result1 = FloatLessOrEqual.Create(left1, right1).AsBitVector(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(SingleBinaryTestData))]
    [ClassData(typeof(DoubleBinaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        var result0 = FloatLessOrEqual.Create(left0, right0).AsBool(Context).Simplify();
        var result1 = FloatLessOrEqual.Create(left1, right1).AsBool(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
