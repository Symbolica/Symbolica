using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IDefinition : IFunction, IEquivalent<ExpressionSubs, IDefinition>, IMergeable<IDefinition>
{
    string Name { get; }
    IBasicBlock Entry { get; }

    IBasicBlock GetBasicBlock(BasicBlockId id);
}
