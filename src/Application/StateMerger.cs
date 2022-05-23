using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Symbolica.Implementation;

namespace Symbolica;

internal sealed class StateMerger
{
    private readonly Dictionary<int, List<IExecutable>> _merged = new();
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
        return _merged.Values.SelectMany(x => x).ToList();
    }

    public void Merge(IExecutable state)
    {
        _mergeQueue.Add(state);
    }

    private void MergeStates()
    {
        void Merge(IExecutable state)
        {
            var hash = state.GetEquivalencyHash();
            var equivalent = _merged.TryGetValue(hash, out var eq)
                ? eq
                : new();
            var (hasMerged, merged) = equivalent.Aggregate(
                (hasMerged: false, equivalent: Enumerable.Empty<IExecutable>()),
                (x, s) =>
                    !x.hasMerged && s.TryMerge(state, out var merged)
                        ? (true, x.equivalent.Append(merged))
                        : (x.hasMerged, x.equivalent.Append(s)));
            _merged[hash] = (hasMerged
                ? merged
                : merged.Append(state)).ToList();
        }

        bool HasBeenSeenBefore(IExecutable state)
        {
            var hash = state.GetEquivalencyHash();
            return _pastStates.Contains(hash) && _pastStates[hash].Any(state.IsEquivalentTo);
        }

        foreach (var state in _mergeQueue.GetConsumingEnumerable())
            if (!HasBeenSeenBefore(state))
                Merge(state);
    }
}
