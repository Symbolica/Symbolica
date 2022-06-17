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
    private int _mergeCount = 0;

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
        // WriteState(state, $"hash({state.GetMergeHash()})_gen{state.Generation}");
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
                {
                    if (!x.hasMerged && s.TryMerge(state, out var merged))
                    {
                        WriteState(s, $"merge_gen{state.Generation}_{_mergeCount}_left");
                        WriteState(state, $"merge_gen{state.Generation}_{_mergeCount}_right");
                        WriteState(merged, $"merge_gen{state.Generation}_{_mergeCount}_result");
                        ++_mergeCount;
                        return (true, x.states.Append(merged));
                    }
                    return (x.hasMerged, x.states.Append(s));
                });
            _merged[hash] = (hasMerged
                ? merged
                : merged.Append(state)).ToList();
        }

        foreach (var state in _mergeQueue.GetConsumingEnumerable())
            Merge(state);
    }

    private static void WriteState(IExecutable state, string name)
    {
        var example = string.Join("_", state.Space.GetExample()
            .OrderBy(p => p.Key)
            .Select(p => $"{p.Key}{p.Value}"));
        File.WriteAllText(
            $"{name}.json",
            JsonSerializer.Serialize(
                state.ToJson(),
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                    MaxDepth = 256
                }));
    }

    private bool HasBeenSeenBefore(IExecutable state)
    {
        var hash = state.GetEquivalencyHash();
        if (_pastStates.Contains(hash))
        {
            static string FileName(IExecutable state) =>
                $"hash({state.GetEquivalencyHash()})_gen{state.Generation}";
            WriteState(state, FileName(state));
            WriteState(_pastStates[hash].First(), FileName(_pastStates[hash].First()));
        }
        return _pastStates.Contains(hash) && _pastStates[hash].Any(state.IsEquivalentTo);
    }
}
