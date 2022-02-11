using System;
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
    private readonly Options _options;

    public Executor(Options options)
    {
        _options = options;
    }

    public async Task<(ulong, Exception?)> Run<TContext>(byte[] bytes)
        where TContext : IContext, new()
    {
        var module = DeserializerFactory.Create(new DeclarationFactory()).DeserializeModule(bytes);

        var collectionFactory = new CollectionFactory();
        var spaceFactory = new SpaceFactory<TContext>(collectionFactory);
        var executableFactory = new ExecutableFactory(CreateFileSystem(), spaceFactory, collectionFactory);

        using var statePool = new StatePool();
        statePool.Add(executableFactory.CreateInitial(module, _options));

        return await statePool.Wait();
    }

    private static IFileSystem CreateFileSystem()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? new WslFileSystem(new FileSystem())
            : new FileSystem();
    }
}
