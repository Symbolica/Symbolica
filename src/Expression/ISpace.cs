using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Symbolica.Expression;

public interface ISpace : IEquatable<ISpace>
{
    IExample GetExample();
    ISpace Substitute(IReadOnlyDictionary<IExpression, IExpression> subs);
    bool TryMerge(ISpace space, [MaybeNullWhen(false)] out ISpace merged);
    object ToJson();
}
