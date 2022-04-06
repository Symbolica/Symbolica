using System.Numerics;
using FluentAssertions;
using Symbolica.Abstraction;
using Xunit;

namespace Symbolica.Representation.Functions;

public class FunnelShiftLeftTests
{
    [Theory]
    [ClassData(typeof(TestData))]
    private void ShouldConcatenateThenShiftLeftThenExtractHigh(Nibble high, Nibble low, Nibble shift)
    {
        Nibble? actual = null;

        var function = new FunnelShiftLeft((FunctionId) 456UL, MockParameters.Create());

        var state = MockState.Create((InstructionId) 123UL, v => { actual = v; });
        var caller = MockCaller.Create((InstructionId) 123UL);
        var arguments = MockArguments.Create(high, low, shift);

        function.Call(state, caller, arguments);

        var expected = ConcatenateThenShiftLeftThenExtractHigh(
            high,
            low,
            shift.UnsignedRemainder((Nibble) (BigInteger) (uint) Nibble.Size));

        actual.Should().Be(expected);
    }

    private static Nibble ConcatenateThenShiftLeftThenExtractHigh(Nibble high, Nibble low, Nibble shift)
    {
        return shift == Nibble.Zero
            ? high
            // ReSharper disable once TailRecursiveCall
            : ConcatenateThenShiftLeftThenExtractHigh(
                high.ShiftLeft(Nibble.One) & (low.B3 ? Nibble.One : Nibble.Zero),
                low.ShiftLeft(Nibble.One),
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
