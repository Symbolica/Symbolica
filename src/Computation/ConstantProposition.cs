using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class ConstantProposition : IProposition
{
    private ConstantProposition(ISpace space, bool isTrue)
    {
        TrueSpace = space;
        CanBeTrue = isTrue;
    }

    public ISpace FalseSpace => TrueSpace;
    public ISpace TrueSpace { get; }
    public bool CanBeFalse => !CanBeTrue;
    public bool CanBeTrue { get; }

    public static IProposition Create(ISpace space, IConstantValue value)
    {
        return new ConstantProposition(space, value.AsBool());
    }
}
