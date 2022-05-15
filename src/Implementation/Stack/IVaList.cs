using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal interface IVaList : IMergeable<IExpression, IVaList>
{
    IExpression Initialize(ISpace space, IStructType vaListType);
}
