using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation;

public sealed class Parameters : IParameters
{
    private readonly Parameter[] _parameters;

    public Parameters(Parameter[] parameters)
    {
        _parameters = parameters;
    }

    public int Count => _parameters.Length;

    public Parameter Get(int index)
    {
        return _parameters[index];
    }

    public (HashSet<(IExpression, IExpression)> subs, bool) IsEquivalentTo(IParameters other)
    {
        return other is Parameters p
            ? _parameters.IsSequenceEquivalentTo<IExpression, Parameter>(p._parameters)
            : (new(), false);
    }

    public object ToJson()
    {
        return _parameters.Select(p => p.ToJson()).ToArray();
    }
}
