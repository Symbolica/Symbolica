using System;
using System.Diagnostics;
using System.Linq;
using Symbolica;
using Symbolica.Computation;
using Symbolica.Implementation;

var bytes = await Serializer.Serialize(args[0], args.LastOrDefault(a => a.StartsWith("--O")) ?? "--O0");
var executor = new Executor(new Options(
    args.Contains("--use-symbolic-garbage"),
    args.Contains("--use-symbolic-addresses"),
    args.Contains("--use-symbolic-continuations")));

var stopwatch = new Stopwatch();
stopwatch.Start();
var executedInstructions = await executor.Run<ContextHandle>(bytes);
Console.WriteLine($"Executed {executedInstructions} instructions in {stopwatch.Elapsed}.");

return 0;
