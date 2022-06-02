using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IBasicBlock : IEquivalent<ExpressionSubs, IBasicBlock>, IMergeable<IBasicBlock>
{
    BasicBlockId Id { get; }

    IInstruction GetInstruction(int index);
}
