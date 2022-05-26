using System.Collections.Generic;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal interface ISavedFrame : IMergeable<ExpressionSubs, ISavedFrame>
{
    IPersistentFrame Restore(bool useJumpBuffer,
        IPersistentJumps jumps, IPersistentProgramCounter programCounter, IPersistentVariables variables);
}
