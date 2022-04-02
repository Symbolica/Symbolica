using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal interface IVaList
{
    IExpression<IType> Initialize(ISpace space, IStructType vaListType);
}
