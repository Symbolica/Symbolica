using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack
{
    internal interface IVaList
    {
        IExpression Initialize(ISpace space, IStructType vaListType);
    }
}
