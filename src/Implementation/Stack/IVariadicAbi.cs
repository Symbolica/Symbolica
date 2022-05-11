using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal interface IVariadicAbi
{
    IVaList DefaultVaList { get; }

    IVaList PassOnStack(ISpace space, IMemory memory, IArguments varargs);
}
