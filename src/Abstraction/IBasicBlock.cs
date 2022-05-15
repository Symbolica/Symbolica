using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IBasicBlock : IMergeable<IExpression, IBasicBlock>
{
    BasicBlockId Id { get; }

    IInstruction GetInstruction(int index);
}
