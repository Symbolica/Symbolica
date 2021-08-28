using System;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Collection;
using Symbolica.Expression;
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

        public IProgram CreateInitial(IProgramPool programPool, IModule module,
            bool useSymbolicGarbage, bool useSymbolicAddresses, bool useSymbolicContinuations)
        {
            var space = _spaceFactory.CreateInitial(module.PointerSize, useSymbolicGarbage);

            var alignment = module.PointerSize.ToBytes();
            var blockFactory = new BlockFactory();

            var memory = useSymbolicAddresses
                ? SymbolicMemory.Create(alignment, blockFactory, _collectionFactory)
                : ConstantMemory.Create(alignment, blockFactory, _collectionFactory);

            var architecture = module.Target.Split('-').First();

            IVariadicAbi variadicAbi = architecture switch
            {
                "x86_64" => new X64VariadicAbi(),
                "x86" => new X86VariadicAbi(),
                _ => throw new Exception($"Architecture {architecture} is unsupported.")
            };

            var frameFactory = new FrameFactory(variadicAbi, _collectionFactory);

            var continuationFactory = useSymbolicContinuations
                ? new SymbolicContinuationFactory()
                : ConstantContinuationFactory.Create();

            var stack = PersistentStack.Create(module, frameFactory,
                continuationFactory, _collectionFactory);

            var descriptionFactory = new DescriptionFactory(_fileSystem);

            var system = PersistentSystem.Create(module, descriptionFactory, _collectionFactory);

            var globals = PersistentGlobals.Create(module, _collectionFactory);

            var memoryProxy = new MemoryProxy(space, memory);
            var stackProxy = new StackProxy(space, memoryProxy, stack);
            var systemProxy = new SystemProxy(space, memoryProxy, system);

            var state = new State(programPool, space,
                memoryProxy, stackProxy, systemProxy,
                module, globals);

            return new Program(() => state);
        }
    }
}
