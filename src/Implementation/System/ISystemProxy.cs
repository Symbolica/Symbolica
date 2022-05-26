using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.System;

internal interface ISystemProxy : ISystem, IMergeable<ExpressionSubs, ISystemProxy>
{
    ISystemProxy Clone();
}
