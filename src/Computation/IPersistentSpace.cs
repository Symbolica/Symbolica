using Symbolica.Expression;

namespace Symbolica.Computation;

internal interface IPersistentSpace : ISpace
{
    IConstraints Constraints { get; }

    IPersistentSpace Assert(IValue assertion);
}
