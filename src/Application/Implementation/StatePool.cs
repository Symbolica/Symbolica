using System;
using System.Runtime.ExceptionServices;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Symbolica.Abstraction;
using Symbolica.Implementation;
using System.Collections.Generic;

namespace Symbolica.Application.Implementation
{
    internal sealed class StatePool : IDisposable
    {
        private readonly CountdownEvent _countdownEvent;
        private readonly List<string> _errors;
        private Exception? _exception;
        private ulong _instructionsProcessed;
        private uint _maxErrors;

        public StatePool(uint maxErrors)
        {
            _countdownEvent = new CountdownEvent(1);
            _errors = new();
            _exception = null;
            _instructionsProcessed = 0UL;
            _maxErrors = maxErrors;
        }

        public void Dispose()
        {
            _countdownEvent.Dispose();
        }

        public void Add(IExecutable executable)
        {
            _countdownEvent.AddCount();

            Task.Run(() =>
            {
                try
                {
                    executable.Run().Fold(children =>
                    {
                        foreach (var child in children)
                            if (_exception == null && _errors.Count < _maxErrors)
                                Add(child);
                    }, error =>
                    {
                        var example = error is StateException stateException
                            ? string.Join(", ", stateException.Space.GetExample().Select(p => $"{p.Key}={p.Value}"))
                            : string.Empty;

                        var errorMessage = $"{error.Message}{Environment.NewLine}{example}";
                        _errors.Add(errorMessage);
                    });
                    Interlocked.Add(ref _instructionsProcessed, executable.InstructionsProcessed);
                }
                catch (Exception exception)
                {
                    _exception = exception;
                }
                finally
                {
                    _countdownEvent.Signal();
                }
            });
        }

        public async Task<(ulong, List<string>)> Wait()
        {
            _countdownEvent.Signal();
            await Task.Run(() => { _countdownEvent.Wait(); });

            if (_exception != null)
                ExceptionDispatchInfo.Capture(_exception).Throw();

            return (_instructionsProcessed, _errors);
        }
    }
}
