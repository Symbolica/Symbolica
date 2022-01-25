using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal interface ISymbolicExpression : IValueExpression
    {
        IExpression Mask(IExpression offset, Bits size);
    }
}
