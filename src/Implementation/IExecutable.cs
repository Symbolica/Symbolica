using System.Collections.Generic;

namespace Symbolica.Implementation;

public interface IExecutable
{
    (ulong ExecutedInstructions, IEnumerable<IExecutable> Forks) Run();
}
