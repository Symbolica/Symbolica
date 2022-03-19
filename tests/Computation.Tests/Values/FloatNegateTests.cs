using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class FloatNegateTests
{
    [Theory]
    [ClassData(typeof(SingleUnaryTestData))]
    [ClassData(typeof(DoubleUnaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IValue value0,
        IValue value1)
    {
        using var solver = PooledSolver.Create();

        using var bv0 = FloatNegate.Create(value0).AsBitVector(solver);
        using var result0 = bv0.Simplify();

        using var bv1 = FloatNegate.Create(value1).AsBitVector(solver);
        using var result1 = bv1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(SingleUnaryTestData))]
    [ClassData(typeof(DoubleUnaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IValue value0,
        IValue value1)
    {
        using var solver = PooledSolver.Create();

        using var b0 = FloatNegate.Create(value0).AsBool(solver);
        using var result0 = b0.Simplify();

        using var b1 = FloatNegate.Create(value1).AsBool(solver);
        using var result1 = b1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(SingleUnaryTestData))]
    [ClassData(typeof(DoubleUnaryTestData))]
    private void ShouldCreateEquivalentFloats(
        IValue value0,
        IValue value1)
    {
        using var solver = PooledSolver.Create();

        using var f0 = FloatNegate.Create(value0).AsFloat(solver);
        using var result0 = f0.Simplify();

        using var f1 = FloatNegate.Create(value1).AsFloat(solver);
        using var result1 = f1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
