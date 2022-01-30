using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Symbolica.Implementation;
using Xunit;

namespace Symbolica;

public class PassingTests
{
    [Theory]
    [ClassData(typeof(TestData))]
    private async Task ShouldPass(string directory, string optimization, Options options)
    {
        var bytes = await Serializer.Serialize(directory, optimization);
        var executor = new Executor(new PooledContextFactory(), options);

        var (_, exception) = await executor.Run(bytes);

        exception.Should().BeNull();
    }

    private sealed class TestData : TheoryData<string, string, Options>
    {
        public TestData()
        {
            Add(SignCases());
        }

        private static IEnumerable<(string, string, Options)> SignCases()
        {
            return
                from optimization in new[] {"--O0", "--O1", "--O2", "--Os", "--Oz"}
                from useSymbolicGarbage in new[] {false, true}
                from useSymbolicAddresses in new[] {false, true}
                from useSymbolicContinuations in new[] {false, true}
                select (
                    "sign",
                    optimization,
                    new Options(useSymbolicGarbage, useSymbolicAddresses, useSymbolicContinuations));
        }

        private void Add(IEnumerable<(string, string, Options)> cases)
        {
            foreach (var (directory, optimization, options) in cases)
                Add(Path.Combine("..", "..", "..", "..", "pass", directory), optimization, options);
        }
    }
}
