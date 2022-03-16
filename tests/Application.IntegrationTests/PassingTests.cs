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
    private async Task ShouldPass(DirectoryInfo directory, string optimization, Options options)
    {
        var bytes = await Serializer.Serialize(directory, optimization);
        var executor = new Executor(options);

        var (_, exception) = await executor.Run(bytes);

        exception.Should().BeNull();
    }

    private sealed class TestData : TheoryData<DirectoryInfo, string, Options>
    {
        public TestData()
        {
            Add(SignCases());
            Add(DivideCases());
        }

        private static IEnumerable<(string, string, Options)> SignCases()
        {
            return
                from optimization in new[] { "--O0", "--O1", "--O2", "--Os", "--Oz" }
                from useSymbolicGarbage in new[] { false, true }
                from useSymbolicAddresses in new[] { false, true }
                from useSymbolicContinuations in new[] { false, true }
                select (
                    "sign",
                    optimization,
                    new Options(useSymbolicGarbage, useSymbolicAddresses, useSymbolicContinuations));
        }

        private static IEnumerable<(string, string, Options)> DivideCases()
        {
            return
                from useSymbolicGarbage in new[] { false, true }
                from useSymbolicAddresses in new[] { false, true }
                from useSymbolicContinuations in new[] { false, true }
                select (
                    "divide",
                    "--O0",
                    new Options(useSymbolicGarbage, useSymbolicAddresses, useSymbolicContinuations));
        }

        private void Add(IEnumerable<(string, string, Options)> cases)
        {
            foreach (var (directory, optimization, options) in cases)
                Add(
                    new DirectoryInfo(Path.Combine("..", "..", "..", "..", "pass", directory)),
                    optimization,
                    options);
        }
    }
}
