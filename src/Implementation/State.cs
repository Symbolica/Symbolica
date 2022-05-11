using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Implementation.Memory;
using Symbolica.Implementation.Stack;
using Symbolica.Implementation.System;

namespace Symbolica.Implementation;

internal sealed class State : IState, IExecutable
{
    private readonly List<IExecutable> _forks;
    private readonly IStateAction _initialAction;
    private readonly IMemoryProxy _memory;
    private readonly IModule _module;
    private readonly IStackProxy _stack;
    private readonly ISystemProxy _system;
    private IPersistentGlobals _globals;
    private IExecutable.Status _status;

    public State(IStateAction initialAction, IModule module, ISpace space,
        IPersistentGlobals globals, IMemoryProxy memory, IStackProxy stack, ISystemProxy system)
    {
        _forks = new List<IExecutable>();
        _status = IExecutable.Status.Active;
        _initialAction = initialAction;
        _module = module;
        Space = space;
        _globals = globals;
        _memory = memory;
        _stack = stack;
        _system = system;
    }

    public (ulong, IExecutable.Status, IEnumerable<IExecutable>) Run()
    {
        var executedInstructions = 0UL;
        _initialAction.Invoke(this);

        while (_status == IExecutable.Status.Active)
        {
            _stack.ExecuteNextInstruction(this);
            ++executedInstructions;
        }

        return (executedInstructions,
            _status,
            _status == IExecutable.Status.Merging
                ? new[] { this }
                : _forks);
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
        _status = IExecutable.Status.Complete;
    }

    public void Fork(IExpression condition, IStateAction trueAction, IStateAction falseAction)
    {
        using var proposition = condition.GetProposition(Space);

        if (proposition.CanBeFalse())
        {
            if (proposition.CanBeTrue())
            {
                _forks.Add(Clone(proposition.CreateFalseSpace(), falseAction));
                _forks.Add(Clone(proposition.CreateTrueSpace(), trueAction));
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

    public void Merge()
    {
        _status = IExecutable.Status.Merging;
    }

    private State Clone(ISpace space, IStateAction initialAction)
    {
        return new State(
            initialAction,
            _module,
            space,
            _globals,
            _memory.Clone(),
            _stack.Clone(),
            _system.Clone());
    }
}
