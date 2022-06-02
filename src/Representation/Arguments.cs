using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation;

internal sealed class Arguments : IArguments
{
    private readonly IExpression[] _expressions;

    public Arguments(IExpression[] expressions)
    {
        _expressions = expressions;
    }

    public IExpression Get(int index)
    {
        return _expressions[index];
    }

    public IEnumerator<IExpression> GetEnumerator()
    {
        return _expressions.AsEnumerable().GetEnumerator();
    }

    public int GetEquivalencyHash()
    {
        var hash = new HashCode();
        foreach (var expression in _expressions)
            hash.Add(expression.GetEquivalencyHash());
        return hash.ToHashCode();
    }

    public int GetMergeHash()
    {
        var hash = new HashCode();
        foreach (var expression in _expressions)
            hash.Add(expression.GetMergeHash());
        return hash.ToHashCode();
    }

    public (HashSet<ExpressionSubs> subs, bool) IsEquivalentTo(IArguments other)
    {
        return other is Arguments a
            ? _expressions.IsSequenceEquivalentTo(
                a._expressions,
                (x, y) => x.IsEquivalentTo(y).ToHashSet())
            : (new(), false);
    }

    public object ToJson()
    {
        return _expressions.Select(e => e.ToJson()).ToArray();
    }

    public bool TryMerge(IArguments other, IExpression predicate, [MaybeNullWhen(false)] out IArguments merged)
    {
        if (other is Arguments a && _expressions.TryMerge(a._expressions, predicate, out var mergedExpressions))
        {
            merged = new Arguments(mergedExpressions.ToArray());
            return true;
        }
        merged = null;
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
