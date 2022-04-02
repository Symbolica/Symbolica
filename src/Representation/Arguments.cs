using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation;

internal sealed class Arguments : IArguments
{
    private readonly IExpression<IType>[] _expressions;

    public Arguments(IExpression<IType>[] expressions)
    {
        _expressions = expressions;
    }

    public IExpression<IType> Get(int index)
    {
        return _expressions[index];
    }

    public IEnumerator<IExpression<IType>> GetEnumerator()
    {
        return _expressions.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
