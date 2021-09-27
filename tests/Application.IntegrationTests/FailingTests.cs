﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Symbolica.Abstraction;
using Symbolica.Application.Computation;
using Symbolica.Implementation;
using Xunit;

namespace Symbolica.Application
{
    public class FailingTests
    {
        [Theory]
        [ClassData(typeof(TestData))]
        private async Task ShouldFail(string directory, string optimization, Options options,
            StateError error, string[] symbols)
        {
            var bytes = await Serializer.Serialize(directory, optimization);
            var executor = new Executor(new ContextFactory(), options);

            executor.Awaiting(e => e.Run(bytes)).Should().Throw<StateException>()
                .Where(e => e.Error == error)
                .Which.Space.GetExample().Select(p => p.Key).Should().BeEquivalentTo(symbols);
        }

        private sealed class TestData : TheoryData<string, string, Options, StateError, string[]>
        {
            public TestData()
            {
                Add(SignCases());
                Add(BufferCases());
                Add(GeometricCases());
            }

            private static IEnumerable<(string, string, Options, StateError, string[])> SignCases()
            {
                return
                    from optimization in new[] {"--O0", "--O1", "--O2", "--Os", "--Oz"}
                    from useSymbolicGarbage in new[] {false, true}
                    from useSymbolicAddresses in new[] {false, true}
                    from useSymbolicContinuations in new[] {false, true}
                    select (
                        "sign",
                        optimization,
                        new Options(useSymbolicGarbage, useSymbolicAddresses, useSymbolicContinuations),
                        StateError.FailingAssertion,
                        new[] {"x"});
            }

            private static IEnumerable<(string, string, Options, StateError, string[])> BufferCases()
            {
                return
                    from optimization in new[] {"--O0", "--O1", "--O2", "--Os", "--Oz"}
                    from useSymbolicGarbage in new[] {false, true}
                    from useSymbolicAddresses in new[] {false, true}
                    from useSymbolicContinuations in new[] {false, true}
                    select (
                        "buffer",
                        optimization,
                        new Options(useSymbolicGarbage, useSymbolicAddresses, useSymbolicContinuations),
                        StateError.InvalidMemoryWrite,
                        Array.Empty<string>());
            }

            private static IEnumerable<(string, string, Options, StateError, string[])> GeometricCases()
            {
                return
                    from optimization in new[] {"--O0", "--O1", "--O2", "--Os", "--Oz"}
                    from useSymbolicGarbage in new[] {false, true}
                    from useSymbolicAddresses in new[] {false, true}
                    from useSymbolicContinuations in new[] {false, true}
                    select (
                        "geometric",
                        optimization,
                        new Options(useSymbolicGarbage, useSymbolicAddresses, useSymbolicContinuations),
                        StateError.DivideByZero,
                        new[] {"n", "r"});
            }

            private void Add(IEnumerable<(string, string, Options, StateError, string[])> cases)
            {
                foreach (var (directory, optimization, options, error, symbols) in cases)
                    Add(Path.Combine("..", "..", "..", "..", "fail", directory), optimization, options, error, symbols);
            }
        }
    }
}
