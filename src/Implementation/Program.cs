using System;
using Symbolica.Execution;

namespace Symbolica.Implementation
{
    internal sealed class Program : IProgram
    {
        private readonly Lazy<IExecutableState> _state;

        public Program(Func<IExecutableState> state)
        {
            _state = new Lazy<IExecutableState>(state);
        }

        public bool TryExecuteNextInstruction()
        {
            return _state.Value.TryExecuteNextInstruction();
        }
    }
}