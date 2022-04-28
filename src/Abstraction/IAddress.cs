using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IAddress : IExpression
{
    IType IndexedType { get; }
    IExpression BaseAddress { get; }
}
