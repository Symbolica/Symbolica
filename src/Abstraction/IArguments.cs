using System.Collections.Generic;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IArguments : IEnumerable<IExpression<IType>>
{
    IExpression<IType> Get(int index);
}
