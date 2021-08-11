using FluentAssertions;
using Symbolica.Abstraction;
using Symbolica.Representation.TestUtils;
using Xunit;

namespace Symbolica.Representation.Functions
{
    public class CountOnesTests
    {
        private readonly CountOnes _function;

        public CountOnesTests()
        {
            _function = new CountOnes((FunctionId) 456UL, MockParameters.Create());
        }

        [Theory]
        [ClassData(typeof(TestData))]
        private void ShouldCountOnes(Nibble value)
        {
            Nibble? actual = null;

            _function.Call(
                MockState.Create((InstructionId) 123UL, v => { actual = v; }),
                MockCaller.Create((InstructionId) 123UL),
                MockArguments.Create(value));

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
}
