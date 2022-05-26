using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal interface IPersistentProgramCounter : IMergeable<ExpressionSubs, IPersistentProgramCounter>
{
    BasicBlockId PredecessorId { get; }
    IInstruction Instruction { get; }

    IPersistentProgramCounter TransferBasicBlock(BasicBlockId id);
    IPersistentProgramCounter MoveNextInstruction();
}
