using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Symbolica.Implementation;

namespace Symbolica;

internal sealed class StateMerger
{
    private readonly List<IExecutable> _merged = new();
    private readonly BlockingCollection<IExecutable> _mergeQueue = new();
    private readonly Task _mergeTask;

    public StateMerger() => _mergeTask = Task.Run(MergeStates);

    public void Complete()
    {
        _mergeQueue.CompleteAdding();
    }

    public async Task<IEnumerable<IExecutable>> GetMerged()
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
        foreach (var state in _mergeQueue.GetConsumingEnumerable())
            // TODO: Actually attempt to merge this state
            _merged.Add(state);
    }
}
