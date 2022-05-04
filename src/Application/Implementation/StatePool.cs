using System;
using System.Threading;
using System.Threading.Tasks;

namespace Symbolica.Implementation;

internal sealed class StatePool : IStatePool, IDisposable
{
    private readonly CountdownEvent _coordinator;
    private readonly SemaphoreSlim _throttler;
    private Exception? _exception;
    private ulong _executedInstructions;

    public StatePool(int maxParallelism)
    {
        _exception = null;
        _executedInstructions = 0UL;
        _coordinator = new CountdownEvent(1);
        _throttler = new SemaphoreSlim(maxParallelism);
    }

    public void Dispose()
    {
        _coordinator.Dispose();
        _throttler.Dispose();
    }

    public void Add(IStateFactory stateFactory)
    {
        _coordinator.AddCount();

        Task.Run(async () =>
        {
            await _throttler.WaitAsync();

            var executedInstructions = 0UL;
            try
            {
                var state = stateFactory.Create(this);
                while (_exception == null && state.TryExecuteNextInstruction())
                    ++executedInstructions;
            }
            catch (Exception exception)
            {
                _exception = exception;
            }
            finally
            {
                Interlocked.Add(ref _executedInstructions, executedInstructions);
                _throttler.Release();
                _coordinator.Signal();
            }
        });
    }

    public async Task<(ulong, Exception?)> Wait()
    {
        _coordinator.Signal();
        await Task.Run(() => { _coordinator.Wait(); });

        return (_executedInstructions, _exception);
    }
}
