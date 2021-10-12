using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Symbolica.Implementation;

namespace Symbolica.Application.Implementation
{
    internal sealed class ProgramPool : IProgramPool, IDisposable
    {
        private readonly CountdownEvent _countdownEvent;
        private Exception? _exception;

        public ProgramPool()
        {
            _countdownEvent = new CountdownEvent(1);
            _exception = null;
        }

        public void Dispose()
        {
            _countdownEvent.Dispose();
        }

        public void Add(IProgram program)
        {
            _countdownEvent.AddCount();

            Task.Run(() =>
            {
                try
                {
                    while (_exception == null && program.TryExecuteNextInstruction())
                    {
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
