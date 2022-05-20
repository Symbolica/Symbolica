using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Symbolica.Implementation;

namespace Symbolica;

internal sealed class StateMerger
{
    private readonly List<IExecutable> _merged = new();
    private readonly BlockingCollection<IExecutable> _mergeQueue = new();
    private readonly Task _mergeTask;
    private readonly ILookup<int, IExecutable> _pastStates;

    public StateMerger(IEnumerable<IExecutable> pastStates)
        => (_mergeTask, _pastStates) = (Task.Run(MergeStates), pastStates.ToLookup(s => s.GetEquivalencyHash()));

    public void Complete()
    {
        _mergeQueue.CompleteAdding();
    }

    public async Task<IReadOnlyCollection<IExecutable>> GetMerged()
    {
        await _mergeTask;
        return _merged;
    }

    public void Merge(IExecutable state)
    {
        _mergeQueue.Add(state);
    }

    private void MergeStates()
    {
        bool HasBeenSeenBefore(IExecutable state)
        {
            var hash = state.GetEquivalencyHash();
            return _pastStates.Contains(hash) && _pastStates[hash].Any(state.IsEquivalentTo);
        }

        foreach (var state in _mergeQueue.GetConsumingEnumerable())
            // TODO: Actually attempt to merge this state
            if (!HasBeenSeenBefore(state))
                _merged.Add(state);
    }
}
