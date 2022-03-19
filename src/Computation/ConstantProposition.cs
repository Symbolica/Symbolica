using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class ConstantProposition : IProposition
{
    private readonly bool _isTrue;
    private readonly ISpace _space;

    private ConstantProposition(ISpace space, bool isTrue)
    {
        _space = space;
        _isTrue = isTrue;
    }

    public void Dispose()
    {
    }

    public ISpace CreateFalseSpace()
    {
        return _space;
    }

    public ISpace CreateTrueSpace()
    {
        return _space;
    }

    public bool CanBeFalse()
    {
        return !CanBeTrue();
    }

    public bool CanBeTrue()
    {
        return _isTrue;
    }

    public static IProposition Create(ISpace space, IConstantValue value)
    {
        return new ConstantProposition(space, value.AsBool());
    }
}
