using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation;

public sealed class UninitializedGlobal : IGlobal
{
    public UninitializedGlobal(GlobalId id, Size size)
    {
        Id = id;
        Size = size;
    }

    public GlobalId Id { get; }
    public Size Size { get; }

    public void Initialize(IState state, IExpression address)
    {
    }
}
