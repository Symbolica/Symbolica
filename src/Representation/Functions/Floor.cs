using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions
{
    internal sealed class Floor : IFunction
    {
        public Floor(FunctionId id, IParameters parameters)
        {
            Id = id;
            Parameters = parameters;
        }

        public FunctionId Id { get; }
        public IParameters Parameters { get; }

        public void Call(IState state, ICaller caller, IArguments arguments)
        {
            var expression = arguments.Get(0);
            var result = expression.FloatFloor();

            state.Stack.SetVariable(caller.Id, result);
        }
    }
}