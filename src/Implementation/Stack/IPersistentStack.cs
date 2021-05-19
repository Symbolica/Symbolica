using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Implementation.Memory;

namespace Symbolica.Implementation.Stack
{
    internal interface IPersistentStack
    {
        bool IsInitialFrame { get; }
        BasicBlockId PredecessorId { get; }
        IInstruction Instruction { get; }

        IPersistentStack Wind(ISpace space, IMemoryProxy memory, ICaller caller, IInvocation invocation);
        (ICaller, IPersistentStack) Unwind(IMemoryProxy memory);
        IPersistentStack Save(ISpace space, IMemory memory, IExpression address, bool useJumpBuffer);
        IPersistentStack Restore(ISpace space, IMemoryProxy memory, IExpression address, bool useJumpBuffer);
        IPersistentStack TransferBasicBlock(BasicBlockId basicBlockId);
        IPersistentStack MoveNextInstruction();
        IExpression GetFormal(int index);
        IExpression GetInitializedVaList(ISpace space);
        IExpression GetVariable(InstructionId instructionId, bool useIncomingValue);
        IPersistentStack SetVariable(InstructionId instructionId, IExpression variable);
        (IExpression, IPersistentStack) Allocate(IMemoryProxy memory, Bits size);
    }
}