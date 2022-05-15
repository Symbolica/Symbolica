using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IParameters : IMergeable<IExpression, IParameters>
{
    int Count { get; }

    Parameter Get(int index);
}
