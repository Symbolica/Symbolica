using System.Collections.Generic;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IAddress : IExpression
{
    IType IndexedType { get; }
    IExpression BaseAddress { get; }
    IEnumerable<IExpression> Offsets { get; }

    IAddress SubtractBase(IExpression baseAddress);
}
