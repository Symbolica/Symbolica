using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions;

internal sealed class Loop : IFunction
{
    public Loop(FunctionId id, IParameters parameters)
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

        using var stream = File.OpenWrite($"{example}.json");
        using var writer = new StreamWriter(stream, Encoding.UTF8);

        Serialize(writer, state);
        state.Merge();

        state.Stack.SetVariable(caller.Id, exprFactory.CreateZero(caller.Size));
    }

    private static void SerializeNullable(TextWriter writer, object? obj)
    {
        if (obj == null)
            writer.Write("null");
        else
            Serialize(writer, obj);
    }

    private static void Serialize(TextWriter writer, object obj)
    {
        var nullableType = obj.GetType();
        var type = Nullable.GetUnderlyingType(nullableType) ?? nullableType;

        if (obj is IDisposable or IModule)
        {
            writer.Write($"\"{type}\"");
        }
        else if (obj is string || type.IsEnum)
        {
            writer.Write($"\"{obj}\"");
        }
        else if (obj is bool b)
        {
            writer.Write(b ? "true" : "false");
        }
        else if (obj is BigInteger || Type.GetTypeCode(type) != TypeCode.Object)
        {
            writer.Write(obj);
        }
        else if (obj is IEnumerable enumerable)
        {
            var comma = false;
            writer.Write("[");
            foreach (var value in enumerable)
            {
                if (comma)
                    writer.Write(",");
                SerializeNullable(writer, value);
                comma = true;
            }

            writer.Write("]");
        }
        else
        {
            writer.Write("{");
            Serialize(writer, "$type");
            writer.Write(":");
            SerializeNullable(writer, type.Name);

            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                         .OrderBy(f => f.Name))
            {
                writer.Write(",");
                Serialize(writer, field.Name);
                writer.Write(":");
                SerializeNullable(writer, field.GetValue(obj));
            }

            writer.Write("}");
        }
    }
}
