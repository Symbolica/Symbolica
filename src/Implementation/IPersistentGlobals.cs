using System;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Implementation.Memory;

namespace Symbolica.Implementation;

internal interface IPersistentGlobals
{
    (IExpression<IType>, Action<IState>, IPersistentGlobals) GetAddress(IMemoryProxy memory, GlobalId id);
}
