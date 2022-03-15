using FluentAssertions;
using Symbolica.Abstraction;
using Xunit;

namespace Symbolica.Representation.Functions;

public class CountOnesTests
{
    [Theory]
    [ClassData(typeof(TestData))]
    private void ShouldCountOnes(Nibble value)
    {
        Nibble? actual = null;

        var function = new CountOnes((FunctionId) 456UL, MockParameters.Create());

        var state = MockState.Create((InstructionId) 123UL, v => { actual = v; });
        var caller = MockCaller.Create((InstructionId) 123UL);
        var arguments = MockArguments.Create(state.Space, value);

        function.Call(state, caller, arguments);

        var expected = (value.B3 ? Nibble.One : Nibble.Zero) +
                       (value.B2 ? Nibble.One : Nibble.Zero) +
                       (value.B1 ? Nibble.One : Nibble.Zero) +
                       (value.B0 ? Nibble.One : Nibble.Zero);

        actual.Should().Be(expected);
    }

    private sealed class TestData : TheoryData<Nibble>
    {
        public TestData()
        {
            foreach (var value in Nibble.GenerateAll())
                Add(value);
        }
    }
}
