using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack
{
    internal interface IPersistentFrame
    {
        ICaller Caller { get; }
        BasicBlockId PredecessorId { get; }
        IInstruction Instruction { get; }

        IPersistentFrame Save(IExpression continuation, bool useJumpBuffer);
        Result<IPersistentFrame> TryRestore(ISpace space, IExpression continuation, bool useJumpBuffer);
        IPersistentFrame TransferBasicBlock(BasicBlockId basicBlockId);
        IPersistentFrame MoveNextInstruction();
        IExpression GetFormal(int index);
        IExpression GetInitializedVaList(ISpace space, IStructType vaListType);
        IExpression GetVariable(InstructionId instructionId, bool useIncomingValue);
        IPersistentFrame SetVariable(InstructionId instructionId, IExpression variable);
        IPersistentFrame AddAllocation(IExpression allocation);
        IAllocations GetAllocations();
    }
}