using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IParameters : IEquivalent<ExpressionSubs, IParameters>, IMergeable<IParameters>
{
    int Count { get; }

    Parameter Get(int index);
}
