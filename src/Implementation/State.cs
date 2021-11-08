using System;
using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Implementation.Memory;
using Symbolica.Implementation.Stack;
using Symbolica.Implementation.System;

namespace Symbolica.Implementation
{
    internal sealed class State : IState, IExecutable
    {
        private readonly Action<IState> _initialAction;
        private readonly IMemoryProxy _memory;
        private readonly IModule _module;
        private readonly IStackProxy _stack;
        private readonly ISystemProxy _system;
        private IPersistentGlobals _globals;
        private bool _isComplete;
        private List<IExecutable> _forks = new();

        public State(Action<IState> initialAction, IModule module, ISpace space,
            IPersistentGlobals globals, IMemoryProxy memory, IStackProxy stack, ISystemProxy system)
        {
            _initialAction = initialAction;
            _isComplete = false;
            _module = module;
            Space = space;
            _globals = globals;
            _memory = memory;
            _stack = stack;
            _system = system;
        }

        public ISpace Space { get; }
        public IMemory Memory => _memory;
        public IStack Stack => _stack;
        public ISystem System => _system;

        public IFunction GetFunction(FunctionId id)
        {
            return _module.GetFunction(id);
        }

        public IEnumerable<IExecutable> Run()
        {
            _initialAction(this);

            while (!_isComplete)
            {
                _stack.ExecuteNextInstruction(this);
            }
            return _forks;
        }

        public IExpression GetGlobalAddress(GlobalId id)
        {
            var (address, action, globals) = _globals.GetAddress(_memory, id);
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
                    _forks.Add(Clone(proposition.FalseSpace, falseAction));
                    _forks.Add(Clone(proposition.TrueSpace, trueAction));
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

        private State Clone(ISpace space, Action<IState> initialAction)
        {
            var memory = _memory.Clone(space);
            return new(
                initialAction,
                _module,
                space,
                _globals,
                memory,
                _stack.Clone(space, memory),
                _system.Clone(space, memory));
        }
    }
}
