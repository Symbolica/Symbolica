using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Implementation.Memory;

namespace Symbolica.Implementation.Stack
{
    internal interface IVariadicAbi
    {
        IVaList DefaultVaList { get; }

        IVaList PassOnStack(ISpace space, IMemoryProxy memory, IArguments varargs);
    }
}
