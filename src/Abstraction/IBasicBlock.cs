using System.Collections.Generic;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IBasicBlock : IMergeable<ExpressionSubs, IBasicBlock>
{
    BasicBlockId Id { get; }

    IInstruction GetInstruction(int index);
}
