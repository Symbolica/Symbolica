using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Symbolica.Application.Collection;
using Symbolica.Application.Computation;
using Symbolica.Application.Implementation;
using Symbolica.Computation;
using Symbolica.Deserialization;
using Symbolica.Implementation;
using Symbolica.Implementation.System;
using Symbolica.Representation;

namespace Symbolica.Application
{
    internal sealed class Executor
    {
        private readonly Options _options;

        public Executor(Options options)
        {
            _options = options;
        }

        public async Task Run(byte[] bytes)
        {
            var module = DeserializerFactory.Create(new DeclarationFactory()).DeserializeModule(bytes);

            using var programPool = new ProgramPool();
            programPool.Add(CreateProgramFactory().CreateInitial(programPool, module, _options));

            await programPool.Wait();
        }

        private static ProgramFactory CreateProgramFactory()
        {
            var collectionFactory = new CollectionFactory();
            var spaceFactory = new SpaceFactory(new SymbolFactory(), new ModelFactory(), collectionFactory);

            return new ProgramFactory(CreateFileSystem(), spaceFactory, collectionFactory);
        }

        private static IFileSystem CreateFileSystem()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? new WslFileSystem(new FileSystem())
                : new FileSystem();
        }
    }
}
