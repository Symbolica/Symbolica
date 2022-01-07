using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal interface IValueExpression : IExpression
    {
        IValue Value { get; }
        IValue[] Constraints { get; }
    }
}
