using System.IO;
using System.Threading.Tasks;
using Symbolica.Abstraction;
using Symbolica.Implementation;

namespace Symbolica.Execution
{
    public sealed class Executor
    {
        private readonly IDeserializer _deserializer;
        private readonly IProgramFactory _programFactory;

        public Executor(IDeserializer deserializer, IProgramFactory programFactory)
        {
            _deserializer = deserializer;
            _programFactory = programFactory;
        }

        public async Task<Result> Run(IAwaitableProgramPool programPool, FileInfo file,
            bool useSymbolicGarbage, bool useSymbolicAddresses, bool useSymbolicContinuations)
        {
            await using var stream = file.OpenRead();
            var module = await _deserializer.DeserializeModule(stream);

            var program = _programFactory.CreateInitial(programPool, module,
                useSymbolicGarbage, useSymbolicAddresses, useSymbolicContinuations);

            programPool.Add(program);

            try
            {
                await programPool.Wait();
            }
            catch (StateException stateException)
            {
                return Result.Failure(stateException.Message, stateException.Space.GetExample());
            }

            return Result.Success();
        }
    }
}