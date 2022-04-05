using System.Numerics;
using FluentAssertions;
using Symbolica.Abstraction;
using Xunit;

namespace Symbolica.Representation.Functions;

public class FunnelShiftRightTests
{
    [Theory]
    [ClassData(typeof(TestData))]
    private void ShouldConcatenateThenShiftRightThenExtractLow(Nibble high, Nibble low, Nibble shift)
    {
        Nibble? actual = null;

        var function = new FunnelShiftRight((FunctionId) 456UL, MockParameters.Create());

        var state = MockState.Create((InstructionId) 123UL, v => { actual = v; });
        var caller = MockCaller.Create((InstructionId) 123UL);
        var arguments = MockArguments.Create(state.Space, high, low, shift);

        function.Call(state, caller, arguments);

        var expected = ConcatenateThenShiftRightThenExtractLow(
            high,
            low,
            shift.UnsignedRemainder((Nibble) (BigInteger) Nibble.Size.Bits));

        actual.Should().Be(expected);
    }

    private static Nibble ConcatenateThenShiftRightThenExtractLow(Nibble high, Nibble low, Nibble shift)
    {
        return shift == Nibble.Zero
            ? low
            // ReSharper disable once TailRecursiveCall
            : ConcatenateThenShiftRightThenExtractLow(
                high.LogicalShiftRight(Nibble.One),
                (high.B0 ? Nibble.One : Nibble.Zero) & low.LogicalShiftRight(Nibble.One),
                --shift);
    }

    private sealed class TestData : TheoryData<Nibble, Nibble, Nibble>
    {
        public TestData()
        {
            foreach (var high in Nibble.GenerateAll())
            foreach (var low in Nibble.GenerateAll())
            foreach (var shift in Nibble.GenerateAll())
                Add(high, low, shift);
        }
    }
}
