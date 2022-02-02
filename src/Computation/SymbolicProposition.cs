using Symbolica.Computation.Values.Symbolics;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class SymbolicProposition : IProposition
{
    private readonly IValue _assertion;
    private readonly IModel _model;
    private readonly IValue _negation;
    private readonly IPersistentSpace _space;

    private SymbolicProposition(IPersistentSpace space, IModel model, IValue assertion, IValue negation)
    {
        _space = space;
        _model = model;
        _assertion = assertion;
        _negation = negation;
    }

    public ISpace FalseSpace => _space.Assert(_negation);
    public ISpace TrueSpace => _space.Assert(_assertion);
    public bool CanBeFalse => _model.IsSatisfiable(_negation);
    public bool CanBeTrue => _model.IsSatisfiable(_assertion);

    public void Dispose()
    {
        _model.Dispose();
    }

    public static IProposition Create(IPersistentSpace space, IValue assertion, IValue[] constraints)
    {
        var model = space.GetModel(constraints);

        return new SymbolicProposition(space, model, assertion, Not.Create(assertion));
    }
}
