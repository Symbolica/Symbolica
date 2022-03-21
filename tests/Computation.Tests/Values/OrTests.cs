using FluentAssertions;
using Symbolica.Computation.Values.Constants;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class OrTests
{
    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        using var solver = PooledSolver.Create();

        using var bv0 = Or.Create(left0, right0).AsBitVector(solver);
        using var result0 = bv0.Simplify();

        using var bv1 = Or.Create(left1, right1).AsBitVector(solver);
        using var result1 = bv1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        using var solver = PooledSolver.Create();

        using var b0 = Or.Create(left0, right0).AsBool(solver);
        using var result0 = b0.Simplify();

        using var b1 = Or.Create(left1, right1).AsBool(solver);
        using var result1 = b1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(IdentityTestData))]
    private void ShouldShortCircuitToLeftWhenRightIsZero(IValue value)
    {
        using var solver = PooledSolver.Create();

        var zero = ConstantUnsigned.CreateZero(value.Size);

        using var or = Or.Create(value, zero).AsBitVector(solver);
        using var actual = or.Simplify();

        using var bv = value.AsBitVector(solver);
        using var expected = bv.Simplify();

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [ClassData(typeof(IdentityTestData))]
    private void ShouldShortCircuitToRightWhenLeftIsZero(IValue value)
    {
        using var solver = PooledSolver.Create();

        var zero = ConstantUnsigned.CreateZero(value.Size);

        using var or = Or.Create(zero, value).AsBitVector(solver);
        using var actual = or.Simplify();

        using var bv = value.AsBitVector(solver);
        using var expected = bv.Simplify();

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [ClassData(typeof(IdentityTestData))]
    private void ShouldShortCircuitToOnesWhenRightIsOnes(IValue value)
    {
        using var solver = PooledSolver.Create();

        var ones = ConstantUnsigned.CreateZero(value.Size).Not();

        using var or = Or.Create(value, ones).AsBitVector(solver);
        using var actual = or.Simplify();

        using var onesBv = ones.AsBitVector(solver);
        using var expected = onesBv.Simplify();

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [ClassData(typeof(IdentityTestData))]
    private void ShouldShortCircuitToOnesWhenLeftIsOnes(IValue value)
    {
        using var solver = PooledSolver.Create();

        var ones = ConstantUnsigned.CreateZero(value.Size).Not();

        using var or = Or.Create(ones, value).AsBitVector(solver);
        using var actual = or.Simplify();

        using var onesBv = ones.AsBitVector(solver);
        using var expected = onesBv.Simplify();

        actual.Should().BeEquivalentTo(expected);
    }
}
