using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal interface IBitVector : IValue
    {
        IBitVector Read(IUnsigned offset, Bits size);
        IBitVector Write(IUnsigned offset, IBitVector value);
    }
}
