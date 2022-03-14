using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class Assertions : IAssertions
{
    private readonly IPersistentSpace _space;

    public Assertions(IPersistentSpace space)
    {
        _space = space;
    }

    public IConstantValue GetValue(IValue value)
    {
        using var constraints = _space.GetConstraints();

        return ConstantUnsigned.Create(value.Size, constraints.GetValue(value));
    }

    public IProposition GetProposition(IValue value)
    {
        return value is IConstantValue v
            ? ConstantProposition.Create(_space, v)
            : SymbolicProposition.Create(_space, value);
    }
}
