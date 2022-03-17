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
    private static readonly TextWriterTraceListener _errorTracer = new(File.CreateText("/Users/Choc/code/symbolica/symbolica/.traces/errors.txt"));
    private static readonly TextWriterTraceListener _examplesTracer = new(File.CreateText("/Users/Choc/code/symbolica/symbolica/.traces/examples.txt"));
    private static readonly TextWriterTraceListener _stateTracer = new(File.CreateText("/Users/Choc/code/symbolica/symbolica/.traces/states.txt"));

    private readonly CountdownEvent _countdownEvent;
    private readonly SemaphoreSlim _throttler;
    private ulong _executedInstructions;

    public StatePool()
    {
        _countdownEvent = new CountdownEvent(1);
        _executedInstructions = 0UL;
        _throttler = new SemaphoreSlim(Environment.ProcessorCount * 8);
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
                var children = executable.Run();
                foreach (var child in children)
                    Add(child, depth + 1);
                if (!children.Any())
                {
                    _examplesTracer.WriteLine(
                        string.Join(
                            ", ",
                            executable.Space.GetExample()
                            .OrderBy(p => p.Key)
                            .Select(p => $"{p.Key}={p.Value}")));
                    _examplesTracer.Flush();
                }
                _stateTracer.WriteLine($"{exeId}, {depth}, {children.Count()}, {executable.ExecutedInstructions}, {stopwatch.Elapsed}");
                _stateTracer.Flush();
            }
            catch (Exception ex)
            {
                if (ex is ErrorException error)
                {
                    var example = error is StateException stateException
                        ? string.Join(", ", stateException.Space.GetExample().Select(p => $"{p.Key}={p.Value}"))
                        : string.Empty;
                    _errorTracer.WriteLine($"{exeId}, {error.Message}, {example}");
                    _errorTracer.Flush();
                }
                else
                {
                    Console.WriteLine($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }
            finally
            {
                Interlocked.Add(ref _executedInstructions, executable.ExecutedInstructions);
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
}
