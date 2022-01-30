using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IStack
{
    bool IsInitialFrame { get; }
    BasicBlockId PredecessorId { get; }

    void Wind(ICaller caller, IInvocation invocation);
    ICaller Unwind();
    void Save(IExpression address, bool useJumpBuffer);
    InstructionId Restore(IExpression address, bool useJumpBuffer);
    void TransferBasicBlock(BasicBlockId id);
    IExpression GetFormal(int index);
    IExpression GetInitializedVaList();
    IExpression GetVariable(InstructionId id, bool useIncomingValue);
    void SetVariable(InstructionId id, IExpression variable);
    IExpression Allocate(Bits size);
}
