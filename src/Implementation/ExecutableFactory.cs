﻿using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Collection;
using Symbolica.Expression;
using Symbolica.Implementation.Exceptions;
using Symbolica.Implementation.Memory;
using Symbolica.Implementation.Stack;
using Symbolica.Implementation.System;

namespace Symbolica.Implementation;

public sealed class ExecutableFactory
{
    private readonly ICollectionFactory _collectionFactory;
    private readonly IFileSystem _fileSystem;
    private readonly ISpaceFactory _spaceFactory;

    public ExecutableFactory(
        IFileSystem fileSystem, ISpaceFactory spaceFactory, ICollectionFactory collectionFactory)
    {
        _fileSystem = fileSystem;
        _spaceFactory = spaceFactory;
        _collectionFactory = collectionFactory;
    }

    public IExecutable CreateInitial(IModule module, Options options)
    {
        return State.CreateInitial(_spaceFactory, module, options,
            CreateGlobals(module), CreateMemory(module, options), CreateStack(module, options), CreateSystem(module));
    }

    private IPersistentGlobals CreateGlobals(IModule module)
    {
        return PersistentGlobals.Create(module, _collectionFactory);
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
