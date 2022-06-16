using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
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

    public void Add(params IExecutable[] executables)
    {
        Interlocked.Add(ref _statesToProcess, (ulong) executables.Length);
        foreach (var executable in executables)
            Run(executable);
    }

    private void Run(IExecutable executable)
    {
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

                if (_exception == null)
                    Add(forks.ToArray());
            }
            catch (Exception exception)
            {
                _exception = exception;
            }
            finally
            {
                Interlocked.Increment(ref _completedStates);
                _throttler.Release();
                if (Interlocked.Decrement(ref _statesToProcess) == 0UL)
                    await Merge();
            }
        });
    }

    private async Task Merge()
    {
        static void WriteState(string name, IExecutable state)
        {
            var example = string.Join("_", state.Space.GetExample()
                .OrderBy(p => p.Key)
                .Select(p => $"{p.Key}{p.Value}"));
            File.WriteAllText(
                $"{name}_gen{state.Generation}_hash({state.GetMergeHash()}).json",
                JsonSerializer.Serialize(state.ToJson(), new JsonSerializerOptions { WriteIndented = true }));
        }

        _merger.Complete();
        var merged = await _merger.GetMerged();
        var states = merged.Where(s => s.Generation < 150).ToArray();
        if (!states.Any())
            _completed.SetResult();
        else
        {
            foreach (var duplicate in states.ToLookup(s => s.GetMergeHash()).Where(g => g.Count() > 1).SelectMany(x => x))
                WriteState("mergeDup", duplicate);
            _pastStates.AddRange(states.Select(s => s.Clone()));
            _merger = new(_pastStates);
            Add(states);
        }
    }

    public async Task<(ulong, ulong, Exception?)> Wait()
    {
        await _completed.Task;
        return (_completedStates, _executedInstructions, _exception);
    }
}
