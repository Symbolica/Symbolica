using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal interface IPersistentJumps
{
    IPersistentJumps Add(IExpression continuation, bool useJumpBuffer, ISavedFrame frame);
    ISavedFrame? TryGet(ISpace space, IExpression continuation, bool useJumpBuffer);
}
