using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Symbolica.Expression;

public interface ISpace : IEquatable<ISpace>
{
    IExample GetExample();
    ISpace Substitute(IReadOnlyDictionary<IExpression, IExpression> subs);
    bool SubsAreEquivalent(IEnumerable<ExpressionSub> subs, ISpace other);
    bool TryMerge(ISpace space, [MaybeNullWhen(false)] out (ISpace Merged, IExpression Predicate) result);
    object ToJson();
}
