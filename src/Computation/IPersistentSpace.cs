using Symbolica.Expression;

namespace Symbolica.Computation;

internal interface IPersistentSpace : ISpace
{
    IPersistentSpace Assert(IExpression<IType> assertion);
    ISolver CreateSolver();
}
