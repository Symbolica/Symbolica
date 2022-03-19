using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values;

public class FloatConvertTests
{
    [Theory]
    [ClassData(typeof(SingleConvertTestData))]
    [ClassData(typeof(DoubleConvertTestData))]
    private void ShouldCreateEquivalentBitVectors(Bits size,
        IValue value0,
        IValue value1)
    {
        using var solver = PooledSolver.Create();

        using var bv0 = FloatConvert.Create(size, value0).AsBitVector(solver);
        using var result0 = bv0.Simplify();

        using var bv1 = FloatConvert.Create(size, value1).AsBitVector(solver);
        using var result1 = bv1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(SingleConvertTestData))]
    [ClassData(typeof(DoubleConvertTestData))]
    private void ShouldCreateEquivalentBooleans(Bits size,
        IValue value0,
        IValue value1)
    {
        using var solver = PooledSolver.Create();

        using var b0 = FloatConvert.Create(size, value0).AsBool(solver);
        using var result0 = b0.Simplify();

        using var b1 = FloatConvert.Create(size, value1).AsBool(solver);
        using var result1 = b1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(SingleConvertTestData))]
    [ClassData(typeof(DoubleConvertTestData))]
    private void ShouldCreateEquivalentFloats(Bits size,
        IValue value0,
        IValue value1)
    {
        using var solver = PooledSolver.Create();

        using var f0 = FloatConvert.Create(size, value0).AsFloat(solver);
        using var result0 = f0.Simplify();

        using var f1 = FloatConvert.Create(size, value1).AsFloat(solver);
        using var result1 = f1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
