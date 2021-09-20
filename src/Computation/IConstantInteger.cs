using System.Numerics;

namespace Symbolica.Computation
{
    internal interface IConstantInteger : IValue
    {
        BigInteger Constant { get; }
    }
}
