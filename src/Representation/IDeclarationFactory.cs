using Symbolica.Abstraction;

namespace Symbolica.Representation;

public interface IDeclarationFactory
{
    IFunction Create(string name, FunctionId id, IParameters parameters);
}
