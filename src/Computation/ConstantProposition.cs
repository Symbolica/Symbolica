using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class ConstantProposition : IProposition
{
    private readonly ISpace _space;
    private readonly bool _isTrue;

    private ConstantProposition(ISpace space, bool isTrue)
    {
        _space = space;
        _isTrue = isTrue;
    }

    public long RefCount => 0;
    public bool CanBeFalse() => !CanBeTrue();
    public bool CanBeTrue() => _isTrue;

    public ISpace FalseSpace() => _space;
    public ISpace TrueSpace() => _space;


    public void Dispose()
    {
    }

    public static IProposition Create(ISpace space, IConstantValue value)
    {
        return new ConstantProposition(space, value.AsBool());
    }
}
