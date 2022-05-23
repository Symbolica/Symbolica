using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        => (_mergeTask, _pastStates) = (Task.Run(MergeStates), CreatePastStates(pastStates));

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

    private static ILookup<int, IExecutable> CreatePastStates(IEnumerable<IExecutable> pastStates)
    {
        static void WriteState(int index, IExecutable state)
        {
            var example = string.Join("_", state.Space.GetExample()
                .OrderBy(p => p.Key)
                .Select(p => $"{p.Key}{p.Value}"));
            File.WriteAllText(
                $"gen{state.Generation}_hash_{state.GetEquivalencyHash()}_{example}.json",
                JsonSerializer.Serialize(state.ToJson(), new JsonSerializerOptions { WriteIndented = true }));
        }

        var lookup = pastStates.ToLookup(s => s.GetEquivalencyHash());
        foreach (var duplicateGroup in lookup.Where(g => g.Count() > 1))
            foreach (var (duplicate, i) in duplicateGroup.Select((g, i) => (g, i)))
                WriteState(i, duplicate);
        return lookup;
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
