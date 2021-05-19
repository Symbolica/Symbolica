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

var file = new FileInfo(args[0]);
var useSymbolicGarbage = args.Contains("--useSymbolicGarbage");
var useSymbolicAddresses = args.Contains("--useSymbolicAddresses");
var useSymbolicContinuations = args.Contains("--useSymbolicContinuations");

IFileSystem fileSystem = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
    ? new FileSystem()
    : RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        ? new WslFileSystem(new FileSystem())
        : throw new Exception("Platform is unsupported.");

var collectionFactory = new CollectionFactory();
var spaceFactory = new SpaceFactory(new SymbolFactory(), new ModelFactory(), collectionFactory);
var programFactory = new ProgramFactory(fileSystem, spaceFactory, collectionFactory);

var executor = new Executor(new Deserializer(), programFactory);

using var programPool = new ProgramPool();
var result = await executor.Run(programPool, file,
    useSymbolicGarbage, useSymbolicAddresses, useSymbolicContinuations);

if (result.IsSuccess)
    return 0;

Console.WriteLine(result.Message);
Console.WriteLine(string.Join(", ", result.Example.Select(p => $"{p.Key}={p.Value}")));

return 1;