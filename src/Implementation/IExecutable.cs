using System;
using System.Collections.Generic;

namespace Symbolica.Implementation;

public interface IExecutable : IDisposable
{
    ulong ExecutedInstructions { get; }

    IEnumerable<IExecutable> Run();
}
