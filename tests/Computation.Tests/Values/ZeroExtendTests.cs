using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values;

public class ZeroExtendTests
{
    private static readonly Context<ContextHandle> Context = new();

    [Theory]
    [ClassData(typeof(ExtendTestData))]
    private void ShouldCreateEquivalentConstants(Bits size,
        IValue value0,
        IValue value1)
    {
        var result0 = ZeroExtend.Create(size, value0).AsConstant(Context);
        var result1 = ZeroExtend.Create(size, value1).AsConstant(Context);

        result0.Should().Be(result1);
    }

    [Theory]
    [ClassData(typeof(ExtendTestData))]
    private void ShouldCreateEquivalentBitVectors(Bits size,
        IValue value0,
        IValue value1)
    {
        var result0 = ZeroExtend.Create(size, value0).AsBitVector(Context).Simplify();
        var result1 = ZeroExtend.Create(size, value1).AsBitVector(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(ExtendTestData))]
    private void ShouldCreateEquivalentBooleans(Bits size,
        IValue value0,
        IValue value1)
    {
        var result0 = ZeroExtend.Create(size, value0).AsBool(Context).Simplify();
        var result1 = ZeroExtend.Create(size, value1).AsBool(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
