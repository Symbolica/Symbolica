using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Symbolica.Expression;

namespace Symbolica.Implementation;

public interface IExecutable
{
    public enum Status { NotStarted, Running, Merging, Complete }

    int Generation { get; }

    ISpace Space { get; }

    (ulong ExecutedInstructions, Status Status, IEnumerable<IExecutable> Forks) Run();

    bool IsEquivalentTo(IExecutable state);

    bool TryMerge(IExecutable state, [MaybeNullWhen(false)] out IExecutable merged);

    IExecutable Clone();

    int GetEquivalencyHash();

    object ToJson();
}
