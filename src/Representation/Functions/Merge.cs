using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions;

internal sealed class Merge : IFunction
{
    public Merge(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IExpressionFactory exprFactory, IState state, ICaller caller, IArguments arguments)
    {
        var example = string.Join("_", state.Space.GetExample()
            .OrderBy(p => p.Key)
            .Select(p => $"{p.Key}{p.Value}"));
        Console.WriteLine(example);

        using var stream = File.OpenWrite($"json_test_gen{state.Generation}_{example}.json");
        using var writer = new StreamWriter(stream, Encoding.UTF8);
        writer.Write(JsonSerializer.Serialize(state.ToJson(), new JsonSerializerOptions { WriteIndented = true }));

        state.Merge();
    }
}
