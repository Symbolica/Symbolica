using System;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Application;

var result = await Executor.Run(args[0],
    args.Contains("--use-symbolic-garbage"),
    args.Contains("--use-symbolic-addresses"),
    args.Contains("--use-symbolic-continuations"));

if (result.IsSuccess)
    return 0;

Console.WriteLine(result.Exception.Message);

if (result.Exception is StateException stateException)
    Console.WriteLine(string.Join(", ", stateException.Space.GetExample().Select(p => $"{p.Key}={p.Value}")));

return 1;
