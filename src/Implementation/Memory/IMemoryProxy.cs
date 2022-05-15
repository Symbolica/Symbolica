using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal interface IMemoryProxy : IMemory, IMergeable<IExpression, IMemoryProxy>
{
    IMemoryProxy Clone();
}
