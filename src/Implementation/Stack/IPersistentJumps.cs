using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal interface IPersistentJumps : IEquivalent<ExpressionSubs, IPersistentJumps>, IMergeable<IPersistentJumps>
{
    IPersistentJumps Add(IExpression continuation, bool useJumpBuffer, ISavedFrame frame);
    Result<ISavedFrame> TryGet(ISpace space, IExpression continuation, bool useJumpBuffer);
}
