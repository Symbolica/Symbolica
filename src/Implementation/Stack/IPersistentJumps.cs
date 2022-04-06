using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal interface IPersistentJumps
{
    IPersistentJumps Add(IExpression<IType> continuation, bool useJumpBuffer, ISavedFrame frame);
    Result<ISavedFrame> TryGet(ISpace space, IExpression<IType> continuation, bool useJumpBuffer);
}
