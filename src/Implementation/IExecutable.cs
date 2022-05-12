using System.Collections.Generic;

namespace Symbolica.Implementation;

public interface IExecutable
{
    public enum Status { NotStarted, Running, Merging, Complete }

    (ulong ExecutedInstructions, Status Status, IEnumerable<IExecutable> Forks) Run();
}
