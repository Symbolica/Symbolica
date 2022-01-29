using System.Numerics;
using Symbolica.Abstraction;

namespace Symbolica.Representation;

internal interface IParameterizedStateAction
{
    void Invoke(IState state, BigInteger value);
}
