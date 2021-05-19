using Symbolica.Abstraction;

namespace Symbolica.Implementation
{
    public interface IProgramFactory
    {
        IProgram CreateInitial(IProgramPool programPool, IModule module,
            bool useSymbolicGarbage, bool useSymbolicAddresses, bool useSymbolicContinuations);
    }
}