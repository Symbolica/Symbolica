using Symbolica.Expression;

namespace Symbolica.Computation;

internal interface IPersistentSpace : ISpace
{
    IAssertions Assertions { get; }

    IPersistentSpace Assert(IValue assertion);
    IConstraints GetConstraints();
}
