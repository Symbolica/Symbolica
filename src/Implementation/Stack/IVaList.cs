using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal interface IVaList : IMergeable<ExpressionSubs, IVaList>
{
    IExpression Initialize(ISpace space, IStructType vaListType);
}
