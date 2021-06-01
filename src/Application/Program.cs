using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Symbolica.Abstraction;
using Symbolica.Application.Collection;
using Symbolica.Application.Computation;
using Symbolica.Application.Implementation;
using Symbolica.Computation;
using Symbolica.Deserialization;
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

using var programPool = new ProgramPool();

await using var stream = File.OpenRead(args[0]);
var module = await Deserializer.DeserializeModule(stream);

programPool.Add(programFactory.CreateInitial(programPool, module,
    args.Contains("--use-symbolic-garbage"),
    args.Contains("--use-symbolic-addresses"),
    args.Contains("--use-symbolic-continuations")));

try
{
    await programPool.Wait();
}
catch (StateException stateException)
{
    Console.WriteLine(stateException.Message);
    Console.WriteLine(string.Join(", ", stateException.Space.GetExample().Select(p => $"{p.Key}={p.Value}")));

    return 1;
}

return 0;
