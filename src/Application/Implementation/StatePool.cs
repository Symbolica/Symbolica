using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Symbolica.Implementation;

namespace Symbolica.Application.Implementation
{
    internal sealed class StatePool : IDisposable
    {
        private readonly CountdownEvent _countdownEvent;
        private Exception? _exception;

        public StatePool()
        {
            _countdownEvent = new CountdownEvent(1);
            _exception = null;
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
                    foreach (var forks in executable.Run())
                    {
                        if (_exception != null)
                        {
                            break;
                        }
                        foreach (var fork in forks)
                        {
                            Add(fork);
                        }
                    }
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

        public async Task Wait()
        {
            _countdownEvent.Signal();
            await Task.Run(() => { _countdownEvent.Wait(); });

            if (_exception != null)
                ExceptionDispatchInfo.Capture(_exception).Throw();
        }
    }
}
