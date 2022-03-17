using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class ArithmeticShiftRightTests
{
    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        using var context = PooledContext.Create();

        var result0 = ArithmeticShiftRight.Create(left0, right0).AsBitVector(context).Simplify();
        var result1 = ArithmeticShiftRight.Create(left1, right1).AsBitVector(context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        using var context = PooledContext.Create();

        var result0 = ArithmeticShiftRight.Create(left0, right0).AsBool(context).Simplify();
        var result1 = ArithmeticShiftRight.Create(left1, right1).AsBool(context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
