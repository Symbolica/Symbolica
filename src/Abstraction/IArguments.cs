using System.Collections.Generic;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IArguments : IEnumerable<IExpression>, IEquivalent<ExpressionSubs, IArguments>, IMergeable<IArguments>
{
    IExpression Get(int index);
}
