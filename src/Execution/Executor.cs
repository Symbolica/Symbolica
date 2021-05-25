using System.Threading.Tasks;
using Symbolica.Abstraction;

namespace Symbolica.Execution
{
    public sealed class Executor
    {
        private readonly IProgramFactory _programFactory;

        public Executor(IProgramFactory programFactory)
        {
            _programFactory = programFactory;
        }

        public async Task<Result> Run(IAwaitableProgramPool programPool, IModule module,
            bool useSymbolicGarbage, bool useSymbolicAddresses, bool useSymbolicContinuations)
        {
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