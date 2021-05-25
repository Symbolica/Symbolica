using Symbolica.Abstraction;

namespace Symbolica.Execution
{
    public interface IProgramFactory
    {
        IProgram CreateInitial(IProgramPool programPool, IModule module,
            bool useSymbolicGarbage, bool useSymbolicAddresses, bool useSymbolicContinuations);
    }
}