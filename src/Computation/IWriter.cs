using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal interface IWriter
    {
        IExpression Mask(IExpression buffer, IExpression offset, Bits size);
        IExpression Write(IExpression buffer, IExpression offset, IExpression value);
    }
}
