using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class Proposition : IProposition
{
    private readonly IExpression<IType> _assertion;
    private readonly IExpression<IType> _negation;
    private readonly ISolver _solver;
    private readonly IPersistentSpace _space;

    private Proposition(IPersistentSpace space, ISolver solver, IExpression<IType> assertion, IExpression<IType> negation)
    {
        _space = space;
        _solver = solver;
        _assertion = assertion;
        _negation = negation;
    }

    public void Dispose()
    {
        _solver.Dispose();
    }

    public ISpace CreateFalseSpace()
    {
        return _space.Assert(_negation);
    }

    public ISpace CreateTrueSpace()
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

    public static IProposition Create(IPersistentSpace space, IExpression<IType> assertion)
    {
        var solver = space.CreateSolver();

        return new Proposition(space, solver, assertion, Symbolica.Expression.Values.LogicalNot.Create(assertion));
    }
}
