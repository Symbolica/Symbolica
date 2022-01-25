using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal interface IReader
    {
        IExpression Read(ISymbolicExpression buffer, IExpression offset, Bits size);
    }
}
