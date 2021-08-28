using Symbolica.Abstraction;

namespace Symbolica.Implementation.Stack
{
    internal interface IPersistentProgramCounter
    {
        BasicBlockId PredecessorId { get; }
        IInstruction Instruction { get; }

        IPersistentProgramCounter TransferBasicBlock(BasicBlockId id);
        IPersistentProgramCounter MoveNextInstruction();
    }
}
