using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Collection;
using Symbolica.Expression;
using Symbolica.Implementation.Exceptions;
using Symbolica.Implementation.Memory;
using Symbolica.Implementation.Stack;
using Symbolica.Implementation.System;

namespace Symbolica.Implementation;

public sealed class InitialStateFactory : IStateFactory
{
    private readonly ICollectionFactory _collectionFactory;
    private readonly IFileSystem _fileSystem;
    private readonly IModule _module;
    private readonly Options _options;
    private readonly ISpaceFactory _spaceFactory;

    public InitialStateFactory(IModule module, Options options,
        IFileSystem fileSystem, ISpaceFactory spaceFactory, ICollectionFactory collectionFactory)
    {
        _module = module;
        _options = options;
        _fileSystem = fileSystem;
        _spaceFactory = spaceFactory;
        _collectionFactory = collectionFactory;
    }

    public IExecutableState Create(IStatePool statePool)
    {
        var space = _spaceFactory.CreateInitial(_module.PointerSize, _options.UseSymbolicGarbage);

        var globals = PersistentGlobals.Create(_module, _collectionFactory);
        var memory = new MemoryProxy(space, CreateMemory(_module, _options));
        var stack = new StackProxy(space, memory, CreateStack(_module, _options));
        var system = new SystemProxy(space, memory, CreateSystem(_module));

        return new State(statePool, _module, space,
            globals, memory, stack, system);
    }

    private IPersistentMemory CreateMemory(IModule module, Options options)
    {
        var alignment = module.PointerSize.ToBytes();
        var blockFactory = new BlockFactory();

        return options.UseSymbolicAddresses
            ? SymbolicMemory.Create(alignment, blockFactory, _collectionFactory)
            : ConstantMemory.Create(alignment, blockFactory, _collectionFactory);
    }

    private IPersistentStack CreateStack(IModule module, Options options)
    {
        IVariadicAbi variadicAbi = module.Target.Split('-').First() switch
        {
            "x86_64" => new X64VariadicAbi(),
            "x86" => new X86VariadicAbi(),
            _ => throw new UnsupportedArchitectureException(module.Target)
        };

        var frameFactory = new FrameFactory(variadicAbi, _collectionFactory);

        var continuationFactory = options.UseSymbolicContinuations
            ? new SymbolicContinuationFactory()
            : ConstantContinuationFactory.Create();

        return PersistentStack.Create(module, frameFactory,
            continuationFactory, _collectionFactory);
    }

    private IPersistentSystem CreateSystem(IModule module)
    {
        var descriptionFactory = new DescriptionFactory(_fileSystem);

        return PersistentSystem.Create(module, descriptionFactory, _collectionFactory);
    }
}
