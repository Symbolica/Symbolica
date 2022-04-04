using System.Collections.Generic;
using Symbolica.Expression;
using Symbolica.Expression.Values;

namespace Symbolica.Abstraction;

public interface IArguments : IEnumerable<IExpression<IType>>
{
    IExpression<IType> Get(int index);
    Address GetAddress(int index);
}
