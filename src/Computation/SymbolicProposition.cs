using Symbolica.Computation.Values;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class SymbolicProposition : IProposition
{
    private readonly IValue _assertion;
    private readonly IConstraints _constraints;
    private readonly IValue _negation;
    private readonly IPersistentSpace _space;

    private SymbolicProposition(IPersistentSpace space, IConstraints constraints, IValue assertion, IValue negation)
    {
        _space = space;
        _constraints = constraints;
        _assertion = assertion;
        _negation = negation;
    }

    public ISpace FalseSpace => _space.Assert(_negation);
    public ISpace TrueSpace => _space.Assert(_assertion);
    public bool CanBeFalse => _constraints.IsSatisfiable(_negation);
    public bool CanBeTrue => _constraints.IsSatisfiable(_assertion);

    public void Dispose()
    {
        _constraints.Dispose();
    }

    public static IProposition Create(IPersistentSpace space, IValue assertion, IValue[] assertions)
    {
        var constraints = space.GetConstraints(assertions);

        return new SymbolicProposition(space, constraints, assertion, Not.Create(assertion));
    }
}
