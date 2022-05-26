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
    private readonly Lazy<int> _mergeHash;

    public Parameters(Parameter[] parameters)
    {
        _parameters = parameters;
        _equivalencyHash = new(() => EquivalencyHash(false));
        _mergeHash = new(() => EquivalencyHash(true));

        int EquivalencyHash(bool includeSubs)
        {
            var parametersHash = new HashCode();
            foreach (var parameter in _parameters)
                parametersHash.Add(parameter.GetEquivalencyHash(includeSubs));

            return parametersHash.ToHashCode();
        }
    }

    public int Count => _parameters.Length;

    public Parameter Get(int index)
    {
        return _parameters[index];
    }

    public int GetEquivalencyHash(bool includeSubs)
    {
        return includeSubs
            ? _mergeHash.Value
            : _equivalencyHash.Value;
    }

    public (HashSet<ExpressionSubs> subs, bool) IsEquivalentTo(IParameters other)
    {
        return other is Parameters p
            ? _parameters.IsSequenceEquivalentTo<ExpressionSubs, Parameter>(p._parameters)
            : (new(), false);
    }

    public object ToJson()
    {
        return _parameters.Select(p => p.ToJson()).ToArray();
    }
}
