using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Implementation;

namespace Symbolica;

internal sealed class StatePool : IDisposable
{
    private readonly TextWriterTraceListener? _errorTracer;
    private readonly TextWriterTraceListener? _stateTracer;

    private readonly CountdownEvent _countdownEvent;
    private readonly SemaphoreSlim _throttler;
    private ulong _executedInstructions;

    public StatePool(int maxParallelism, bool trace)
    {
        _errorTracer = trace ? CreateTracer("errors.txt") : null;
        _stateTracer = trace ? CreateTracer("states.txt") : null;
        _countdownEvent = new CountdownEvent(1);
        _executedInstructions = 0UL;
        _throttler = new SemaphoreSlim(maxParallelism);
    }

    public void Dispose()
    {
        _countdownEvent.Dispose();
    }

    public void Add(IExecutable executable, ulong depth)
    {
        _countdownEvent.AddCount();
        Task.Run(async () =>
        {
            await _throttler.WaitAsync();
            var exeId = Guid.NewGuid();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                foreach (var child in executable.Run())
                    Add(child, depth + 1);
            }
            catch (Exception ex)
            {
                if (ex is ErrorException error)
                {
                    var example = error is StateException stateException
                        ? string.Join(", ", stateException.Space.GetExample().Select(p => $"{p.Key}={p.Value}"))
                        : string.Empty;
                    _errorTracer?.WriteLine($"{exeId}, {error.Message}, {example}");
                    _errorTracer?.Flush();
                }
                else
                {
                    Console.WriteLine($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }
            finally
            {
                Interlocked.Add(ref _executedInstructions, executable.ExecutedInstructions);
                _stateTracer?.WriteLine($"{exeId}, {depth}, {executable.ExecutedInstructions}, {stopwatch.Elapsed}");
                _stateTracer?.Flush();
                _throttler.Release();
                _countdownEvent.Signal();
            }
        });
    }

    public async Task<ulong> Wait()
    {
        _countdownEvent.Signal();
        await Task.Run(() => { _countdownEvent.Wait(); });

        return _executedInstructions;
    }

    private static TextWriterTraceListener CreateTracer(string fileName)
    {
        string dir = Path.Join(Directory.GetCurrentDirectory(), ".traces");
        Directory.CreateDirectory(dir);
        return new(File.CreateText(Path.Join(dir, fileName)));
    }
}
