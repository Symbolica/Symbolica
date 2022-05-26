using System.Collections.Generic;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IDefinition : IFunction, IMergeable<ExpressionSubs, IDefinition>
{
    string Name { get; }
    IBasicBlock Entry { get; }

    IBasicBlock GetBasicBlock(BasicBlockId id);
}
