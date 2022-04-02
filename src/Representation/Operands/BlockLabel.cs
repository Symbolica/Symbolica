using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation.Operands;

public sealed class BlockLabel : IOperand
{
    private readonly BasicBlockId _id;

    public BlockLabel(BasicBlockId id)
    {
        _id = id;
    }

    public IExpression<IType> Evaluate(IState state)
    {
        return ConstantUnsigned.Create(state.Space.PointerSize, (ulong) _id);
    }
}
