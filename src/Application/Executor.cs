using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Symbolica.Application.Collection;
using Symbolica.Application.Implementation;
using Symbolica.Deserialization;
using Symbolica.Expression;
using Symbolica.Implementation;
using Symbolica.Implementation.System;
using Symbolica.Representation;

namespace Symbolica.Application
{
    internal sealed class Executor
    {
        private readonly Options _options;
        private readonly ISpaceFactory _spaceFactory;

        public Executor(ISpaceFactory spaceFactory, Options options)
        {
            _spaceFactory = spaceFactory;
            _options = options;
        }

        public async Task Run(byte[] bytes)
        {
            var module = DeserializerFactory.Create(new DeclarationFactory()).DeserializeModule(bytes);

            var executableFactory = new ExecutableFactory(CreateFileSystem(), _spaceFactory, new CollectionFactory());

            using var statePool = new StatePool();
            statePool.Add(executableFactory.CreateInitial(module, _options));

            await statePool.Wait();
        }

        private static IFileSystem CreateFileSystem()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? new WslFileSystem(new FileSystem())
                : new FileSystem();
        }
    }
}
