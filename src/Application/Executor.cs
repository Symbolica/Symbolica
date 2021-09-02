using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Symbolica.Application.Collection;
using Symbolica.Application.Computation;
using Symbolica.Application.Implementation;
using Symbolica.Computation;
using Symbolica.Deserialization;
using Symbolica.Expression;
using Symbolica.Implementation;
using Symbolica.Implementation.System;
using Symbolica.Representation;

namespace Symbolica.Application
{
    internal static class Executor
    {
        public static async Task<Result> Run(string directory, Options options)
        {
            var bytes = await Serializer.Serialize(directory);

            try
            {
                await Run(bytes, options);
            }
            catch (SymbolicaException exception)
            {
                return Result.Failure(exception);
            }

            return Result.Success();
        }

        private static async Task Run(byte[] bytes, Options options)
        {
            var module = DeserializerFactory.Create(new DeclarationFactory()).DeserializeModule(bytes);

            using var programPool = new ProgramPool();
            programPool.Add(CreateProgramFactory().CreateInitial(programPool, module, options));

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
