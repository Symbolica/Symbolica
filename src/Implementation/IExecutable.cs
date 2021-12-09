using System.Collections.Generic;

namespace Symbolica.Implementation
{
    public interface IExecutable
    {
        ulong InstructionsProcessed { get; }

        IEnumerable<IExecutable> Run();
    }
}
