using Symbolica.Computation.Values;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class SymbolicProposition : IProposition
{
    private readonly IValue _assertion;
    private readonly IValue _negation;
    private readonly IPersistentSpace _space;

    private SymbolicProposition(IPersistentSpace space, IValue assertion, IValue negation)
    {
        _space = space;
        _assertion = assertion;
        _negation = negation;
    }

    public ISpace FalseSpace => _space.Assert(_negation);
    public ISpace TrueSpace => _space.Assert(_assertion);
    public bool CanBeFalse => _space.Constraints.IsSatisfiable(_negation);
    public bool CanBeTrue => _space.Constraints.IsSatisfiable(_assertion);

    public static IProposition Create(IPersistentSpace space, IValue assertion)
    {
        return new SymbolicProposition(space, assertion, LogicalNot.Create(assertion));
    }
}
