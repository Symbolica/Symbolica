using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class UnsignedRemainderTests
{
    private static readonly IContext Context = ContextFactory.Create();

    [Theory]
    [ClassData(typeof(DivideTestData))]
    private void ShouldCreateEquivalentConstants(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        var result0 = UnsignedRemainder.Create(left0, right0).AsConstant(Context);
        var result1 = UnsignedRemainder.Create(left1, right1).AsConstant(Context);

        result0.Should().Be(result1);
    }

    [Theory]
    [ClassData(typeof(DivideTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        var result0 = UnsignedRemainder.Create(left0, right0).AsBitVector(Context).Simplify();
        var result1 = UnsignedRemainder.Create(left1, right1).AsBitVector(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(DivideTestData))]
    private void ShouldCreateEquivalentBooleans(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        var result0 = UnsignedRemainder.Create(left0, right0).AsBool(Context).Simplify();
        var result1 = UnsignedRemainder.Create(left1, right1).AsBool(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
