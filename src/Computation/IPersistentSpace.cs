using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal interface IPersistentSpace : ISpace
    {
        IPersistentSpace Assert(IBool assertion);
        IModel GetModel(params IBool[] constraints);
    }
}
