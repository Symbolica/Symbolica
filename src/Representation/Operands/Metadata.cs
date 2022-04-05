using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands;

// Metadata is currently only used as arguments for llvm.experimental intrinsics and they are not correct at all ...
public sealed class Metadata : IOperand
{
    public IExpression Evaluate(IState state)
    {
        // ... so this is total nonsense that is only implemented because function arguments are greedily evaluated.
        return state.Space.CreateGarbage(Size.Byte);
    }
}
