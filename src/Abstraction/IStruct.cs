using System.Numerics;
using Symbolica.Expression;

namespace Symbolica.Abstraction
{
    public interface IStruct
    {
        IExpression Expression { get; }

        IExpression Read(ISpace space, int index);
        IStruct Write(ISpace space, int index, IExpression value);
        IStruct Write(ISpace space, int index, BigInteger value);
    }
}
