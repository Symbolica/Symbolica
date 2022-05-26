using System.Collections.Generic;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IType : IMergeable<ExpressionSubs, IType>
{
    Bytes Size { get; }
    IEnumerable<IType> Types { get; }
    IEnumerable<Bytes> Offsets { get; }

    IType GetType(ISpace space, IExpression index);
    IExpression GetOffsetBits(IExpressionFactory exprFactory, ISpace space, IExpression index);
    IExpression GetOffsetBytes(IExpressionFactory exprFactory, ISpace space, IExpression index);
}
