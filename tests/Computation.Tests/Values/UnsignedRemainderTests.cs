using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class UnsignedRemainderTests
{
    [Theory]
    [ClassData(typeof(DivideTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        using var solver = PooledSolver.Create();

        using var bv0 = UnsignedRemainder.Create(left0, right0).AsBitVector(solver);
        using var result0 = bv0.Simplify();

        using var bv1 = UnsignedRemainder.Create(left1, right1).AsBitVector(solver);
        using var result1 = bv1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(DivideTestData))]
    private void ShouldCreateEquivalentBooleans(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        using var solver = PooledSolver.Create();

        using var b0 = UnsignedRemainder.Create(left0, right0).AsBool(solver);
        using var result0 = b0.Simplify();

        using var b1 = UnsignedRemainder.Create(left1, right1).AsBool(solver);
        using var result1 = b1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
