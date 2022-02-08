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

        function.Call(
            MockState.Create((InstructionId) 123UL, v => { actual = v; }),
            MockCaller.Create((InstructionId) 123UL),
            MockArguments.Create(value));

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
