using System;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Application;
using Symbolica.Application.Collection;
using Symbolica.Application.Computation;
using Symbolica.Computation;
using Symbolica.Expression;
using Symbolica.Implementation;

var spaceFactory = new SpaceFactory(
    new SymbolFactory(), new ModelFactory(),
    new ContextFactory(), new CollectionFactory());

var bytes = await Serializer.Serialize(args[0], args.Last(a => a.StartsWith("--O")));
var executor = new Executor(spaceFactory, new Options(
    args.Contains("--use-symbolic-garbage"),
    args.Contains("--use-symbolic-addresses"),
    args.Contains("--use-symbolic-continuations")));

try
{
    await executor.Run(bytes);
}
catch (SymbolicaException exception)
{
    Console.WriteLine(exception.Message);

    if (exception is StateException stateException)
        Console.WriteLine(string.Join(", ", stateException.Space.GetExample().Select(p => $"{p.Key}={p.Value}")));

    return 1;
}

return 0;
