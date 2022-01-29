using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Implementation.Memory;

namespace Symbolica.Implementation.System;

internal interface ISystemProxy : ISystem
{
    ISystemProxy Clone(ISpace space, IMemoryProxy memory);
}
