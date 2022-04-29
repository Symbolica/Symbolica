using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    private bool _isActive;
    private Stopwatch _stopwatch;

    public State(IStateAction initialAction, IModule module, ISpace space,
        IPersistentGlobals globals, IMemoryProxy memory, IStackProxy stack, ISystemProxy system)
    {
        ExecutedInstructions = 0UL;
        _forks = new List<IExecutable>();
        _isActive = true;
        _initialAction = initialAction;
        _module = module;
        Space = space;
        _globals = globals;
        _memory = memory;
        _stack = stack;
        _system = system;
        _stopwatch = new Stopwatch();
    }

    public IEnumerable<IExecutable> Run()
    {
        _stopwatch.Start();
        _initialAction.Invoke(this);

        while (_isActive)
        {
            _stack.ExecuteNextInstruction(this);
            ++ExecutedInstructions;
        }

        return _forks;
    }

    public ulong ExecutedInstructions { get; private set; }
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
        _stopwatch.Stop();
        _isActive = false;
        string example = string.Join(", ", Space.GetExample().Select(p => $"{p.Key}={p.Value}"));
        Console.WriteLine($"{_stopwatch.Elapsed} {ExecutedInstructions} {example}");
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
                _isActive = false;
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

    private State Clone(ISpace space, IStateAction initialAction)
    {
        var memory = _memory.Clone(space);
        var stack = _stack.Clone(space, memory);
        var system = _system.Clone(space, memory);

        return new State(initialAction, _module, space,
            _globals, memory, stack, system);
    }
}
