using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal sealed class StackProxy : IStackProxy
{
    private readonly IExpressionFactory _exprFactory;
    private IPersistentStack _stack;

    public StackProxy(IExpressionFactory exprFactory, IPersistentStack stack)
    {
        _exprFactory = exprFactory;
        _stack = stack;
    }

    public bool IsInitialFrame => _stack.IsInitialFrame;
    public BasicBlockId PredecessorId => _stack.PredecessorId;

    public IStackProxy Clone()
    {
        return new StackProxy(_exprFactory, _stack);
    }

    public void ExecuteNextInstruction(IState state)
    {
        _stack = _stack.MoveNextInstruction();
        _stack.Instruction.Execute(_exprFactory, state);
    }

    public void Wind(ISpace space, IMemory memory, ICaller caller, IInvocation invocation)
    {
        _stack = _stack.Wind(space, memory, caller, invocation);
    }

    public ICaller Unwind(ISpace space, IMemory memory)
    {
        var (caller, stack) = _stack.Unwind(space, memory);
        _stack = stack;

        return caller;
    }

    public void Save(ISpace space, IMemory memory, IExpression address, bool useJumpBuffer)
    {
        _stack = _stack.Save(space, memory, address, useJumpBuffer);
    }

    public InstructionId Restore(ISpace space, IMemory memory, IExpression address, bool useJumpBuffer)
    {
        _stack = _stack.Restore(space, memory, address, useJumpBuffer);

        return _stack.Instruction.Id;
    }

    public void TransferBasicBlock(BasicBlockId id)
    {
        _stack = _stack.TransferBasicBlock(id);
    }

    public IExpression GetFormal(int index)
    {
        return _stack.GetFormal(index);
    }

    public IExpression GetInitializedVaList(ISpace space)
    {
        return _stack.GetInitializedVaList(space);
    }

    public IExpression GetVariable(InstructionId id, bool useIncomingValue)
    {
        return _stack.GetVariable(id, useIncomingValue);
    }

    public void SetVariable(InstructionId id, IExpression variable)
    {
        _stack = _stack.SetVariable(id, variable);
    }

    public IExpression Allocate(IMemory memory, Bits size)
    {
        var (address, stack) = _stack.Allocate(memory, size);
        _stack = stack;

        return address;
    }

    public (HashSet<(IExpression, IExpression)> subs, bool) IsEquivalentTo(IStackProxy stack)
    {
        return stack is IPersistentStack ps
            ? _stack.IsEquivalentTo(ps)
            : (new(), false);
    }
}
