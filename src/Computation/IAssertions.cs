using Symbolica.Expression;

namespace Symbolica.Computation;

internal interface IAssertions
{
    IConstantValue GetValue(IValue value);
    IProposition GetProposition(IValue value);
}
