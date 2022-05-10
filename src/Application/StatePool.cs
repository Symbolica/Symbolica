using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Symbolica.Implementation;

namespace Symbolica;

internal sealed class StatePool : IDisposable
{
    private readonly CountdownEvent _countdownEvent;
    private readonly SemaphoreSlim _throttler;
    private Exception? _exception;
    private ulong _executedInstructions;
    private ulong _completedStates;

    public StatePool(int maxParallelism)
    {
        _countdownEvent = new CountdownEvent(1);
        _exception = null;
        _executedInstructions = 0UL;
        _throttler = new SemaphoreSlim(maxParallelism);
    }

    public void Dispose()
    {
        _countdownEvent.Dispose();
    }

    public void Add(IExecutable executable)
    {
        _countdownEvent.AddCount();
        Task.Run(async () =>
        {
            await _throttler.WaitAsync();
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var (executedInstructions, forks) = executable.Run();
                Interlocked.Add(ref _executedInstructions, executedInstructions);
                stopwatch.Stop();
                Console.WriteLine($"{stopwatch.Elapsed} {executedInstructions} {_executedInstructions}");

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
                _throttler.Release();
                _countdownEvent.Signal();
            }
        });
    }

    public async Task<(ulong, ulong, Exception?)> Wait()
    {
        _countdownEvent.Signal();
        await Task.Run(() => { _countdownEvent.Wait(); });

        return (_completedStates, _executedInstructions, _exception);
    }
}
