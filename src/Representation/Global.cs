using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation;

public sealed class Global : IGlobal
{
    private readonly IOperand _initializer;

    public Global(GlobalId id, Size size, IOperand initializer)
    {
        _initializer = initializer;
        Id = id;
        Size = size;
    }

    public GlobalId Id { get; }
    public Size Size { get; }

    public void Initialize(IState state, IExpression address)
    {
        var value = _initializer.Evaluate(state);

        state.Memory.Write(address, value);
    }
}
