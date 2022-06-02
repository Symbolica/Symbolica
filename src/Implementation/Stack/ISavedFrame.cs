using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal interface ISavedFrame : IEquivalent<ExpressionSubs, ISavedFrame>, IMergeable<ISavedFrame>
{
    IPersistentFrame Restore(bool useJumpBuffer,
        IPersistentJumps jumps, IPersistentProgramCounter programCounter, IPersistentVariables variables);
}
