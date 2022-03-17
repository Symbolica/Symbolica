using System.Collections.Generic;
using Symbolica.Expression;

namespace Symbolica.Implementation;

public interface IExecutable
{
    ulong ExecutedInstructions { get; }
    ISpace Space { get; }

    IEnumerable<IExecutable> Run();
}
