using System.Collections.Generic;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IAddress : IExpression
{
    IType IndexedType { get; }
    IExpression BaseAddress { get; }
    IEnumerable<(IType, IExpression)> Offsets { get; }

    IAddress AddImplicitOffsets(ISpace space);
    IAddress? SubtractBase(ISpace space, IExpression baseAddress);
}
