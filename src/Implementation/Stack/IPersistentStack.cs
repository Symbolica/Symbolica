using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal interface IPersistentStack
{
    bool IsInitialFrame { get; }
    BasicBlockId PredecessorId { get; }
    IInstruction Instruction { get; }

    IPersistentStack Wind(ISpace space, IMemory memory, ICaller caller, IInvocation invocation);
    (ICaller, IPersistentStack) Unwind(ISpace space, IMemory memory);
    IPersistentStack Save(ISpace space, IMemory memory, IExpression address, bool useJumpBuffer);
    IPersistentStack Restore(ISpace space, IMemory memory, IExpression address, bool useJumpBuffer);
    IPersistentStack TransferBasicBlock(BasicBlockId id);
    IPersistentStack MoveNextInstruction();
    IExpression GetFormal(int index);
    IExpression GetInitializedVaList(ISpace space);
    IExpression GetVariable(InstructionId id, bool useIncomingValue);
    IPersistentStack SetVariable(InstructionId id, IExpression variable);
    (IExpression, IPersistentStack) Allocate(IMemory memory, Bits size);
}
