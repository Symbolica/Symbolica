using System;
using Symbolica.Abstraction;
using Symbolica.Execution;
using Symbolica.Expression;
using Symbolica.Implementation.Memory;
using Symbolica.Implementation.Stack;
using Symbolica.Implementation.System;

namespace Symbolica.Implementation
{
    internal sealed class State : IState, IExecutableState
    {
        private readonly IFunctions _functions;
        private readonly IMemoryProxy _memory;
        private readonly IProgramPool _programPool;
        private readonly IStackProxy _stack;
        private readonly ISystemProxy _system;
        private IPersistentGlobals _globals;
        private bool _isComplete;

        public State(IProgramPool programPool, ISpace space,
            IMemoryProxy memory, IStackProxy stack, ISystemProxy system,
            IFunctions functions, IPersistentGlobals globals)
        {
            _isComplete = false;
            _programPool = programPool;
            Space = space;
            _memory = memory;
            _stack = stack;
            _system = system;
            _functions = functions;
            _globals = globals;
        }

        public bool TryExecuteNextInstruction()
        {
            if (_isComplete)
                return false;

            _stack.ExecuteNextInstruction(this);

            return true;
        }

        public ISpace Space { get; }
        public IMemory Memory => _memory;
        public IStack Stack => _stack;
        public ISystem System => _system;

        public IFunction GetFunction(FunctionId functionId)
        {
            return _functions.Get(functionId);
        }

        public IExpression GetGlobalAddress(GlobalId globalId)
        {
            var (address, action, globals) = _globals.GetAddress(_memory, globalId);
            _globals = globals;

            action(this);
            return address;
        }

        public void Complete()
        {
            _isComplete = true;
        }

        public void Fork(IExpression condition, Action<IState> trueAction, Action<IState> falseAction)
        {
            using var proposition = condition.GetProposition(Space);

            if (proposition.CanBeFalse)
            {
                if (proposition.CanBeTrue)
                {
                    Clone(proposition.FalseSpace, falseAction);
                    Clone(proposition.TrueSpace, trueAction);

                    Complete();
                }
                else
                {
                    falseAction(this);
                }
            }
            else
            {
                trueAction(this);
            }
        }

        private void Clone(ISpace space, Action<IState> action)
        {
            var program = new Program(() => Create(space, action));

            _programPool.Add(program);
        }

        private IExecutableState Create(ISpace space, Action<IState> action)
        {
            var memory = _memory.Clone(space);
            var stack = _stack.Clone(space, memory);
            var system = _system.Clone(space, memory);

            var state = new State(_programPool, space,
                memory, stack, system,
                _functions, _globals);

            action(state);

            return state;
        }
    }
}
