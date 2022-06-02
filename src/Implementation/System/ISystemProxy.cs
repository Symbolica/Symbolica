using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.System;

internal interface ISystemProxy : ISystem, IEquivalent<ExpressionSubs, ISystemProxy>, IMergeable<ISystemProxy>
{
    ISystemProxy Clone();
}
