using System.Collections.Generic;
using Symbolica.Expression;

namespace Symbolica.Implementation
{
    public interface IExecutable
    {
        ulong InstructionsProcessed { get; }

        IResult<IEnumerable<IExecutable>, ErrorException> Run();
    }
}
