using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal interface IVaList : IEquivalent<ExpressionSubs, IVaList>, IMergeable<IVaList>
{
    IExpression Initialize(ISpace space, IStructType vaListType);
}
