using Symbolica.Expression;

namespace Symbolica.Computation;

internal interface IPersistentSpace : ISpace
{
    IConstantValue GetConstant(IValue value);
    IProposition GetProposition(IValue value);
    IPersistentSpace Assert(IValue assertion);
    IConstraints GetConstraints();
}
