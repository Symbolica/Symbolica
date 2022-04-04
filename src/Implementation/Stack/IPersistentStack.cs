using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values;
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
    IPersistentStack Save(IMemory memory, Address address, bool useJumpBuffer);
    IPersistentStack Restore(ISpace space, IMemoryProxy memory, Address address, bool useJumpBuffer);
    IPersistentStack TransferBasicBlock(BasicBlockId id);
    IPersistentStack MoveNextInstruction();
    IExpression<IType> GetFormal(int index);
    IExpression<IType> GetInitializedVaList(ISpace space);
    IExpression<IType> GetVariable(InstructionId id, bool useIncomingValue);
    IPersistentStack SetVariable(InstructionId id, IExpression<IType> variable);
    (Address, IPersistentStack) Allocate(IMemoryProxy memory, Bits size);
}
