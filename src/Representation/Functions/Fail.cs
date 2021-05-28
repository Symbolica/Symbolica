using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions
{
    internal sealed class Fail : IFunction
    {
        public Fail(FunctionId id, IParameters parameters)
        {
            Id = id;
            Parameters = parameters;
        }

        public FunctionId Id { get; }
        public IParameters Parameters { get; }

        public void Call(IState state, ICaller caller, IArguments arguments)
        {
            throw new StateException("Assertion could fail.", state.Space);
        }
    }
}
