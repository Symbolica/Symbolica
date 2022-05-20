using System.Collections.Generic;

namespace Symbolica.Implementation;

public interface IExecutable
{
    public enum Status { NotStarted, Running, Merging, Complete }

    public int Generation { get; }

    (ulong ExecutedInstructions, Status Status, IEnumerable<IExecutable> Forks) Run();

    bool IsEquivalentTo(IExecutable state);

    IExecutable Clone();

    int GetEquivalencyHash();
}
