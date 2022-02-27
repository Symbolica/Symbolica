using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class WriteTests
{
    [Theory]
    [ClassData(typeof(WriteTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IValue buffer0, IValue offset0, IValue value0,
        IValue buffer1, IValue offset1, IValue value1)
    {
        var collectionFactory = new CollectionFactory();
        using var solver = PooledSolver.Create();

        using var bv0 = AggregateWrite.Create(collectionFactory, solver, buffer0, offset0, value0).AsBitVector(solver);
        using var result0 = bv0.Simplify();

        using var bv1 = AggregateWrite.Create(collectionFactory, solver, buffer1, offset1, value1).AsBitVector(solver);
        using var result1 = bv1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(WriteTestData))]
    private void ShouldCreateEquivalentBooleans(
        IValue buffer0, IValue offset0, IValue value0,
        IValue buffer1, IValue offset1, IValue value1)
    {
        var collectionFactory = new CollectionFactory();
        using var solver = PooledSolver.Create();

        using var b0 = AggregateWrite.Create(collectionFactory, solver, buffer0, offset0, value0).AsBool(solver);
        using var result0 = b0.Simplify();

        using var b1 = AggregateWrite.Create(collectionFactory, solver, buffer1, offset1, value1).AsBool(solver);
        using var result1 = b1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
