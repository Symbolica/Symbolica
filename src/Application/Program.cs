using System;
using System.Linq;
using Symbolica.Application;
using Symbolica.Application.Computation;
using Symbolica.Implementation;

var bytes = await Serializer.Serialize(args[0], args.Last(a => a.StartsWith("--O")));
var executor = new Executor(new ContextFactory(), new Options(
    UInt32.TryParse(args.SkipWhile(a => a != "--max-errors").Skip(1).FirstOrDefault(), out var maxErrors) ? maxErrors : 1,
    args.Contains("--use-symbolic-garbage"),
    args.Contains("--use-symbolic-addresses"),
    args.Contains("--use-symbolic-continuations")));

var (instructionsProcessed, errors) = await executor.Run(bytes);
Console.WriteLine($"Processed {instructionsProcessed} instructions.");
if (errors.Any())
{
    Console.WriteLine("The following errors were found:");
    Console.WriteLine(String.Join(Environment.NewLine, errors));
}
else
{
    Console.WriteLine("No errors were found.");
}

return 0;
