using Symbolica.Computation.Values;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class SymbolicProposition : IProposition
{
    private readonly IValue _assertion;
    private readonly IContext _context;
    private readonly IValue _negation;
    private readonly IPersistentSpace _space;

    private SymbolicProposition(IPersistentSpace space, IContext context, IValue assertion, IValue negation)
    {
        _space = space;
        _context = context;
        _assertion = assertion;
        _negation = negation;
    }

    public ISpace FalseSpace => _space.Assert(_negation);
    public ISpace TrueSpace => _space.Assert(_assertion);
    public bool CanBeFalse => _context.IsSatisfiable(_negation.AsBool(_context));
    public bool CanBeTrue => _context.IsSatisfiable(_assertion.AsBool(_context));

    public void Dispose()
    {
        _context.Dispose();
    }

    public static IProposition Create(IPersistentSpace space, IValue assertion)
    {
        var context = space.CreateContext();

        return new SymbolicProposition(space, context, assertion, LogicalNot.Create(assertion));
    }
}
