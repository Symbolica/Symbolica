using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
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
        return _merged.Values.SelectMany(x => x).Where(x => !HasBeenSeenBefore(x)).ToList();
    }

    public void Merge(IExecutable state)
    {
        WriteState(state);
        _mergeQueue.Add(state);
    }

    private void MergeStates()
    {
        void Merge(IExecutable state)
        {
            var hash = state.GetMergeHash();
            var mergeCandidates = _merged.TryGetValue(hash, out var eq)
                ? eq
                : new();
            var (hasMerged, merged) = mergeCandidates.Aggregate(
                (hasMerged: false, states: Enumerable.Empty<IExecutable>()),
                (x, s) =>
                    !x.hasMerged && s.TryMerge(state, out var merged)
                        ? (true, x.states.Append(merged))
                        : (x.hasMerged, x.states.Append(s)));
            _merged[hash] = (hasMerged
                ? merged
                : merged.Append(state)).ToList();
        }

        foreach (var state in _mergeQueue.GetConsumingEnumerable())
            Merge(state);
    }

    private static void WriteState(IExecutable state)
    {
        var example = string.Join("_", state.Space.GetExample()
            .OrderBy(p => p.Key)
            .Select(p => $"{p.Key}{p.Value}"));
        File.WriteAllText(
            $"hash({state.GetEquivalencyHash()})_gen{state.Generation}_{example}.json",
            JsonSerializer.Serialize(state.ToJson(), new JsonSerializerOptions { WriteIndented = true }));
    }

    private bool HasBeenSeenBefore(IExecutable state)
    {
        var hash = state.GetEquivalencyHash();
        if (_pastStates.Contains(hash))
        {
            WriteState(state);
            WriteState(_pastStates[hash].First());
        }
        return _pastStates.Contains(hash) && _pastStates[hash].Any(state.IsEquivalentTo);
    }
}
