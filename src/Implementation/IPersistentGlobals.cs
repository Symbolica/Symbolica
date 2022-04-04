using System;
using Symbolica.Abstraction;
using Symbolica.Expression.Values;
using Symbolica.Implementation.Memory;

namespace Symbolica.Implementation;

internal interface IPersistentGlobals
{
    (Address, Action<IState>, IPersistentGlobals) GetAddress(IMemoryProxy memory, GlobalId id);
}
