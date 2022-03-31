using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Implementation.Memory;

namespace Symbolica.Implementation.Stack;

internal interface IPersistentStack
{
    bool IsInitialFrame { get; }
    BasicBlockId PredecessorId { get; }
    IInstruction Instruction { get; }
    IEnumerable<string> StackTrace { get; }
    IPersistentStack Wind(ISpace space, IMemoryProxy memory, ICaller caller, IInvocation invocation);
    (ICaller, IPersistentStack) Unwind(IMemoryProxy memory);
    IPersistentStack Save(ISpace space, IMemory memory, IExpression address, bool useJumpBuffer);
    IPersistentStack Restore(ISpace space, IMemoryProxy memory, IExpression address, bool useJumpBuffer);
    IPersistentStack TransferBasicBlock(BasicBlockId id);
    IPersistentStack MoveNextInstruction();
    IExpression GetFormal(int index);
    IExpression GetInitializedVaList(ISpace space);
    IExpression GetVariable(InstructionId id, bool useIncomingValue);
    IPersistentStack SetVariable(InstructionId id, IExpression variable);
    (IExpression, IPersistentStack) Allocate(IMemoryProxy memory, Bits size);
}
