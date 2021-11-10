using System.Numerics;
using Symbolica.Abstraction;

namespace Symbolica.Representation
{
    internal interface IPartialAction
    {
        void Invoke(IState state, BigInteger value);
    }
}
