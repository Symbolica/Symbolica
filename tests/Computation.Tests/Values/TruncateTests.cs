using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values;

public class TruncateTests
{
    private static readonly IContext Context = PooledContext.Create();

    [Theory]
    [ClassData(typeof(TruncateTestData))]
    private void ShouldCreateEquivalentBitVectors(Bits size,
        IValue value0,
        IValue value1)
    {
        var result0 = Truncate.Create(size, value0).AsBitVector(Context).Simplify();
        var result1 = Truncate.Create(size, value1).AsBitVector(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(TruncateTestData))]
    private void ShouldCreateEquivalentBooleans(Bits size,
        IValue value0,
        IValue value1)
    {
        var result0 = Truncate.Create(size, value0).AsBool(Context).Simplify();
        var result1 = Truncate.Create(size, value1).AsBool(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
