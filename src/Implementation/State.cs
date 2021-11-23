using System;
using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Implementation.Memory;
using Symbolica.Implementation.Stack;
using Symbolica.Implementation.System;

namespace Symbolica.Implementation
{
    [Serializable]
    internal sealed class State : IState, IExecutable
    {
        private readonly List<IExecutable> _forks;
        private readonly IFunc<IState, Unit> _initialAction;
        private readonly IMemoryProxy _memory;
        private readonly IModule _module;
        private readonly IStackProxy _stack;
        private readonly ISystemProxy _system;
        private IPersistentGlobals _globals;
        private bool _isActive;

        public State(IFunc<IState, Unit> initialAction, IModule module, ISpace space,
            IPersistentGlobals globals, IMemoryProxy memory, IStackProxy stack, ISystemProxy system)
        {
            _forks = new List<IExecutable>();
            _isActive = true;
            _initialAction = initialAction;
            _module = module;
            Space = space;
            _globals = globals;
            _memory = memory;
            _stack = stack;
            _system = system;
        }

        public IEnumerable<IExecutable> Run()
        {
            _initialAction.Run(this);

            while (_isActive)
                _stack.ExecuteNextInstruction(this);

            return _forks;
        }

        public ISpace Space { get; }
        public IMemory Memory => _memory;
        public IStack Stack => _stack;
        public ISystem System => _system;

        public IFunction GetFunction(FunctionId id)
        {
            return _module.GetFunction(id);
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
            _isActive = false;
        }

        public void Fork(IExpression condition, IFunc<IState, Unit> trueProgram, IFunc<IState, Unit> falseProgram)
        {
            using var proposition = condition.GetProposition(Space);

            if (proposition.CanBeFalse)
            {
                if (proposition.CanBeTrue)
                {
                    _forks.Add(Clone(proposition.FalseSpace, falseProgram));
                    _forks.Add(Clone(proposition.TrueSpace, trueProgram));
                    Complete();
                }
                else
                {
                    falseProgram.Run(this);
                }
            }
            else
            {
                trueProgram.Run(this);
            }
        }

        private State Clone(ISpace space, IFunc<IState, Unit> initialAction)
        {
            var memory = _memory.Clone(space);
            var stack = _stack.Clone(space, memory);
            var system = _system.Clone(space, memory);

            return new State(initialAction, _module, space,
                _globals, memory, stack, system);
        }
    }
}
