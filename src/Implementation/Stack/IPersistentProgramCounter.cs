using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal interface IPersistentProgramCounter : IEquivalent<ExpressionSubs, IPersistentProgramCounter>, IMergeable<IPersistentProgramCounter>
{
    BasicBlockId PredecessorId { get; }
    IInstruction Instruction { get; }

    IPersistentProgramCounter TransferBasicBlock(BasicBlockId id);
    IPersistentProgramCounter MoveNextInstruction();
}
