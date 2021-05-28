using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Symbolica.Application;
using Symbolica.Application.Collection;
using Symbolica.Application.Computation;
using Symbolica.Application.Implementation;
using Symbolica.Computation;
using Symbolica.Deserialization;
using Symbolica.Execution;
using Symbolica.Implementation;
using Symbolica.Implementation.System;

IFileSystem fileSystem = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
    ? new FileSystem()
    : RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        ? new WslFileSystem(new FileSystem())
        : throw new Exception("Platform is unsupported.");

var collectionFactory = new CollectionFactory();
var spaceFactory = new SpaceFactory(new SymbolFactory(), new ModelFactory(), collectionFactory);
var programFactory = new ProgramFactory(fileSystem, spaceFactory, collectionFactory);

var executor = new Executor(programFactory);

await using var stream = File.OpenRead(args[0]);
var module = await Deserializer.DeserializeModule(stream);

using var programPool = new ProgramPool();
var result = await executor.Run(programPool, module,
    args.Contains("--use-symbolic-garbage"),
    args.Contains("--use-symbolic-addresses"),
    args.Contains("--use-symbolic-continuations"));

if (result.IsSuccess)
    return 0;

Console.WriteLine(result.Message);
Console.WriteLine(string.Join(", ", result.Example.Select(p => $"{p.Key}={p.Value}")));

return 1;
