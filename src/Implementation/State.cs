using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        _status = IExecutable.Status.NotStarted;
        _initialAction = initialAction;
        _module = module;
        Space = space;
        _globals = globals;
        _memory = memory;
        _stack = stack;
        _system = system;
        Generation = 0;
    }

    public (ulong, IExecutable.Status, IEnumerable<IExecutable>) Run()
    {
        if (_status == IExecutable.Status.NotStarted)
            _initialAction.Invoke(this);
        else
            Generation++;

        _status = IExecutable.Status.Running;

        var executedInstructions = 0UL;
        while (_status == IExecutable.Status.Running)
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

    public int Generation { get; private set; }
    public ISpace Space { get; }
    public IMemory Memory => _memory;
    public IStack Stack => _stack;
    public ISystem System => _system;

    public bool IsEquivalentTo(IExecutable other)
    {
        return other is State s && IsEquivalentTo(s);
    }

    private bool IsEquivalentTo(State other)
    {
        bool IsSubEqual(IExpression old, IExpression @new)
        {
            var equal = old.Equal(@new);
            using var proposition = equal.GetProposition(other.Space);
            return !proposition.CanBeFalse();
        }
        var (subs, isEquivalent) = _stack.IsEquivalentTo(other._stack)
            .And(_memory.IsEquivalentTo(other._memory))
            .And(_system.IsEquivalentTo(other._system));

        return isEquivalent && subs.All(s => IsSubEqual(s.Item1, s.Item2));
    }

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
        if (!_forks.Any())
        {
            var example = string.Join(", ", Space.GetExample()
                .OrderBy(p => p.Key)
                .Select(p => $"{p.Key}={p.Value}"));
            Console.WriteLine(example);
        }
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
