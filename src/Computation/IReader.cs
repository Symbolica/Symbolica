using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal interface IReader
    {
        IExpression Read(IExpression buffer, IExpression offset, Bits size);
    }
}
