using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IDefinition : IFunction, IMergeable<IExpression, IDefinition>
{
    string Name { get; }
    IBasicBlock Entry { get; }

    IBasicBlock GetBasicBlock(BasicBlockId id);
}
