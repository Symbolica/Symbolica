using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Implementation.Memory;
using Symbolica.Implementation.Stack;
using Symbolica.Implementation.System;

namespace Symbolica.Implementation;

internal sealed class State : IState, IExecutableState
{
    private readonly IMemoryProxy _memory;
    private readonly IModule _module;
    private readonly IStackProxy _stack;
    private readonly IStatePool _statePool;
    private readonly ISystemProxy _system;
    private IPersistentGlobals _globals;
    private bool _isActive;

    public State(IStatePool statePool, IModule module, ISpace space,
        IPersistentGlobals globals, IMemoryProxy memory, IStackProxy stack, ISystemProxy system)
    {
        _isActive = true;
        _statePool = statePool;
        _module = module;
        Space = space;
        _globals = globals;
        _memory = memory;
        _stack = stack;
        _system = system;
    }

    public bool TryExecuteNextInstruction()
    {
        return _isActive && _stack.ExecuteNextInstruction(this);
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

    public void Fork(IExpression condition, IStateAction trueAction, IStateAction falseAction)
    {
        using var proposition = condition.GetProposition(Space);

        if (proposition.CanBeFalse())
        {
            if (proposition.CanBeTrue())
            {
                _statePool.Add(new ForkedStateFactory(this, proposition.CreateFalseSpace(), falseAction));
                _statePool.Add(new ForkedStateFactory(this, proposition.CreateTrueSpace(), trueAction));
                Complete();
            }
            else
            {
                falseAction.Invoke(this);
            }
        }
        else
        {
            trueAction.Invoke(this);
        }
    }

    private sealed class ForkedStateFactory : IStateFactory
    {
        private readonly IStateAction _action;
        private readonly ISpace _space;
        private readonly State _state;

        public ForkedStateFactory(State state, ISpace space, IStateAction action)
        {
            _state = state;
            _space = space;
            _action = action;
        }

        public IExecutableState Create(IStatePool statePool)
        {
            var memory = _state._memory.Clone(_space);
            var stack = _state._stack.Clone(_space, memory);
            var system = _state._system.Clone(_space, memory);

            var state = new State(_state._statePool, _state._module, _space,
                _state._globals, memory, stack, system);

            _action.Invoke(state);
            return state;
        }
    }
}
