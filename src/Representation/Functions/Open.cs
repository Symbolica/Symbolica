using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions
{
    internal sealed class Open : IFunction
    {
        public Open(FunctionId id, IParameters parameters)
        {
            Id = id;
            Parameters = parameters;
        }

        public FunctionId Id { get; }
        public IParameters Parameters { get; }

        public void Call(IState state, ICaller caller, IArguments arguments)
        {
            var path = state.ReadString(arguments.Get(0));

            var descriptor = state.System.Open(path);

            state.Stack.SetVariable(caller.Id, state.Space.CreateConstant(caller.Size, descriptor));
        }
    }
}