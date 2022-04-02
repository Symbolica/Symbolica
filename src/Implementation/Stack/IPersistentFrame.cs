using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal interface IPersistentFrame
{
    ICaller Caller { get; }
    BasicBlockId PredecessorId { get; }
    IInstruction Instruction { get; }
    string Name { get; }

    IPersistentFrame Save(IExpression<IType> continuation, bool useJumpBuffer);
    Result<IPersistentFrame> TryRestore(ISpace space, IExpression<IType> continuation, bool useJumpBuffer);
    IPersistentFrame TransferBasicBlock(BasicBlockId id);
    IPersistentFrame MoveNextInstruction();
    IExpression<IType> GetFormal(int index);
    IExpression<IType> GetInitializedVaList(ISpace space, IStructType vaListType);
    IExpression<IType> GetVariable(InstructionId id, bool useIncomingValue);
    IPersistentFrame SetVariable(InstructionId id, IExpression<IType> variable);
    IPersistentFrame AddAllocation(IExpression<IType> allocation);
    IAllocations GetAllocations();
}
