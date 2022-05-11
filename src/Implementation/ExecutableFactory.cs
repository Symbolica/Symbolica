using System.Linq;
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
    private readonly IExpressionFactory _exprFactory;
    private readonly IFileSystem _fileSystem;
    private readonly ISpaceFactory _spaceFactory;

    public ExecutableFactory(IFileSystem fileSystem, ISpaceFactory spaceFactory,
        ICollectionFactory collectionFactory, IExpressionFactory exprFactory)
    {
        _fileSystem = fileSystem;
        _spaceFactory = spaceFactory;
        _collectionFactory = collectionFactory;
        _exprFactory = exprFactory;
    }

    public IExecutable CreateInitial(IModule module, Options options)
    {
        var space = _spaceFactory.CreateInitial();

        var globals = PersistentGlobals.Create(module, _collectionFactory, _exprFactory);
        var memory = new MemoryProxy(CreateMemory(module, options));
        var stack = new StackProxy(_exprFactory, CreateStack(module, options));
        var system = new SystemProxy(CreateSystem(module));

        return new State(new NoOp(), module, space,
            globals, memory, stack, system);
    }

    private IPersistentMemory CreateMemory(IModule module, Options options)
    {
        var alignment = module.PointerSize.ToBytes();
        var blockFactory = new BlockFactory(_exprFactory);

        return options.UseSymbolicAddresses
            ? SymbolicMemory.Create(alignment, blockFactory, _collectionFactory, _exprFactory)
            : ConstantMemory.Create(alignment, blockFactory, _collectionFactory, _exprFactory);
    }

    private IPersistentStack CreateStack(IModule module, Options options)
    {
        IVariadicAbi variadicAbi = module.Target.Split('-').First() switch
        {
            "x86_64" => new X64VariadicAbi(_exprFactory),
            "x86" => new X86VariadicAbi(_exprFactory),
            _ => throw new UnsupportedArchitectureException(module.Target)
        };

        var frameFactory = new FrameFactory(variadicAbi, _collectionFactory);

        var continuationFactory = options.UseSymbolicContinuations
            ? new SymbolicContinuationFactory(_exprFactory)
            : ConstantContinuationFactory.Create(_exprFactory);

        return PersistentStack.Create(module, frameFactory,
            continuationFactory, _collectionFactory);
    }

    private IPersistentSystem CreateSystem(IModule module)
    {
        var descriptionFactory = new DescriptionFactory(_exprFactory, _fileSystem);

        return PersistentSystem.Create(module, descriptionFactory, _collectionFactory, _exprFactory);
    }

    private sealed class NoOp : IStateAction
    {
        public void Invoke(IState state)
        {
        }
    }
}
