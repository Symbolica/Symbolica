using System;
using System.Collections.Generic;

namespace Symbolica.Expression;

public interface ISpace : IEquatable<ISpace>
{
    IExample GetExample();
    ISpace Substitute(IReadOnlyDictionary<IExpression, IExpression> subs);
}
