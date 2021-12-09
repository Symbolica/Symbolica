using System;
using System.Threading;
using System.Threading.Tasks;
using Symbolica.Implementation;

namespace Symbolica.Application.Implementation
{
    internal sealed class StatePool : IDisposable
    {
        private readonly CountdownEvent _countdownEvent;
        private Exception? _exception;
        private ulong _executedInstructions;

        public StatePool()
        {
            _countdownEvent = new CountdownEvent(1);
            _exception = null;
            _executedInstructions = 0UL;
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
}
