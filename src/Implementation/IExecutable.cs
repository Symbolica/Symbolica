using System;
using System.Collections.Generic;

namespace Symbolica.Implementation;

public interface IExecutable
{
    Guid Id { get; }
    ulong ExecutedInstructions { get; }

    IEnumerable<IExecutable> Run();
}
