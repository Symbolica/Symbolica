using Symbolica.Collection;

namespace Symbolica.Computation
{
    internal interface IConstantBitVector : IValue
    {
        IPersistentList<byte> Constant { get; }
    }
}
