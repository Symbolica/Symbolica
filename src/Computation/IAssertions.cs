using Symbolica.Expression;

namespace Symbolica.Computation;

internal interface IAssertions
{
    IConstantValue GetConstant(IValue value);
    IProposition GetProposition(IValue value);
}
