﻿using System;
using System.Linq;
using Symbolica;
using Symbolica.Abstraction;
using Symbolica.Computation;
using Symbolica.Implementation;

var bytes = await Serializer.Serialize(args[0], args.Last(a => a.StartsWith("--O")));
var executor = new Executor(new ContextFactory(), new Options(
    args.Contains("--use-symbolic-garbage"),
    args.Contains("--use-symbolic-addresses"),
    args.Contains("--use-symbolic-continuations")));

var (executedInstructions, exception) = await executor.Run(bytes);
Console.WriteLine($"Executed {executedInstructions} instructions.");

if (exception != null)
{
    Console.WriteLine(exception.Message);

    if (exception is StateException stateException)
        Console.WriteLine(string.Join(", ", stateException.Space.GetExample().Select(p => $"{p.Key}={p.Value}")));

    return 1;
}
else
{
    Console.WriteLine("No errors were found.");
}

return 0;
