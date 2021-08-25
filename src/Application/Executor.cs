using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Symbolica.Abstraction;
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
    internal static class Executor
    {
        public static async Task<Result> Run(string directory,
            bool useSymbolicGarbage, bool useSymbolicAddresses, bool useSymbolicContinuations)
        {
            var buildImage = Environment.GetEnvironmentVariable("SYMBOLICA_BUILD_IMAGE");
            var translateImage = Environment.GetEnvironmentVariable("SYMBOLICA_TRANSLATE_IMAGE");

            File.Delete(Path.Combine(directory, "symbolica.bc"));
            File.Delete(Path.Combine(directory, ".symbolica.bc"));

            await CallExternalProcess(directory, buildImage == null
                ? "./symbolica.sh"
                : $"docker run -v $(pwd):/code {buildImage}");

            await CallExternalProcess(directory, translateImage == null
                ? $"~/.symbolica/translate \"{DeclarationMapper.Pattern}\""
                : $"docker run -v $(pwd):/code {translateImage} \"{DeclarationMapper.Pattern}\"");

            var bytes = await File.ReadAllBytesAsync(Path.Combine(directory, ".symbolica.bc"));
            var module = Deserializer.DeserializeModule(bytes);

            var collectionFactory = new CollectionFactory();
            var spaceFactory = new SpaceFactory(new SymbolFactory(), new ModelFactory(), collectionFactory);
            var programFactory = new ProgramFactory(CreateFileSystem(), spaceFactory, collectionFactory);

            using var programPool = new ProgramPool();
            programPool.Add(programFactory.CreateInitial(programPool, module,
                useSymbolicGarbage, useSymbolicAddresses, useSymbolicContinuations));

            try
            {
                await programPool.Wait();
            }
            catch (StateException stateException)
            {
                return Result.Failure(stateException);
            }

            return Result.Success();
        }

        private static IFileSystem CreateFileSystem()
        {
            return IsWindows()
                ? new WslFileSystem(new FileSystem())
                : new FileSystem();
        }

        private static async Task CallExternalProcess(string directory, string command)
        {
            using var process = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = directory,
                    FileName = IsWindows() ? "wsl" : "bash",
                    Arguments = IsWindows() ? command : $"-c \"{command.Replace("\"", "\\\"")}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
                throw new Exception($"{await process.StandardError.ReadToEndAsync()}");
        }

        private static bool IsWindows()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }
    }
}
