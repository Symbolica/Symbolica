using FluentAssertions;
using Symbolica.Abstraction;
using Xunit;

namespace Symbolica.Representation.Functions;

public class AbsoluteTests
{
    [Theory]
    [ClassData(typeof(TestData))]
    private void ShouldCalculateAbsoluteValue(Nibble value)
    {
        Nibble? actual = null;

        var function = new Absolute((FunctionId) 456UL, MockParameters.Create());

        var state = MockState.Create((InstructionId) 123UL, v => { actual = v; });
        var caller = MockCaller.Create((InstructionId) 123UL);
        var arguments = MockArguments.Create(state.Space, value);

        function.Call(state, caller, arguments);

        var expected = value.B3
            ? -value
            : value;

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
