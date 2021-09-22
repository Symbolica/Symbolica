using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions
{
    internal sealed class Duplicate : IFunction
    {
        public Duplicate(FunctionId id, IParameters parameters)
        {
            Id = id;
            Parameters = parameters;
        }

        public FunctionId Id { get; }
        public IParameters Parameters { get; }

        public void Call(IState state, ICaller caller, IArguments arguments)
        {
            var descriptor = (int) arguments.Get(0).Constant;

            var result = state.System.Duplicate(descriptor);

            state.Stack.SetVariable(caller.Id, state.Space.CreateConstant(caller.Size, result));
        }
    }
}
