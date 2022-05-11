using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IStack
{
    bool IsInitialFrame { get; }
    BasicBlockId PredecessorId { get; }

    void Wind(ISpace space, IMemory memory, ICaller caller, IInvocation invocation);
    ICaller Unwind(ISpace space, IMemory memory);
    void Save(ISpace space, IMemory memory, IExpression address, bool useJumpBuffer);
    InstructionId Restore(ISpace space, IMemory memory, IExpression address, bool useJumpBuffer);
    void TransferBasicBlock(BasicBlockId id);
    IExpression GetFormal(int index);
    IExpression GetInitializedVaList(ISpace space);
    IExpression GetVariable(InstructionId id, bool useIncomingValue);
    void SetVariable(InstructionId id, IExpression variable);
    IExpression Allocate(IMemory memory, Bits size);
}
