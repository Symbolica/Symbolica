using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.System;

internal interface ISystemProxy : ISystem, IMergeable<IExpression, ISystemProxy>
{
    ISystemProxy Clone();
}
