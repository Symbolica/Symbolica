using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal interface IPersistentFrame : IMergeable<ExpressionSubs, IPersistentFrame>
{
    ICaller Caller { get; }
    BasicBlockId PredecessorId { get; }
    IInstruction Instruction { get; }

    IPersistentFrame Save(IExpression continuation, bool useJumpBuffer);
    Result<IPersistentFrame> TryRestore(ISpace space, IExpression continuation, bool useJumpBuffer);
    IPersistentFrame TransferBasicBlock(BasicBlockId id);
    IPersistentFrame MoveNextInstruction();
    IExpression GetFormal(int index);
    IExpression GetInitializedVaList(ISpace space, IStructType vaListType);
    IExpression GetVariable(InstructionId id, bool useIncomingValue);
    IPersistentFrame SetVariable(InstructionId id, IExpression variable);
    IPersistentFrame AddAllocation(IExpression allocation);
    IAllocations GetAllocations();
}
