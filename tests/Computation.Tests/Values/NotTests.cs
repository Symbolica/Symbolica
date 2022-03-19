using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class NotTests
{
    [Theory]
    [ClassData(typeof(UnaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IValue value0,
        IValue value1)
    {
        using var solver = PooledSolver.Create();

        using var bv0 = Not.Create(value0).AsBitVector(solver);
        using var result0 = bv0.Simplify();

        using var bv1 = Not.Create(value1).AsBitVector(solver);
        using var result1 = bv1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(UnaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IValue value0,
        IValue value1)
    {
        using var solver = PooledSolver.Create();

        using var b0 = Not.Create(value0).AsBool(solver);
        using var result0 = b0.Simplify();

        using var b1 = Not.Create(value1).AsBool(solver);
        using var result1 = b1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
