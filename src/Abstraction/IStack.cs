using Symbolica.Expression;

namespace Symbolica.Abstraction
{
    public interface IStack
    {
        bool IsInitialFrame { get; }
        BasicBlockId PredecessorId { get; }

        void Wind(ICaller caller, IInvocation invocation);
        ICaller Unwind();
        void Save(IExpression address, bool useJumpBuffer);
        InstructionId Restore(IExpression address, bool useJumpBuffer);
        void TransferBasicBlock(BasicBlockId basicBlockId);
        IExpression GetFormal(int index);
        IExpression GetInitializedVaList();
        IExpression GetVariable(InstructionId instructionId, bool useIncomingValue);
        void SetVariable(InstructionId instructionId, IExpression variable);
        IExpression Allocate(Bits size);
    }
}