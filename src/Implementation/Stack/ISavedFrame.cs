using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal interface ISavedFrame : IMergeable<IExpression, ISavedFrame>
{
    IPersistentFrame Restore(bool useJumpBuffer,
        IPersistentJumps jumps, IPersistentProgramCounter programCounter, IPersistentVariables variables);
}
