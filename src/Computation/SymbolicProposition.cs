using Symbolica.Computation.Values;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class SymbolicProposition : IProposition
{
    private readonly IValue _assertion;
    private readonly IValue _negation;
    private readonly ISolver _solver;
    private readonly IPersistentSpace _space;

    private SymbolicProposition(IPersistentSpace space, ISolver solver, IValue assertion, IValue negation)
    {
        _space = space;
        _solver = solver;
        _assertion = assertion;
        _negation = negation;
    }

    public ISpace FalseSpace()
    {
        return _space.Assert(_negation);
    }

    public ISpace TrueSpace()
    {
        return _space.Assert(_assertion);
    }

    public bool CanBeFalse()
    {
        return _solver.IsSatisfiable(_negation);
    }

    public bool CanBeTrue()
    {
        return _solver.IsSatisfiable(_assertion);
    }

    public void Dispose()
    {
        _solver.Dispose();
    }

    public static IProposition Create(IPersistentSpace space, IValue assertion)
    {
        var solver = space.CreateSolver();

        return new SymbolicProposition(space, solver, assertion, LogicalNot.Create(assertion));
    }
}
