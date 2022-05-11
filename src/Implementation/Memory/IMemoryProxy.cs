using Symbolica.Abstraction;

namespace Symbolica.Implementation.Memory;

internal interface IMemoryProxy : IMemory
{
    IMemoryProxy Clone();
}
