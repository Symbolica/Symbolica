using Symbolica.Expression;

namespace Symbolica.Computation;

internal interface IPersistentSpace : ISpace
{
    IPersistentSpace Assert(IValue assertion);
    IModel GetModel(params IValue[] constraints);
}
