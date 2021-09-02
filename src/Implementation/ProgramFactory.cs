using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Collection;
using Symbolica.Expression;
using Symbolica.Implementation.Exceptions;
using Symbolica.Implementation.Memory;
using Symbolica.Implementation.Stack;
using Symbolica.Implementation.System;

namespace Symbolica.Implementation
{
    public sealed class ProgramFactory
    {
        private readonly ICollectionFactory _collectionFactory;
        private readonly IFileSystem _fileSystem;
        private readonly ISpaceFactory _spaceFactory;

        public ProgramFactory(IFileSystem fileSystem, ISpaceFactory spaceFactory, ICollectionFactory collectionFactory)
        {
            _fileSystem = fileSystem;
            _spaceFactory = spaceFactory;
            _collectionFactory = collectionFactory;
        }

        public IProgram CreateInitial(IProgramPool programPool, IModule module, Options options)
        {
            var space = _spaceFactory.CreateInitial(module.PointerSize, options.UseSymbolicGarbage);

            var globals = PersistentGlobals.Create(module, _collectionFactory);
            var memory = new MemoryProxy(space, CreateMemory(module, options));
            var stack = new StackProxy(space, memory, CreateStack(module, options));
            var system = new SystemProxy(space, memory, CreateSystem(module));

            var state = new State(programPool, module, space,
                globals, memory, stack, system);

            return new Program(() => state);
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
}
