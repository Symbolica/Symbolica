using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IType
{
    Bytes Size { get; }

    IType GetType(ISpace space, IExpression index);
    IExpression GetOffsetBits(ISpace space, IExpression index);
    IExpression GetOffsetBytes(ISpace space, IExpression index);
}
