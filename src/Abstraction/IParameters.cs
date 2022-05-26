using System.Collections.Generic;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IParameters : IMergeable<ExpressionSubs, IParameters>
{
    int Count { get; }

    Parameter Get(int index);
}
