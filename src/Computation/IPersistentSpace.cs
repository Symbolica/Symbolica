using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal interface IPersistentSpace : ISpace
    {
        IPersistentSpace Assert(SymbolicBool assertion);
        IModel GetModel(params SymbolicBool[] constraints);
    }
}