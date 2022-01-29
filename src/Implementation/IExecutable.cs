using System.Collections.Generic;

namespace Symbolica.Implementation;

public interface IExecutable
{
    ulong ExecutedInstructions { get; }

    IEnumerable<IExecutable> Run();
}
