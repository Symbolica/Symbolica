using System;
using System.Linq;
using Symbolica.Application;

var result = await Executor.Run(args[0],
    args.Contains("--use-symbolic-garbage"),
    args.Contains("--use-symbolic-addresses"),
    args.Contains("--use-symbolic-continuations"));

if (result.IsSuccess)
    return 0;

Console.WriteLine(result.Exception.Message);
Console.WriteLine(string.Join(", ", result.Exception.Space.GetExample().Select(p => $"{p.Key}={p.Value}")));

return 1;
