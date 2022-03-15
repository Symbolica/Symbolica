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
    private readonly IMemoryProxy _memoryProxy;
    private readonly IModule _module;
    private readonly IStackProxy _stackProxy;
    private readonly ISystemProxy _systemProxy;
    private IPersistentGlobals _globals;
    private bool _isActive;

    private State(IStateAction initialAction, IModule module, ISpace space,
        IPersistentGlobals globals, IMemoryProxy memoryProxy, IStackProxy stackProxy, ISystemProxy systemProxy)
    {
        ExecutedInstructions = 0UL;
        _forks = new List<IExecutable>();
        _isActive = true;
        _initialAction = initialAction;
        _module = module;
        Space = space;
        _globals = globals;
        _memoryProxy = memoryProxy;
        _stackProxy = stackProxy;
        _systemProxy = systemProxy;
    }

    public ulong ExecutedInstructions { get; private set; }

    public IEnumerable<IExecutable> Run()
    {
        _initialAction.Invoke(this);

        while (_isActive)
        {
            _stackProxy.ExecuteNextInstruction(this);
            ++ExecutedInstructions;
        }

        return _forks;
    }

    public void Dispose()
    {
        Space.Dispose();
    }

    public ISpace Space { get; }
    public IMemory Memory => _memoryProxy;
    public IStack Stack => _stackProxy;
    public ISystem System => _systemProxy;

    public IFunction GetFunction(FunctionId id)
    {
        return _module.GetFunction(id);
    }

    public IExpression GetGlobalAddress(GlobalId id)
    {
        var (address, action, globals) = _globals.GetAddress(_memoryProxy, id);
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
        var proposition = condition.GetProposition(Space);

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
        var memoryProxy = _memoryProxy.Clone(space);
        var stackProxy = _stackProxy.Clone(space, memoryProxy);
        var systemProxy = _systemProxy.Clone(space, memoryProxy);

        return new State(initialAction, _module, space,
            _globals, memoryProxy, stackProxy, systemProxy);
    }

    public static IExecutable CreateInitial(ISpaceFactory spaceFactory, IModule module, Options options,
        IPersistentGlobals globals, IPersistentMemory memory, IPersistentStack stack, IPersistentSystem system)
    {
        var space = spaceFactory.CreateInitial(module.PointerSize, options.UseSymbolicGarbage);

        var memoryProxy = new MemoryProxy(space, memory);
        var stackProxy = new StackProxy(space, memoryProxy, stack);
        var systemProxy = new SystemProxy(space, memoryProxy, system);

        return new State(new NoOp(), module, space,
            globals, memoryProxy, stackProxy, systemProxy);
    }

    private sealed class NoOp : IStateAction
    {
        public void Invoke(IState state)
        {
        }
    }
}
