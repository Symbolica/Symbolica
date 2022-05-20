using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation;

public sealed class Parameters : IParameters
{
    private readonly Parameter[] _parameters;
    private readonly Lazy<int> _equivalencyHash;

    public Parameters(Parameter[] parameters)
    {
        _parameters = parameters;
        _equivalencyHash = new(() =>
        {
            var parametersHash = new HashCode();
            foreach (var parameter in _parameters)
                parametersHash.Add(parameter.GetEquivalencyHash());

            return parametersHash.ToHashCode();
        });
    }

    public int Count => _parameters.Length;

    public Parameter Get(int index)
    {
        return _parameters[index];
    }

    public int GetEquivalencyHash()
    {
        return _equivalencyHash.Value;
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
