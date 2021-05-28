using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Implementation.Memory;

namespace Symbolica.Implementation.Stack
{
    internal interface IVariadicAbi
    {
        IExpression PassOnStack(ISpace space, IMemoryProxy memory, IArguments varargs);
        IExpression CreateVaList(ISpace space, IStructType vaListType, IExpression address);
    }
}
