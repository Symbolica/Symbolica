using System;
using System.Collections.Generic;

namespace Symbolica.Expression;

public interface ISpace : IEquatable<ISpace>
{
    IExample GetExample();
    ISpace Substitute(IReadOnlyDictionary<IExpression, IExpression> subs);
    bool SubsAreEquivalent(IEnumerable<ExpressionSub> subs, ISpace other);
    bool TryMerge(ISpace space, out (ISpace Merged, IExpression Predicate) result);
    object ToJson();
}
