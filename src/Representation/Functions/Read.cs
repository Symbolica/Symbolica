using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions
{
    internal sealed class Read : IFunction
    {
        public Read(FunctionId id, IParameters parameters)
        {
            Id = id;
            Parameters = parameters;
        }

        public FunctionId Id { get; }
        public IParameters Parameters { get; }

        public void Call(IState state, ICaller caller, IArguments arguments)
        {
            var descriptor = (int) arguments.Get(0).Integer;
            var address = arguments.Get(1);
            var count = (int) arguments.Get(2).Integer;

            var result = state.System.Read(descriptor, address, count);

            state.Stack.SetVariable(caller.Id, state.Space.CreateConstant(caller.Size, result));
        }
    }
}