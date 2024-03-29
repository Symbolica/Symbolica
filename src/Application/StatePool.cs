﻿using System;
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
                foreach (var child in executable.Run())
                    if (_exception == null)
                        Add(child);
            }
            catch (Exception exception)
            {
                _exception = exception;
            }
            finally
            {
                Interlocked.Add(ref _executedInstructions, executable.ExecutedInstructions);
                _throttler.Release();
                _countdownEvent.Signal();
            }
        });
    }

    public async Task<(ulong, Exception?)> Wait()
    {
        _countdownEvent.Signal();
        await Task.Run(() => { _countdownEvent.Wait(); });

        return (_executedInstructions, _exception);
    }
}
