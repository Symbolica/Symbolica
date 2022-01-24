using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Symbolica.Collection;
using Symbolica.Computation;
using Symbolica.Deserialization;
using Symbolica.Implementation;
using Symbolica.Implementation.System;
using Symbolica.Representation;

namespace Symbolica;

internal sealed class Executor
{
    private readonly int _maxParallelism;
    private readonly Options _options;
    private readonly bool _trace;

    public Executor(Options options, int maxParallelism, bool trace = false)
    {
        _options = options;
        _maxParallelism = maxParallelism;
        _trace = trace;
    }

    public async Task<ulong> Run(byte[] bytes)
    {
        var module = DeserializerFactory.Create(new DeclarationFactory()).DeserializeModule(bytes);

        var collectionFactory = new CollectionFactory();
        var spaceFactory = new SpaceFactory(collectionFactory);
        var executableFactory = new ExecutableFactory(CreateFileSystem(), spaceFactory, collectionFactory);

        using var statePool = new StatePool(_maxParallelism, _trace);
        statePool.Add(executableFactory.CreateInitial(module, _options), 0);

        return await statePool.Wait();
    }

    private static IFileSystem CreateFileSystem()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? new WslFileSystem(new FileSystem())
            : new FileSystem();
    }
}
