using System.Numerics;
using FluentAssertions;
using Symbolica.Abstraction;
using Symbolica.Representation.TestUtils;
using Xunit;

namespace Symbolica.Representation.Functions
{
    public class FunnelShiftRightTests
    {
        private readonly FunnelShiftRight _function;

        public FunnelShiftRightTests()
        {
            _function = new FunnelShiftRight((FunctionId) 456UL, MockParameters.Create());
        }

        [Theory]
        [ClassData(typeof(TestData))]
        private void ShouldConcatenateThenShiftRightThenExtractLow(Nibble high, Nibble low, Nibble shift)
        {
            Nibble? actual = null;

            _function.Call(
                MockState.Create((InstructionId) 123UL, v => { actual = v; }),
                MockCaller.Create((InstructionId) 123UL),
                MockArguments.Create(high, low, shift));

            var expected = ConcatenateThenShiftRightThenExtractLow(
                high,
                low,
                shift.UnsignedRemainder((Nibble) (BigInteger) (uint) Nibble.Size));

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
}
