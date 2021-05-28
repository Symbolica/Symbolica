using System.Collections.Generic;
using Symbolica.Expression;

namespace Symbolica.Abstraction
{
    public interface IArguments : IEnumerable<IExpression>
    {
        IExpression Get(int index);
    }
}
