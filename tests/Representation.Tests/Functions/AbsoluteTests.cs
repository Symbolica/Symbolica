using FluentAssertions;
using Symbolica.Abstraction;
using Symbolica.Representation.TestUtils;
using Xunit;

namespace Symbolica.Representation.Functions
{
    public class AbsoluteTests
    {
        private readonly Absolute _function;

        public AbsoluteTests()
        {
            _function = new Absolute((FunctionId) 456UL, MockParameters.Create());
        }

        [Theory]
        [ClassData(typeof(TestData))]
        private void ShouldCalculateAbsoluteValue(Nibble value)
        {
            Nibble? actual = null;

            _function.Call(
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
}
