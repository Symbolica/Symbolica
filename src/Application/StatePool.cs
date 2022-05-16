using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Symbolica.Implementation;

namespace Symbolica;

internal sealed class StatePool : IDisposable
{
    private readonly TaskCompletionSource _completed;
    private readonly SemaphoreSlim _throttler;
    private readonly List<IExecutable> _pastStates;

    private StateMerger _merger;
    private Exception? _exception;
    private ulong _statesToProcess;
    private ulong _executedInstructions;
    private ulong _completedStates;

    public StatePool(int maxParallelism)
    {
        _pastStates = new();
        _merger = new(_pastStates);
        _completed = new();
        _exception = null;
        _statesToProcess = 0UL;
        _executedInstructions = 0UL;
        _throttler = new SemaphoreSlim(maxParallelism);
    }

    public void Dispose()
    {
        _throttler.Dispose();
    }

    public void Add(IExecutable executable)
    {
        Interlocked.Increment(ref _statesToProcess);
        Task.Run(async () =>
        {
            await _throttler.WaitAsync();
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var (executedInstructions, status, forks) = executable.Run();
                Interlocked.Add(ref _executedInstructions, executedInstructions);
                stopwatch.Stop();
                Console.WriteLine($"{stopwatch.Elapsed} {executedInstructions} {_executedInstructions}");

                if (status == IExecutable.Status.Merging)
                    _merger.Merge(executable);

                foreach (var fork in forks)
                    if (_exception == null)
                        Add(fork);
            }
            catch (Exception exception)
            {
                _exception = exception;
            }
            finally
            {
                Interlocked.Increment(ref _completedStates);
                Interlocked.Decrement(ref _statesToProcess);
                if (_statesToProcess == 0UL)
                    await Merge();
                _throttler.Release();
            }
        });
    }

    private async Task Merge()
    {
        _merger.Complete();
        var merged = await _merger.GetMerged();
        var states = merged.Where(s => s.Generation < 3);
        _pastStates.AddRange(states.Select(s => s.Clone()));
        foreach (var state in states)
            Add(state);
        if (!states.Any())
            _completed.SetResult();
        else
            _merger = new(_pastStates);
    }

    public async Task<(ulong, ulong, Exception?)> Wait()
    {
        await _completed.Task;

        return (_completedStates, _executedInstructions, _exception);
    }
}
