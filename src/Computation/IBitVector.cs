using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal interface IBitVector : IValue
    {
        IValue Read(IUnsigned offset, Bits size);
        IValue Write(IUnsigned offset, IBitVector value);
    }
}
