using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        _equivalencyHash = new(() =>
        {
            var parametersHash = new HashCode();
            foreach (var parameter in _parameters)
                parametersHash.Add(parameter.GetEquivalencyHash());

            return parametersHash.ToHashCode();
        });
        _mergeHash = new(() =>
        {
            var parametersHash = new HashCode();
            foreach (var parameter in _parameters)
                parametersHash.Add(parameter.GetMergeHash());

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

    public int GetMergeHash()
    {
        return _mergeHash.Value;
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

    public bool TryMerge(IParameters other, IExpression predicate, [MaybeNullWhen(false)] out IParameters merged)
    {
        if (other is Parameters p && _parameters.TryMerge(p._parameters, predicate, out var mergedParameters))
        {
            merged = new Parameters(mergedParameters.ToArray());
            return true;
        }

        merged = null;
        return false;
    }
}
