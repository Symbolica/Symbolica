using Symbolica.Expression;

namespace Symbolica.Computation;

internal interface IPersistentSpace : ISpace
{
    IPersistentSpace Assert(IExpression assertion);
    ISolver CreateSolver();
}
