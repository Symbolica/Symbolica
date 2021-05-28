using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions
{
    internal sealed class Symbolize : IFunction
    {
        public Symbolize(FunctionId id, IParameters parameters)
        {
            Id = id;
            Parameters = parameters;
        }

        public FunctionId Id { get; }
        public IParameters Parameters { get; }

        public void Call(IState state, ICaller caller, IArguments arguments)
        {
            var address = arguments.Get(0);
            var size = (Bytes) (uint) arguments.Get(1).Integer;
            var name = state.ReadString(arguments.Get(2));

            state.Memory.Write(address, state.Space.CreateSymbolic(size.ToBits(), name, null));
        }
    }
}
