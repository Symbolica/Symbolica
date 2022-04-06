using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IStack
{
    bool IsInitialFrame { get; }
    BasicBlockId PredecessorId { get; }

    void Wind(ICaller caller, IInvocation invocation);
    ICaller Unwind();
    void Save(IExpression<IType> address, bool useJumpBuffer);
    InstructionId Restore(IExpression<IType> address, bool useJumpBuffer);
    void TransferBasicBlock(BasicBlockId id);
    IExpression<IType> GetFormal(int index);
    IExpression<IType> GetInitializedVaList();
    IExpression<IType> GetVariable(InstructionId id, bool useIncomingValue);
    void SetVariable(InstructionId id, IExpression<IType> variable);
    IExpression<IType> Allocate(Bits size);
}
