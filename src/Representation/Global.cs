using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation;

public sealed class Global : IGlobal
{
    private readonly IOperand _initializer;

    public Global(GlobalId id, Bits size, IOperand initializer)
    {
        _initializer = initializer;
        Id = id;
        Size = size;
    }

    public GlobalId Id { get; }
    public Bits Size { get; }

    public void Initialize(IExpressionFactory exprFactory, IState state, IExpression address)
    {
        var value = _initializer.Evaluate(exprFactory, state);

        state.Memory.Write(address, value);
    }
}
