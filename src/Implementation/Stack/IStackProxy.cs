using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal interface IStackProxy : IStack, IMergeable<ExpressionSubs, IStackProxy>
{
    IStackProxy Clone();
    void ExecuteNextInstruction(IState state);
}
