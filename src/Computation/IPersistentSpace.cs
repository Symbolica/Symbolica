using Symbolica.Expression;

namespace Symbolica.Computation;

internal interface IPersistentSpace : ISpace
{
    IPersistentSpace Assert(IValue assertion);
    IConstraints GetConstraints(params IValue[] assertions);
}
