using Symbolica.Abstraction;

namespace Symbolica.Implementation
{
    internal interface IFunctions
    {
        IFunction Get(FunctionId functionId);
    }
}
